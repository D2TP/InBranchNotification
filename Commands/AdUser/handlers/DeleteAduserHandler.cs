
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
    public class DeleteAduserHandler : ICommandHandler<DeleteAdUser>
    {
        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<DeleteAduserHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
    //    private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
    //    private readonly IMessageOutbox _outbox;
        public DeleteAduserHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<DeleteAduserHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox, IBusPublisher  publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
        //    _publisher = publisher;
           // _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task HandleAsync(DeleteAdUser command)
        {

       

            object[] paramRoleId = { command.AdUserId };
            var checkIfUserExist = await _dbController.SQLFetchAsync(Sql.SelectADUserById, paramRoleId) ?? null;
            if (checkIfUserExist.Rows.Count == 0)
            {
                _logger.LogError("Server Error occured while deleting  ||Caller:ADUserController/DeleteADUser  || [DeleteAduserHandler][Handle]", command.AdUserId);
                throw new HandleGeneralException(404, "Invalid AdUser Id supplied");

            } 
            //delete ADUser
            var deleteAdUser = await _dbController.SQLExecuteAsync(Sql.DeleteADUser, paramRoleId)  ;
            if (deleteAdUser == 0)
            {
                _logger.LogError("Server Error occured while deleting  ||Caller:ADUserController/DeleteADUser  || [DeleteAduserHandler][Handle]", command.AdUserId);
                throw new HandleGeneralException(400, "Server Error occured");

            }
            //check if role Id exists
            var checkAllRole = await _dbController.SQLFetchAsync(Sql.SelectUserBaseOnAdUserId, paramRoleId) ?? null;
            if (checkAllRole.Rows.Count == 0)
            {
                _logger.LogError("Server Error occured while deleting  ||Caller:ADUserController/DeleteADUser  || [DeleteAduserHandler][Handle]", command.AdUserId);
                throw new HandleGeneralException(400, "Server Error occured");
            }

            object[] paramRoleuserRowId = { command.AdUserId };
            //delete all roles attached to user
            var deleteRole = await _dbController.SQLExecuteAsync(Sql.DeleteUserRoles, paramRoleId);

            if (deleteRole == 0)
            {
                _logger.LogError("Server Error occured while deleting  ||Caller:ADUserController/DeleteADUser  || [DeleteAduserHandler][Handle]", command.AdUserId);
                throw new HandleGeneralException(400, "Server Error occured");
            }

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
 
