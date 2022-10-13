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
    public class ServiceRequestStatusService : IServiceRequestStatusService
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<ServiceRequestStatusService> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;

        public ServiceRequestStatusService(IMemoryCache memoryCache, IDbController dbController, ILogger<ServiceRequestStatusService> logger, IConvertDataTableToObject convertDataTableToObject)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _convertDataTableToObject = convertDataTableToObject;
        }
        //async Task HandleAsync(NotificationCommand command)  
        public async Task AddServiceRequestStatusAsync(ServiceRequestStatusDTO command)
        {
            command.id = Guid.NewGuid().ToString();
            object[] param = { command.id, command.status };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.InsertServiceRequestStatus, param);
            }
            catch (Exception ex)
            {
                //
                _logger.LogError(" Server returned no result |Caller:ServiceRequestStatusController/CreateServiceRequestStatus|| [ServiiceRequestStatusService][Handle]", ex);
                throw new HandleGeneralException(400, "Creation failed");
            }


            var @event = new GenericCreatedEvent("New Notification created", command.id);
        }


        public async Task UpdateServiceRequestStatusAsync(ServiceRequestStatusDTO command)
        {
            object[] param = { command.status, command.id };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.UpdateServiceRequestStatus, param);
            }
            catch (Exception ex)
            {

                _logger.LogError(" Server returned no result |Caller:ServiceRequestStatusController/UpdateServiceRequestStatusById|| [ServiiceRequestStatusService][Handle]", ex);
                throw new HandleGeneralException(400, "Creation failed");
            }


            var @event = new GenericCreatedEvent("New Notification created", command.id);
        }

        public async Task<ServiceRequestStatusDTO> GetServiceRequestStatusByIdAsync(ServiceRequestStatusDTO command)
        {

            //SelectOneotificationStatus
            object[] param = { command.id };


            var entity = await _dbController.SQLFetchAsync(Sql.SelectOneServiceRequestStatus, param);
            if (entity.Rows.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:ServiceRequestStatusController/GetServiceRequestStatusById -Get|| [ServiiceRequestStatusService][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            ServiceRequestStatusDTO serviceRequestStatusDto = new ServiceRequestStatusDTO();
            serviceRequestStatusDto = _convertDataTableToObject.ConvertDataTable<ServiceRequestStatusDTO>(entity).FirstOrDefault();

            return serviceRequestStatusDto;
        }


        public async Task<List<ServiceRequestStatusDTO>> GetServiceRequestStatussAsync()
        {
            var getEntity = await _dbController.SQLFetchAsync(Sql.SelectServiceRequestStatus);
            var entity = getEntity.AsEnumerable().ToList(); ;
            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:ServiceRequestStatusController/GetAllServiceRequestStatus-Get|| [ServiiceRequestStatusService][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }

            var selectnotificationStatuss = _convertDataTableToObject.ConvertDataRowList<ServiceRequestStatusDTO>(entity).ToList();


            return selectnotificationStatuss;
        }

    }
}
