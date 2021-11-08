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
 

namespace InBranchDashboard.Commands.RolePriviledges.handler
{
    public class DeleteRolePrivledegeHandler : ICommandHandler<RolePriviledgeDeleteCommand>
    {
        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        private readonly ILogger<DeleteRolePrivledegeHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
        private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
      //  private readonly IMessageOutbox _outbox;

        public DeleteRolePrivledegeHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<DeleteRolePrivledegeHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
            //_publisher = publisher;
            //_outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task HandleAsync(RolePriviledgeDeleteCommand command)
        {


            object[] param = { command.id };

            var PriviledgeSearch = await _dbController.SQLFetchAsync(Sql.SelectOneRolePriviledge, param);
            if (PriviledgeSearch.Rows.Count == 0)
            {
                _logger.LogError("Error: Server returned no result |Caller:RolePriviledgeController/DeleteRolePriviledge || [DeleteRolePriviledgeHandler][Handle]");
                throw new HandleGeneralException(500, "The Role, Priviledge and Permission were not valid");
            }
            int entity;
            try
            {

                entity = await _dbController.SQLExecuteAsync(Sql.DeleteRolePriviledge, param);
            }
            catch (Exception ex)
            {

                _logger.LogError("ex syetem error stack: {ex}Error: Server returned no result |Caller:RolePriviledgeController/DeleteRolePriviledge|| [DeleteRolePriviledgeHandler][Handle]", ex);
                throw new HandleGeneralException(500, "Delete failed");
            }

            var spanContext = _tracer.ActiveSpan.Context.ToString();

            var @event = new GenericCreatedEvent("New Role, Priviledge and Permission created", command.id);

            //if (_outbox.Enabled)
            //{
            //    await _outbox.SendAsync(@event, spanContext: spanContext);
            //    return;
            //}
            //await _publisher.PublishAsync(@event, spanContext: spanContext);

        }
    }
}
