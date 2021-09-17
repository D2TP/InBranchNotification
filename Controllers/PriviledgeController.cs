using AutoMapper;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.Commands.Priviledges;
using InBranchDashboard.Commands.UserRole;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using InBranchDashboard.Queries.ADUser.queries;
using InBranchDashboard.Queries.Priviledges;
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
    public class PriviledgeController : Controller
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<PriviledgeController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        public PriviledgeController(ILogger<PriviledgeController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("GetAllPriviledge")]
        public async Task<ActionResult<List<Priviledge>>> GetAllPriviledge([FromQuery] QueryStringParameters queryStringParameters)
        {
            //AD Login
            var priviledges = new PagedList<Priviledge>();
            try
            {
                var priviledgeQueries = new PriviledgeQueries(queryStringParameters);
                priviledges = await _queryDispatcher.QueryAsync(priviledgeQueries);
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
                    _logger.LogError("Server Error occured while getting all Priviledge ||Caller:PriviledgeController/GetAllPriviledge  || [PriviledgeQueries][Handle] error:{error}", ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured while getting all ADUser||Caller:PriviledgeController/GetAllPriviledge  || [PriviledgeQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            var metadata = new
            {
                priviledges.TotalCount,
                priviledges.PageSize,
                priviledges.CurrentPage,
                priviledges.TotalPages,
                priviledges.HasNext,
                priviledges.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(priviledges);

        }

        [HttpGet("GetPriviledgeById")]
        public async Task<ActionResult<PriviledgeDTO>> GetPriviledgeById(string id)
        {
            //AD Login
            var Priviledge = new Priviledge();
            try
            {
                var PriviledgeQuery = new PriviledgeQuery(id);
                Priviledge = await _queryDispatcher.QueryAsync(PriviledgeQuery);
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
                    _logger.LogError("Server Error occured while getting all Priviledge ||Caller:PriviledgeController/GetPriviledgeById  || [PriviledgeQueries][Handle] error:{error}", ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured while getting all Priviledge||Caller:PriviledgeController/GetPriviledgeById  || [PriviledgeQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(Priviledge);

        }


        [HttpPost("CreatePriviledge")]
        public async Task<ActionResult> CreatePriviledge([FromBody] PriviledgeDTO PriviledgeDTO)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {Priviledge} was not created ||Caller:PriviledgeController/CreatePriviledge  || [SingleCategorHandler][Handle]  ", PriviledgeDTO.priviledge_name);
                return BadRequest(ModelState);
            }

            var command = new PriviledgeCommand
            {
                priviledge_name = PriviledgeDTO.priviledge_name
            };
            try
            {

                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetPriviledgeById), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured Priviledge was not created for {Priviledge}||Caller:PriviledgeController/CreatePriviledge  || [SingleCategorHandler][Handle] error:{error}", command.priviledge_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Priviledge was not created for {Priviledge}||Caller:PriviledgeController/CreatePriviledge  || [SingleCategorHandler][Handle] error:{error}", command.priviledge_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }



        [HttpPut("UpdatePriviledge")]
        public async Task<ActionResult> UpdatePriviledge(UpdatePriviledgeCommand command)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {Priviledge} was not upadated ||Caller:PriviledgeController/CreatePriviledge  || [SingleCategorHandler][Handle]  ", command.id);
                return BadRequest(ModelState);
            }


            try
            {

                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetPriviledgeById), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured Priviledge was not updated for {Priviledge}||Caller:PriviledgeController/UpdatePriviledge  || [UpdatePriviledgeHandler][Handle] error:{error}", command.priviledge_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Priviledge was not updated for {Priviledge}||Caller:PriviledgeController/UpdatePriviledge  || [UpdatePriviledgeHandler][Handle] error:{error}", command.priviledge_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }


        [HttpDelete("DeletePriviledge")]
        public async Task<ActionResult> DeletePriviledge(string id)
        {

            if (id == string.Empty)
            {
                _logger.LogError("Validation error {Priviledge} was not deleted ||Caller:PriviledgeController/CreatePriviledge  || [SingleCategorHandler][Handle]  ", id);
                return BadRequest(ModelState);
            }
            var command = new PriviledgeDeleteComand
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
                    _logger.LogError("Server Error occured Priviledge was not deleted   {Priviledge}||Caller:PriviledgeController/DeletePriviledge  || [DeletePriviledgeHandler][Handle] error:{error}", command.priviledge_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Priviledge was not deleted   {Priviledge}||Caller:PriviledgeController/DeletePriviledge  || [DeletePriviledgeHandler][Handle] error:{error}", command.priviledge_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }
    }
}
