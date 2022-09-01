
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InBranchNotification.Queries.Branches.handler
{
    public class SingleNotificationHandler : IQueryHandler<NotificationQuery, Notification>
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

        public async Task<Notification> HandleAsync(NotificationQuery query)
        {

            object[] param = { query.id };
            var entity = await _dbController.SQLFetchAsync(Sql.SelectOneNotification, param);
            if (entity.Rows.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:RolesController/GetRolById -Get|| [SingleBranchHandler][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            var notification = new Notification();
            notification = _convertDataTableToObject.ConvertDataTable<Notification>(entity).FirstOrDefault();

            return notification;
        }

      
    }
}