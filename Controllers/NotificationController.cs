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
    [Authorize]
    [Route("api/[controller]")]
    public class NotificationController  : Controller
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ILogger<NotificationController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;
        private readonly IBaseUrlService _baseUrlService;
        public NotificationController(IBaseUrlService baseUrlService, IHttpContextAccessor accessor, ILogger<NotificationController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _mapper = mapper;
            _accessor = accessor;
            _baseUrlService = baseUrlService;
        }


  
        [HttpGet("SearchAllNotification/{PageNumber}/{PageSize}")]

        // [Authorize(Roles = "Trustee")]
        // [Authorize(Roles = "Trustee")]
        public async Task<ActionResult<ObjectResponse>> SearchAllNotification(int PageNumber, int PageSize, string id, string title, string type, DateTime? from_entry_date, DateTime? to_entry_date, string sender, string body, bool completed)
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
            var addAuditItem = await _baseUrlService.AddAuditItem(audit, userAgent);

            //AD users
            var notificationSearchParameters = new NotificationSearchParameters()
            {
                PageSize = PageSize,
                PageNumber = PageNumber,
                id = id,
                body = body,
                from_entry_date  = from_entry_date,
                to_entry_date = to_entry_date,
                sender = sender,
                completed = completed,
                title = title,
                type = type


            };
            var notificationDTO = new PagedList<NotificationSearchDTO>();
            var objectResponse = new ObjectResponse();
            try
            {
                var getAllNotificationQuery = new GetAllNotificationSearchQuery(notificationSearchParameters);
                notificationDTO = await _queryDispatcher.QueryAsync(getAllNotificationQuery);
                objectResponse.Data = notificationDTO;
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
                    _logger.LogError(" [#Notification001-1-C] Server Error occured while getting all ADUser ||Caller:NotificationController /SearchAllNotification  || [GetAllNotificationSearcHandler][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#AdUser001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#Notification001-1-C] Server Error occured while getting all ADUser||Caller:NotificationController /SearchAllNotification   || [GetAllNotificationSearcHandler][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#Notification001-1-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            var metadata = new
            {
                notificationDTO.TotalCount,
                notificationDTO.PageSize,
                notificationDTO.CurrentPage,
                notificationDTO.TotalPages,
                notificationDTO.HasNext,
                notificationDTO.HasPrevious
            };
            objectResponse.Message = new[] { JsonConvert.SerializeObject(metadata) };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(objectResponse);

        }


 
        [HttpPost("CreateNotificationItem")]
        public async Task<ActionResult> CreateNotification([FromBody] NotificationDTO notificationDTO)
        {
            //Audit Item

            var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            var claimsItems = HttpContext.User.Claims;
            var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            var audit = new Audit();
            audit.inb_aduser_id = userName;
            audit.activity = "Create Notification";
            audit.activity_module = "NotificationController";
            audit.activity_submodule = "CreateNotification";
            var addAuditItem = await _baseUrlService.AddAuditItem(audit, userAgent);

            var objectResponse = new ObjectResponse();
            if (!ModelState.IsValid )
            {
                _logger.LogError("[#Notification002-2-C] Validation error {Notification} was not created ||Caller:NotificationController/CreateNotification  || [AddNotificationHandler][Handle]  ", notificationDTO.title +" User"+ notificationDTO.sender);
                var modeerror = ModelState.Values.SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage).ToArray();
                objectResponse.Error = new[] { "[#Notification002-2-C] Validation error {Notification} was not created ||Caller:NotificationController/CreateNotification  || [AddNotificationHandler][Handle]  ", notificationDTO.title + " User" + notificationDTO.sender };
                objectResponse.Error = objectResponse.Error.ToList().Union(modeerror).ToArray();
                 
                return BadRequest(objectResponse);
            }

            var command = new NotificationCommand
            {
                title = notificationDTO.title,
                body = notificationDTO.body,
                sender = notificationDTO.sender,
                type = notificationDTO.type,
                notification_date = DateTime.Now,
                completed= false, 
                
            };
            try
            {

                await _commandDispatcher.SendAsync(command);

                objectResponse.Success = true;

                objectResponse.Data = new { id = command.id };
                return CreatedAtAction(nameof(GetNotificationById), new { id = command.id }, objectResponse);
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

                    objectResponse.Error = new[] { "[#Notification002-2-C]", ex.InnerException.Message };
                    _logger.LogError("[#Notification002-2-C] Server Error occured Branch was not created for {Notification}||Caller:NotificationController/CreateBranch  || [AddNotificationHandler][Handle] error:{error}", notificationDTO.title + " User" + notificationDTO.sender, ex.InnerException.Message);
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }

                _logger.LogError("[#Notification002-2-C] Server Error occured Branch was not created for {Notification}||Caller:NotificationController/CreateBranch  || [AddNotificationHandler][Handle] error:{error}", notificationDTO.title + " User" + notificationDTO.sender, ex.Message);
                objectResponse.Error = new[] { "[#Notification002-2-C]", ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);

            }
        }

        [HttpGet("NotificationTypes")]
        public    ActionResult<ObjectResponse>   NotificationTypes()
        {
            //Audit Item

            var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            var claimsItems = HttpContext.User.Claims;
            var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            var audit = new Audit();
            audit.inb_aduser_id = userName;
            audit.activity = "Notification Types";
            audit.activity_module = "NotificationController";
            audit.activity_submodule = "NotificationTypes";
            var addAuditItem =  _baseUrlService.AddAuditItem(audit, userAgent);

            var objectResponse = new ObjectResponse();
            try
            {
                List<string> listType = new List<string>();
                listType.Add("SMS");
                listType.Add("Email");
                listType.Add("Push");
                objectResponse.Data = listType;
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
                    _logger.LogError(" [#Notification001-1-C] Server Error occured while getting all Notification Type ||Caller:NotificationController /NotificationTypes  }", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#AdUser001-1-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                     
                    return StatusCode(400, objectResponse);
                }
                _logger.LogError("[#Notification001-1-C] Server Error occured while getting all ADUser||Caller:NotificationController /SearchAllNotification   || [GetAllNotificationSearcHandler][Handle] error:{error}", ex.Message);
                objectResponse.Error = new[] { "[#Notification001-1-C]", ex.Message };

                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

          

            return Ok(objectResponse);

        }

        [HttpGet("GetNotificationById/{id}")]
        public async Task<ActionResult<ObjectResponse>> GetNotificationById(string id)
        {
            //Audit Item

            var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"];
            var claimsItems = HttpContext.User.Claims;
            var userName = claimsItems.First(claim => claim.Type == "UserName").Value;
            var audit = new Audit();
            audit.inb_aduser_id = userName;
            audit.activity = "Get Notification By Id";
            audit.activity_module = "NotificationController";
            audit.activity_submodule = "GetNotificationById";
            var addAuditItem = _baseUrlService.AddAuditItem(audit, userAgent);
            //AD Login
            var branch = new Branch();
            var objectResponse = new ObjectResponse();
            try
            {
                var branchQuery = new NotificationQuery(id);
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
                    _logger.LogError("[#Notification004-4-C] Server Error occured while getting all Branch ||Caller:NotificationController/GetNotificationById  || [NotificationQueries][Handle] error:{error}", ex.InnerException.Message);


                    objectResponse.Error = new[] { "[#Notification004-4-C]", ex.InnerException.Message };

                    objectResponse.Message = new[] { resp.ToString() };
                    return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
                }
                _logger.LogError("[#Branch002-4-C] Server Error occured while getting a Notification||Caller:NotificationController/GetNotificationById  || [NotificationQueries][Handle] error:{error}", ex.Message);

                objectResponse.Error = new[] { "[#Notification004-4-C]", ex.Message };


                return StatusCode(StatusCodes.Status400BadRequest, objectResponse);
            }

            return Ok(objectResponse);

        }

    }
}