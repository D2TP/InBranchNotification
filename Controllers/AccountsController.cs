using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Queries.ADLogin.queries;
using InBranchDashboard.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace InBranchDashboard.Controllers
{
    public class AccountsController : Controller
    {
        //private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<AccountsController> _logger;
        public readonly IErrorList _errorList;
        public AccountsController(ILogger<AccountsController> logger, IErrorList errorList, IQueryDispatcher queryDispatcher)
        {
         //   _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _errorList = errorList;
        }
        [HttpPost("login")]
        public async Task<ActionResult<ObjectResponse>> Login([FromBody] LoginWithAdQuery loginDto)
        {
            //AD Login
            var objectResponse = new ObjectResponse();
            try
            {
                objectResponse = await _queryDispatcher.QueryAsync(loginDto);
            }
            catch (Exception ex)
            {
                objectResponse.Success = false;
                if (ex.InnerException != null)
                {

                    objectResponse.Error = _errorList.AddError(ex.InnerException.Message).ToArray();
                }
                else
                {
                    //_errorList.AddError(ex.Message);
                    objectResponse.Error = _errorList.AddError(ex.Message).ToArray();
                }
                return StatusCode(StatusCodes.Status401Unauthorized, objectResponse);
            }

            return Ok(objectResponse);

        }


    }
}
