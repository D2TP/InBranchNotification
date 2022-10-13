using AutoMapper;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
 
using InBranchAuditTrail.DTOs;
using InBranchNotification.Commands.Audit;
 

using InBranchNotification.Domain;
using InBranchNotification.DTOs;
using InBranchNotification.Helpers;
 
using InBranchNotification.Queries.Branches;
using InBranchNotification.Queries.Notifications;
using InBranchNotification.Services;
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

namespace InBranchNotification.Controllers
{
 //   [Authorize]
    [Route("api/[controller]")]
    public class ServiceRequestTypeController : Controller
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<ServiceRequestTypeController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;
        private readonly IBaseUrlService _baseUrlService;
        private readonly IServiiceRequestTypeService _serviiceRequestTypeService;
    
      
    public ServiceRequestTypeController(IServiiceRequestTypeService serviiceRequestTypeService, IBaseUrlService baseUrlService, IHttpContextAccessor accessor, ILogger<ServiceRequestTypeController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
            _accessor = accessor;
            _baseUrlService = baseUrlService;
            _serviiceRequestTypeService = serviiceRequestTypeService;
        }


  
        [HttpGet("GetAllServiceRequestType")]

        // [Authorize(Roles = "Trustee")]
        // [Authorize(Roles = "Trustee")]
        public async Task<ActionResult<ObjectResponse>> GetAllServiceRequestType( )
        {
            //Audit Item

            var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            var claimsItems = HttpContext.User.Claims;
            var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            var audit = new Audit();
            audit.inb_aduser_id = userName;
            audit.activity = "Get All Service Request Type";
            audit.activity_module = "ServiceRequestTypeController";
            audit.activity_submodule = "GetAllServiceRequestType";
            audit.action_type = "system";
            audit.clients = "system";
            var addAuditItem = await _baseUrlService.AddAuditItem(audit, userAgent);

            //AD users

            var serviceRequestTypeDto = new List<ServiceRequestTypeDto>();
            var objectResponse = new ObjectResponse();
            try
            {

                serviceRequestTypeDto = await _serviiceRequestTypeService.GetServiceRequestTypesAsync();
                objectResponse.Data = serviceRequestTypeDto;
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
                    _logger.LogError(" [#ServiceRequestType001-1-C] Server Error occured while getting all Service Request Types ||Caller:ServiceRequestTypeController /GetAllServiceRequestType  || [ServiiceRequestTypeService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#AdUser001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#ServiceRequestType001-1-C] Server Error occured while getting all Service Request Types ||Caller:ServiceRequestTypeController /GetAllServiceRequestType   || [ServiiceRequestTypeService][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#Notification001-1-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            
         

            return Ok(objectResponse);

        }


 
        [HttpPost("CreateServiceRequestTypeItem")]
        public async Task<ActionResult> CreateServiceRequestType([FromBody] string serviceRequestType)
        {
            //Audit Item

            var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            var claimsItems = HttpContext.User.Claims;
            var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            var audit = new Audit();
            audit.inb_aduser_id = userName;
            audit.activity = "Create Notification Type";
            audit.activity_module = "ServiceRequestTypeController";
            audit.activity_submodule = "CreateServiceRequestType";
            audit.action_type = "system";
            audit.clients = "system";
            var addAuditItem = await _baseUrlService.AddAuditItem(audit, userAgent);

            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid )
            {
                _logger.LogError("[#ServiceRequestType002-2-C] Validation error {ServiceRequest} was not created ||Caller:ServiceRequestTypeController/CreateServiceRequestType  || [ServiiceRequestTypeService][Handle]  ", serviceRequestType +" User"+ serviceRequestType);
                var modeerror = ModelState.Values.SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage).ToArray();
                objectResponse.Error = new[] { "[#ServiceRequestType002-2-C] Validation error {ServiceRequestType} was not created ||Caller:ServiceRequestTypeController/CreateServiceRequestType  || [ServiiceRequestTypeService][Handle]  ", serviceRequestType + " User" + serviceRequestType };
                objectResponse.Error = objectResponse.Error.ToList().Union(modeerror).ToArray();
                 
                return BadRequest(objectResponse);
            }

            
            try
            {
                var serviceRequestTypeDto = new ServiceRequestTypeDto();
                serviceRequestTypeDto.request_type= serviceRequestType;
                await _serviiceRequestTypeService.AddServiceRequestTypeAsync(serviceRequestTypeDto);

                objectResponse.Success = true;

                objectResponse.Data = new { id = serviceRequestTypeDto.id };
                return CreatedAtAction(nameof(GetServiceRequestTypeById), new { id = serviceRequestTypeDto.id }, objectResponse);
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

                    objectResponse.Error = new[] { "[#ServiceRequestType003-3-C]", ex.InnerException.Message };
                    _logger.LogError("[#ServiceRequestType003-3-C] Server Error occured Branch was not created for {ServiceRequestType}||Caller:ServiceRequestTypeController/CreateServiceRequestType  || [ServiiceRequestTypeService][Handle] error:{error}", serviceRequestType + " User" + serviceRequestType, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }

                _logger.LogError("[#ServiceRequestType003-3-C] Server Error occured Branch was not created for {ServiceRequestType}||Caller:ServiceRequestTypeController/CreateServiceRequestType  || [ServiiceRequestTypeService][Handle] error:{error}", serviceRequestType + " User" + serviceRequestType, ex.Message);
                objectResponse.Error = new[] { "[#NotificationType002-2-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }
         
        [HttpGet("GetServiceRequestTypeById/{id}")]
        public async Task<ActionResult<ObjectResponse>> GetServiceRequestTypeById(string id)
        {
            //Audit Item

            var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            var claimsItems = HttpContext.User.Claims;
            var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            var audit = new Audit();
            audit.inb_aduser_id = userName;
            audit.activity = "Get Notification Type By Id";
            audit.activity_module = "ServiceRequestTypeController";
            audit.activity_submodule = "GetServiceRequestTypeById";
            audit.action_type = "system";
            audit.clients = "system";
            var addAuditItem = _baseUrlService.AddAuditItem(audit, userAgent);
            //AD Login
            var branch = new Branch();
            var objectResponse = new ObjectResponse();
            try
            {
                var sServiceRequestTypeDto = new ServiceRequestTypeDto();
                sServiceRequestTypeDto.id = id;
                objectResponse.Data = await _serviiceRequestTypeService.GetServiceRequestTypeByIdAsync(sServiceRequestTypeDto);
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
                    _logger.LogError("[#ServiceRequestType004-4-C] Server Error occured while getting a Service Request Type by Id||Caller:ServiceRequestTypeController/GetServiceRequestTypeById  || [ServiiceRequestTypeService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#NotificationType003-3-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#ServiceRequestType004-4-C] Server Error occured while getting a Service Request Type||Caller:ServiceRequestTypeController/GetServiceRequestTypeById  || [ServiiceRequestTypeService][Handle] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#ServiceRequestType004-4-C", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }


        [HttpPut("UpdateServiceRequestType")]
        public async Task<ActionResult<ObjectResponse>> UpdateServiceRequestTypeById([FromBody] ServiceRequestTypeDto serviceRequestTypeDto)
        {
            //Audit Item

            var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            var claimsItems = HttpContext.User.Claims;
            var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            var audit = new Audit();
            audit.inb_aduser_id = userName;
            audit.activity = "Update Notification By Id";
            audit.activity_module = "ServiceRequestTypeController";
            audit.activity_submodule = "UpdateServiceRequestTypeById";
            audit.action_type = "system";
            audit.clients = "system";
            var addAuditItem = _baseUrlService.AddAuditItem(audit, userAgent);
           
            var branch = new Branch();
            var objectResponse = new ObjectResponse();
            try
            {
                var serviceRequestType = new ServiceRequestTypeDto();
                serviceRequestType.id = serviceRequestTypeDto.id;
                serviceRequestType.request_type = serviceRequestTypeDto.request_type;
                await _serviiceRequestTypeService.UpdateServiceRequestTypeAsync(serviceRequestType);
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
                    _logger.LogError("[#ServiceRequestType005-5-C] Server Error occured while updating a  Service Request Type ||Caller:ServiceRequestTypeController/UpdateServiceRequestTypeById  || [ServiiceRequestTypeService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#Notification004-4-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#ServiceRequestType005-5-C] Server Error occured while updating  a Service Request  Type||Caller:ServiceRequestTypeController/UpdateServiceRequestTypeById  || [ServiiceRequestTypeService] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#ServiceRequestType005-5-C]", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }

    }
}