using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using VolunteerMgt.Server.Abstraction.Mail;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Common.Settings;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.Auth;
using VolunteerMgt.Server.Models.PasswordModel;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Services.Identity;

public class AuthService(
    ILogger<AuthService> _logger,
    UserManager<ApplicationUser> _userManager,
    IHttpContextAccessor _httpContextAccessor,
    IMailService _mailService,
    IOptions<JwtConfiguration> _jwtConfiguration
    ) : IAuthService
{
    public async Task<Result<TokenResponse>> GetTokenAsync(TokenRequest request)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return await Result<TokenResponse>.FailAsync("Invalid email or password");

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return await Result<TokenResponse>.FailAsync("Invalid email or password");

        // Generate token
        (JwtSecurityToken token, DateTime expiry) = GetJwtSecurityToken(user);

        return await Result<TokenResponse>.SuccessAsync(new TokenResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiry = expiry,
            Email = user.Email ?? ""
        });
    }
    public async Task<Result> ForgotPasswordAsync(ForgotPasswordModel model)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Result.Fail("User with this email does not exist.", HttpStatusCode.NotFound);
            }

            // Generate reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var scheme = _httpContextAccessor?.HttpContext?.Request.Scheme;
            var host = _httpContextAccessor?.HttpContext?.Request.Host;

            var resetLink = $"{scheme}://{host}/api/reset-password?email={user.Email}&token={WebUtility.UrlEncode(token)}";


            // Send email
            var emailBody = $"Click <a href='{resetLink}'>here</a> to reset your password.";
            await _mailService.SendEmailAsync(user.Email ?? string.Empty, "Password Reset Request", emailBody);

            return Result.Success("Password reset link has been sent to your email.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while while forgeting the users");
            return Result<List<ApplicationUser>>.Fail("An internal error occurred while forgeting the users.", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result> ResetPasswordAsync(string email, string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Invalid reset attempt: Missing email or token.");
                return Result.Fail("Invalid request. Email and token are required.", HttpStatusCode.BadRequest);
            }
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("Password reset attempt failed: User with email {Email} not found.", email);
                return Result.Fail("User not found.", HttpStatusCode.NotFound);
            }

            var isTokenValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider,
                UserManager<ApplicationUser>.ResetPasswordTokenPurpose, WebUtility.UrlDecode(token));

            if (!isTokenValid)
                return Result.Fail("Invalid or expired token.", HttpStatusCode.BadRequest);

            _logger.LogInformation("Password reset request valid for user: {Email}", email);
            return await Result.SuccessAsync("You can reset your password.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the password reset for {Email}", email);
            return Result.Fail("An internal error occurred. Please try again later.", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result> ResetPasswordPostAsync(ResetPasswordModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Token))
            {
                _logger.LogWarning("Invalid reset attempt: Missing email or token.");
                return Result.Fail("Invalid request. Email and token are required.", HttpStatusCode.BadRequest);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                _logger.LogWarning("Password reset attempt failed: User with email {Email} not found.", model.Email);
                return Result.Fail("User not found.", HttpStatusCode.NotFound);
            }

            var resetResult = await _userManager.ResetPasswordAsync(user, WebUtility.UrlDecode(model.Token), model.NewPassword);
            if (!resetResult.Succeeded)
            {
                var errors = string.Join("; ", resetResult.Errors.Select(e => e.Description));
                _logger.LogError("Password reset failed for {Email}: {Errors}", model.Email, errors);
                return Result.Fail($"Failed to reset password: {errors}", HttpStatusCode.BadRequest);
            }
            var emailBody = $"Password reset successfully";
            await _mailService.SendEmailAsync(user.Email ?? string.Empty, "Password Reset", emailBody);

            _logger.LogInformation("Password successfully reset for {Email}", model.Email);
            return await Result.SuccessAsync("Password reset successfully.");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the password reset for {Email}", model.Email);
            return Result.Fail("An internal error occurred. Please try again later.", HttpStatusCode.InternalServerError);
        }
    }
    private Tuple<JwtSecurityToken, DateTime> GetJwtSecurityToken(ApplicationUser applicationUser)
    {
        // Security stamp set into the claims
        var securityStampClaimType = new ClaimsIdentityOptions().SecurityStampClaimType;
        // Get user roles
        var userRoles = _userManager.GetRolesAsync(applicationUser);
        // User claims
        var authClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, applicationUser.Id),
                new(ClaimTypes.Name, applicationUser.UserName?? ""),
                new(ClaimTypes.Email, applicationUser.Email??string.Empty),
                new(ClaimTypes.Role, userRoles.Result?.FirstOrDefault()??string.Empty)
            };

        return GetToken(authClaims);
    }

    private Tuple<JwtSecurityToken, DateTime> GetToken(IEnumerable<Claim> authClaims)
    {
        _logger.LogInformation("Generating token");
        // Set token expiry
        var tokenExpiry = DateTime.UtcNow.AddMinutes(_jwtConfiguration.Value.TokenExpiry);

        // Get secret key from the appsettings.json file
        var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Value.Secret!));

        // Create token
        var token = new JwtSecurityToken(
            issuer: _jwtConfiguration.Value.ValidIssuer,
            audience: _jwtConfiguration.Value.ValidAudience,
            expires: tokenExpiry,
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
        );

        return new Tuple<JwtSecurityToken, DateTime>(token, tokenExpiry);
    }

}
