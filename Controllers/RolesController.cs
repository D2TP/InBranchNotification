using AutoMapper;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.Commands.Roles;
using InBranchDashboard.Commands.UserRole;
using InBranchDashboard.Domain;
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
        [HttpGet("GetAllRoles")]
        public async Task<ActionResult<List<Role>>> GetAllRoles()
        {
            //AD Login
            var Role = new List<Role>();
            try
            {
                var roleQueries = new RoleQueries();
                Role = await _queryDispatcher.QueryAsync(roleQueries);
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
                    _logger.LogError("Server Error occured while getting all Roles ||Caller:RolesController/GetAllRoles  || [RoleQueries][Handle] error:{error}", ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured while getting all ADUser||Caller:RolesController/GetAllRoles  || [RoleQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(Role);

        }

        [HttpGet("GetRolesById")]
        public async Task<ActionResult<Role>> GetRolesById(string id)
        {
            //AD Login
            var Role = new Role();
            try
            {
                var RoleQuery = new RoleQuery(id);
                Role = await _queryDispatcher.QueryAsync(RoleQuery);
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
                    _logger.LogError("Server Error occured while getting all Role ||Caller:RolesController/GetRolesById  || [RoleQueries][Handle] error:{error}", ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured while getting all Role||Caller:RolesController/GetRolesById  || [RoleQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(Role);

        }


        [HttpPost("CreateRole")]
        public async Task<ActionResult> CreateRole([FromBody] AddRoleDTO addRoleDto)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {Role} was not created ||Caller:RolesController/CreateRole  || [AddRoleHandler][Handle]  ", addRoleDto.role_name);
                return BadRequest(ModelState);
            }

            var command = new RoleCommand
            {
                role_name = addRoleDto.role_name,
                category_id=addRoleDto.category_id
            };
            try
            {

                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetRolesById), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured Role was not created for {Role}||Caller:RolesController/CreateRole  || [AddRoleHandler][Handle] error:{error}", command.role_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Role was not created for {Role}||Caller:RolesController/CreateRole  || [AddRoleHandler][Handle] error:{error}", command.role_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }



        [HttpPut("UpdateRole")]
        public async Task<ActionResult> UpdateRole(UpdateRoleCommand command)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {Role} was not upadated ||Caller:RolesController/CreateRole  || [UpdateRoleHandler][Handle]  ", command.id);
                return BadRequest(ModelState);
            }


            try
            {

                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetRolesById), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured Role was not updated for {Role}||Caller:RolesController/UpdateRole  || [UpdateRoleHandler][Handle] error:{error}", command.role_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Role was not updated for {Role}||Caller:RolesController/UpdateRole  || [UpdateRoleHandler][Handle] error:{error}", command.role_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }


        [HttpDelete("DeleteRole")]
        public async Task<ActionResult> DeleteRole(string id)
        {

            if (id == string.Empty)
            {
                _logger.LogError("Validation error {Role} was not deleted ||Caller:RolesController/DeleteRole  || [DeleteRoleHandler][Handle]  ", id);
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
                    _logger.LogError("Server Error occured Role was not deleted   {Role}||Caller:RolesController/DeleteRole  || [DeleteRoleHandler][Handle] error:{error}", command.role_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Role was not deleted   {Role}||Caller:RolesController/DeleteRole  || [DeleteRoleHandler][Handle] error:{error}", command.role_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }
    }
}