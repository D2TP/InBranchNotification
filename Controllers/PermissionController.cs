using AutoMapper;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.Commands.Permissions;
using InBranchDashboard.Commands.UserRole;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using InBranchDashboard.Queries.ADUser.queries;
using InBranchDashboard.Queries.Permissions;
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
    public class PermissionController : Controller
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<PermissionController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        public PermissionController(ILogger<PermissionController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("GetAllPermission")]
        public async Task<ActionResult<List<Permission>>> GetAllPermission([FromQuery] QueryStringParameters queryStringParameters)
        {
            //AD Login
            var permissions = new PagedList<Permission>();
            try
            {
                var PermissionQueries = new PermissionQueries(queryStringParameters);
                permissions = await _queryDispatcher.QueryAsync(PermissionQueries);
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
                    _logger.LogError("Server Error occured while getting all Permission ||Caller:PermissionController/GetAllPermission  || [PermissionQueries][Handle] error:{error}", ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured while getting all ADUser||Caller:PermissionController/GetAllPermission  || [PermissionQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            var metadata = new
            {
                permissions.TotalCount,
                permissions.PageSize,
                permissions.CurrentPage,
                permissions.TotalPages,
                permissions.HasNext,
                permissions.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(permissions);

        }

        [HttpGet("GetPermissionById")]
        public async Task<ActionResult<PermissionDTO>> GetPermissionById(string id)
        {
            //AD Login
            var permission = new Permission();
            try
            {
                var PermissionQuery = new PermissionQuery(id);
                permission = await _queryDispatcher.QueryAsync(PermissionQuery);
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
                    _logger.LogError("Server Error occured while getting all Permission ||Caller:PermissionController/GetPermissionById  || [PermissionQueries][Handle] error:{error}", ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured while getting all Permission||Caller:PermissionController/GetPermissionById  || [PermissionQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(permission);

        }


        [HttpPost("CreatePermission")]
        public async Task<ActionResult> CreatePermission([FromBody] PermissionDTO permissionDTO)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {Permission} was not created ||Caller:PermissionController/CreatePermission  || [SingleCategorHandler][Handle]  ", permissionDTO.permission_name);
                return BadRequest(ModelState);
            }

            var command = new PermissionCommand
            {
                permission_name = permissionDTO.permission_name
            };
            try
            {

                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetPermissionById), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured Permission was not created for {Permission}||Caller:PermissionController/CreatePermission  || [SingleCategorHandler][Handle] error:{error}", command.permission_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Permission was not created for {Permission}||Caller:PermissionController/CreatePermission  || [SingleCategorHandler][Handle] error:{error}", command.permission_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }



        [HttpPut("UpdatePermission")]
        public async Task<ActionResult> UpdatePermission(UpdatePermissionCommand command)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {Permission} was not upadated ||Caller:PermissionController/CreatePermission  || [SingleCategorHandler][Handle]  ", command.id);
                return BadRequest(ModelState);
            }


            try
            {

                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetPermissionById), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured Permission was not updated for {Permission}||Caller:PermissionController/UpdatePermission  || [UpdatePermissionHandler][Handle] error:{error}", command.permission_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Permission was not updated for {Permission}||Caller:PermissionController/UpdatePermission  || [UpdatePermissionHandler][Handle] error:{error}", command.permission_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }


        [HttpDelete("DeletePermission")]
        public async Task<ActionResult> DeletePermission(string id)
        {

            if (id == string.Empty)
            {
                _logger.LogError("Validation error {Permission} was not deleted ||Caller:PermissionController/CreatePermission  || [SingleCategorHandler][Handle]  ", id);
                return BadRequest(ModelState);
            }
            var command = new PermissionDeleteComand
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
                    _logger.LogError("Server Error occured Permission was not deleted   {Permission}||Caller:PermissionController/DeletePermission  || [DeletePermissionHandler][Handle] error:{error}", command.permission_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Permission was not deleted   {Permission}||Caller:PermissionController/DeletePermission  || [DeletePermissionHandler][Handle] error:{error}", command.permission_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }
    }
}
