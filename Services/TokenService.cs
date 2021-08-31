
using InBranchDashboard.DTOs;
using InBranchDashboard.Queries.ADLogin.queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InBranchDashboard.Services
{
    public class TokenService : ITokenService
    {

        private readonly IConfiguration _config;


        public TokenService(IConfiguration config)
        {
            _config = config;


        }

        public Task<string> GetToken(ADUserDTO query)
        {

            var _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"]));
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, query.Email),
                new Claim(type: "UserName", value: query.UserName),
                new Claim(ClaimTypes.GivenName, query.DisplayName)
        };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["Token:Issuer"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token));
        }
    }
}
 
