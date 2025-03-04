using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Common.Settings;
using VolunteerMgt.Server.Models.Auth;
using VolunteerMgt.Server.Models.User;
using VolunteerMgt.Server.Models.Wrapper;
using VolunteerMgt.Server.Persistence;

namespace VolunteerMgt.Server.Services.Identity;

public class AuthService : IAuthService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<AuthService> _logger;
    private readonly JwtConfiguration _jwtConfig;

    public AuthService(DatabaseContext context, ILogger<AuthService> logger, IOptions<JwtConfiguration> jwtConfig)
    {
        _context = context;
        _logger = logger;
        _jwtConfig = jwtConfig.Value;
    }

    public async Task<Result<TokenResponse>> RegisterAsync(UserModel request)
    {
        if (_context.User.Any(u => u.Email == request.Email))
        {
            return await Result<TokenResponse>.FailAsync("Email already in use.");
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new UserModel
        {
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Username = request.Username,
            Roles = request.Roles,
            Email = request.Email,
            Phone = request.Phone,
            Password = hashedPassword
        };

        _context.User.Add(user);
        await _context.SaveChangesAsync();

        return await Result<TokenResponse>.SuccessAsync(new TokenResponse { Email = user.Email });
    }

    public async Task<Result<TokenResponse>> LoginAsync(TokenRequest request)
    {
        var user = _context.User.FirstOrDefault(u => u.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            
            return await Result<TokenResponse>.FailAsync("Invalid email or password.", HttpStatusCode.Unauthorized);
        }

        // Generate JWT token
        (string token, DateTime expiry) = GenerateJwtToken(user);

        return await Result<TokenResponse>.SuccessAsync(new TokenResponse
        {
            Token = token,
            Expiry = expiry,
            Email = user.Email
            
        });
    }

    private Tuple<string, DateTime> GenerateJwtToken(UserModel user)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Roles)
        };

        var tokenExpiry = DateTime.UtcNow.AddMinutes(_jwtConfig.TokenExpiry);
        var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret!));

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.ValidIssuer,
            audience: _jwtConfig.ValidAudience,
            expires: tokenExpiry,
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
        );

        return new Tuple<string, DateTime>(new JwtSecurityTokenHandler().WriteToken(token), tokenExpiry);
    }
}
