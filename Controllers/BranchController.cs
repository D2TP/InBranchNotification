using AutoMapper;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.Commands.Branches;
 
 
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Queries.ADUser.queries;
using InBranchDashboard.Queries.Branches;
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
        [HttpGet("GetAllBranches")]
        public async Task<ActionResult<List<Branch>>> GetAllBranches()
        {
            //AD Login
            var Branch = new List<Branch>();
            try
            {
                var BranchQueries = new BranchQueries();
                Branch = await _queryDispatcher.QueryAsync(BranchQueries);
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
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured while getting all ADUser||Caller:BranchsController/GetAllBranchs  || [BranchQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(Branch);

        }

        [HttpGet("GetBranchsById")]
        public async Task<ActionResult<Branch>> GetBranchsById(string id)
        {
            //AD Login
            var Branch = new Branch();
            try
            {
                var branchQuery = new BranchQuery(id);
                Branch = await _queryDispatcher.QueryAsync(branchQuery);
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
                    _logger.LogError("Server Error occured while getting all Branch ||Caller:BranchsController/GetBranchsById  || [BranchQueries][Handle] error:{error}", ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured while getting all Branch||Caller:BranchsController/GetBranchsById  || [BranchQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(Branch);

        }


        [HttpPost("CreateBranch")]
        public async Task<ActionResult> CreateBranch([FromBody] BranchDTO branchDTO)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {Branch} was not created ||Caller:BranchsController/CreateBranch  || [SingleCategorHandler][Handle]  ", branchDTO.branch_name);
                return BadRequest(ModelState);
            }

            var command = new BranchCommand
            {
                branch_name = branchDTO.branch_name,
                region_id= branchDTO.region_id
            };
            try
            {

                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetBranchsById), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured Branch was not created for {Branch}||Caller:BranchsController/CreateBranch  || [SingleCategorHandler][Handle] error:{error}", command.branch_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Branch was not created for {Branch}||Caller:BranchsController/CreateBranch  || [SingleCategorHandler][Handle] error:{error}", command.branch_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }



        [HttpPut("UpdateBranch")]
        public async Task<ActionResult> UpdateBranch(UpdateBranchCommand command)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {Branch} was not upadated ||Caller:BranchsController/CreateBranch  || [SingleCategorHandler][Handle]  ", command.id);
                return BadRequest(ModelState);
            }


            try
            {

                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetBranchsById), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured Branch was not updated for {Branch}||Caller:BranchsController/UpdateBranch  || [UpdateBranchHandler][Handle] error:{error}", command.branch_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Branch was not updated for {Branch}||Caller:BranchsController/UpdateBranch  || [UpdateBranchHandler][Handle] error:{error}", command.branch_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }


        [HttpDelete("DeleteBranch")]
        public async Task<ActionResult> DeleteBranch(string id)
        {

            if (id == string.Empty)
            {
                _logger.LogError("Validation error {Branch} was not deleted ||Caller:BranchsController/DeleteBranch  || [DeleteBranchHandler][Handle]  ", id);
                return BadRequest(ModelState);
            }
            var command = new BranchDeleteComand
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
                    _logger.LogError("Server Error occured Branch was not deleted   {Branch}||Caller:BranchsController/DeleteBranch  || [DeleteBranchHandler][Handle] error:{error}", command.branch_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Branch was not deleted   {Branch}||Caller:BranchsController/DeleteBranch  || [DeleteBranchHandler][Handle] error:{error}", command.branch_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }
    }
}