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
    public class NotificationTypeController : Controller
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<NotificationTypeController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;
        private readonly IBaseUrlService _baseUrlService;
        private readonly INotificationTypeService _notificationTypeService;
    
      
    public NotificationTypeController(INotificationTypeService notificationTypeService,IBaseUrlService baseUrlService, IHttpContextAccessor accessor, ILogger<NotificationTypeController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
            _accessor = accessor;
            _baseUrlService = baseUrlService;
            _notificationTypeService = notificationTypeService;
        }


  
        [HttpGet("GetAllNotificationTypes")]

        // [Authorize(Roles = "Trustee")]
        // [Authorize(Roles = "Trustee")]
        public async Task<ActionResult<ObjectResponse>> GetAllNotificationTypes( )
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

            var notificationTypesDTO = new List<NotificationTypeDTO>();
            var objectResponse = new ObjectResponse();
            try
            {
                
                notificationTypesDTO = await _notificationTypeService.GetNotificationTypesAsync();
                objectResponse.Data = notificationTypesDTO;
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
                    _logger.LogError(" [#NotificationType001-1-C] Server Error occured while getting all Notification ||Caller:NotificationTypeController /GetAllNotificationTypes  || [NotificationTypeService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#AdUser001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#NotificationType001-1-C] Server Error occured while getting all NotificationType||Caller:NotificationController /GetAllNotificationTypes   || [NotificationTypeService][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#Notification001-1-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            
         

            return Ok(objectResponse);

        }


 
        [HttpPost("CreateNotificationTypeItem")]
        public async Task<ActionResult> CreateNotificationType([FromBody] string notificationType)
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
                _logger.LogError("[#NotificationType002-2-C] Validation error {NotificationType} was not created ||Caller:NotificationTypeController/CreateNotificationType  || [NotificationTypeService][Handle]  ", notificationType +" User"+ notificationType);
                var modeerror = ModelState.Values.SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage).ToArray();
                objectResponse.Error = new[] { "[#NotificationType002-2-C] Validation error {NotificationType} was not created ||Caller:NotificationTypeController/CreateNotificationType  || [NotificationTypeService][Handle]  ", notificationType + " User" + notificationType };
                objectResponse.Error = objectResponse.Error.ToList().Union(modeerror).ToArray();
                 
                return BadRequest(objectResponse);
            }

            
            try
            {
                var notificationTypeDto = new NotificationTypeDTO();
                notificationTypeDto.notification_type=notificationType;
                await _notificationTypeService.AddNotificationTypeAsync(notificationTypeDto);

                objectResponse.Success = true;

                objectResponse.Data = new { id = notificationTypeDto.id };
                return CreatedAtAction(nameof(GetNotificationTypeById), new { id = notificationTypeDto.id }, objectResponse);
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

                    objectResponse.Error = new[] { "[#NotificationType002-2-C]", ex.InnerException.Message };
                    _logger.LogError("[#NotificationType002-2-C] Server Error occured Branch was not created for {NotificationType}||Caller:NotificationTypeController/CreateBranch  || [AddNotificationHandler][Handle] error:{error}", notificationType + " User" + notificationType, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }

                _logger.LogError("[#NotificationType002-2-C] Server Error occured Branch was not created for {NotificationType}||Caller:NotificationTypeController/CreateBranch  || [AddNotificationHandler][Handle] error:{error}", notificationType + " User" + notificationType, ex.Message);
                objectResponse.Error = new[] { "[#NotificationType002-2-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }
         
        [HttpGet("GetNotificationTypeById/{id}")]
        public async Task<ActionResult<ObjectResponse>> GetNotificationTypeById(string id)
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
            //AD Login
            var branch = new Branch();
            var objectResponse = new ObjectResponse();
            try
            {
                var notificationTypeDto = new NotificationTypeDTO();
                notificationTypeDto.id = id;
                objectResponse.Data = await _notificationTypeService.GetNotificationTypeByIdAsync(notificationTypeDto);
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
                    _logger.LogError("[#NotificationType003-3-C] Server Error occured while getting all Notification Type by Id||Caller:NotificationTypeController/GetNotificationTypeById  || [NotificationTypeService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#NotificationType003-3-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#NotificationType003-3-C] Server Error occured while getting a NotificationType||Caller:NotificationTypeController/GetNotificationTypeById  || [NotificationTypeService][Handle] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#NotificationType003-3-C", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }


        [HttpPut("UpdateNotificationType")]
        public async Task<ActionResult<ObjectResponse>> UpdateNotificationById([FromBody] NotificationTypeDTO notificationTypeDTO)
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
            //AD Login
            var branch = new Branch();
            var objectResponse = new ObjectResponse();
            try
            {
                var notificationTypeDto = new NotificationTypeDTO();
                notificationTypeDto.id = notificationTypeDTO.id;
                notificationTypeDto.notification_type = notificationTypeDTO.notification_type;
                await _notificationTypeService.UpdateNotificationTypeAsync(notificationTypeDto);
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
                    _logger.LogError("[#NotificationType004-4-C] Server Error occured while updating a  Notification Type ||Caller:NotificationTypeController/UpdateNotificationType  || [NotificationTypeService][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#Notification004-4-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#NotificationType004-4-C] Server Error occured while updating  a Notification Type||Caller:NotificationTypeController/UpdateNotificationType  || [NotificationTypeService] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#NotificationType004-4-C]", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }

    }
}