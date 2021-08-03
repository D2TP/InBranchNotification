using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.Auth;
using Convey.CQRS.Queries;

namespace Convey.Test.Accounts.Queries.handlers
{
  
    public class GetJwtTokenHandler : IQueryHandler<GetJwtToken, string>
    {

        private readonly IJwtHandler _jwtHandler;

        public GetJwtTokenHandler(IJwtHandler jwtHandler)
        {
            _jwtHandler = jwtHandler;
        }


        public async Task<string> HandleAsync(GetJwtToken query)
        {
            var claims = new { sub = "1234567890", name = "John Doe", admin = true };
            var user = new { Id = "1233", Role = "User", Claims = claims };
            //ValidateCredentials(user, password); //Validate the credentials etc.

            //Generate the token with an optional role and other claims
            var token = _jwtHandler.CreateToken(user.Id, user.Role, user.Claims.ToString());
           

            //generate refresh token as desired and store 
            return await Task.FromResult(token.AccessToken); ;
        }
    }
}
