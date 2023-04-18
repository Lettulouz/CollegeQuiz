using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CollegeQuizWeb.Config;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CollegeQuizWeb.Jwt;

public class JwtService : IJwtService
{
    private readonly ApplicationDbContext _context;
    private readonly byte[] tokenBytesRepres = Encoding.ASCII.GetBytes(ConfigLoader.JwtSecret);
    
    public JwtService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserEntity?> ValidateToken(Controller controller)
    {
        try
        {
            var bearer = controller.Request.Headers.Authorization.ToString();
            if (!bearer.StartsWith("Bearer ")) throw new ApplicationException();
            var rawJwt = bearer.Substring(7);
        
            JsonWebTokenHandler tokenHandler = new JsonWebTokenHandler();

            var result = tokenHandler.ValidateToken(rawJwt, new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidIssuer = ConfigLoader.JwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(tokenBytesRepres)
            });

            var username = result.Claims.FirstOrDefault(e => e.Key.Equals("USER_ISS"));
            if (username.Key == null || username.Value == null) throw new ApplicationException();
            
            var user = await _context.Users.FirstOrDefaultAsync(e => e.Username.Equals(username.Value));
            if (user == null) throw new ApplicationException();
            
            return user;
        }
        catch (Exception _)
        {
            controller.Response.StatusCode = 401;
            return null;
        }
    }

    public string GenerateToken(string username)
    {
        JwtSecurityTokenHandler jaHandler = new JwtSecurityTokenHandler();
        SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new []
            {
                new Claim("USER_ISS", username)
            }),
            Expires = DateTime.UtcNow.AddDays(ConfigLoader.JwtExpiredDays),
            Issuer = ConfigLoader.JwtIssuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenBytesRepres),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var securityToken = jaHandler.CreateToken(securityTokenDescriptor);
        return jaHandler.WriteToken(securityToken);
    }
}