using Convey.CQRS.Queries;
using InBranchDashboard.DTOs;
using InBranchDashboard.Exceptions;
using InBranchDashboard.Queries.ADLogin.queries;
using InBranchDashboard.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.ADLogin.handlers
{
    public class LoginWithAdHandler : IQueryHandler<LoginWithAdQuery, ADUserDTO>
    {
        private readonly ILogger<LoginWithAdHandler> _logger;
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;
        public readonly IValidateService _validateService;
        public LoginWithAdHandler(ILogger<LoginWithAdHandler> logger, IConfiguration config, ITokenService tokenService, IValidateService validateService)
        {
            _config = config;
            _logger = logger;
            _tokenService = tokenService;
            _validateService = validateService;
        }



        public Task<ADUserDTO> HandleAsync(LoginWithAdQuery query)
        { 
            var validationErrorLsit = string.Empty;
           if(!_validateService.IsString(query.UserName))
            {
                validationErrorLsit+="Invalid string. ";
            }
            if (!_validateService.IsString(query.Password))
            {
                validationErrorLsit += "Invalid password string. ";
            }

            if (validationErrorLsit!=string.Empty)
            {
                _logger.LogWarning("Validation error --> username: {request.UserName} || [LoginWithAdHandler][Handle]", validationErrorLsit);
                throw new HandleGeneralException(401, "Failed the following validation : "+ validationErrorLsit);
            }
            var adPassed = false;

            string userNamePass = "springo";
            //User name password fail
            if (query.UserName != userNamePass)
            {
                _logger.LogWarning("Failed User name and Password --> username: {request.UserName} || [LoginWithAdHandler][Handle]", query.UserName);
                throw new HandleGeneralException(401, "Failed User name and Password");
            }
            //Check AD first here
            adPassed = true;
            if (adPassed == false)
            {
                _logger.LogWarning("Failed to retrive Active Directory details contact admin --> username: {request.UserName} || [LoginWithAdHandler][Handle]", query.UserName);
                //throw
                throw new HandleGeneralException(401, "Failed to retrive Active Directory details contact System Admin");
            }

            //getting detail from AD LDAP service not available yet
            var aDUserDTO = new ADUserDTO
            {
                UserName = query.UserName,
                DisplayName = "Test User",
                Email = "test.use@Fbn.com",
                FirstName = "Test",
                LastNmae = "User",
                BranchId=1,
                Token = string.Empty
            };
            if (aDUserDTO == null)
            {
                _logger.LogWarning("Failed to retrive Active Directory details contact admin --> username: {request.UserName} || [LoginWithAdHandler][Handle]", query.UserName);
                //throw
                throw new HandleGeneralException(401, "Failed to retrive Active Directory details contact System Admin");
            }
            var token = _tokenService.GetToken(aDUserDTO);
            aDUserDTO.Token = token.Result;
            return Task.FromResult(aDUserDTO);



        }
    }
}
