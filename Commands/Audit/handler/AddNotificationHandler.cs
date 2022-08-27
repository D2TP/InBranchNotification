
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using Convey.MessageBrokers;
////using Convey.MessageBrokers.Outbox;
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

namespace InBranchNotification.Commands.Audit.handler
{
    public class AddNotificationHandler : ICommandHandler<NotificationCommand>
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<AddNotificationHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
        //  private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
        //  private readonly IMessageOutbox _outbox;

        public AddNotificationHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<AddNotificationHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
            //  _publisher = publisher;
            //   _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task HandleAsync(NotificationCommand command)
        {


            command.id = Guid.NewGuid().ToString();

            //id,title,type,notification_date,sender,body,completed

            object[] param = { command.id,command.title, command.type, command.notification_date, command.sender, command.body, command.completed};


          
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.InsertNotification, param);
            }
            catch (Exception ex)
            {

                _logger.LogError(" Server returned no result |Caller:NotificationController/CreateNotification|| [AddNotificationHandler][Handle]", ex);
                throw new HandleGeneralException(400, "Creation failed");
            }

            var spanContext = _tracer.ActiveSpan.Context.ToString();

            var @event = new GenericCreatedEvent("New Notification created", command.id);

            //if (_outbox.Enabled)
            //{
            //    await _outbox.SendAsync(@event, spanContext: spanContext);
            //    return;
            //}
            // await _publisher.PublishAsync(@event, spanContext: spanContext);

        }


    }
}
