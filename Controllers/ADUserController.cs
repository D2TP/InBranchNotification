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

namespace InBranchDashboard.Controllers
{
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
       
        [HttpGet("GetAllADusers/{PageNumber}/{PageSize}")]
        public async Task<ActionResult<PagedList<ADCreateCommandDTO>>> GetAllADusers(int PageNumber, int PageSize)
        {
            //AD users
            var aDUserParameters = new ADUserParameters()
            {
                PageSize = PageSize,
                PageNumber = PageNumber,
            };
            var aDCreateCommandDTO = new PagedList<ADUserBranchDTO>();
            try
            {
                var getAllADUserQuery = new GetAllADUserQuery(aDUserParameters);
                aDCreateCommandDTO = await _queryDispatcher.QueryAsync(getAllADUserQuery);
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
                aDCreateCommandDTO.TotalCount,
                aDCreateCommandDTO.PageSize,
                aDCreateCommandDTO.CurrentPage,
                aDCreateCommandDTO.TotalPages,
                aDCreateCommandDTO.HasNext,
                aDCreateCommandDTO.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(aDCreateCommandDTO);

        }

        [HttpGet("GetADUserbyId/{id}")]
 
        public async Task<ActionResult<ADCreateCommandDTO>> GetADUserbyId(string id)
        {
            //AD Login
            var aDCreateCommandDTO = new ADCreateCommandDTO();
            try
            {
                var getOneADUserQuery = new GetOneADUserQuery(id);
                aDCreateCommandDTO = await _queryDispatcher.QueryAsync(getOneADUserQuery);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(ex.InnerException.Message),
                        ReasonPhrase = ex.InnerException.Message

                    };
                    _logger.LogError("Server Error occured while getting ADUser: {id}||Caller:ADUserController/getADUserbyId  || [GetOneADUserQuery][Handle] error:{error}", id, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, resp);
                }
                _logger.LogError("Server Error occured while getting ADUser: {id}||Caller:ADUserController/getADUserbyId  || [GetOneADUserQuery][Handle] error:{error}", id, ex.Message);
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
            return Ok(aDCreateCommandDTO);

        }



        [HttpPost("CreateADUser")]
        public async Task<ActionResult> CreateADUser([FromBody] ADUserInsertDTO aDUserInsertDTO)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {username} was not created ||Caller:ADUserController/Create  || [CreateADUserHandler][Handle]  ");
                return BadRequest(ModelState);
            }
            var command = _mapper.Map<ADUserInsertDTO, CreateADUserCommand>(aDUserInsertDTO);

            try
            {
                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetADUserbyId), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured ADuser was not created for {username}||Caller:ADUserController/Create  || [CreateADUserHandler][Handle] error:{error}", command.UserName, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured ADuser was not created for {username}||Caller:ADUserController/Create  || [CreateADUserHandler][Handle] error:{error}", command.UserName, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }


        [HttpPut("UpdateADUser")]
        public async Task<ActionResult> UpdateADUser([FromBody] UpdateAduser command)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {username} was not updated||Caller:ADUserController/UpdateADUser  || [UpdateADUserADUserHandler][Handle]");
                return BadRequest(ModelState);
            }


            try
            {

                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetADUserbyId), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured ADuser was not created for {username}||Caller:ADUserController/Create  || [CreateADUserHandler][Handle] error:{error}", command.user_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured ADuser was not created for {username}||Caller:ADUserController/Create  || [CreateADUserHandler][Handle] error:{error}", command.user_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }

       
        [HttpDelete("DeleteADUser/{id}")]
        public async Task<ActionResult> DeleteADUser(string id)
        {

            if (id == string.Empty)
            {
                _logger.LogError("Validation error {username} was not updated||Caller:ADUserController/UpdateADUser  || [UpdateADUserADUserHandler][Handle]");
                return BadRequest(ModelState);
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
                    _logger.LogError("Server Error occured ADuser was not deleted for {id}||Caller:ADUserController/DeleteADUser  || [DeleteADUser][Handle] error:{error}", id, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured ADuser was not Deleted for {id}||Caller:ADUserController/DeleteADUser  || [DeleteADUser][Handle] error:{error}", id, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }




    }
}
