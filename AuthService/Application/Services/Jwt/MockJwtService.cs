using AuthService.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Api.Services.Jwt;

public class MockJwtService(IConfiguration configuration) : IMockJwtService
{
    #region Services

    private readonly IConfiguration _configuration = configuration;

    #endregion Services

    /// <summary>
    /// Simula la generación de un usuario para obtener un token con claims y role
    /// </summary>
    /// <returns></returns>
    /// <exception cref="EmptyConfigException"></exception>
    public string GenerateToken()
    {
        var keyStr = _configuration["Jwt:Key"];

        if (string.IsNullOrEmpty(keyStr))
        {
            throw new EmptyConfigException(nameof(keyStr));
        }

        var key = Encoding.ASCII.GetBytes(keyStr);
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "MockId"),
            new(ClaimTypes.Role, "Admin"),
            new(ClaimTypes.Name, "MockUserName"),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = _configuration["Jwt:Issuer"] ?? string.Empty,
            Audience = _configuration["Jwt:Audience"] ?? string.Empty,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}