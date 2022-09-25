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
    public class NotificationTypeService : INotificationTypeService
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<NotificationTypeService> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;

        public NotificationTypeService(IMemoryCache memoryCache, IDbController dbController, ILogger<NotificationTypeService> logger, IConvertDataTableToObject convertDataTableToObject)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _convertDataTableToObject = convertDataTableToObject;
        }
        //async Task HandleAsync(NotificationCommand command)
        public async Task AddNotificationTypeAsync(NotificationTypeDTO command)
        {
            command.id = Guid.NewGuid().ToString();
            object[] param = { command.id, command.notification_type };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.InsertNotificationTypes, param);
            }
            catch (Exception ex)
            {

                _logger.LogError(" Server returned no result |Caller:NotificationTypeController/CreateTypeNotification|| [NotificationTypeService][Handle]", ex);
                throw new HandleGeneralException(400, "Creation failed");
            }


            var @event = new GenericCreatedEvent("New Notification created", command.id);
        }


        public async Task UpdateNotificationTypeAsync(NotificationTypeDTO command)
        {
            object[] param = {  command.notification_type, command.id };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.UpdateNotificationTypes, param);
            }
            catch (Exception ex)
            {

                _logger.LogError(" Server returned no result |Caller:NotificationTypeController/CreateTypeNotification|| [NotificationTypeService][Handle]", ex);
                throw new HandleGeneralException(400, "Creation failed");
            }


            var @event = new GenericCreatedEvent("New Notification created", command.id);
        }

        public async Task ApproveNotificationAsync(Approve command)
        {
            object[] param = { command.approved, command.approver,command.completed, command.id };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.ApproveNotification, param);
            }
            catch (Exception ex)
            {

                _logger.LogError(" Server returned no result |Caller:NotificationController/ApproveNotification|| [NotificationTypeService][Handle]", ex);
                throw new HandleGeneralException(400, "Approval failed");
            }


            var @event = new GenericCreatedEvent("New Approval updated", command.id);
        }


        public async Task<NotificationTypeDTO> GetNotificationTypeByIdAsync(NotificationTypeDTO notificationTypeDTOpram)
        {

            //SelectOneotificationType
            object[] param = { notificationTypeDTOpram.id };


            var entity = await _dbController.SQLFetchAsync(Sql.SelectOneotificationType, param);
            if (entity.Rows.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:NotificationTypeController/GetRolById -Get|| [NotificationTypeService][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            NotificationTypeDTO notificationTypeDTO = new NotificationTypeDTO();
            notificationTypeDTO = _convertDataTableToObject.ConvertDataTable<NotificationTypeDTO>(entity).FirstOrDefault();

            return notificationTypeDTO;
        }


        public async Task<List<NotificationTypeDTO>> GetNotificationTypesAsync()
        {
            var getEntity = await _dbController.SQLFetchAsync(Sql.SelectnotificationTypes);
            var entity = getEntity.AsEnumerable().ToList(); ;
            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:NotificationTypeController/GetAllNotificationTypes-Get|| [NotificationTypeService][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }

            var selectnotificationTypes = _convertDataTableToObject.ConvertDataRowList<NotificationTypeDTO>(entity).ToList();


            return selectnotificationTypes;
        }

    }
}
