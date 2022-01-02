using AutoMapper;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.Commands.UserRole;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Queries.ADUser.queries;
using InBranchDashboard.Queries.Branches;
using InBranchDashboard.Queries.Roles;
using InBranchMgt.Commands.AdUser;
using InBranchMgt.Commands.AdUser.Handlers;
using Microsoft.AspNetCore.Authorization;
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
   // [Authorize]
    [Route("api/[controller]")]
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
        [HttpGet("GellAllRoleUserIsAssinged/{adUserid}")]
        public async Task<ActionResult> GellAllRoleUserIsAssinged(string adUserid)
        {
            //AD Login
            var objectResponse = new ObjectResponse();
            var adUserRoleDTO = new List<AdUserRoleDTO>();
            try
            {
                var adUserRoleQuery = new AdUserRoleQuery(adUserid);
                adUserRoleDTO = await _queryDispatcher.QueryAsync(adUserRoleQuery);
                objectResponse.Data = adUserRoleDTO;
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
                    _logger.LogError("[#UserRole1-1-C] Server Error occured while getting all ADUser ||Caller:ADUserController/GellAllRoleUserIsAssinged  || [GetOneADUserQuery][Handle] error:{error}", ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#UserRole1-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

                }
                _logger.LogError("[#UserRole-1-C] Server Error occured while getting all ADUser||Caller:ADUserController/GellAllRoleUserIsAssinged  || [GetOneADUserQuery][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#UserRole1-1-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }

        [HttpGet("GellAllRoleUserIsAssingedByUserName/{userName}")]
        public async Task<ActionResult> GellAllRoleUserIsAssingedByUserName(string userName)
        {
            //AD Login
            var objectResponse = new ObjectResponse();
            var adUserRoleDTO = new List<AdUserRoleDTO>();
            try
            {
                var adUserRoleQuery = new UserRoleQuery(userName);
                adUserRoleDTO = await _queryDispatcher.QueryAsync(adUserRoleQuery);
                objectResponse.Data = adUserRoleDTO;
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
                    _logger.LogError("[#UserRole1-1-C] Server Error occured while getting all ADUser ||Caller:ADUserController/GellAllRoleUserIsAssinged  || [GetOneADUserQuery][Handle] error:{error}", ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#UserRole1-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

                }
                _logger.LogError("[#UserRole-1-C] Server Error occured while getting all ADUser||Caller:ADUserController/GellAllRoleUserIsAssinged  || [GetOneADUserQuery][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#UserRole1-1-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }
        //stoped here 

        [HttpPut("ActivaeDeactivateSingleRole")]
        public async Task<ActionResult> ActivaeDeactivateSingleRole([FromBody] ADUserAndRold removeSingleRole)
        {
            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid)
            {
                _logger.LogError("[#UserRole5-5-C] Validation error {username} was not deleted||Caller:UserRoleController/RemoveSingleRoleComand  || [DeleteRoleHandler][Handle]");
                return BadRequest(ModelState);
            };
            try
            {

                await _commandDispatcher.SendAsync(removeSingleRole);

                objectResponse.Success = true;
                objectResponse.Message = new[] { "Active status change for userId " + removeSingleRole.AdUserId + "and roleId " + removeSingleRole.RoleId + " was completed succesfully" };


                return StatusCode(StatusCodes.Status200OK, objectResponse);

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
                    _logger.LogError("[#UserRole5-5-C] Server Error occured Role was not deleted for {id}||Caller:UserRoleController/RemoveARolefromADUser  || [RemoveSingleRoleComand][Handle] error:{error}", removeSingleRole.AdUserId, ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#UserRole5-5-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#UserRole5-5-C]  Server Error occured ADuser role was not deleted for {id}||Caller:UserRoleController/RemoveARolefromADUser  || [RemoveSingleRoleComand][Handle] error:{error}", removeSingleRole.AdUserId, ex.Message);
                objectResponse.Error = new[] { "[##UserRole5-5-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }


        [HttpPost("AddRoleToADUser")]
        public async Task<ActionResult> AddRoleToADUser([FromBody] CreateUserRoleComand command)
        {
            var objectResponse = new ObjectResponse();

            if (!ModelState.IsValid)
            {
                _logger.LogError("[##UserRole3-3-C] Validation error {username} role was no added ||Caller:UserRoleController/AddRoleToADUser  || [CreateUserRoleHandler][Handle]  ");
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
                response.Headers.Add("Succsess", "Succsessfuly Added!!!");


                objectResponse.Success = true;

                objectResponse.Data = new { id = "AD User role id: " + command.RoleId + "was created,for the following user: " + command.ADUserId };


                return StatusCode(StatusCodes.Status200OK, objectResponse);
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
                    _logger.LogError("[##UserRole3-3-C]  Server Error occured role was not created for {username}||Caller:UserRoleController/AddRoleToADUser  || [CreateUserRoleHandler][Handle] error:{error}", command.ADUserId, ex.InnerException.Message);

                    objectResponse.Error = new[] { "[##UserRole3-3-C]", ex.InnerException.Message };

                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[##UserRole3-3-C] Server Error occured  role was not created for {username}||Caller:UserRoleController/AddRoleToADUser  || [CreateUserRoleHandler][Handle] error:{error}", command.ADUserId, ex.Message);
                objectResponse.Error = new[] { "[##UserRole3-3-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }





    }
}
