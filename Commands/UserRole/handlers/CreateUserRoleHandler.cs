
using Convey.CQRS.Commands;
using Convey.MessageBrokers;
//using Convey.MessageBrokers.Outbox;
using DbFactory;
using InBranchDashboard.Commands.UserRole;
using InBranchDashboard.DbFactory;
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

namespace InBranchMgt.Commands.UserRole.Handlers
{
    public class CreateUserRoleHandler : ICommandHandler<CreateUserRoleComand>
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<CreateUserRoleHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
        //   private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
        // private readonly IMessageOutbox _outbox;
        public CreateUserRoleHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<CreateUserRoleHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
            // _publisher = publisher;
            //_outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task HandleAsync(CreateUserRoleComand command)
        {
            object[] paramRole_id = { command.RoleId };
            var role_id = await _dbController.SQLFetchAsync(Sql.SelectOneRole, paramRole_id);
            if (role_id.Rows.Count == 0)
            {
                _logger.LogError("  Server returned no result |Caller:UserRoleController/AddRoleToADUser|| [AddRolePrivledegeHandler][Handle]");
                throw new HandleGeneralException(400, "Creation failed, role_id not valid");
            }
            var userRoleId = Guid.NewGuid().ToString();
            //check if user is already in role
            object[] paramCheckIfroleExistAlready = { command.ADUserId, command.RoleId };
            var checkIfExist = await _dbController.SQLFetchAsync(Sql.GetUserRole, paramCheckIfroleExistAlready);
            if (checkIfExist.Rows.Count > 0)
            {
                _logger.LogError("  Server returned no result  role has already been added |Caller:UserRoleController/AddRoleToADUser|| [AddRolePrivledegeHandler][Handle]");
                throw new HandleGeneralException(400, "Creation failed, role has already been added ");
            }

            object[] param = { userRoleId, command.ADUserId, command.RoleId };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.InsertUserRole, param);
            }
            catch (Exception ex)
            {
                //UserRoleController/RemoveARolefromADUser 
                _logger.LogError("ex syetem error stack: {ex}Error: Server returned no result |Caller:UserRoleController/AddRoleToADUser|| [CreateUserRoleHandler][Handle]", ex);
                throw new HandleGeneralException(400, "Server returned no result");
            }

            var spanContext = _tracer.ActiveSpan.Context.ToString();

            var @event = new GenericCreatedEvent("User was Assinged a Role", userRoleId);

            //if (_outbox.Enabled)
            //{
            //    await _outbox.SendAsync(@event, spanContext: spanContext);
            //    return;
            //}
            //  await _publisher.PublishAsync(@event, spanContext: spanContext);

        }


    }
}
