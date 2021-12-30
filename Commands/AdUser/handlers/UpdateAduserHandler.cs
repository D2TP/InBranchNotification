
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
using InBranchDashboard.Services;
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
        private readonly IAuthenticateRestClient _authenticateRestClient;
        //  private readonly IMessageOutbox _outbox;
        public UpdateAduserHandler(IMemoryCache memoryCache, IAuthenticateRestClient authenticateRestClient, IDbController dbController, ILogger<UpdateAduserHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox, IBusPublisher  publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
         //   _publisher = publisher;
         //   _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
            _authenticateRestClient = authenticateRestClient;
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
            // check if branchId exists 
            //SelectOneBranch

            object[] paramBranchId = { command.branch_Id };
            var branchResult = await _dbController.SQLFetchAsync(Sql.SelectOneBranch, paramBranchId) ?? null;


            if (roleResult.Rows.Count == 0)
            {
                _logger.LogError("Error: There is no Branch with the supplied branchId {roleid}|Caller:ADUserController/UpdateADUser|| [UpdateADUserHandler][Handle]", command.branch_Id);
                throw new HandleGeneralException(422, "There is no  Branch with the supplied branchId");

            }
            //role_name
            Role role = new Role();
            role = _convertDataTableToObject.ConvertDataTable<Role>(roleResult).FirstOrDefault();

            var userDetail =await _authenticateRestClient.GetXtradotAdUserDetails(command.user_name, command.Domain);
            if (userDetail == null)
            {
                _logger.LogError("Error: User:{Username} does not  exists in Active Directory ||Caller:ADUserController/Create  || [CreateADUserHandler][Handle]", command.user_name);
                throw new HandleGeneralException(400, "User: " + command.user_name + " does not exist in Active Directory");
            };
            //    email=#,branch_id=#
            object[] paramADUser = {  command.user_name, userDetail.data.firstName, userDetail.data.lastName, command.active = command.active, userDetail.data.email,command.branch_Id,command.modified_by, DateTime.Now, command.id };

            var adUser =await _dbController.SQLExecuteAsync(Sql.UpdateADUser, paramADUser)  ;

            if (adUser == 0)
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
 
