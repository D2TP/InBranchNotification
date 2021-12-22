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

        [HttpGet("GetAllPriviledge/{PageNumber}/{PageSize}")]

        public async Task<ActionResult<ObjectResponse>> GetAllPriviledge(int PageNumber, int PageSize)
        {
            var queryStringParameters = new QueryStringParameters
            {
                PageNumber = PageNumber,
                PageSize = PageSize
            };
            var priviledges = new PagedList<Priviledge>();
            var objectResponse = new ObjectResponse();
            try
            {
                var priviledgeQueries = new PriviledgeQueries(queryStringParameters);
                priviledges = await _queryDispatcher.QueryAsync(priviledgeQueries);


                objectResponse.Data = priviledges;
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
                    _logger.LogError("[#InbPriv001-1-C] Server Error occured while getting all Priviledge ||Caller:PriviledgeController/GetAllPriviledge  || [PriviledgeQueries][Handle] error:{error}", ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#InbPriv001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }


                _logger.LogError("[#InbPriv001-1-C] Server Error occured while getting all Priviledge ||Caller:PriviledgeController/GetAllPriviledge  || [PriviledgeQueries][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#InbPriv001-1-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
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
            objectResponse.Message = new[] {   JsonConvert.SerializeObject(metadata) };  ;
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(objectResponse);
        }

        [HttpGet("GetPriviledgeById/{id}")]
        public async Task<ActionResult<ObjectResponse>> GetPriviledgeById(string id)
        {
            //AD Login
           
            var objectResponse = new ObjectResponse();
            var Priviledge = new Priviledge();
            try
            {
                var PriviledgeQuery = new PriviledgeQuery(id);
             
                objectResponse.Data = await _queryDispatcher.QueryAsync(PriviledgeQuery);
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
                    _logger.LogError("[#InbPriv002-2-C] Server Error occured while getting all Priviledge ||Caller:PriviledgeController/GetPriviledgeById  || [PriviledgeQueries][Handle] error:{error}", ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#InbPriv002-2-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#InbPriv002-2-C] Server Error occured while getting all Priviledge||Caller:PriviledgeController/GetPriviledgeById  || [PriviledgeQueries][Handle] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#InbPriv002-2-C]", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }


        [HttpPost("CreatePriviledge")]
        public async Task<ActionResult> CreatePriviledge([FromBody] PriviledgeDTO PriviledgeDTO)
        {

            var objectResponse = new ObjectResponse();

            if (!ModelState.IsValid)
            {
                _logger.LogError("[#InbPriv003-3-C] Validation error {Priviledge} was not created ||Caller:PriviledgeController/CreatePriviledge  || [SingleCategorHandler][Handle]  ", PriviledgeDTO.priviledge_name);
                return BadRequest(ModelState);
            }

            var command = new PriviledgeCommand
            {
                priviledge_name = PriviledgeDTO.priviledge_name
            };
            try
            {

                await _commandDispatcher.SendAsync(command);


                objectResponse.Success = true;

                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetPriviledgeById), new { id = command.id }, objectResponse);
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

                    objectResponse.Error = new[] { "[#InbPriv003-3-C]", ex.InnerException.Message };
                    _logger.LogError("[#InbPriv003-3-C] Server Error occured Priviledge was not created for {Priviledge}||Caller:PriviledgeController/CreatePriviledge  || [SingleCategorHandler][Handle] error:{error}", command.priviledge_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }

                _logger.LogError("[#InbPriv003-3-C] Server Error occured Priviledge was not created for {Priviledge}||Caller:PriviledgeController/CreatePriviledge  || [SingleCategorHandler][Handle] error:{error}", command.priviledge_name, ex.Message);
                objectResponse.Error = new[] { "[#InbPriv003-3-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }



        [HttpPut("UpdatePriviledge")]
        public async Task<ActionResult> UpdatePriviledge(UpdatePriviledgeCommand command)
        {
            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid)
            {

                _logger.LogError("[#InbPriv4-4-C] Validation error {Priviledge} was not upadated ||Caller:PriviledgeController/CreatePriviledge  || [SingleCategorHandler][Handle]  ", command.id);
                return BadRequest(ModelState);

            }


            try
            {

                await _commandDispatcher.SendAsync(command);
                objectResponse.Success = true;
                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetPriviledgeById), new { id = command.id }, objectResponse);
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
                    _logger.LogError("[#InbPriv4-4-C] Server Error occured Priviledge was not updated for {Priviledge}||Caller:PriviledgeController/UpdatePriviledge  || [UpdatePriviledgeHandler][Handle] error:{error}", command.priviledge_name, ex.InnerException.Message);
                    objectResponse.Error = new[] { "[#InbPriv4-4-C]", ex.InnerException.Message };
                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

                }
                _logger.LogError("[#InbPriv4-4-C] Server Error occured Priviledge was not updated for {Priviledge}||Caller:PriviledgeController/UpdatePriviledge  || [UpdatePriviledgeHandler][Handle] error:{error}", command.priviledge_name, ex.Message);
                objectResponse.Error = new[] { "[#InbPriv4-4-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }
         




    [HttpDelete("DeletePriviledge/{id}")]
       
        public async Task<ActionResult> DeletePriviledge(string id)
        {
            var objectResponse = new ObjectResponse();
            if (id == string.Empty)
            {
                _logger.LogError(" [#InbPriv5-5-C] Validation error {Priviledge} was not deleted ||Caller:PriviledgeController/CreatePriviledge  || [SingleCategorHandler][Handle]  ", id);
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
                    _logger.LogError("[#InbPriv5-5-C] Server Error occured Priviledge was not deleted   {Priviledge}||Caller:PriviledgeController/DeletePriviledge  || [DeletePriviledgeHandler][Handle] error:{error}", command.priviledge_name, ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#InbPriv5-5-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[##InbPriv5-5-C] Server Error occured Priviledge was not deleted   {Priviledge}||Caller:PriviledgeController/DeletePriviledge  || [DeletePriviledgeHandler][Handle] error:{error}", command.priviledge_name, ex.Message);
                objectResponse.Error = new[] { "[##InbPriv5-5-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }
    }
}
