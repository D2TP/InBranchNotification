
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
using InBranchNotification.Helpers;
 
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InBranchNotification.Queries.Branches.handler
{ 

    public class NotificationsQueryHandler : IQueryHandler<NotificationQueries, PagedList<Notification>>
{

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<NotificationsQueryHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
       // private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
    //    private readonly IMessageOutbox _outbox;
        public NotificationsQueryHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<NotificationsQueryHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox)//, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
       //     _publisher = publisher;
        //    _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task<PagedList<Notification>> HandleAsync(NotificationQueries query)
    {
            var getEntity = await _dbController.SQLFetchAsync(Sql.SelectNotifications);
            var entity = getEntity.AsEnumerable().OrderBy(on => on.Field<string>("sender"))
 .ToList();
            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:RegionController/GetAllCatigories-Get|| [CategoryQueryHandler][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
           
            var notifications = _convertDataTableToObject.ConvertDataRowList<Notification>(entity).AsQueryable();
          var notificationItems = PagedList<Notification>.ToPagedList(notifications,
             query._queryStringParameters.PageNumber,
             query._queryStringParameters.PageSize);


            return notificationItems;
        }
}
}
