
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using Convey.MessageBrokers;
//using Convey.MessageBrokers.Outbox;
using DbFactory;
 
using InBranchNotification.DbFactory;
using InBranchNotification.Domain;
using InBranchNotification.DTOs;
using InBranchNotification.Events;
using InBranchNotification.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace InBranchNotification.Queries.Branches.handler
{
    public class SingleNotificationHandler : IQueryHandler<NotificationQuery, SingleNotificationDto>
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<SingleNotificationHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
     //   private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
    //    private readonly IMessageOutbox _outbox;
        public SingleNotificationHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<SingleNotificationHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox)//, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
            //_publisher = publisher;
            //_outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task<SingleNotificationDto> HandleAsync(NotificationQuery query)
        {

            object[] param = { query.id };
            var entity = await _dbController.SQLFetchAsync(Sql.SelectOneNotification, param);

            if (entity.Rows.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:RolesController/GetRolById -Get|| [SingleBranchHandler][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            var singleNotificationDto = new SingleNotificationDto();

             
            foreach (var item in entity.AsEnumerable())
            {

                singleNotificationDto.title = item.Field<string>("id") != null ? Convert.ToString(item.Field<string>("id")) : null;
                singleNotificationDto.title = item.Field<string>("title") != null ? Convert.ToString(item.Field<string>("title")) : null;
                singleNotificationDto.notification_date = item.Field<DateTime>("notification_date") != null ? Convert.ToDateTime(item.Field<DateTime>("notification_date")) : null;
                singleNotificationDto.sender = item.Field<string>("sender") != null ? Convert.ToString(item.Field<string>("sender")) : "";
                singleNotificationDto.completed =   Convert.ToBoolean(item.Field<Boolean>("completed")) ;
                singleNotificationDto.recipents = item.Field<string>("recipents") != null ? JsonSerializer.Deserialize<List<string>>(item.Field<string>("recipents")) : null;
                singleNotificationDto.type = item.Field<string>("type") != null ? Convert.ToString(item.Field<string>("type")) : null;
                singleNotificationDto.body = item.Field<string>("body") != null ? Convert.ToString(item.Field<string>("body")) : null;
                
                //JsonSerializer.Deserialize(item.Field<string>("sender"),);
            }

         //   singleNotificationDto = _convertDataTableToObject.ConvertDataTable<SingleNotificationDto>(entity).FirstOrDefault();

            return singleNotificationDto;
        }

      
    }
}