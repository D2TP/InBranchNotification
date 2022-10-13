using DbFactory;
using InBranchNotification.DTOs;
using InBranchNotification.Events;
using InBranchNotification.Exceptions;
using System.Threading.Tasks;
using System;
using InBranchNotification.DbFactory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using InBranchNotification.Domain;
using InBranchNotification.Helpers;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace InBranchNotification.Services
{
    public class ServiceRequestHistory : IServiceRequestHistory
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<ServiceRequestHistory> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;

        public ServiceRequestHistory(IMemoryCache memoryCache, IDbController dbController, ILogger<ServiceRequestHistory> logger, IConvertDataTableToObject convertDataTableToObject)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _convertDataTableToObject = convertDataTableToObject;
        }
        public async Task AddServiceRequestHistoryAsync(ServiceHistory command)
        {

            command.id = Guid.NewGuid().ToString();
            object[] param = { command.id, command.actor, command.activity, DateTime.Now, command.comment, command.service_request_id, command.status };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.InsertServiceRequestHistory, param);
            }
            catch (Exception ex)
            {

                _logger.LogError(" Creation of Hostory failed |Caller:Actors/AddServiceRequestHistoryAsync|| [ServiceRequestHistory][Handle]", ex);
                throw new HandleGeneralException(400, "Creation failed");
            }


            var @event = new GenericCreatedEvent("New Notification created", command.id);
        }
        public async Task<PagedList<ServiceRequestHistoryDto>> SearchAllServiceRequestHistoryAsync(ServiceHistroySearchDto query)
        {

            object[] param = {
                    query.id!=null? query.id : DBNull.Value,
                    query.actor!=null? query.actor : DBNull.Value,
                    query.status!=null? query.status : DBNull.Value,
                      query.from_entry_date!=null? query.from_entry_date : DBNull.Value,
                    query.to_entry_date!=null? query.to_entry_date : DBNull.Value ,
            query.service_request_id != null ? query.service_request_id : DBNull.Value,
            query.comment != null ? query.comment : DBNull.Value,
            query.activity != null ? query.activity : DBNull.Value,
            };

        

            var getItems = await _dbController.SQLFetchAsync(Sql.StoredProcSearcServiceHistoryRequest, param);

            var entity = getItems.AsEnumerable().OrderBy(on => on.Field<DateTime?>("activity_date")).ToList();

            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:ServiceRequestController/SearchAllServiceRequestHistory -Get|| [ServiceRequestHistory][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            var steList = new List<ServiceRequestHistoryDto>();
            foreach (var item in entity)
            {

 
                var serviceRequestDetail = new ServiceRequestHistoryDto();
                serviceRequestDetail.id = item.Field<string>("id") != null ? Convert.ToString(item.Field<string>("id")) : null;
                serviceRequestDetail.activity = item.Field<string>("activity") != null ? Convert.ToString(item.Field<string>("activity")) : null;
                
                serviceRequestDetail.activity_date = Convert.ToDateTime(item.Field<DateTime?>("activity_date"));
             
               
                serviceRequestDetail.actor = item.Field<string>("actor") != null ? Convert.ToString(item.Field<string>("actor")) : "";
                serviceRequestDetail.comment = item.Field<string>("comment") != null ? Convert.ToString(item.Field<string>("comment")) : "";
                serviceRequestDetail.service_request_id = item.Field<string>("service_request_id") != null ? Convert.ToString(item.Field<string>("service_request_id")) : "";
                serviceRequestDetail.status = item.Field<string>("status") != null ? Convert.ToString(item.Field<string>("status")) : "";
                serviceRequestDetail.service_status = item.Field<string>("service_status") != null ? Convert.ToString(item.Field<string>("service_status")) : "";
                serviceRequestDetail.request_type = item.Field<string>("request_type") != null ? Convert.ToString(item.Field<string>("request_type")) : "";
                 
                steList.Add(serviceRequestDetail);

            }




            var pagelist = PagedList<ServiceRequestHistoryDto>.ToPagedList(steList.AsQueryable(),
         query.PageNumber,
         query.PageSize);

            return pagelist;


        }
    }
}
