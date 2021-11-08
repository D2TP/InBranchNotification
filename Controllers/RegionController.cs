using AutoMapper;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.Commands.Regions;
using InBranchDashboard.Commands.UserRole;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using InBranchDashboard.Queries.ADUser.queries;
using InBranchDashboard.Queries.Regions;
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
    public class RegionController : Controller
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<RegionController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        public RegionController(ILogger<RegionController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet("GetAllRegion/{PageNumber}/{PageSize}")]

 
        public async Task<ActionResult<PagedList<RegionDTO>>> GetAllRegion(int PageNumber, int PageSize )
        {
            //AD Login
            var queryStringParameters = new QueryStringParameters
            {
                PageNumber = PageNumber,
                PageSize = PageSize
            };
            var regionDTO = new PagedList<RegionDTO>();
            try
            {
                var RegionQueries = new RegionQueries(queryStringParameters);
                regionDTO = await _queryDispatcher.QueryAsync(RegionQueries);
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
                    _logger.LogError("Server Error occured while getting all Region ||Caller:RegionController/GetAllRegion  || [RegionQueries][Handle] error:{error}", ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured while getting all ADUser||Caller:RegionController/GetAllRegion  || [RegionQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }


             var metadata = new
            {
                 regionDTO.TotalCount,
                 regionDTO.PageSize,
                 regionDTO.CurrentPage,
                 regionDTO.TotalPages,
                 regionDTO.HasNext,
                 regionDTO.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(regionDTO);

        }

        [HttpGet("GetRegionById/{id}")]
        public async Task<ActionResult<RegionDTO>> GetRegionById(string id)
        {
            //AD Login
            var RegionDTO = new RegionDTO();
            try
            {
                var regionQuery = new RegionQuery(id);
                RegionDTO = await _queryDispatcher.QueryAsync(regionQuery);
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
                    _logger.LogError("Server Error occured while getting all Region ||Caller:RegionController/GetRegionById  || [RegionQueries][Handle] error:{error}", ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured while getting all Region||Caller:RegionController/GetRegionById  || [RegionQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(RegionDTO);

        }


        [HttpPost("CreateRegion")]
        public async Task<ActionResult> CreateRegion([FromBody] AddRegionDTO addRegionDTO)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {Region} was not created ||Caller:RegionController/CreateRegion  || [SingleCategorHandler][Handle]  ", addRegionDTO.region_name);
                return BadRequest(ModelState);
            }

            var command = new RegionCommand
            {
                region_name = addRegionDTO.region_name
            };
            try
            {

                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetRegionById), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured Region was not created for {Region}||Caller:RegionController/CreateRegion  || [SingleRegionHandler][Handle] error:{error}", command.region_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Region was not created for {Region}||Caller:RegionController/CreateRegion  || [SingleRegionHandler][Handle] error:{error}", command.region_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }



        [HttpPut("UpdateRegion")]
        public async Task<ActionResult> UpdateRegion(UpdateRegionCommand command)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {Region} was not upadated ||Caller:RegionController/CreateRegion  || [SingleRegionHandler][Handle]  ", command.id);
                return BadRequest(ModelState);
            }


            try
            {

                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetRegionById), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured Region was not updated for {Region}||Caller:RegionController/UpdateRegion  || [UpdateRegionHandler][Handle] error:{error}", command.region_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Region was not updated for {Region}||Caller:RegionController/UpdateRegion  || [UpdateRegionHandler][Handle] error:{error}", command.region_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }


        [HttpDelete("DeleteRegion/{id}")]
        public async Task<ActionResult> DeleteRegion(string id)
        {

            if (id == string.Empty)
            {
                _logger.LogError("Validation error {Region} was not deleted ||Caller:RegionController/CreateRegion  || [SingleCategorHandler][Handle]  ", id);
                return BadRequest(ModelState);
            }
            var command = new RegionDeleteComand
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
                    _logger.LogError("Server Error occured Region was not deleted   {Region}||Caller:RegionController/DeleteRegion  || [DeleteRegionHandler][Handle] error:{error}", command.region_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured Region was not deleted   {Region}||Caller:RegionController/DeleteRegion  || [DeleteRegionHandler][Handle] error:{error}", command.region_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }
    }
}
