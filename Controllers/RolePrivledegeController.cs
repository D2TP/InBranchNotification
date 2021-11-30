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


        [HttpGet("GellAllRolePrivledege/{PageNumber}/{PageSize}")]
   
        public async Task<ActionResult<ObjectResponse>> GellAllRoleUserIsAssinged(int PageNumber, int PageSize)
        {
            var queryStringParameters = new QueryStringParameters
            {
                PageSize = PageSize,
                PageNumber = PageNumber,
            };

            var rolePriviledgeDTO = new PagedList<RolePriviledgeDTO>();
            var objectResponse = new ObjectResponse();
            try
            {
                var rolePriviledgeQueries = new RolePriviledgeQueries(queryStringParameters);
                rolePriviledgeDTO = await _queryDispatcher.QueryAsync(rolePriviledgeQueries);

                objectResponse.Data = rolePriviledgeDTO;
                objectResponse.Success = true;
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
                    _logger.LogError("[#RolePriv001-1-C] Server Error occured while getting all ADUser ||Caller:ADUserController/getADUserbyId  || [GetOneADUserQuery][Handle] error:{error}", ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#RolePriv001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }


                _logger.LogError("[#RolePriv001-1-C] Server Error occured while getting all ADUser||Caller:ADUserController/getADUserbyId  || [GetOneADUserQuery][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#RolePriv001-1-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
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
            objectResponse.Message = new[] { "X-Pagination" + JsonConvert.SerializeObject(metadata) }; ;
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(objectResponse);
        }

        [HttpDelete("RemoveRolePrivledege/{id}")]
     
        public async Task<ActionResult> RemoveARolePrivledege(string id)
        {
            var objectResponse = new ObjectResponse();
            if (id == string.Empty)
            {
                _logger.LogError("[#RolePriv002-2-C] Validation error {Permission} was not deleted ||Caller:PermissionController/CreatePermission  || [SingleCategorHandler][Handle]  ", id);
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
                    _logger.LogError("[#RolePriv002-2-C] Server Error occured Permission was not deleted   {Permission}||Caller:PermissionController/DeletePermission  || [DeletePermissionHandler][Handle] error:{error}", command.id, ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#RolePriv002-2-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#RolePriv002-2-C] Server Error occured Permission was not deleted   {Permission}||Caller:PermissionController/DeletePermission  || [DeletePermissionHandler][Handle] error:{error}", command.id, ex.Message);
                objectResponse.Error = new[] { "[#RolePriv002-2-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }



        [HttpPost("AddRolePrivledege")]
        public async Task<ActionResult> AddRolePrivledege([FromBody] AddRolePriviledgeDTO addRolePriviledgeDTO)
        {

            var objectResponse = new ObjectResponse();
          if (!ModelState.IsValid)
            {
                _logger.LogError("[#RolePriv003-3-C] Validation error {username} role was no added ||Caller:UserRoleController/AddRoleToADUser  || [CreateUserRoleHandler][Handle]  ");
                return BadRequest(ModelState);
            }

            var command = new RolePrivledegeCommand
            {
                permission_id = addRolePriviledgeDTO.permission_id,
                priviledge_id = addRolePriviledgeDTO.priviledge_id,
                role_id = addRolePriviledgeDTO.role_id
            };
            try
            {

                await _commandDispatcher.SendAsync(command);


                objectResponse.Success = true;

                objectResponse.Data = new { id = command.id };
                return StatusCode(StatusCodes.Status200OK, objectResponse);
              
            }
            catch (Exception ex)
            {
                objectResponse.Success = false;
                if (ex.InnerException != null)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
                    {
                        Content = new StringContent(ex.InnerException.Message),
                        ReasonPhrase = ex.InnerException.Message

                    };

                    objectResponse.Error = new[] { "[#RolePriv003-3-C]", ex.InnerException.Message };
                    _logger.LogError("[#RolePriv003-3-C] Server Error occured role was not created for {username}||Caller:UserRoleController/AddRoleToADUser  || [CreateUserRoleHandler][Handle] error:{error}", command.id, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }

                _logger.LogError("[#RolePriv003-3-C] Server Error occured  role was not created for {username}||Caller:UserRoleController/AddRoleToADUser  || [CreateUserRoleHandler][Handle] error:{error}", command.id, ex.Message);
                objectResponse.Error = new[] { "[#RolePriv003-3-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }


    }
}
