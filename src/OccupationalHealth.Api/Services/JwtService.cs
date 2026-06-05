using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OccupationalHealth.Api.Services;

public class JwtService
{
    private readonly string _secret;
    private readonly int _expireHours;

    public JwtService(IConfiguration config)
    {
        _secret = config["Jwt:Secret"] ?? "occupational_health_secret_key_2024";
        _expireHours = int.Parse(config["Jwt:ExpiresInHours"] ?? "24");
    }

    public string GenerateToken(int userId, string role, string name, string orgName)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("id", userId.ToString()),
            new Claim("role", role),
            new Claim("name", name),
            new Claim("org_name", orgName)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_expireHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key
            }, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
