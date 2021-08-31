using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.DTOs;
using InBranchDashboard.Queries.ADLogin.queries;
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
        public AccountsController(ILogger<AccountsController> logger,  IQueryDispatcher queryDispatcher)
        {
         //   _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
        }
        [HttpPost("login")]
        public async Task<ActionResult<ADUserDTO>> Login([FromBody] LoginWithAdQuery loginDto)
        {
            //AD Login
            var aDUserDTO = new ADUserDTO();
            try
            {
                aDUserDTO = await _queryDispatcher.QueryAsync(loginDto);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent( ex.InnerException.Message),
                    ReasonPhrase = ex.InnerException.Message
                    
                };

                return StatusCode(StatusCodes.Status401Unauthorized, resp);
            }

            return Ok(aDUserDTO);

        }
    }
}
