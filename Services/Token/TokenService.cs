using PropMan.Models;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using System.Text;
using PropMan.Services.Token;

namespace Propman.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;

        public TokenService(IConfiguration config,DataContext context)
        {
            _config = config;
            _context = context;
        }
        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("UserName", user.Name),
                new Claim("CompanyId",user.CompanyId.ToString()),
                new Claim(ClaimTypes.Role,user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AppSettings:Token"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(50),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        

        //     public async Task<string> GenerateRefreshToken(User user)
        //     {
        //         var randomBytes = new byte[64];
        //         using var rng = RandomNumberGenerator.Create();
        //         rng.GetBytes(randomBytes);
        //         var tokenString = Convert.ToBase64String(randomBytes);

        //         var refreshToken = new RefreshToken
        //         {
        //             Token = tokenString,
        //             Expiry = DateTime.UtcNow.AddDays(1),
        //             Created = DateTime.UtcNow,
        //             UserId = user.UserId.ToString(),
        //             IsRevoked = false
        //         };

        //         _context.RefreshToken.Add(refreshToken);
        //         await _context.SaveChangesAsync();

        //         return tokenString; 
        // }



    }
  
}
