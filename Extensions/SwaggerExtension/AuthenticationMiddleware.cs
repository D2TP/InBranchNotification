
using InBranchNotification.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Swagger.Extension
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthenticationMiddleware> _logger;
        public AuthenticationMiddleware(RequestDelegate next, IConfiguration config,ILogger<AuthenticationMiddleware> logger)
        {
            _config = config;
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            //Reading the AuthHeader which is signed with JWT

            string authHeader = !string.IsNullOrEmpty(context.Request.Headers["Authorization"]) ? context.Request.Headers["Authorization"]:"";

            if (AuthenticationHeaderValue.TryParse(authHeader, out var headerValue))
            {
                // we have a valid AuthenticationHeaderValue that has the following details:

                var scheme = headerValue.Scheme;
                var parameter = headerValue.Parameter;

                // scheme will be "Bearer"
                // parmameter will be the token itself.
            }
           

            var getAllClaims = headerValue!=null? await  ValidateToken(headerValue.Parameter):null  ;
            if (getAllClaims != null)
            {
                var identity = (ClaimsIdentity)getAllClaims.Identity;
                IEnumerable<Claim> claims = identity.Claims;
                var displayName=claims.First(x => x.Type == "DisplayName").Value;

                var userName = claims.First(claim => claim.Type == "UserName").Value;
                var firstName = claims.First(claim => claim.Type == "FirstName").Value;
                var lastName = claims.First(claim => claim.Type == "LastName").Value;
                var branchName = claims.First(claim => claim.Type == "BranchName").Value;
                var email = claims.First(claim => claim.Type == "Email").Value;
                var appUser = claims.First(claim => claim.Type == ClaimTypes.Name).Value; // new Claim(ClaimTypes.Name, value: query.UserName),
                var userRole = claims.Where(c => c.Type == ClaimTypes.Role).ToList();

               

                    var claimPrincipal = new List<Claim>
            {
                new Claim(ClaimTypes.Email,email),
                new Claim(type: "UserName", value: userName),
                new Claim(ClaimTypes.GivenName, displayName),
                  new Claim(type: "DisplayName", value: displayName),
                    new Claim(type: "FirstName", value:  firstName),
                     new Claim(type: "LastName", value:  lastName),
                      new Claim(type: "BranchName", value:  branchName),
                         new Claim(ClaimTypes.Name,  value:appUser),
        };

                foreach (var role in userRole)
                {
                    claimPrincipal.Add(new Claim(ClaimTypes.Role, role.Value));

                };
                var identityPrincipal = new ClaimsIdentity(claimPrincipal, "basic");
                context.User = new ClaimsPrincipal(identityPrincipal);
            }
             
           
            //Pass to the next middleware
            await _next(context);
        }

         
        public  Task<IPrincipal> ValidateToken(string token)
        {
            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null)
                return Task.FromResult<IPrincipal>(null);
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
                IPrincipal Iprincipal = new ClaimsPrincipal(identity);
                return Task.FromResult(Iprincipal);
            }
            catch (NullReferenceException)
            {
                return Task.FromResult<IPrincipal>(null);
            }
        }

        private   ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                var checkconfkey = _config["jwt:secretKey"];
                var checkIssues = _config["jwt:Issuer"];
                byte[] key = Encoding.UTF8.GetBytes(_config["jwt:secretKey"]);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    ValidIssuer = _config["jwt:Issuer"], 
                  //  ValidAudience = _config["Token:Issuer"],
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);
                return principal;
            }
            catch(Exception ex)
            {
                var j = ex.Message;
                return null;
            }
        }
    }
}