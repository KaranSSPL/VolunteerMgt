﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Common.Settings;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.Auth;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Services.Identity;

public class AuthService(
    ILogger<AuthService> _logger,
    UserManager<ApplicationUser> _userManager,
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
