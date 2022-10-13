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
    public class ServiceRequestStatusController : Controller
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<ServiceRequestStatusController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;
        private readonly IBaseUrlService _baseUrlService;
        private readonly IServiceRequestStatusService _serviceRequestStatusService;
    
      
    public ServiceRequestStatusController(IServiceRequestStatusService serviceRequestStatusService, IBaseUrlService baseUrlService, IHttpContextAccessor accessor, ILogger<ServiceRequestStatusController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
            _accessor = accessor;
            _baseUrlService = baseUrlService;
            _serviceRequestStatusService = serviceRequestStatusService;
        }


  
        [HttpGet("GetAllServiceRequestStatus")]

        // [Authorize(Roles = "Trustee")]
        // [Authorize(Roles = "Trustee")]
        public async Task<ActionResult<ObjectResponse>> GetAllServiceRequestStatus( )
        {
            //Audit Item

            var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            var claimsItems = HttpContext.User.Claims;
            var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            var audit = new Audit();
            audit.inb_aduser_id = userName;
            audit.activity = "Search All Notification";
            audit.activity_module = "NotificationController";
            audit.activity_submodule = "SearchAllNotification";
            audit.action_type = "system";
            audit.clients = "system";
            var addAuditItem = await _baseUrlService.AddAuditItem(audit, userAgent);

            //AD users

            var serviceRequestStatusDTO = new List<ServiceRequestStatusDTO>();
            var objectResponse = new ObjectResponse();
            try
            {
                //GetServiceRequestStatusAsync
                serviceRequestStatusDTO = await _serviceRequestStatusService.GetServiceRequestStatussAsync();
                objectResponse.Data = serviceRequestStatusDTO;
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
                    _logger.LogError(" [#ServiceRequestStatus001-1-C] Server Error occured while getting all Service Request Status ||Caller:ServiceRequestStatusController /GetAllServiceRequestStatus  || [ServiiceRequestStatusService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#AdUser001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#ServiceRequestStatus001-1-C] Server Error occured while getting all Service Request Status ||Caller:ServiceRequestStatusController /GetAllServiceRequestStatus   || [ServiiceRequestStatusService][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#Notification001-1-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            
         

            return Ok(objectResponse);

        }


 
        [HttpPost("CreateServiceRequestStatusItem")]
        public async Task<ActionResult> CreateServiceRequestStatus([FromBody] string serviceRequestStatus)
        {
            //Audit Item

            var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            var claimsItems = HttpContext.User.Claims;
            var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            var audit = new Audit();
            audit.inb_aduser_id = userName;
            audit.activity = "Create Notification Type";
            audit.activity_module = "NotificationTypeController";
            audit.activity_submodule = "CreateNotificationType";
            audit.action_type = "system";
            audit.clients = "system";
            var addAuditItem = await _baseUrlService.AddAuditItem(audit, userAgent);

            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid )
            {
                _logger.LogError("[#ServiceRequestStatus002-2-C] Validation error {ServiceRequest} was not created ||Caller:ServiceRequestStatusController/CreateServiceRequestStatus  || [ServiiceRequestStatusService][Handle]  ", serviceRequestStatus + " User"+ serviceRequestStatus);
                var modeerror = ModelState.Values.SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage).ToArray();
                objectResponse.Error = new[] { "[#ServiceRequestStatus002-2-C] Validation error {ServiceRequestStatus} was not created ||Caller:ServiceRequestStatusController/CreateServiceRequestStatus  || [ServiiceRequestStatusService][Handle]  ", serviceRequestStatus + " User" + serviceRequestStatus };
                objectResponse.Error = objectResponse.Error.ToList().Union(modeerror).ToArray();
                 
                return BadRequest(objectResponse);
            }

            
            try
            {
                var ServiceRequestStatusDto = new ServiceRequestStatusDTO();
                ServiceRequestStatusDto.status = serviceRequestStatus;
                await _serviceRequestStatusService.AddServiceRequestStatusAsync(ServiceRequestStatusDto);

                objectResponse.Success = true;

                objectResponse.Data = new { id = ServiceRequestStatusDto.id };
                return CreatedAtAction(nameof(GetServiceRequestStatusById), new { id = ServiceRequestStatusDto.id }, objectResponse);
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

                    objectResponse.Error = new[] { "[#ServiceRequestStatus003-3-C]", ex.InnerException.Message };
                    _logger.LogError("[#ServiceRequestStatus003-3-C] Server Error occured Branch was not created for {ServiceRequestStatus}||Caller:ServiceRequestStatusController/CreateServiceRequestStatus  || [ServiiceRequestStatusService][Handle] error:{error}", serviceRequestStatus + " User" + serviceRequestStatus, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }

                _logger.LogError("[#ServiceRequestStatus003-3-C] Server Error occured Branch was not created for {ServiceRequestStatus}||Caller:ServiceRequestStatusController/CreateServiceRequestStatus  || [ServiiceRequestStatusService][Handle] error:{error}", serviceRequestStatus + " User" + serviceRequestStatus, ex.Message);
                objectResponse.Error = new[] { "[#NotificationType002-2-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }
         
        [HttpGet("GetServiceRequestStatusById/{id}")]
        public async Task<ActionResult<ObjectResponse>> GetServiceRequestStatusById(string id)
        {
            //Audit Item

            var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            var claimsItems = HttpContext.User.Claims;
            var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            var audit = new Audit();
            audit.inb_aduser_id = userName;
            audit.activity = "Get Notification Type By Id";
            audit.activity_module = "NotificationTypeController";
            audit.activity_submodule = "GetNotificationTypeById";
            audit.action_type = "system";
            audit.clients = "system";
            var addAuditItem = _baseUrlService.AddAuditItem(audit, userAgent);
   
            var branch = new Branch();
            var objectResponse = new ObjectResponse();
            try
            {
                var serviceRequestStatusDTO = new ServiceRequestStatusDTO();
                serviceRequestStatusDTO.id = id;
                objectResponse.Data = await _serviceRequestStatusService.GetServiceRequestStatusByIdAsync(serviceRequestStatusDTO);
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
                    _logger.LogError("[#ServiceRequestStatus004-4-C] Server Error occured while getting a Service Request Type by Id||Caller:ServiceRequestStatusController/GetServiceRequestStatusById  || [ServiiceRequestStatusService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#NotificationType003-3-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#ServiceRequestStatus004-4-C] Server Error occured while getting a Service Request Type||Caller:ServiceRequestStatusController/GetServiceRequestStatusById  || [ServiiceRequestStatusService][Handle] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#ServiceRequestStatus004-4-C", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }


        [HttpPut("UpdateServiceRequestStatus")]
        public async Task<ActionResult<ObjectResponse>> UpdateServiceRequestStatusById([FromBody] ServiceRequestStatusDTO serviceRequestStatusDTO)
        {
            //Audit Item

            var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            var claimsItems = HttpContext.User.Claims;
            var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            var audit = new Audit();
            audit.inb_aduser_id = userName;
            audit.activity = "Update Notification By Id";
            audit.activity_module = "NotificationTypeController";
            audit.activity_submodule = "UpdateNotificationType";
            audit.action_type = "system";
            audit.clients = "system";
            var addAuditItem = _baseUrlService.AddAuditItem(audit, userAgent);

            var branch = new Branch();
            var objectResponse = new ObjectResponse();
            try
            {
                var serviceRequestStatus  = new ServiceRequestStatusDTO();
                serviceRequestStatus.id = serviceRequestStatusDTO.id;
                serviceRequestStatus.status = serviceRequestStatusDTO.status;
                await _serviceRequestStatusService.UpdateServiceRequestStatusAsync(serviceRequestStatus);
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
                    _logger.LogError("[#ServiceRequestStatus005-5-C] Server Error occured while updating a  Service Request Type ||Caller:ServiceRequestStatusController/UpdateServiceRequestStatusById  || [ServiiceRequestStatusService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#Notification004-4-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#ServiceRequestStatus005-5-C] Server Error occured while updating  a Service Request  Type||Caller:ServiceRequestStatusController/UpdateServiceRequestStatusById  || [ServiiceRequestStatusService] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#ServiceRequestStatus005-5-C]", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }

    }
}