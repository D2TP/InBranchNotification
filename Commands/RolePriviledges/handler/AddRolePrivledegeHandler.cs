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
    public class AddRolePrivledegeHandler : ICommandHandler<RolePrivledegeCommand>
    {
        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        private readonly ILogger<AddRolePrivledegeHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
       // private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
        //private readonly IMessageOutbox _outbox;

        public AddRolePrivledegeHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<AddRolePrivledegeHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
        //    _publisher = publisher;
            //_outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }
        public async Task HandleAsync(RolePrivledegeCommand command)
        {

            //check role and and privilegdge check of Ids
            object[] paramPriviledge_id = {   command.priviledge_id};
         var priviledge_id = await _dbController.SQLFetchAsync(Sql.SelectOnePriviledge, paramPriviledge_id);
            if (priviledge_id.Rows.Count == 0)
            {
                _logger.LogError(" Server returned no result |Caller:RolePrivledegeController/CreateRolePrivledege|| [AddRolePrivledegeHandler][Handle]");
                throw new HandleGeneralException(400, "Creation failed, priviledge_id not valid");
            }
            object[] paramRole_id = { command.role_id };
            var role_id = await _dbController.SQLFetchAsync(Sql.SelectOneRole, paramRole_id);
            if (role_id.Rows.Count == 0)
            {
                _logger.LogError("  Server returned no result |Caller:RolePrivledegeController/CreateRolePrivledege|| [AddRolePrivledegeHandler][Handle]");
                throw new HandleGeneralException(400, "Creation failed, role_id not valid");
            }

            object[] paramPermission_id = { command.permission_id };
            var permission_id = await _dbController.SQLFetchAsync(Sql.SelectOnePermission, paramPermission_id);
            if (permission_id.Rows.Count == 0)
            {
                _logger.LogError("  Server returned no result |Caller:RolePrivledegeController/CreateRolePrivledege|| [AddRolePrivledegeHandler][Handle]");
                throw new HandleGeneralException(400, "Creation failed, permission_id not valid");
            }
            //command.role_id, command.permission_id 
            command.id = Guid.NewGuid().ToString();



            object[] param = { command.id, command.priviledge_id, command.role_id,command.permission_id };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.InsertRolePriviledge, param);
            }
            catch (Exception ex)
            {

                _logger.LogError("ex syetem error stack: {ex}Error: Server returned no result |Caller:RolePrivledegeController/CreateRolePrivledege|| [AddRolePrivledegeHandler][Handle]", ex);
                throw new HandleGeneralException(400, "Creation failed");
            }

            var spanContext = _tracer.ActiveSpan.Context.ToString();

            var @event = new GenericCreatedEvent("New Role, Privledege and Permission  created", command.id);

            //if (_outbox.Enabled)
            //{
            //    await _outbox.SendAsync(@event, spanContext: spanContext);
            //    return;
            //}
       //     await _publisher.PublishAsync(@event, spanContext: spanContext);

        }
    }
}
