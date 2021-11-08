﻿using AutoMapper;
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
 
        public async Task<ActionResult<PagedList<CategoryDTO>>> GetAllCatigories(  int PageNumber, int PageSize)
        {
            var queryStringParameters = new QueryStringParameters
            {
                PageSize = PageSize,
                PageNumber = PageNumber
            };
            var categoryDTO = new PagedList<CategoryDTO>();
            try
            {
                var categoryQueries = new CategoryQueries(queryStringParameters);
                categoryDTO = await _queryDispatcher.QueryAsync(categoryQueries);
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
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured while getting all ADUser||Caller:CategoriesController/GetAllCatigories  || [CategoryQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


            return Ok(categoryDTO);

        }

        [HttpGet("GetCatigoriesById/{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCatigoriesById(string id)
        {
            //AD Login
            var categoryDTO = new CategoryDTO();
            try
            {
                var categoryQuery = new CategoryQuery(id);
                categoryDTO = await _queryDispatcher.QueryAsync(categoryQuery);
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
                    _logger.LogError("Server Error occured while getting all Category ||Caller:CategoriesController/GetCatigoriesById  || [CategoryQueries][Handle] error:{error}", ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured while getting all Category||Caller:CategoriesController/GetCatigoriesById  || [CategoryQueries][Handle] error:{error}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(categoryDTO);

        }


        [HttpPost("CreateCategory")]
        public async Task<ActionResult> CreateCategory([FromBody] AddCategoryDTO addCategoryDTO)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {category} was not created ||Caller:CategoriesController/CreateCategory  || [SingleCategorHandler][Handle]  ", addCategoryDTO.category_name);
                return BadRequest(ModelState);
            }

            var command = new CategoryCommand
            {
                category_name = addCategoryDTO.category_name
            };
            try
            {

                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetCatigoriesById), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured category was not created for {category}||Caller:CategoriesController/CreateCategory  || [SingleCategorHandler][Handle] error:{error}", command.category_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured category was not created for {category}||Caller:CategoriesController/CreateCategory  || [SingleCategorHandler][Handle] error:{error}", command.category_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }



        [HttpPut("UpdateCategory")]
        public async Task<ActionResult> UpdateCategory(UpdateCategoryCommand command)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation error {category} was not upadated ||Caller:CategoriesController/CreateCategory  || [SingleCategorHandler][Handle]  ", command.id);
                return BadRequest(ModelState);
            }

            
            try
            {

                await _commandDispatcher.SendAsync(command);

                return CreatedAtAction(nameof(GetCatigoriesById), new { id = command.id }, null);
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
                    _logger.LogError("Server Error occured category was not updated for {category}||Caller:CategoriesController/UpdateCategory  || [UpdateCategoryHandler][Handle] error:{error}", command.category_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured category was not updated for {category}||Caller:CategoriesController/UpdateCategory  || [UpdateCategoryHandler][Handle] error:{error}", command.category_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }


        [HttpDelete("DeleteCategory/{id}")]
        public async Task<ActionResult> DeleteCategory(string id)
        {

            if (id==string.Empty)
            {
                _logger.LogError("Validation error {category} was not deleted ||Caller:CategoriesController/CreateCategory  || [SingleCategorHandler][Handle]  ", id);
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
                if (ex.InnerException != null)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
                    {
                        Content = new StringContent(ex.InnerException.Message),
                        ReasonPhrase = ex.InnerException.Message

                    };
                    _logger.LogError("Server Error occured category was not deleted   {category}||Caller:CategoriesController/DeleteCategory  || [DeleteCategoryHandler][Handle] error:{error}", command.category_name, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, resp);
                }
                _logger.LogError("Server Error occured category was not deleted   {category}||Caller:CategoriesController/DeleteCategory  || [DeleteCategoryHandler][Handle] error:{error}", command.category_name, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }
    }
}
