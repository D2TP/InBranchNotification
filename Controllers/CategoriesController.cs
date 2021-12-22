using AutoMapper;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.Commands.Categories;
using InBranchDashboard.Commands.Categories.handler;
using InBranchDashboard.Commands.UserRole;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using InBranchDashboard.Queries.ADUser.queries;
using InBranchDashboard.Queries.Categories;
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
    public class CategoriesController : Controller
    {

        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<CategoriesController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        public CategoriesController(ILogger<CategoriesController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("GetAllCatigories/{PageNumber}/{PageSize}")]

        public async Task<ActionResult<ObjectResponse>> GetAllCatigories(int PageNumber, int PageSize)
        {
            var queryStringParameters = new QueryStringParameters
            {
                PageSize = PageSize,
                PageNumber = PageNumber
            };
            var categoryDTO = new PagedList<CategoryDTO>();
            var objectResponse = new ObjectResponse();
            try
            {
                var categoryQueries = new CategoryQueries(queryStringParameters);
                categoryDTO = await _queryDispatcher.QueryAsync(categoryQueries);


                objectResponse.Data = categoryDTO;
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
                    _logger.LogError("Server Error occured while getting all Categories ||Caller:CategoriesController/GetAllCatigories  || [CategoryQueries][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#InBCAT001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#InBCAT001-1-C] Server Error occured while getting all ADUser||Caller:CategoriesController/GetAllCatigories  || [CategoryQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            var metadata = new
            {
                categoryDTO.TotalCount,
                categoryDTO.PageSize,
                categoryDTO.CurrentPage,
                categoryDTO.TotalPages,
                categoryDTO.HasNext,
                categoryDTO.HasPrevious
            };
            objectResponse.Message = new[] {   JsonConvert.SerializeObject(metadata) };  ;
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


            return Ok(objectResponse);

        }

        [HttpGet("GetCatigoriesById/{id}")]
        public async Task<ActionResult<ObjectResponse>> GetCatigoriesById(string id)
        {
            //AD Login
            var categoryDTO = new CategoryDTO();
            var objectResponse = new ObjectResponse();
            try
            {
                var categoryQuery = new CategoryQuery(id);
                categoryDTO = await _queryDispatcher.QueryAsync(categoryQuery);
                objectResponse.Success = true;
                objectResponse.Data = categoryDTO;
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
                    _logger.LogError("[#InBCAT002-2-C] Server Error occured while getting all Category ||Caller:CategoriesController/GetCatigoriesById  || [CategoryQueries][Handle] error:{error}", ex.InnerException.Message);

                    objectResponse.Error = new[] { "[#InBCAT002-2-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#InBCAT002-2-C] Server Error occured while getting all Category||Caller:CategoriesController/GetCatigoriesById  || [CategoryQueries][Handle] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#InBCAT002-2-C]", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }


        [HttpPost("CreateCategory")]
        public async Task<ActionResult> CreateCategory([FromBody] AddCategoryDTO addCategoryDTO)
        {
            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid)
            {

                _logger.LogError(" [#InBCAT003-3-C] Validation error {category} was not created ||Caller:CategoriesController/CreateCategory  || [SingleCategorHandler][Handle]  ", addCategoryDTO.category_name);
                return BadRequest(ModelState);
            }

            var command = new CategoryCommand
            {
                category_name = addCategoryDTO.category_name
            };
            try
            {

                await _commandDispatcher.SendAsync(command);
                objectResponse.Success = true;
                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetCatigoriesById), new { id = command.id }, objectResponse);



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
                    objectResponse.Error = new[] { "[#Branch003-3-C]", ex.InnerException.Message };
                    _logger.LogError(" [#InBCAT003-3-C] Server Error occured category was not created for {category}||Caller:CategoriesController/CreateCategory  || [SingleCategorHandler][Handle] error:{error}", command.category_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);


                }
                _logger.LogError("  [#InBCAT003-3-C]  Server Error occured category was not created for {category}||Caller:CategoriesController/CreateCategory  || [SingleCategorHandler][Handle] error:{error}", command.category_name, ex.Message);
              objectResponse.Error = new[] { "[#Branch003-3-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }



        [HttpPut("UpdateCategory")]
        public async Task<ActionResult> UpdateCategory(UpdateCategoryCommand command)
        {
            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid)
            {
                _logger.LogError(" [#InBCAT004-4-C] Validation error {category} was not upadated ||Caller:CategoriesController/CreateCategory  || [SingleCategorHandler][Handle]  ", command.id);
                return BadRequest(ModelState);
            }


            try
            {

                await _commandDispatcher.SendAsync(command);

                objectResponse.Success = true;
                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetCatigoriesById), new { id = command.id }, objectResponse);
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
                    _logger.LogError("[#InBCAT004-4-C] Server Error occured category was not updated for {category}||Caller:CategoriesController/UpdateCategory  || [UpdateCategoryHandler][Handle] error:{error}", command.category_name, ex.InnerException.Message);
                    
                    objectResponse.Error = new[] { "[#InBCAT004-4-C]", ex.InnerException.Message };
                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#InBCAT004-4-C] Server Error occured category was not updated for {category}||Caller:CategoriesController/UpdateCategory  || [UpdateCategoryHandler][Handle] error:{error}", command.category_name, ex.Message);

                objectResponse.Error = new[] { "[#InBCAT004-4-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                 
            }
        }


        [HttpDelete("DeleteCategory/{id}")]
        public async Task<ActionResult> DeleteCategory(string id)
        {
            var objectResponse = new ObjectResponse();
            if (id == string.Empty)
            {
                _logger.LogError(" [#InBCAT004-4-C]  Validation error {category} was not deleted ||Caller:CategoriesController/CreateCategory  || [SingleCategorHandler][Handle]  ", id);
                return BadRequest(ModelState);
            }
            var command = new CategoryDeleteComand
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
                objectResponse.Success = false;
                if (ex.InnerException != null)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
                    {
                        Content = new StringContent(ex.InnerException.Message),
                        ReasonPhrase = ex.InnerException.Message

                    };
                    _logger.LogError(" [#InBCAT004-4-C]  Server Error occured category was not deleted   {category}||Caller:CategoriesController/DeleteCategory  || [DeleteCategoryHandler][Handle] error:{error}", command.category_name, ex.InnerException.Message);
                   
                    objectResponse.Error = new[] { "[#InBCAT004-4-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest   , objectResponse);
                }
                _logger.LogError(" [#InBCAT004-4-C]  Server Error occured category was not deleted   {category}||Caller:CategoriesController/DeleteCategory  || [DeleteCategoryHandler][Handle] error:{error}", command.category_name, ex.Message);
                objectResponse.Error = new[] { "[#InBCAT004-4-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }
    }
}
