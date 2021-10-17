
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using Convey.MessageBrokers;
//using Convey.MessageBrokers.Outbox;
using DbFactory;
using InBranchDashboard.Commands.UserRole;
using InBranchDashboard.DbFactory;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Events;
using InBranchDashboard.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.Permissions.handler
{
    public class UpdatePermissionHandler : ICommandHandler<UpdatePermissionCommand>
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<UpdatePermissionHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
    //    private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
//        private readonly IMessageOutbox _outbox;

        public UpdatePermissionHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<UpdatePermissionHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
        //    _publisher = publisher;
         //   _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task HandleAsync(UpdatePermissionCommand command)
        {


            object[] param = { command.id };

            var PermissionSearch = await _dbController.SQLFetchAsync(Sql.SelectOnePermission, param);
            if (PermissionSearch.Rows.Count == 0)
            {
                _logger.LogError("Error: Server returned no result |Caller:PermissionController/DeletePermission || [DeletePermissionHandler][Handle]");
                throw new HandleGeneralException(500, "The PermissionId not valid");
            }
            int entity;
            try
            {
                object[] param1 = { command.permission_name,command.id,  };
                entity = await _dbController.SQLExecuteAsync(Sql.UpdatePermission, param1);
            }
            catch (Exception ex)
            {

                _logger.LogError("ex syetem error stack: {ex}Error: Server returned no result |Caller:PermissionController/UpdatePermission|| [UpdatePermissionHandler][Handle]", ex);
                throw new HandleGeneralException(500, "Update failed");
            }

            var spanContext = _tracer.ActiveSpan.Context.ToString();

            var @event = new GenericCreatedEvent(" Permission Updated", command.id);

            //if (_outbox.Enabled)
            //{
            //    await _outbox.SendAsync(@event, spanContext: spanContext);
            //    return;
            //}
          //  await _publisher.PublishAsync(@event, spanContext: spanContext);

        }


    }
}
