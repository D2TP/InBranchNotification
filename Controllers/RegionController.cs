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
         
        public async Task<ActionResult<ObjectResponse>> GetAllPriviledge(int PageNumber, int PageSize)
        {
            var queryStringParameters = new QueryStringParameters
            {
                PageNumber = PageNumber,
                PageSize = PageSize
            };
            var regionDTO = new PagedList<RegionDTO>();
            var objectResponse = new ObjectResponse();
            try
            {
                var RegionQueries = new RegionQueries(queryStringParameters);
                regionDTO = await _queryDispatcher.QueryAsync(RegionQueries);


                objectResponse.Data = regionDTO;
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
                    _logger.LogError("[#Region001-1-C] Server Error occured while getting all Region ||Caller:RegionController/GetAllRegion  || [RegionQueries][Handle] error:{error}", ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#Region001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }


                _logger.LogError("[#Region001-1-C] Server Error occured while getting all ADUser||Caller:RegionController/GetAllRegion  || [RegionQueries][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#Region001-1-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
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
            objectResponse.Message = new[] { "X-Pagination" + JsonConvert.SerializeObject(metadata) }; ;
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(objectResponse);
        }



        [HttpGet("GetRegionById/{id}")]
     
        public async Task<ActionResult<ObjectResponse>> GetRegionById(string id)
        {
            //AD Login
            var RegionDTO = new RegionDTO();
            var objectResponse = new ObjectResponse();

            try
            {
                var regionQuery = new RegionQuery(id);
                    objectResponse.Data = await _queryDispatcher.QueryAsync(regionQuery);
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
                    _logger.LogError("[#Region002-2-C] Server Error occured while getting all Region ||Caller:RegionController/GetRegionById  || [RegionQueries][Handle] error:{error}", ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#Region002-2-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#Region002-2-C] Server Error occured while getting all Region ||Caller:RegionController/GetRegionById  || [RegionQueries][Handle] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#Region002-2-C]", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }


        [HttpPost("CreateRegion")]
   
        public async Task<ActionResult> CreateRegion([FromBody] AddRegionDTO addRegionDTO)
        {

            var objectResponse = new ObjectResponse();

            if (!ModelState.IsValid)
            {
                _logger.LogError("[#Region002-3-C] Validation error {Region} was not created ||Caller:RegionController/CreateRegion  || [SingleCategorHandler][Handle]  ", addRegionDTO.region_name);
                return BadRequest(ModelState);
            }

            var command = new RegionCommand
            {
                region_name = addRegionDTO.region_name
            };
            try
            {

                await _commandDispatcher.SendAsync(command);


                objectResponse.Success = true;

                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetRegionById), new { id = command.id }, objectResponse);
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
                    _logger.LogError("[#Region002-3-C] Server Error occured Region was not created for {Region}||Caller:RegionController/CreateRegion  || [SingleRegionHandler][Handle] error:{error}", command.region_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }

                _logger.LogError("[#Region002-3-C] Server Error occured Region was not created for {Region}||Caller:RegionController/CreateRegion  || [SingleRegionHandler][Handle] error:{error}", command.region_name, ex.InnerException.Message);
                objectResponse.Error = new[] { "[#Region002-3-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }



        [HttpPut("UpdateRegion")]
      
        public async Task<ActionResult> UpdateRegion(UpdateRegionCommand command)
        {
            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid)
            {

                _logger.LogError("[#Region004-4-C] Validation error {Region} was not upadated ||Caller:RegionController/CreateRegion  || [SingleRegionHandler][Handle]  ", command.id);
                return BadRequest(ModelState);

            }


            try
            {

                await _commandDispatcher.SendAsync(command);
                objectResponse.Success = true;
                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetRegionById), new { id = command.id }, objectResponse);
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
                    _logger.LogError("[#Region004-4-C] Server Error occured Region was not updated for {Region}||Caller:RegionController/UpdateRegion  || [UpdateRegionHandler][Handle] error:{error}", command.region_name, ex.InnerException.Message);
                    objectResponse.Error = new[] { "[#Region004-4-C]", ex.InnerException.Message };
                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

                }
                _logger.LogError("[#Region004-4-C] Server Error occured Region was not updated for {Region}||Caller:RegionController/UpdateRegion  || [UpdateRegionHandler][Handle] error:{error}", command.region_name, ex.Message);
                objectResponse.Error = new[] { "[#Region004-4-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }



        [HttpDelete("DeleteRegion/{id}")]
      
        public async Task<ActionResult> DeleteRegion(string id)
        {
            var objectResponse = new ObjectResponse();
            if (id == string.Empty)
            {
                _logger.LogError(" [#Region005-5-C] Validation error {Region} was not deleted ||Caller:RegionController/CreateRegion  || [SingleCategorHandler][Handle]  ", id);
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
                    _logger.LogError("[#Region005-5-C] Server Error occured Region was not deleted   {Region}||Caller:RegionController/DeleteRegion  || [DeleteRegionHandler][Handle] error:{error}", command.region_name, ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#Region005-5-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[##Region005-5-C] Server Error occured Region was not deleted   {Region}||Caller:RegionController/DeleteRegion  || [DeleteRegionHandler][Handle] error:{error}", command.region_name, ex.Message);
                objectResponse.Error = new[] { "[#Region005-5-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }
    }
}
