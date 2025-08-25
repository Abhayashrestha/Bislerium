using Bislerium.API.Respositories.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Bislerium.API.Respositories.Implementation
{
    public class TokenRespository : ITokenRespository
    {
        public TokenRespository(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public string createJwtToken(IdentityUser user, List<string> roles)
        {
            // Create Claims 
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            //Jwt security Token Parameters
            var Key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));

            var cerdentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Configuration["Jwt:Issuer"],
                audience: Configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: cerdentials);

            //Return Token 
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
