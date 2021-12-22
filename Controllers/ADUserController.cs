using AutoMapper;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.Commands.UserRole;
using InBranchDashboard.DTOs;
using InBranchDashboard.Queries.ADUser.queries;
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
using LightQuery;
using InBranchDashboard.Domain;
using Newtonsoft.Json;
using InBranchDashboard.Helpers;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace InBranchDashboard.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ADUserController : Controller
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<ADUserController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        public ADUserController(ILogger<ADUserController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
        }
        //Support Office    Trustee
        [HttpGet("GetAllADusers/{PageNumber}/{PageSize}")]

        // [Authorize(Roles = "Trustee")]
        // [Authorize(Roles = "Trustee")]
        public async Task<ActionResult<ObjectResponse>> GetAllADusers(int PageNumber, int PageSize)
        {


            //AD users
            var aDUserParameters = new ADUserParameters()
            {
                PageSize = PageSize,
                PageNumber = PageNumber,
            };
            var aDCreateCommandDTO = new PagedList<ADUserBranchDTO>();
            var objectResponse = new ObjectResponse();
            try
            {
                var getAllADUserQuery = new GetAllADUserQuery(aDUserParameters);
                aDCreateCommandDTO = await _queryDispatcher.QueryAsync(getAllADUserQuery);
                objectResponse.Data = aDCreateCommandDTO;
                objectResponse.Success = true;
            }
            catch (Exception ex)
            {
                objectResponse.Success = false;
                if (ex.InnerException != null)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(ex.InnerException.Message),
                        ReasonPhrase = ex.InnerException.Message

                    };
                    _logger.LogError(" [#AdUser001-1-C] Server Error occured while getting all ADUser ||Caller:ADUserController/getADUserbyId  || [GetOneADUserQuery][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#AdUser001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#AdUser001-1-C] Server Error occured while getting all ADUser||Caller:ADUserController/getADUserbyId  || [GetOneADUserQuery][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#AdUser001-1-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            var metadata = new
            {
                aDCreateCommandDTO.TotalCount,
                aDCreateCommandDTO.PageSize,
                aDCreateCommandDTO.CurrentPage,
                aDCreateCommandDTO.TotalPages,
                aDCreateCommandDTO.HasNext,
                aDCreateCommandDTO.HasPrevious
            };
            objectResponse.Message = new[] {   JsonConvert.SerializeObject(metadata) }; 
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(objectResponse);

        }

        [HttpGet("GetADUserbyId/{id}")]

        public async Task<ActionResult<ObjectResponse>> GetADUserbyId(string id)
        {
            //AD Login
            var objectResponse = new ObjectResponse();

            try
            {
                var getOneADUserQuery = new GetOneADUserQuery(id);
                objectResponse = await _queryDispatcher.QueryAsync(getOneADUserQuery);

            }
            catch (Exception ex)
            {
                objectResponse.Success = false;
                if (ex.InnerException != null)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(ex.InnerException.Message),
                        ReasonPhrase = ex.InnerException.Message

                    };
                    _logger.LogError("Server Error occured while getting ADUser: {id}||Caller:ADUserController/getADUserbyId  || [GetOneADUserQuery][Handle] error:{error}", id, ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#AdUser001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("Server Error occured while getting ADUser: {id}||Caller:ADUserController/getADUserbyId  || [GetOneADUserQuery][Handle] error:{error}", id, ex.Message);
                objectResponse.Error = new[] { "[#AdUser001-1-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }


            return Ok(objectResponse);

        }


        [HttpPost("ActivateOrDeactivateADUser")]

        public async Task<ActionResult<ObjectResponse>> ActivateOrDeactivateADUser(ActivaeDeactivateAduser activaeDeactivateAduser)
        {
            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error activation or deactivation failed ||Caller:ADUserController/ActivateOrDeactivateADUser   || [CreateADUserHandler][ActivaeDeactivateAduserHandler]  ");
                var modeerror = ModelState.Values.SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage);
                objectResponse.Error = new[] { "Validation error activation or deactivation failed ||Caller:ADUserController/ActivateOrDeactivateADUser   || [ActivaeDeactivateAduserHandler][Handle] " };
                objectResponse.Error = objectResponse.Error.ToList().Union(modeerror).ToArray();
                return BadRequest(objectResponse);
            }
            

            try
            {
                //  await _commandDispatcher.SendAsync(command);

               // var getOneADUserQuery = new GetOneADUserQuery(id);
                await _commandDispatcher.SendAsync(activaeDeactivateAduser);
                objectResponse.Success = true;
                objectResponse.Message = new[] { "Active status change for " + activaeDeactivateAduser.AdUserId + " was completed succesfully" };
            }
            catch (Exception ex)
            {
                objectResponse.Success = false;
                if (ex.InnerException != null)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(ex.InnerException.Message),
                        ReasonPhrase = ex.InnerException.Message

                    };
                    _logger.LogError("Server Error occured while getting ADUser: {id}||Caller:ADUserController/ActivateOrDeactivateADUser  || [ActivaeDeactivateAduserHandler][Handle] error:{error}", activaeDeactivateAduser.AdUserId, ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#AdUser001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("Server Error occured while getting ADUser: {id}||Caller:ADUserController/ActivateOrDeactivateADUser  || [ActivaeDeactivateAduserHandler][Handle] error:{error}", activaeDeactivateAduser.AdUserId, ex.Message);
                objectResponse.Error = new[] { "[#AdUser001-1-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }


            return Ok(objectResponse);

        }

        [HttpPost("CreateADUser")]
        public async Task<ActionResult> CreateADUser([FromBody] ADUserInsertDTO aDUserInsertDTO)
        {
            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {username} was not created ||Caller:ADUserController/Create  || [CreateADUserHandler][Handle]  ");
                var modeerror = ModelState.Values.SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage);
                objectResponse.Error = new[] { "Validation error {username} was not created ||Caller:ADUserController/Create  || [CreateADUserHandler][Handle] " };
                objectResponse.Error = objectResponse.Error.ToList().Union(modeerror).ToArray();
                return BadRequest(objectResponse);
            }

            var command = _mapper.Map<ADUserInsertDTO, CreateADUserCommand>(aDUserInsertDTO);
            try
            {

                command.created_by = User.Identity.Name;
                await _commandDispatcher.SendAsync(command);
                objectResponse.Success = true;

                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetADUserbyId), new { id = command.id }, objectResponse);
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
                    objectResponse.Error = new[] { "[#AdUser002-2-C]", ex.InnerException.Message };
                    _logger.LogError("Server Error occured ADuser was not created for {username}||Caller:ADUserController/Create  || [CreateADUserHandler][Handle] error:{error}", command.UserName, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#AdUser001-2-C] Server Error occured ADuser was not created for {username}||Caller:ADUserController/Create  || [CreateADUserHandler][Handle] error:{error}", command.UserName, ex.Message);
                objectResponse.Error = new[] { "[#AdUser001-2-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }






        [HttpPut("UpdateADUser")]
        public async Task<ActionResult> UpdateADUser([FromBody] UpdateAdUserDto updateAduser)
        {
            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid)
            {
                _logger.LogError("[#AdUser001-2-C] Validation error {username} was not updated||Caller:ADUserController/UpdateADUser  || [UpdateADUserADUserHandler][Handle]");

                var modeerror = ModelState.Values.SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage).ToArray();
                objectResponse.Error = new[] { "#AdUser001-2-C] Validation error {username} was not created ||Caller:ADUserController/UpdateADUser  || [UpdateADUserADUserHandler][Handle] " };
                objectResponse.Error = modeerror.ToArray();
                return BadRequest(objectResponse);
            }

            var command = _mapper.Map<UpdateAdUserDto, UpdateAduser>(updateAduser);
            try
            {


                command.modified_by = User.Identity.Name;
                await _commandDispatcher.SendAsync(command);

                objectResponse.Success = true;

                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetADUserbyId), new { id = command.id }, objectResponse);
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
                    _logger.LogError("[#AdUser003-3-C] Server Error occured ADuser was not created for {username}||Caller:ADUserController/Create  || [CreateADUserHandler][Handle] error:{error}", command.user_name, ex.InnerException.Message);
                    objectResponse.Error = new[] { "[#AdUser003-3-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#AdUser003-3-C] Server Error occured ADuser was not created for {username}||Caller:ADUserController/Create  || [CreateADUserHandler][Handle] error:{error}", command.user_name, ex.Message);
                objectResponse.Error = new[] { "[#AdUser003-3-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);



            }
        }



        [HttpDelete("DeleteADUser/{id}")]
        public async Task<ActionResult> DeleteADUser(string id)
        {
            var objectResponse = new ObjectResponse();
            if (id == string.Empty)
            {
                _logger.LogError(" [#AdUser001-4-C]  Validation error {user with } " + id + " was not updated due to validation issue||Caller:ADUserController/UpdateADUser  || [UpdateADUserADUserHandler][Handle]");


                objectResponse.Error = new[] { "#AdUser001-4-C] Validation error {user with } " + id + " was not updated due to validation issue||Caller:ADUserController/UpdateADUser  || [UpdateADUserADUserHandler][Handle] " };

                return BadRequest(objectResponse);
            }

            var deleteAdUser = new DeleteAdUser { AdUserId = id };
            try
            {

                await _commandDispatcher.SendAsync(deleteAdUser);


                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Succsessfuly Deleted!!!"),
                    ReasonPhrase = "AD User with id: " + id + "deleted, all assinged role also deleted"
                };
                response.Headers.Add("DeleteMessage", "Succsessfuly Deleted!!!");


                objectResponse.Success = true;

                objectResponse.Data = new { id = id };
                objectResponse.Success = true;
                objectResponse.Message = new[] { "AD User with id: " + id + "deleted, all assinged role also deleted" };

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
                    _logger.LogError(" [#AdUser001-4-C]  Server Error occured ADuser was not deleted for {id}||Caller:ADUserController/DeleteADUser  || [DeleteADUser][Handle] error:{error}", id, ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#AdUser003-4-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);


                }
                _logger.LogError(" [#AdUser001-4-C]  Server Error occured ADuser was not Deleted for {id}||Caller:ADUserController/DeleteADUser  || [DeleteADUser][Handle] error:{error}", id, ex.Message);


                objectResponse.Error = new[] { "[#AdUser003-3-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }



    }
}
