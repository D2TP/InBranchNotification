
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


namespace InBranchDashboard.Commands.UserRole.handlers
{
    public class ActivaeDeactivateSingleRoleCommand : ICommandHandler<ADUserAndRold>
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<ActivaeDeactivateSingleRoleCommand> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
  //      private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
        //private readonly IMessageOutbox _outbox;
        public ActivaeDeactivateSingleRoleCommand(IMemoryCache memoryCache, IDbController dbController, ILogger<ActivaeDeactivateSingleRoleCommand> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
        //    _publisher = publisher;
          //  _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task HandleAsync(ADUserAndRold command)
        {
            object[] paramRole_id = { command.RoleId };
             var role_id = await _dbController.SQLFetchAsync(Sql.SelectOneRole, paramRole_id);
            if (role_id.Rows.Count == 0)
            {
                _logger.LogError("  Server returned no result |Caller:UserRoleController/RemoveARolefromADUser|| [RemoveSingleRoleComand][Handle]");
                throw new HandleGeneralException(400, "Creation failed, role_id not valid");
            }
            object[] param = { command.Active, command.AdUserId, command.RoleId };
            if (command.Active)
            {
                var checkAllRole = await _dbController.SQLSelectAsync(Sql.CheckIdAdUserRoleIsActive, param) ?? null;
                if (Convert.ToInt32(checkAllRole) > 0)
                {
                    _logger.LogError("User Already Active  ||Caller:ADUserController/ActivateOrDeactivateADUser  || [ActivaeDeactivateAduserHandler][Handle]", command.AdUserId);
                    throw new HandleGeneralException(400, "User Role Already Active");
                }
                var activateAdUser = await _dbController.SQLExecuteAsync(Sql.ActivateDeactivateSingleUserRoles, param);
                if (activateAdUser == 0)
                {
                    _logger.LogError("Server Error occured while deleting  ||Caller:ADUserController/ActivateOrDeactivateADUser  || [ActivaeDeactivateAduserHandler][Handle]", command.AdUserId);
                    throw new HandleGeneralException(400, "Server Error occured");

                }

            }
            //check if user is already inactive  
            else if (!command.Active)
            {
                var checkAllRole = await _dbController.SQLSelectAsync(Sql.CheckIdAdUserRoleIsActive, param);
                // var checkAllRole = await _dbController.SQLExecuteAsync(Sql.CheckIdAdUserIsActive, paramAdUserActive) ?? null;
                if (Convert.ToInt32(checkAllRole) > 0)
                {
                    _logger.LogError("User Already Active  ||Caller:ADUserController/ActivateOrDeactivateADUser  || [ActivaeDeactivateAduserHandler][Handle]", command.AdUserId);
                    throw new HandleGeneralException(400, "User Role Already Inactive");
                }
                var deactivateAdUser = await _dbController.SQLExecuteAsync(Sql.ActivateDeactivateSingleUserRoles, param);
                if (deactivateAdUser == 0)
                {
                    _logger.LogError("Server Error occured while deleting  ||Caller:ADUserController/ActivateOrDeactivateADUser  || [ActivaeDeactivateAduserHandler][Handle]", command.AdUserId);
                    throw new HandleGeneralException(400, "Server Error occured");

                }

            }




            //if (role_id.Rows.Count == 1)
            //{
            //    _logger.LogError("  Server returned only 1 result |Caller:UserRoleController/RemoveARolefromADUser|| [RemoveSingleRoleComand][Handle]");
            //    throw new HandleGeneralException(400, "Creation failed, you can't remove all roles from user, please update user or delete user");
            //}
           
            //int entity;
            //try
            //{
            //    entity = await _dbController.SQLExecuteAsync(Sql.ActivateDeactivateSingleUserRoles, param);

            //}
            //catch (Exception ex)
            //{

            //    _logger.LogError("ex syetem error stack: {ex}Error: Server returned no result |Caller:UserRoleController/RemoveARolefromADUser|| [RemoveSingleRoleComand][Handle]", ex);
            //    throw new HandleGeneralException(400, "Server returned no result");
            //}

            var spanContext = _tracer.ActiveSpan.Context.ToString();

            var @event = new GenericCreatedEvent("User was Removed from role", command.AdUserId );

            //if (_outbox.Enabled)
            //{
            //    await _outbox.SendAsync(@event, spanContext: spanContext);
            //    return;
            //}
         //   await _publisher.PublishAsync(@event, spanContext: spanContext);
        }
    }
}
