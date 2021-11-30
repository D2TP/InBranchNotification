using Convey.CQRS.Queries;
using DbFactory;
using InBranchDashboard.DbFactory;
using InBranchDashboard.Domain;
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
    public class LoginWithAdHandler : IQueryHandler<LoginWithAdQuery, ObjectResponse>
    {
        private readonly IDbController _dbController;
        private readonly ILogger<LoginWithAdHandler> _logger;
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;
        private readonly IAuthenticateRestClient _authenticateRestClient;
        public readonly IValidateService _validateService;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
        public LoginWithAdHandler(ILogger<LoginWithAdHandler> logger, IDbController dbController, IConfiguration config, ITokenService tokenService, IValidateService validateService, IAuthenticateRestClient authenticateRestClient, IConvertDataTableToObject convertDataTableToObject)
        {
            _config = config;
            _logger = logger;
            _tokenService = tokenService;
            _validateService = validateService;
            _authenticateRestClient = authenticateRestClient;
            _dbController = dbController;
            _convertDataTableToObject = convertDataTableToObject;
        }



        public async Task<ObjectResponse> HandleAsync(LoginWithAdQuery query)
        {
            var validationErrorLsit = string.Empty;
            if (!_validateService.IsString(query.UserName))
            {
                validationErrorLsit += "Invalid string. ";
            }
            if (!_validateService.IsString(query.Password))
            {
                validationErrorLsit += "Invalid password string. ";
            }

            if (validationErrorLsit != string.Empty)
            {
                _logger.LogWarning("Validation error --> username: {request.UserName} || [LoginWithAdHandler][Handle]", validationErrorLsit);
                throw new HandleGeneralException(401, "Failed the following validation : " + validationErrorLsit);
            }
            var adPassed = false;

            //  var _authenticateRestClient = new AuthenticateRestClient();
            var validateADuser = _authenticateRestClient.CheckXtradotAdUser(query);
            if (!validateADuser)
            {
                _logger.LogWarning("Failed User name and Password --> username: {request.UserName} || [LoginWithAdHandler][Handle]", query.UserName);
                throw new HandleGeneralException(401, "Failed User name and Password");
            }
            //Get domain for ad details
            var xtradotGetDomains = _authenticateRestClient.XtradotGetDomains();
            adPassed = true;
            if (adPassed == false)
            {
                _logger.LogWarning("Failed to retrive Active Directory details contact admin --> username: {request.UserName} || [LoginWithAdHandler][Handle]", query.UserName);
                //throw
                throw new HandleGeneralException(401, "Failed to retrive Active Directory details contact System Admin");
            }

            //getting detail from AD LDAP service not available yet

            // use method below for registration 
            //var xtradotAdUserDetails=  _authenticateRestClient.GetXtradotAdUserDetails(query.UserName, query.Domanin);
            //if (xtradotAdUserDetails != null) { 

            //// when not null get values from reg 
            //};


            object[] param = { query.UserName };
            var entity = await _dbController.SQLFetchAsync(Sql.SelectADUserAndBranchName, param);
            if (entity.Rows.Count == 0)
            {
                _logger.LogError("Error: There is no user with {User Id} |Caller:ADUserController/GetAnDUsers-Get|| [CreateOneADUserHandler][Handle]", query.UserName);
                throw new HandleGeneralException(404, "User does not exist");
            }
            ADCreateCommandDTO aDCreateCommandDTO = new ADCreateCommandDTO();
            aDCreateCommandDTO = _convertDataTableToObject.ConvertDataTable<ADCreateCommandDTO>(entity).FirstOrDefault();
            var objectResponse = new ObjectResponse();
            var aDUserDTO = new ADUserDTORespons
            {
                UserName = aDCreateCommandDTO.user_name,
                DisplayName = aDCreateCommandDTO.DisplayName,
                Email = aDCreateCommandDTO.email,
                FirstName = aDCreateCommandDTO.first_name,
                LastNmae = aDCreateCommandDTO.last_name,
                BranchName = aDCreateCommandDTO.branch_name,
                Token = string.Empty
            };
            objectResponse.Data = aDUserDTO;
            objectResponse.Success = true;

            if (aDUserDTO == null)
            {
                _logger.LogWarning("Failed to retrive Active Directory details contact admin --> username: {request.UserName} || [LoginWithAdHandler][Handle]", query.UserName);
                //throw
                throw new HandleGeneralException(401, "Failed to retrive Active Directory details contact System Admin");
            }
            var token = _tokenService.GetToken(aDUserDTO);
            aDUserDTO.Token = token.Result;
            return objectResponse;



        }
    }
}
