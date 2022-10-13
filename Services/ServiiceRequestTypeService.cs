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
    public class ServiiceRequestTypeService : IServiiceRequestTypeService
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<ServiiceRequestTypeService> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;

        public ServiiceRequestTypeService(IMemoryCache memoryCache, IDbController dbController, ILogger<ServiiceRequestTypeService> logger, IConvertDataTableToObject convertDataTableToObject)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _convertDataTableToObject = convertDataTableToObject;
        }
        //async Task HandleAsync(NotificationCommand command)
        public async Task AddServiceRequestTypeAsync(ServiceRequestTypeDto command)
        {
            command.id = Guid.NewGuid().ToString();
            object[] param = { command.id, command.request_type };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.InsertServiceRequestTypes, param);
            }
            catch (Exception ex)
            {

                _logger.LogError(" Server returned no result |Caller:ServiceRequestTypeController/CreateServiceRequestType|| [ServiiceRequestTypeService][Handle]", ex);
                throw new HandleGeneralException(400, "Creation failed");
            }


            var @event = new GenericCreatedEvent("New Notification created", command.id);
        }


        public async Task UpdateServiceRequestTypeAsync(ServiceRequestTypeDto command)
        {
            object[] param = { command.request_type, command.id };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.UpdateServiceRequestTypes, param);
            }
            catch (Exception ex)
            {

                _logger.LogError(" Server returned no result |Caller:ServiceRequestTypeController/UpdateServiceRequestTypeById|| [ServiiceRequestTypeService][Handle]", ex);
                throw new HandleGeneralException(400, "Creation failed");
            }


            var @event = new GenericCreatedEvent("New Notification created", command.id);
        }

        public async Task<ServiceRequestTypeDto> GetServiceRequestTypeByIdAsync(ServiceRequestTypeDto command)
        {

            //SelectOneotificationType
            object[] param = { command.id };


            var entity = await _dbController.SQLFetchAsync(Sql.SelectOneServiceRequestType, param);
            if (entity.Rows.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:ServiceRequestTypeController/GetServiceRequestTypeById -Get|| [ServiiceRequestTypeService][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            ServiceRequestTypeDto serviceRequestTypeDto = new ServiceRequestTypeDto();
            serviceRequestTypeDto = _convertDataTableToObject.ConvertDataTable<ServiceRequestTypeDto>(entity).FirstOrDefault();

            return serviceRequestTypeDto;
        }


        public async Task<List<ServiceRequestTypeDto>> GetServiceRequestTypesAsync()
        {
            var getEntity = await _dbController.SQLFetchAsync(Sql.SelectServiceRequestTypes);
            var entity = getEntity.AsEnumerable().ToList(); ;
            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:ServiceRequestTypeController/GetAllServiceRequestType-Get|| [ServiiceRequestTypeService][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }

            var selectnotificationTypes = _convertDataTableToObject.ConvertDataRowList<ServiceRequestTypeDto>(entity).ToList();


            return selectnotificationTypes;
        }

    }
}
