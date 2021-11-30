
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InBranchMgt.Commands.AdUser.Handlers
{
    public class UpdateAduserHandler : ICommandHandler<UpdateAduser>
    {
        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<UpdateAduserHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
     //   private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
      //  private readonly IMessageOutbox _outbox;
        public UpdateAduserHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<UpdateAduserHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox, IBusPublisher  publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
         //   _publisher = publisher;
         //   _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task HandleAsync(UpdateAduser command)
        {


            //G. OMONI check if role is valid 

            object[] paramRoleId = { command.role_id };
            var roleResult = await _dbController.SQLFetchAsync(Sql.SelectRole, paramRoleId) ?? null;


            if (roleResult.Rows.Count == 0)
            {
                _logger.LogError("Error: There is no Role with the supplied roleid {roleid}|Caller:ADUserController/UpdateADUser|| [UpdateADUserHandler][Handle]", command.role_id );
                throw new HandleGeneralException(422, "There is no Role with the supplied roleid");

            }
            //role_name
            Role role = new Role();
            role = _convertDataTableToObject.ConvertDataTable<Role>(roleResult).FirstOrDefault();
                    //    email=#,branch_id=#
            object[] paramADUser = {  command.user_name, command.first_name, command.last_name, command.active = command.active,command.email,command.branch_Id, command.id };

            var adUser = _dbController.SQLExecuteAsync(Sql.UpdateADUser, paramADUser) ?? null;

            if (adUser.Result == 0)
            {
                _logger.LogError("Server Error occured, user was not created ||Caller:ADUserController/UpdateADUser  || [UpdateADUserHandler][Handle]", command.user_name);
                throw new HandleGeneralException(400, "Server Error occured");

            }
            object[] paramRole = {  command.id, command.role_id, command.role_id };

            var userRole1 = _dbController.SQLExecuteAsync(Sql.UpdatetUserRole, paramRole);
            var j = userRole1.Status.ToString();
            if (  userRole1.Status.ToString()== "Faulted")
            {
                _logger.LogError("Server Error occured role was not created||Caller:ADUserController/UpdateADUser  || [UpdateADUserHandler][Handle]", command.user_name);
                throw new HandleGeneralException(400, "Server Error occured");

            }


            var spanContext = _tracer.ActiveSpan.Context.ToString();
             
            var @event = new ADUserCreated(command.id, command.role_id, command.role_id);

            //if (_outbox.Enabled)
            //{
            //    await _outbox.SendAsync(@event, spanContext: spanContext);
            //    return;
            //}
           //await _publisher.PublishAsync(@event, spanContext: spanContext);





        }


    }
    }
 
