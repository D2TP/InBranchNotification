
using DbFactory;
using InBranchNotification.Domain;
using InBranchNotification.Exceptions;
using InBranchNotification.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
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
        public class JwtHolder
        {
            public string id { get; set; }
            public string user_name { get; set; }
            public string token { get; set; }
            public DateTime? expires_on { get; set; }
            public DateTime? created_on { get; }
            public DateTime? revoked_on { get; set; }
            public bool active { get; set; }
            public string refreshToken { get; set; }


        }
        // private readonly IBaseUrlService _baseUrlService;
        public AuthenticationMiddleware(RequestDelegate next, IConfiguration config, ILogger<AuthenticationMiddleware> logger)
        {
            _config = config;
            _next = next;
            _logger = logger;
            // _baseUrlService = baseUrlService;
        }

        public async Task Invoke(HttpContext context, IBaseUrlService baseUrlService, IAuthenticateRestClient authenticateRestClient, IDbController dbController)
        {
            //Reading the AuthHeader which is signed with JWT

            string authHeader = !string.IsNullOrEmpty(context.Request.Headers["Authorization"]) ? context.Request.Headers["Authorization"] : "";

            if (AuthenticationHeaderValue.TryParse(authHeader, out var headerValue))
            {
                // we have a valid AuthenticationHeaderValue that has the following details:

                var scheme = headerValue.Scheme;
                var parameter = headerValue.Parameter;


                // parmameter will be the token itself.
            }


            var getAllClaims = headerValue != null ? await ValidateToken(headerValue.Parameter) : null;


            if (getAllClaims != null)
            {
                // var verifyToken = await authenticateRestClient.VerifyToken(headerValue.Parameter);
                var verifyToken = await VerifyToken(headerValue.Parameter, dbController);
                if (verifyToken != null)
                {
                    if (!verifyToken.Success)
                    {
                        _logger.LogError("Error: There is no user with {User Id} |Caller:InBranchDashboard/AuthenticationMidleWare || [Invoke][Handle]");
                        throw new HandleGeneralException(404, "Token not valid. ");
                    }
                }
                var identity = (ClaimsIdentity)getAllClaims.Identity;
                IEnumerable<Claim> claims = identity.Claims;
                var displayName = claims.First(x => x.Type == "DisplayName").Value;

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


        public Task<IPrincipal> ValidateToken(string token)
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

        private ClaimsPrincipal GetPrincipal(string token)
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
            catch (Exception ex)
            {
                var j = ex.Message;
                return null;
            }
        }



        public async Task<ObjectResponse> VerifyToken(string token, IDbController dbController)
        {
            ObjectResponse objectResponse = new ObjectResponse();
            objectResponse.Success = false;
            bool active = false;
            string revoked_on_String = string.Empty;
            DateTime revoked_on = new();
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    objectResponse.Success = false;
                    objectResponse.Message = new[] { "Failed", "Invalid request. Missing header authorization" };
                    return objectResponse;
                }
                var principal = DecodeJWTAndGetPrincipal(token);
                var userJwt = new JwtHolder();
                userJwt.user_name = principal.Claims.First(claim => claim.Type == "UserName").Value;
                userJwt.token = token;
                //  userJwt.refreshToken = tokenViewModel.RefreshToken;
                userJwt.expires_on = Epoch2Date(Int64.Parse(principal.Claims.First(claim => claim.Type == "exp").Value));

                if (userJwt is null)
                {
                    objectResponse.Success = false;
                    objectResponse.Message = new[] { "Failed", "Bearer token not valid" };

                    return objectResponse;
                }

                if (userJwt.expires_on < DateTime.Now)
                {
                    objectResponse.Success = false;
                    objectResponse.Message = new[] { "Failed", "Bearer token has expired." };

                    return objectResponse;
                }

                //if (!string.Equals(userJwt.IpAddress, ipAddress, StringComparison.OrdinalIgnoreCase))
                //{
                //    objectResponse.Success = false;
                //    objectResponse.Message = new[] { "Failed", "Fraudulent calls." };

                //    return objectResponse;
                //}


                object[] param = { userJwt.token };
                var entity = await dbController.SQLFetchAsync(Sql.SelectToken, param);
                if (entity.Rows.Count == 0)
                {
                    _logger.LogError("[#InBranchAUTH002-1-C] Error: There is no user with {User Id} |Caller:ADUserController/GetAnDUsers-Get|| [CreateOneADUserHandler][Handle]", "VerifyToken");
                    throw new HandleGeneralException(404, "User does not exist");
                }
                else
                {
                    foreach (DataRow i in entity.Rows)
                    {
                        active = Convert.ToBoolean(i["active"]);
                        revoked_on_String = i["revoked_on"].ToString();
                    }
                }

                if (!string.IsNullOrWhiteSpace(revoked_on_String))
                {
                    revoked_on = Convert.ToDateTime(revoked_on_String);

                    if (revoked_on <= DateTime.Now)
                    {
                        objectResponse.Success = false;
                        objectResponse.Message = new[] { "Failed", "Token not valid." };

                        return objectResponse;
                    }
                }

                if (!active)
                {
                    objectResponse.Success = false;
                    objectResponse.Message = new[] { "Failed", "Token has expired." };

                    return objectResponse;
                }

                objectResponse.Success = true;
                objectResponse.Message = new[] { "Success", "Token is valid." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[#AUT013] Verify token [AuthenticationService][VerifyToken]");
                objectResponse.Success = false;
            }
            return objectResponse;
        }


        private DateTime? Epoch2Date(Int64 epoch)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch);
            dateTime = dateTime.ToLocalTime();

            return dateTime;
        }

        public ClaimsPrincipal DecodeJWTAndGetPrincipal(string token)
        {



            var stream = token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokens = jsonToken as JwtSecurityToken;

            var tokenHandler = new JwtSecurityTokenHandler();
            // var jti = tokenS.Claims.First(claim => claim.Type == "jti").Value;
            var principal = new ClaimsPrincipal(new ClaimsIdentity(tokens.Claims));


            return principal;

        }
    }
}
