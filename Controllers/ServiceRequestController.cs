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
    public class ServiceRequestController : Controller
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<ServiceRequestController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;
        private readonly IBaseUrlService _baseUrlService;
        private readonly IServiceRequestService _serviceRequestService;
        private readonly IServiceRequestHistory _serviceRequestHistory;


        public ServiceRequestController(IServiceRequestHistory serviceRequestHistory, IServiceRequestService serviceRequestService, IBaseUrlService baseUrlService, IHttpContextAccessor accessor, ILogger<ServiceRequestController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
            _accessor = accessor;
            _baseUrlService = baseUrlService;
            _serviceRequestService = serviceRequestService;
            _serviceRequestHistory = serviceRequestHistory;
        }

        [HttpGet("SearchAllServiceRequest/{PageNumber}/{PageSize}")]
        public async Task<ActionResult<ObjectResponse>> SearchAllServiceRequest(ServiceRequestSearch serviceRequestSearch)
        {
            //Audit Item

            //var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            //var claimsItems = HttpContext.User.Claims;
            //var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            //var audit = new Audit();
            //audit.inb_aduser_id = userName;
            //audit.activity = "Search All Notification";
            //audit.activity_module = "NotificationController";
            //audit.activity_submodule = "SearchAllNotification";
            //audit.action_type = "system";
            //audit.clients = "system";
            //var addAuditItem = await _baseUrlService.AddAuditItem(audit, userAgent);

            //AD users

            var serviceRequestDetail = new PagedList<ServiceRequestDetail>();
            var objectResponse = new ObjectResponse();
            try
            {

                serviceRequestDetail = await _serviceRequestService.GetAllServiceRequestAsync(serviceRequestSearch);
                objectResponse.Data = serviceRequestDetail;
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
                    _logger.LogError(" [#ServiceRequest001-1-C] Server Error occured while getting    Service Requests ||Caller:ServiceRequestController /SearchAllServiceRequest  || [ServiceRequestService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#AdUser001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#ServiceRequest001-1-C] Server Error occured while getting   Service Requests||Caller:ServiceRequestController /SearchAllServiceRequest   || [ServiceRequestService][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#Notification001-1-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            var metadata = new
            {
                serviceRequestDetail.TotalCount,
                serviceRequestDetail.PageSize,
                serviceRequestDetail.CurrentPage,
                serviceRequestDetail.TotalPages,
                serviceRequestDetail.HasNext,
                serviceRequestDetail.HasPrevious
            };
            objectResponse.Message = new[] { JsonConvert.SerializeObject(metadata) };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(objectResponse);

        }




        [HttpPost("CreateServiceRequestItem")]
        public async Task<ActionResult> CreateServiceRequest([FromBody] ServiceRequestDTO serviceRequestDTO)
        {
          //  Audit Item

            var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            var claimsItems = HttpContext.User.Claims;
            var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            var audit = new Audit();
            audit.inb_aduser_id = userName;
            audit.activity = "Create Service Request";
            audit.activity_module = "ServiceRequestController";
            audit.activity_submodule = "CreateServiceRequest";
            audit.action_type = "client";
            audit.clients = serviceRequestDTO.cif_id;
            var addAuditItem = await _baseUrlService.AddAuditItem(audit, userAgent);

            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid)
            {
                _logger.LogError("[#ServiceRequest002-2-C] Validation error {ServiceRequest} was not created ||Caller:ServiceRequestController/CreateServiceRequestItem  || [ServiceRequestService][Handle]  ", serviceRequestDTO.cif_id + " User" + serviceRequestDTO.cif_id);
                var modeerror = ModelState.Values.SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage).ToArray();
                objectResponse.Error = new[] { "[#ServiceRequest002-2-C] Validation error {ServiceRequest} was not created ||Caller:ServiceRequestController/CreateServiceRequestItem  || [ServiceRequestService][Handle]  ", serviceRequestDTO.cif_id + " User" + serviceRequestDTO.cif_id };
                objectResponse.Error = objectResponse.Error.ToList().Union(modeerror).ToArray();

                return BadRequest(objectResponse);
            }


            try
            {

                var serviceRequestCreateDto = _mapper.Map<ServiceRequestCreateDto>(serviceRequestDTO);
                await _serviceRequestService.AddServiceRequestAsync(serviceRequestCreateDto);

                objectResponse.Success = true;

                objectResponse.Data = new { id = serviceRequestCreateDto.id };
                return CreatedAtAction(nameof(GetServiceRequestById), new { id = serviceRequestCreateDto.id }, objectResponse);
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

                    objectResponse.Error = new[] { "[#ServiceRequest002-2-C]", ex.InnerException.Message };
                    _logger.LogError("[#ServiceRequest003-3-C] Server Error occured Branch was not created for {ServiceRequest}||Caller:ServiceRequestController/CreateServiceRequestItem  || [ServiceRequestService][Handle] error:{error}", serviceRequestDTO.cif_id + " User" + serviceRequestDTO.cif_id, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }

                _logger.LogError("[#ServiceRequest003-3-C] Server Error occured Branch was not created for {ServiceRequest}||Caller:ServiceRequestController/CreateServiceRequestItem  || [ServiceRequestService][Handle] error:{error}", serviceRequestDTO.cif_id + " User" + serviceRequestDTO.cif_id, ex.Message);
                objectResponse.Error = new[] { "[#ServiceRequest003-3-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }

        [HttpGet("GetServiceRequestById/{id}")]
        public async Task<ActionResult<ObjectResponse>> GetServiceRequestById(string id)
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

                objectResponse.Data = await _serviceRequestService.GetServiceRequestByIdAsync(id);
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
                    _logger.LogError("[#ServiceRequest004-4-C] Server Error occured while getting appproving ||Caller:ServiceRequestController/GetServiceRequestById  || [ServiceRequestService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#ServiceRequest004-4-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#ServiceRequest003-3-C] Server Error occured while approving service request||Caller:ServiceRequestController/GetServiceRequestById  || [ServiceRequestService][Handle] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#ServiceRequest004-4-C", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }

        [HttpPut("ApproverRequest")]
        public async Task<ActionResult<ObjectResponse>> ApproverRequest([FromBody] ApproveServiceRequest approveServiceRequest)
        {
            //Audit Item

            var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            var claimsItems = HttpContext.User.Claims;
            var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            var audit = new Audit();
            audit.inb_aduser_id = userName;
            audit.activity = approveServiceRequest.service_request_status_id;
            audit.activity_module = "ServiceRequestController";
            audit.activity_submodule = "ApproverRequest";
            var requestDetail = await _serviceRequestService.GetServiceRequestByIdNoJoinsAsync(approveServiceRequest.id);
            audit.clients = requestDetail.cif_id;

            audit.action_type = "clients";
            var addAuditItem = _baseUrlService.AddAuditItem(audit, userAgent);
            //AD Login
            var branch = new Branch();
            var objectResponse = new ObjectResponse();
            try
            {
                
                await _serviceRequestService.ApproveServiceRequestAsync(approveServiceRequest);
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
                    _logger.LogError("[#ServiceRequest005-5-C] Server Error occured while approving service request ||Caller:ServiceRequestController/ApproverAct  || [ServiceRequestService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#ServiceRequest005-5-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#ServiceRequest005-5-C] Server Error occured  while approving approving service request||Caller:ServiceRequestController/ApproverAct  || [ServiceRequestService][Handle] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#ServiceRequest005-5-C]", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }


        [HttpPut("ReviewRequest")]
        public async Task<ActionResult<ObjectResponse>> ReviewRequest([FromBody] ReviewServiceRequest reviewServiceRequest)
        {
            //Audit Item

            //var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            //var claimsItems = HttpContext.User.Claims;
            //var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
             var audit = new Audit();
            //audit.inb_aduser_id = userName;
            //audit.activity = reviewServiceRequest.service_request_status_id;
            //audit.activity_module = "ServiceRequestController";
            //audit.activity_submodule = "ReviewRequest";
            var requestDetail = await _serviceRequestService.GetServiceRequestByIdNoJoinsAsync(reviewServiceRequest.id);
            //audit.clients = requestDetail.cif_id;

            //audit.action_type = "clients";
            //var addAuditItem = _baseUrlService.AddAuditItem(audit, userAgent);
            //AD Login
            var branch = new Branch();
            var objectResponse = new ObjectResponse();
            try
            {

                await _serviceRequestService.ReviewServiceRequestAsync(reviewServiceRequest);
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
                    _logger.LogError("[#ServiceRequest006-5-C] Server Error occured while approving service request ||Caller:ServiceRequestController/ReviewRequest  || [ServiceRequestService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#ServiceRequest006-6-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#ServiceRequest006-6-C] Server Error occured  while approving approving service request||Caller:ServiceRequestController/ReviewRequest  || [ServiceRequestService][Handle] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#ServiceRequest006-6-C]", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }


        [HttpGet("SearchAllServiceRequestHistory/{PageNumber}/{PageSize}")]
        public async Task<ActionResult<ObjectResponse>> SearchAllServiceRequestHistory(ServiceHistroySearchDto serviceHistroySearchDto)
        {
            //Audit Item

            //var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            //var claimsItems = HttpContext.User.Claims;
            //var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            //var audit = new Audit();
            //audit.inb_aduser_id = userName;
            //audit.activity = "Search All Notification";
            //audit.activity_module = "NotificationController";
            //audit.activity_submodule = "SearchAllNotification";
            //audit.action_type = "system";
            //audit.clients = "system";
            //var addAuditItem = await _baseUrlService.AddAuditItem(audit, userAgent);

            //AD users

            var serviceRequestHistoryDto = new PagedList<ServiceRequestHistoryDto>();
            var objectResponse = new ObjectResponse();
            try
            {

                serviceRequestHistoryDto = await _serviceRequestHistory.SearchAllServiceRequestHistoryAsync(serviceHistroySearchDto);
                objectResponse.Data = serviceRequestHistoryDto;
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
                    _logger.LogError(" [#ServiceRequest007-7-C] Server Error occured while getting    Service Requests ||Caller:ServiceRequestController /SearchAllServiceRequest  || [ServiceRequestService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#AdUser007-7-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#ServiceRequest007-7-C] Server Error occured while getting   Service Requests||Caller:ServiceRequestController /SearchAllServiceRequest   || [ServiceRequestService][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#Notification007-7-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            var metadata = new
            {
                serviceRequestHistoryDto.TotalCount,
                serviceRequestHistoryDto.PageSize,
                serviceRequestHistoryDto.CurrentPage,
                serviceRequestHistoryDto.TotalPages,
                serviceRequestHistoryDto.HasNext,
                serviceRequestHistoryDto.HasPrevious
            };
            objectResponse.Message = new[] { JsonConvert.SerializeObject(metadata) };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(objectResponse);

        }

    }
}