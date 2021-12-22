
using Convey.CQRS.Commands;
using Convey.MessageBrokers;
////using Convey.MessageBrokers.Outbox;
using DbFactory;
using InBranchDashboard.Commands.AdUser;
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
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InBranchMgt.Commands.AdUser.Handlers
{
    public class ActivaeDeactivateAduserHandler : ICommandHandler<ActivaeDeactivateAduser>
    {
        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<DeleteAduserHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
        //    private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
        //    private readonly IMessageOutbox _outbox;
        public ActivaeDeactivateAduserHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<DeleteAduserHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox, IBusPublisher  publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
            //    _publisher = publisher;
            // _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task HandleAsync(ActivaeDeactivateAduser command)
        {



            object[] paramAdUserName = { command.AdUserId  };
            var checkIfUserExist = await _dbController.SQLFetchAsync(Sql.SelectADUser, paramAdUserName) ?? null;
             if (checkIfUserExist.Rows.Count == 0)
            {
                _logger.LogError("An Error occured while deleting  ||Caller:ADUserController/DeleteADUser  || [DeleteAduserHandler][Handle]", command.AdUserId);
                throw new HandleGeneralException(404, "Invalid AdUser Id supplied");

            }

            //check if user is already active  
            object[] paramAdUserActive = {command.Active, command.AdUserId };
            if (command.Active)
            {
                var checkAllRole = await _dbController.SQLSelectAsync(Sql.CheckIdAdUserIsActive, paramAdUserActive) ?? null;
                if (Convert.ToInt32(checkAllRole) > 0)
                {
                    _logger.LogError("User Already Active  ||Caller:ADUserController/DeleteADUser  || [DeleteAduserHandler][Handle]", command.AdUserId);
                    throw new HandleGeneralException(400, "User Already Active");
                }
                var activateAdUser = await _dbController.SQLExecuteAsync(Sql.ActivateDeactivateADUser, paramAdUserActive);
                if (activateAdUser == 0)
                {
                    _logger.LogError("Server Error occured while deleting  ||Caller:ADUserController/DeleteADUser  || [DeleteAduserHandler][Handle]", command.AdUserId);
                    throw new HandleGeneralException(400, "Server Error occured");

                }

            }
            //check if user is already inactive  
            else if (!command.Active)
            {
               var checkAllRole = await _dbController.SQLSelectAsync(Sql.CheckIdAdUserIsActive, paramAdUserActive) ;
                // var checkAllRole = await _dbController.SQLExecuteAsync(Sql.CheckIdAdUserIsActive, paramAdUserActive) ?? null;
                if (Convert.ToInt32(checkAllRole) > 0)
                {
                    _logger.LogError("User Already Active  ||Caller:ADUserController/DeleteADUser  || [DeleteAduserHandler][Handle]", command.AdUserId);
                    throw new HandleGeneralException(400, "User Already Inactive");
                }
                var deactivateAdUser = await _dbController.SQLExecuteAsync(Sql.ActivateDeactivateADUser, paramAdUserActive);
                if (deactivateAdUser == 0)
                {
                    _logger.LogError("Server Error occured while deleting  ||Caller:ADUserController/DeleteADUser  || [DeleteAduserHandler][Handle]", command.AdUserId);
                    throw new HandleGeneralException(400, "Server Error occured");

                }

            }
            //delete ADUser


            var spanContext = _tracer.ActiveSpan.Context.ToString();

            var @event = new GenericCreatedEvent("AdUser with Id" + command.AdUserId + "Delete", command.AdUserId);

            //if (_outbox.Enabled)
            //{
            //    await _outbox.SendAsync(@event, spanContext: spanContext);
            //    return;
            //}
            // await _publisher.PublishAsync(@event, spanContext: spanContext);


        }


    }
}

