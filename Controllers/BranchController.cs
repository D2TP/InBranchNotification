using AutoMapper;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.Commands.Branches;


using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using InBranchDashboard.Queries.ADUser.queries;
using InBranchDashboard.Queries.Branches;
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
    public class BranchController : Controller
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<BranchController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        public BranchController(ILogger<BranchController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet("GetAllBranches/{PageNumber}/{PageSize}")]

        public async Task<ActionResult<ObjectResponse>> GetAllBranches(int PageNumber, int PageSize)
        {
            var queryStringParameters = new QueryStringParameters
            {
                PageNumber = PageNumber,
                PageSize = PageSize

            };
            var branch = new PagedList<Branch>();
            var objectResponse = new ObjectResponse();
            try
            {
                var BranchQueries = new BranchQueries(queryStringParameters);

                branch = await _queryDispatcher.QueryAsync(BranchQueries);
                objectResponse.Data = branch;
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
                    _logger.LogError("Server Error occured while getting all Categories ||Caller:BranchsController/GetAllBranchs  || [BranchQueries][Handle] error:{error}", ex.InnerException.Message);



                    objectResponse.Error = new[] { "[#Branch001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#Branch001-1-C] Server Error occured while getting all Brnach||Caller:BranchsController/GetAllBranchs  || [BranchQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            var metadata = new
            {
                branch.TotalCount,
                branch.PageSize,
                branch.CurrentPage,
                branch.TotalPages,
                branch.HasNext,
                branch.HasPrevious
            };

            objectResponse.Message = new[] { "X-Pagination" + JsonConvert.SerializeObject(metadata) }; ;
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(objectResponse);
        }

        [HttpGet("GetBranchsById/{id}")]
        public async Task<ActionResult<ObjectResponse>> GetBranchsById(string id)
        {
            //AD Login
            var branch = new Branch();
            var objectResponse = new ObjectResponse();
            try
            {
                var branchQuery = new BranchQuery(id);
                objectResponse.Data = await _queryDispatcher.QueryAsync(branchQuery);
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
                    _logger.LogError("[#Branch002-1-C] Server Error occured while getting all Branch ||Caller:BranchsController/GetBranchsById  || [BranchQueries][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#Branch002-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#Branch002-1-C] Server Error occured while getting a Branch||Caller:BranchsController/GetBranchsById  || [BranchQueries][Handle] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#Branch002-1-C]", ex.Message };

                 
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }


        [HttpPost("CreateBranch")]
        public async Task<ActionResult> CreateBranch([FromBody] BranchDTO branchDTO)
        {

            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid )
            {
                _logger.LogError("[#Branch003-3-C] Validation error {Branch} was not created ||Caller:BranchsController/CreateBranch  || [SingleCategorHandler][Handle]  ", branchDTO.branch_name);
                var modeerror = ModelState.Values.SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage).ToArray();
                objectResponse.Error = new[] { "[#Branch003-3-C] Validation error {Branch} was not created ||Caller:BranchsController/CreateBranch  || [SingleCategorHandler][Handle]  ", branchDTO.branch_name };
                objectResponse.Error = objectResponse.Error.ToList().Union(modeerror).ToArray();
                 
                return BadRequest(objectResponse);
            }

            var command = new BranchCommand
            {
                branch_name = branchDTO.branch_name,
                region_id = branchDTO.region_id
            };
            try
            {

                await _commandDispatcher.SendAsync(command);

                objectResponse.Success = true;

                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetBranchsById), new { id = command.id }, objectResponse);
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

                    objectResponse.Error = new[] { "[#Branch003-3-C]", ex.InnerException.Message };
                    _logger.LogError("[#Branch003-3-C] Server Error occured Branch was not created for {Branch}||Caller:BranchsController/CreateBranch  || [SingleCategorHandler][Handle] error:{error}", command.branch_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }

                _logger.LogError("[#Branch003-3-C] Server Error occured Branch was not created for {Branch}||Caller:BranchsController/CreateBranch  || [SingleCategorHandler][Handle] error:{error}", command.branch_name, ex.Message);
                objectResponse.Error = new[] { "[#Branch003-3-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }



        [HttpPut("UpdateBranch")]
        public async Task<ActionResult> UpdateBranch(UpdateBranchCommand command)
        {
            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid)
            {

                var modeerror = ModelState.Values.SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage).ToArray();
                objectResponse.Error = new[] { "[#Branch004-4-C] Validation error {Branch} was not upadated ||Caller:BranchsController/CreateBranch  || [SingleCategorHandler][Handle]  ", command.id };
                objectResponse.Error = modeerror.ToArray();
                return BadRequest(objectResponse);
            }


            try
            {

                await _commandDispatcher.SendAsync(command);
                objectResponse.Success = true;
                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetBranchsById), new { id = command.id }, objectResponse);
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
                    _logger.LogError("[#Branch004-4-C] Server Error occured Branch was not updated for {Branch}||Caller:BranchsController/UpdateBranch  || [UpdateBranchHandler][Handle] error:{error}", command.branch_name, ex.InnerException.Message);
                    objectResponse.Error = new[] { "[#Branch004-4-C]", ex.InnerException.Message };
                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

                }
                _logger.LogError("[#Branch004-4-C] Server Error occured Branch was not updated for {Branch}||Caller:BranchsController/UpdateBranch  || [UpdateBranchHandler][Handle] error:{error}", command.branch_name, ex.Message);
                objectResponse.Error = new[] { "[#Branch004-4-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }


        [HttpDelete("DeleteBranch/{id}")]
        public async Task<ActionResult> DeleteBranch(string id)
        {
            var objectResponse = new ObjectResponse();
            if (id == string.Empty)
            {
                _logger.LogError("[#Branch004-5-C] Validation error {Branch} was not deleted ||Caller:BranchsController/DeleteBranch  || [DeleteBranchHandler][Handle]  ", id);

                objectResponse.Error = new[] { "[#Branch004-5-C]  Validation error {Branch} was not deleted ||Caller:BranchsController/DeleteBranch  || [DeleteBranchHandler][Handle]  ", id };
                return BadRequest(objectResponse);
            }
            var command = new BranchDeleteComand
            {
                id = id
            };

            try
            {
                await _commandDispatcher.SendAsync(command);


                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Succsessfuly Deleted!!!"),
                    ReasonPhrase = "[#Branch004-5-C] Branch with id: " + id + "was deleted"
                };
                response.Headers.Add("DeleteMessage", "Succsessfuly Deleted!!!");


                objectResponse.Success = true;

                objectResponse.Data = new { id = id };
                objectResponse.Success = true;
                objectResponse.Message = new[] { StatusCodes.Status204NoContent.ToString(), "Branch with id: " + id + "was deleted" };

                return StatusCode(StatusCodes.Status204NoContent, objectResponse);


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
                    _logger.LogError("[#Branch004-5-C] Server Error occured Branch was not deleted   {Branch}||Caller:BranchsController/DeleteBranch  || [DeleteBranchHandler][Handle] error:{error}", command.branch_name, ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#Branch004-5-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                     
                }
                _logger.LogError("[#Branch004-5-C] Server Error occured Branch was not deleted   {Branch}||Caller:BranchsController/DeleteBranch  || [DeleteBranchHandler][Handle] error:{error}", command.branch_name, ex.Message);

                objectResponse.Error = new[] { "[#AdUser003-3-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }
    }
}