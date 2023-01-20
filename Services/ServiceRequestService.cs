using DbFactory;
using InBranchNotification.DbFactory;
using InBranchNotification.Exceptions;
using InBranchNotification.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using InBranchNotification.DTOs;
using InBranchNotification.Events;
using InBranchAuditTrail.DTOs;
using InBranchNotification.Queries.Notifications;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using InBranchNotification.Domain;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InBranchNotification.Services
{
    public class ServiceRequestService : IServiceRequestService
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<ServiceRequestService> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
        private readonly IServiceRequestHistory _serviceRequestHistory;
        
        public ServiceRequestService(IServiceRequestHistory serviceRequestHistory,IMemoryCache memoryCache, IDbController dbController, ILogger<ServiceRequestService> logger, IConvertDataTableToObject convertDataTableToObject)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _convertDataTableToObject = convertDataTableToObject;
            _serviceRequestHistory = serviceRequestHistory;
        }

        public async Task AddServiceRequestAsync(ServiceRequestCreateDto command)
        {

            command.id = Guid.NewGuid().ToString();
            object[] param = { command.id, command.service_request_type_id, command.service_request_status_id, command.client, command.cif_id, DateTime.Now };
            int entity;   
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.InsertServiceRequest, param);
                //Create History
                var history = new ServiceHistory();
                history.service_request_id = command.id;
                history.actor = command.cif_id;
                history.activity = command.service_request_type_id;
                history.status=command.service_request_status_id;
                history.comment = command.comment;
                history.id= Guid.NewGuid().ToString();
                history.activity_date = DateTime.Now;
                await  _serviceRequestHistory.AddServiceRequestHistoryAsync(history);
            }
            catch (Exception ex)
            {

                _logger.LogError(" Server returned no result |Caller:ServiceRequestController/CreateTypeNotification|| [ServiceRequestService][Handle]", ex);
                throw new HandleGeneralException(400, "Creation failed");
            }


            var @event = new GenericCreatedEvent("New Notification created", command.id);
        }

        public async Task<PagedList<ServiceRequestDetail>> GetAllServiceRequestAsync(ServiceRequestSearch query)
        {

            object[] param = {
                    query.id!=null? query.id : DBNull.Value,
                    query.service_request_status_id!=null? query.service_request_status_id : DBNull.Value,
                    query.service_request_type_id!=null? query.service_request_type_id : DBNull.Value,
                    query.client!=null? query.client : DBNull.Value,
                    query.cif_id!=null? query.cif_id : DBNull.Value,
                    query.approver!=null? query.approver : DBNull.Value,
                     query.reviewer!=null? query.reviewer : DBNull.Value,
                    query.fromdate!=null? query.fromdate : DBNull.Value,
                    query.todate!=null? query.todate : DBNull.Value };



            var getItems = await _dbController.SQLFetchAsync(Sql.StoredProcSearcServiceRequest, param);

            var entity = getItems.AsEnumerable().OrderBy(on => on.Field<string>("client")).ToList();

            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:SearchAllServiceRequest/GetServiceRequestByIdAsync -Get|| [ServiceRequestService][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            var steList = new List<ServiceRequestDetail>();
            foreach (var item in entity)
            {
                var serviceRequestDetail = new ServiceRequestDetail();
                serviceRequestDetail.client = item.Field<string>("client") != null ? Convert.ToString(item.Field<string>("client")) : null;
                serviceRequestDetail.reviewer = item.Field<string>("reviewer") != null ? Convert.ToString(item.Field<string>("reviewer")) : null;
                serviceRequestDetail.service_request_date =  item.Field<DateTime?>("service_request_date") != null ? Convert.ToDateTime(item.Field<DateTime>("service_request_date")) : null;
                serviceRequestDetail.review_date = item.Field<DateTime?>("review_date") != null ? Convert.ToDateTime(item.Field<DateTime>("review_date")) : null;
                serviceRequestDetail.approval_date = item.Field<DateTime?>("approval_date") != null ? Convert.ToDateTime(item.Field<DateTime>("approval_date")) : null;
                serviceRequestDetail.status = item.Field<string>("status") != null ? Convert.ToString(item.Field<string>("status")) : "";
                serviceRequestDetail.cif_id = item.Field<string>("cif_id") != null ? Convert.ToString(item.Field<string>("cif_id")) : "";
                serviceRequestDetail.id = item.Field<string>("id") != null ? Convert.ToString(item.Field<string>("id")) : "";
                serviceRequestDetail.approver = item.Field<string>("approver") != null ? Convert.ToString(item.Field<string>("approver")) : "";
                serviceRequestDetail.request_type = item.Field<string>("request_type") != null ? Convert.ToString(item.Field<string>("request_type")) : "";
              
                
                steList.Add(serviceRequestDetail);

            }




            var pagelist = PagedList<ServiceRequestDetail>.ToPagedList(steList.AsQueryable(),
         query.PageNumber,
         query.PageSize);

            return pagelist;


        }

        public async Task<ServiceRequestDetail> GetServiceRequestByIdAsync(string id)
        {

            //SelectOneotificationType
            object[] param = { id };


            var entity = await _dbController.SQLFetchAsync(Sql.SelectOneServiceRequest, param);
            if (entity.Rows.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:ServiceRequestController/GetServiceRequestByIdAsync -Get|| [ServiceRequestService][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            ServiceRequestDetail notificationTypeDTO = new ServiceRequestDetail();
            notificationTypeDTO = _convertDataTableToObject.ConvertDataTable<ServiceRequestDetail>(entity).FirstOrDefault();

            return notificationTypeDTO;
        }

        public async Task<ServiceRequest> GetServiceRequestByIdNoJoinsAsync(string id)
        {

            //SelectOneotificationType
            object[] param = { id };


            var entity = await _dbController.SQLFetchAsync(Sql.SelectRequest, param);
            if (entity.Rows.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:ServiceRequestController/GetServiceRequestByIdAsync -Get|| [ServiceRequestService][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            var serviceRequest = new ServiceRequest();
            serviceRequest = _convertDataTableToObject.ConvertDataTable<ServiceRequest>(entity).FirstOrDefault();

            return serviceRequest;
        }
        public async Task UpdateServiceRequestAsync(NotificationTypeDTO command)
        {
            object[] param = { command.notification_type, command.id };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.UpdateNotificationTypes, param);
            }
            catch (Exception ex)
            {

                _logger.LogError(" Server returned no result |Caller:ServiceRequestController/CreateTypeNotification|| [ServiceRequestService][Handle]", ex);
                throw new HandleGeneralException(400, "Creation failed");
            }


            var @event = new GenericCreatedEvent("New Notification created", command.id);
        }

        public async Task ApproveServiceRequestAsync(ApproveServiceRequest command)
        { 
            object[] param = { command.service_request_status_id, command.approver,DateTime.Now, command.id };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.ApproveServiceRequest, param);
                //Create History
                var history = new ServiceHistory();
                history.service_request_id = command.id;
                history.actor = command.approver;
                var requestDetail = await this.GetServiceRequestByIdNoJoinsAsync(command.id);
                history.activity = requestDetail.service_request_type_id;
                history.status = command.service_request_status_id;
                history.comment = command.comment;
                history.id = Guid.NewGuid().ToString();
                history.activity_date = DateTime.Now;
                await _serviceRequestHistory.AddServiceRequestHistoryAsync(history);
            }
            catch (Exception ex)
            {
        _logger.LogError(" Server returned no result |Caller:ServiceRequestController/ApproveNotification|| [ServiceRequestService][Handle]", ex);
                throw new HandleGeneralException(400, "Approval failed");
            }


            var @event = new GenericCreatedEvent("New Approval updated", command.id);
        }
     
        public async Task ReviewServiceRequestAsync(ReviewServiceRequest command)
        {
            
            object[] param = { command.service_request_status_id, command.reviewer, DateTime.Now, command.id };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.ReviewServiceRequest, param);
                //Create History
                var history = new ServiceHistory();
                history.service_request_id = command.id;
                history.actor = command.reviewer;
                var requestDetail = await this.GetServiceRequestByIdNoJoinsAsync(command.id);
                history.activity = requestDetail.service_request_type_id;
                history.status = command.service_request_status_id;
                history.comment = command.comment;
                history.id = Guid.NewGuid().ToString();
                history.activity_date = DateTime.Now;
                await _serviceRequestHistory.AddServiceRequestHistoryAsync(history);
            }
            catch (Exception ex)
            {

                _logger.LogError(" Server returned no result |Caller:ServiceRequestController/ApproveNotification|| [ServiceRequestService][Handle]", ex);
                throw new HandleGeneralException(400, "Approval failed");
            }


            var @event = new GenericCreatedEvent("New Approval updated", command.id);
        }

        public async Task<List<NotificationTypeDTO>> GetServiceRequestAsync()
        {
            var getEntity = await _dbController.SQLFetchAsync(Sql.SelectnotificationTypes);
            var entity = getEntity.AsEnumerable().ToList(); ;
            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:ServiceRequestController/GetAllNotificationTypes-Get|| [ServiceRequestService][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }

            var selectnotificationTypes = _convertDataTableToObject.ConvertDataRowList<NotificationTypeDTO>(entity).ToList();


            return selectnotificationTypes;
        }

    }
}
