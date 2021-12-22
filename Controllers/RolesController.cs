using AutoMapper;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.Commands.Roles;
using InBranchDashboard.Commands.UserRole;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using InBranchDashboard.Queries.ADUser.queries;
using InBranchDashboard.Queries.Roles;
using InBranchMgt.Commands.AdUser;
using InBranchMgt.Commands.AdUser.Handlers;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [Route("api/[controller]")]
    public class RolesController : Controller
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<RolesController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        public RolesController(ILogger<RolesController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet("GetAllRoles/{PageNumber}/{PageSize}")]
        public async Task<ActionResult<ObjectResponse>> GetAllRoles(int PageNumber, int PageSize)
        {
            var queryStringParameters = new QueryStringParameters
            {
                PageNumber = PageNumber,
                PageSize = PageSize
            };
            var role = new PagedList<Role>();
            var objectResponse = new ObjectResponse();
            try
            {
                var roleQueries = new RoleQueries(queryStringParameters);
                role = await _queryDispatcher.QueryAsync(roleQueries);

                objectResponse.Data = role;
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
                    _logger.LogError("[#Role001-1-C] Server Error occured while getting all Roles ||Caller:RolesController/GetAllRoles  || [RoleQueries][Handle] error:{error}", ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#Role001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }


                _logger.LogError("[#Role001-1-C] Server Error occured while getting all Roles ||Caller:RolesController/GetAllRoles  || [RoleQueries][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#Role001-1-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            var metadata = new
            {
                role.TotalCount,
                role.PageSize,
                role.CurrentPage,
                role.TotalPages,
                role.HasNext,
                role.HasPrevious
            };
            objectResponse.Message = new[] {   JsonConvert.SerializeObject(metadata) };  ;
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(objectResponse);
        }


        [HttpGet("GetRolesById/{id}")]
        public async Task<ActionResult<ObjectResponse>> GetRolesById(string id)
        {
            //AD Login

            var objectResponse = new ObjectResponse();
            var role = new Role();
            try
            {
                var RoleQuery = new RoleQuery(id);
                role = await _queryDispatcher.QueryAsync(RoleQuery);
                objectResponse.Success = true;
                objectResponse.Data = role;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(ex.InnerException.Message),
                        ReasonPhrase = ex.InnerException.Message
                        //[#Role001-1-C]
                    };
                    _logger.LogError("[#Role001-2-C] Server Error occured while getting all Role ||Caller:RolesController/GetRolesById  || [RoleQueries][Handle] error:{error}", ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#Role001-2-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#Role001-2-C] Server Error occured while getting all Role ||Caller:RolesController/GetRolesById  || [RoleQueries][Handle] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#Role002-2-C]", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }

        [HttpPost("CreateRole")]
        public async Task<ActionResult> CreateRole([FromBody] AddRoleDTO addRoleDto)
        {

            var objectResponse = new ObjectResponse();

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {Role} was not created ||Caller:RolesController/CreateRole  || [AddRoleHandler][Handle]  ", addRoleDto.role_name);
                return BadRequest(ModelState);
            }

            var command = new RoleCommand
            {
                role_name = addRoleDto.role_name,
                category_id = addRoleDto.category_id
            };
            try
            {

                await _commandDispatcher.SendAsync(command);

                

                objectResponse.Success = true;

                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetRolesById), new { id = command.id }, objectResponse);
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

                    objectResponse.Error = new[] { "[#Role003-3-C]", ex.InnerException.Message };
                    _logger.LogError("[#Role003-3-C] Server Error occured Role was not created for {Role}||Caller:RolesController/CreateRole  || [AddRoleHandler][Handle] error:{error}", command.role_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }

                _logger.LogError("[#Role003-3-C] Server Error occured Role was not created for {Role}||Caller:RolesController/CreateRole  || [AddRoleHandler][Handle] error:{error}", command.role_name, ex.Message);
                objectResponse.Error = new[] { "[#Role003-3-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }

        [HttpPut("UpdateRole")]
        public async Task<ActionResult> UpdateRole(UpdateRoleCommand command)
        {
            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid)
            {

                _logger.LogError("[#Role002-4-C] Validation error {Role} was not upadated ||Caller:RolesController/CreateRole  || [UpdateRoleHandler][Handle]  ", command.id);
                return BadRequest(ModelState);

            }


            try
            {

                await _commandDispatcher.SendAsync(command);
                objectResponse.Success = true;
                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetRolesById), new { id = command.id }, objectResponse);
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
                    _logger.LogError("[#Role004-4-C] Server Error occured Role was not updated for {Role}||Caller:RolesController/UpdateRole  || [UpdateRoleHandler][Handle] error:{error}", command.role_name, ex.InnerException.Message);
                    objectResponse.Error = new[] { "[#Role004-4-C]", ex.InnerException.Message };
                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

                }
                _logger.LogError("[#Role004-4-C] Server Error occured Role was not updated for {Role}||Caller:RolesController/UpdateRole  || [UpdateRoleHandler][Handle] error:{error}", command.role_name, ex.Message);
                objectResponse.Error = new[] { "[#Role004-4-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }

        [HttpDelete("DeleteRole/{id}")]
 
        public async Task<ActionResult> DeleteRole(string id)
        {
            var objectResponse = new ObjectResponse();
            if (id == string.Empty)
            {
                _logger.LogError(" [#InbPriv5-5-C] Validation error {Role} was not deleted ||Caller:RolesController/DeleteRole  || [DeleteRoleHandler][Handle]  ", id);
                return BadRequest(ModelState);
            }
            var command = new RoleDeleteComand
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
                    _logger.LogError("[#Role004-4-C] Server Error occured Role was not updated for {Role}||Caller:RolesController/UpdateRole  || [UpdateRoleHandler][Handle] error:{error}", command.role_name, ex.InnerException.Message);

                    objectResponse.Error = new[] { "[##Role004-4-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#Role004-4-C] Server Error occured Role was not updated for {Role}||Caller:RolesController/UpdateRole  || [UpdateRoleHandler][Handle] error:{error}", command.role_name, ex.Message);
                objectResponse.Error = new[] { "[##Role004-4-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }
    }
}