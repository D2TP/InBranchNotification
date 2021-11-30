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
    [Route("api/[controller]")]
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

        [HttpGet("GetAllPermission/{PageNumber}/{PageSize}")]
        public async Task<ActionResult<ObjectResponse>> GetAllPermission(int PageNumber, int PageSize)
        {
            var queryStringParameters = new QueryStringParameters
            {
                PageNumber = PageNumber,
                PageSize = PageSize

            };
            var permissions = new PagedList<Permission>();
            var objectResponse = new ObjectResponse();
            try
            {
                var PermissionQueries = new PermissionQueries(queryStringParameters);
                permissions = await _queryDispatcher.QueryAsync(PermissionQueries);


                objectResponse.Data = permissions;
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
                    _logger.LogError("Server Error occured while getting all Permission ||Caller:PermissionController/GetAllPermission  || [PermissionQueries][Handle] error:{error}", ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#InbPerm001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }


                _logger.LogError(" Server Error occured while getting all ADUser||Caller:PermissionController/GetAllPermission  || [PermissionQueries][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#InbPerm001-1-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
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
            objectResponse.Message = new[] { "X-Pagination" + JsonConvert.SerializeObject(metadata) }; ;
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(objectResponse);
        }

        [HttpGet("GetPermissionById/{id}")]
        public async Task<ActionResult<ObjectResponse>> GetPermissionById(string id)
        {
            //AD Login
            var branch = new Branch();
            var objectResponse = new ObjectResponse();
            try
            {
                var permissionQuery = new PermissionQuery(id);
                objectResponse.Data = await _queryDispatcher.QueryAsync(permissionQuery);
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
                    _logger.LogError("[InbPerm2-1-C Server Error occured while getting all Permission ||Caller:PermissionController/GetPermissionById  || [PermissionQueries][Handle] error:{error}", ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#InbPerm2-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#InbPerm2-1-C] Server Error occured while getting all Permission||Caller:PermissionController/GetPermissionById  || [PermissionQueries][Handle] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#InbPerm2-1-C]", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }

        [HttpPost("CreatePermission")]
    
        public async Task<ActionResult> CreatePermission([FromBody] PermissionDTO permissionDTO)
        {

            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid)
            {
                _logger.LogError("[#InbPerm3-3-C] Validation error {Permission} was not created ||Caller:PermissionController/CreatePermission  || [SingleCategorHandler][Handle]  ", permissionDTO.permission_name);
                return BadRequest(ModelState);
            }

            var command = new PermissionCommand
            {
                permission_name = permissionDTO.permission_name
            };
            try
            {

                await _commandDispatcher.SendAsync(command);

                objectResponse.Success = true;

                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetPermissionById), new { id = command.id }, objectResponse);
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

                    objectResponse.Error = new[] { "[#InbPerm3-3-C]", ex.InnerException.Message };
                    _logger.LogError("[#InbPerm3-3-C] Server Error occured Permission was not created for {Permission}||Caller:PermissionController/CreatePermission  || [SingleCategorHandler][Handle] error:{error}", command.permission_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }

                _logger.LogError("[#InbPerm3-3-C] Server Error occured Permission was not created for {Permission}||Caller:PermissionController/CreatePermission  || [SingleCategorHandler][Handle] error:{error}", command.permission_name, ex.Message);
                objectResponse.Error = new[] { "[#Branch003-3-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }



        [HttpPut("UpdatePermission")]
        public async Task<ActionResult> UpdatePermission(UpdatePermissionCommand command)
        {
            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid)
            {

                _logger.LogError("[#InbPerm4-4-C] Validation error {Permission} was not upadated ||Caller:PermissionController/CreatePermission  || [SingleCategorHandler][Handle]  ", command.id);
                return BadRequest(ModelState);
 
            }


            try
            {

                await _commandDispatcher.SendAsync(command);
                objectResponse.Success = true;
                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetPermissionById), new { id = command.id }, objectResponse);
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
                    _logger.LogError("[#InbPerm4-4-C] Server Error occured Permission was not updated for {Permission}||Caller:PermissionController/UpdatePermission  || [UpdatePermissionHandler][Handle] error:{error}", command.permission_name, ex.InnerException.Message);
                    objectResponse.Error = new[] { "[#Branch004-4-C]", ex.InnerException.Message };
                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

                }
                _logger.LogError("[#InbPerm4-4-C] Server Error occured Permission was not updated for {Permission}||Caller:PermissionController/UpdatePermission  || [UpdatePermissionHandler][Handle] error:{error}", command.permission_name, ex.Message);
                objectResponse.Error = new[] { "[#InbPerm4-4-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }

        [HttpDelete("DeletePermission/{id}")]
        public async Task<ActionResult> DeletePermission(string id)
        {
            var objectResponse = new ObjectResponse();
            if (id == string.Empty)
            {
                _logger.LogError(" [#InbPerm5-5-C]  Validation error {Permission} was not deleted ||Caller:PermissionController/CreatePermission  || [SingleCategorHandler][Handle]  ", id);
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
                    _logger.LogError("[#InbPerm5-5-C] Server Error occured Permission was not deleted   {Permission}||Caller:PermissionController/DeletePermission  || [DeletePermissionHandler][Handle] error:{error}", command.permission_name, ex.InnerException.Message);
                 
                    objectResponse.Error = new[] { "[#InbPerm5-5-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#InBCAT004-4-C] Server Error occured Permission was not deleted   {Permission}||Caller:PermissionController/DeletePermission  || [DeletePermissionHandler][Handle] error:{error}", command.permission_name, ex.Message);
                objectResponse.Error = new[] { "[#InBCAT004-4-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }
    }
}
