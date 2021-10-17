
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
             
            var userRoleId = Guid.NewGuid().ToString();
            object[] param = { userRoleId, command.ADUserId,command.RoleId };
            int entity;
            try
            {
                  entity = await _dbController.SQLExecuteAsync(Sql.InsertUserRole, param);
            }
            catch (Exception ex)
            {

                _logger.LogError("ex syetem error stack: {ex}Error: Server returned no result |Caller:ADUserController/CreateADUser|| [CreateUserRoleHandler][Handle]", ex);
                throw new HandleGeneralException(500, "Server returned no result");
            }
             
            var spanContext = _tracer.ActiveSpan.Context.ToString();
            
            var @event = new GenericCreatedEvent("User was Assinged a Role",userRoleId);
            
            //if (_outbox.Enabled)
            //{
            //    await _outbox.SendAsync(@event, spanContext: spanContext);
            //    return;
            //}
          //  await _publisher.PublishAsync(@event, spanContext: spanContext);
           
        }

        
    }
}
