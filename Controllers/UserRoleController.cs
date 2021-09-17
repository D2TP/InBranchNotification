using AutoMapper;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.Commands.UserRole;
 
using InBranchDashboard.DTOs;
using InBranchDashboard.Queries.ADUser.queries;
using InBranchDashboard.Queries.Roles;
using InBranchMgt.Commands.AdUser;
using InBranchMgt.Commands.AdUser.Handlers;
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
    public class UserRoleController : Controller
    {
             private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<UserRoleController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        public UserRoleController(ILogger<UserRoleController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpDelete("RemoveARolefromADUser")]
        public async Task<ActionResult> RemoveARolefromADUser(ADUserAndRold removeSingleRole)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {username} was not updated||Caller:UserRoleController/RemoveSingleRoleComand  || [UpdateADUserADUserHandler][Handle]");
                return BadRequest(ModelState);
            };
            try
            {

                await _commandDispatcher.SendAsync(removeSingleRole);


                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Succsessfuly Deleted!!!"),
                    ReasonPhrase = "AD User role id: " + removeSingleRole.RoleId + "was deleted,for the following user: " + removeSingleRole.AdUserId
                };
                response.Headers.Add("DeleteMessage", "Succsessfuly Deleted!!!");

                return StatusCode(StatusCodes.Status200OK, response);

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
                    {
                        Content = new StringContent(ex.InnerException.Message),
                        ReasonPhrase = ex.InnerException.Message

                    };
                    _logger.LogError("Server Error occured Role was not deleted for {id}||Caller:UserRoleController/RemoveARolefromADUser  || [RemoveSingleRoleComand][Handle] error:{error}", removeSingleRole.AdUserId, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured ADuser role was not deleted for {id}||Caller:UserRoleController/DeleteADUser  || [RemoveSingleRoleComand][Handle] error:{error}", removeSingleRole.AdUserId, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }


        [HttpPost("AddRoleToADUser")]
        public async Task<ActionResult> AddRoleToADUser([FromBody] CreateUserRoleComand command)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {username} role was no added ||Caller:UserRoleController/AddRoleToADUser  || [CreateUserRoleHandler][Handle]  ");
                return BadRequest(ModelState);
            }
          

            try
            {
                await _commandDispatcher.SendAsync(command);

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Succsessfuly Created!!!"),
                    ReasonPhrase = "AD User role id: " + command.RoleId + "was created,for the following user: " + command.ADUserId
                };
                response.Headers.Add("DeleteMessage", "Succsessfuly Added!!!");

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
                    {
                        Content = new StringContent(ex.InnerException.Message),
                        ReasonPhrase = ex.InnerException.Message

                    };
                    _logger.LogError("Server Error occured role was not created for {username}||Caller:UserRoleController/AddRoleToADUser  || [CreateUserRoleHandler][Handle] error:{error}", command.ADUserId, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured  role was not created for {username}||Caller:UserRoleController/AddRoleToADUser  || [CreateUserRoleHandler][Handle] error:{error}", command.ADUserId, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }

        [HttpGet("GellAllRoleUserIsAssinged")]
        public async Task<ActionResult<AdUserRoleQuery>> GellAllRoleUserIsAssinged(string adUserid)
        {
            //AD Login
            var adUserRoleDTO = new List<AdUserRoleDTO>();
            try
            {
                var adUserRoleQuery = new AdUserRoleQuery(adUserid);
                adUserRoleDTO = await _queryDispatcher.QueryAsync(adUserRoleQuery);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(ex.InnerException.Message),
                        ReasonPhrase = ex.InnerException.Message

                    };
                    _logger.LogError("Server Error occured while getting all ADUser ||Caller:ADUserController/getADUserbyId  || [GetOneADUserQuery][Handle] error:{error}", ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured while getting all ADUser||Caller:ADUserController/getADUserbyId  || [GetOneADUserQuery][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(adUserRoleDTO);

        }


      
    }
}
