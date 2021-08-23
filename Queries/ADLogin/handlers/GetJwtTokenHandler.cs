using Convey.CQRS.Queries;
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

namespace InBranchDashboard.Queries.ADLogin.handlers
{
    public class GetJwtTokenHandler : IQueryHandler<GetJwtTokenQuery, string>
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly ILogger<GetJwtTokenHandler> _logger;
        public GetJwtTokenHandler(IConfiguration config, ILogger<GetJwtTokenHandler> logger)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"]));
            _logger = logger;
        }

        public Task<string> HandleAsync(GetJwtTokenQuery query)
        {
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
