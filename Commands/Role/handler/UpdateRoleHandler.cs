
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;
using DbFactory;
using InBranchDashboard.Commands.Roles;
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

namespace InBranchDashboard.Commands.Roles.handler
{
    public class UpdateRoleHandler : ICommandHandler<UpdateRoleCommand>
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<UpdateRoleHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
        private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
        private readonly IMessageOutbox _outbox;

        public UpdateRoleHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<UpdateRoleHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer, IMessageOutbox outbox, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
            _publisher = publisher;
            _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task HandleAsync(UpdateRoleCommand command)
        {


            object[] param = { command.id };

            var categorySearch = await _dbController.SQLFetchAsync(Sql.SelectOneRole, param);
            if (categorySearch.Rows.Count == 0)
            {
                _logger.LogError("Error: Server returned no result |Caller:RolesController/DeleteRole || [DeleteRoleHandler][Handle]");
                throw new HandleGeneralException(500, "The RoleId not valid");
            }
            int entity;
            try
            {
                object[] param1 = { command.role_name,command.category_id,command.id  };
                entity = await _dbController.SQLExecuteAsync(Sql.UpdateRole, param1);
            }
            catch (Exception ex)
            {

                _logger.LogError("ex syetem error stack: {ex}Error: Server returned no result |Caller:RoleController/UpdateCategory|| [UpdateRoleHandler][Handle]", ex);
                throw new HandleGeneralException(500, "Update failed");
            }

            var spanContext = _tracer.ActiveSpan.Context.ToString();

            var @event = new GenericCreatedEvent(" Role Updated", command.id);

            if (_outbox.Enabled)
            {
                await _outbox.SendAsync(@event, spanContext: spanContext);
                return;
            }
            await _publisher.PublishAsync(@event, spanContext: spanContext);

        }


    }
}
