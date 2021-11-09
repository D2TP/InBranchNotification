using AutoMapper;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.Commands.RolePriviledges;
using InBranchDashboard.Commands.UserRole;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using InBranchDashboard.Queries.ADUser.queries;
using InBranchDashboard.Queries.RolePriviledges;
using InBranchDashboard.Queries.Roles;
using InBranchMgt.Commands.AdUser;
using InBranchMgt.Commands.AdUser.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace InBranchDashboard.Controllers
{
    [Route("api/[controller]")]
    public class RolePrivledegeController : Controller
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<RolePrivledegeController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        public RolePrivledegeController(ILogger<RolePrivledegeController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpDelete("RemoveRolePrivledege/{id}")]
        public async Task<ActionResult> RemoveARolePrivledege(string id)
        {

            if (id == string.Empty)
            {
                _logger.LogError("Validation error {Permission} was not deleted ||Caller:PermissionController/CreatePermission  || [SingleCategorHandler][Handle]  ", id);
                return BadRequest(ModelState);
            }
            var command = new RolePriviledgeDeleteCommand
            {
                id = id
            };

            try
            {
                await _commandDispatcher.SendAsync(command);

                return StatusCode(StatusCodes.Status204NoContent);
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
                    _logger.LogError("Server Error occured Permission was not deleted   {Permission}||Caller:PermissionController/DeletePermission  || [DeletePermissionHandler][Handle] error:{error}", command.id, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Permission was not deleted   {Permission}||Caller:PermissionController/DeletePermission  || [DeletePermissionHandler][Handle] error:{error}", command.id, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }


            [HttpPost("AddRolePrivledege")]
        public async Task<ActionResult> AddRolePrivledege([FromBody] AddRolePriviledgeDTO addRolePriviledgeDTO)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {username} role was no added ||Caller:UserRoleController/AddRoleToADUser  || [CreateUserRoleHandler][Handle]  ");
                return BadRequest(ModelState);
            }

            var command = new RolePrivledegeCommand
            {
                permission_id = addRolePriviledgeDTO.permission_id,
                priviledge_id = addRolePriviledgeDTO.priviledge_id,
                role_id=addRolePriviledgeDTO.role_id
            };
            try
            {
                await _commandDispatcher.SendAsync(command);

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Succsessfuly Created!!!"),
                    ReasonPhrase = "Priviledge with the following Id : " + command.id + "was created. "  
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
                    _logger.LogError("Server Error occured role was not created for {username}||Caller:UserRoleController/AddRoleToADUser  || [CreateUserRoleHandler][Handle] error:{error}", command.id, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured  role was not created for {username}||Caller:UserRoleController/AddRoleToADUser  || [CreateUserRoleHandler][Handle] error:{error}", command.id, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }

        [HttpGet("GellAllRolePrivledege/{PageNumber}/{PageSize}")]
        public async Task<ActionResult<AdUserRoleListQuery>> GellAllRoleUserIsAssinged(int PageNumber, int PageSize)
        {
            //AD Login
            var queryStringParameters = new QueryStringParameters
            {
                PageSize = PageSize,
                PageNumber = PageNumber,
            };

            var rolePriviledgeDTO = new PagedList<RolePriviledgeDTO>();
            try
            {
                var rolePriviledgeQueries = new RolePriviledgeQueries(queryStringParameters);
                 rolePriviledgeDTO = await _queryDispatcher.QueryAsync(rolePriviledgeQueries);
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


            var metadata = new
            {
                rolePriviledgeDTO.TotalCount,
                rolePriviledgeDTO.PageSize,
                rolePriviledgeDTO.CurrentPage,
                rolePriviledgeDTO.TotalPages,
                rolePriviledgeDTO.HasNext,
                rolePriviledgeDTO.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(rolePriviledgeDTO);

        }



    }
}
