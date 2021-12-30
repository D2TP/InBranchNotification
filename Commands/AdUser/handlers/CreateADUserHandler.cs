
using Convey.CQRS.Commands;
using Convey.MessageBrokers;
////using Convey.MessageBrokers.Outbox;
using DbFactory;
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

namespace InBranchDashboard.Commands.AdUser.Handlers
{
    public class CreateADUserHandler : ICommandHandler<CreateADUserCommand>
    {
        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<CreateADUserHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
         private readonly IAuthenticateRestClient _authenticateRestClient;
        private readonly ITracer _tracer;
        //private readonly IMessageOutbox _outbox;
        public CreateADUserHandler(IMemoryCache memoryCache, IAuthenticateRestClient authenticateRestClient, IDbController dbController, ILogger<CreateADUserHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox, IBusPublisher  publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
            //   _publisher = publisher;
            // _outbox = outbox;
            _authenticateRestClient = authenticateRestClient;
             _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task HandleAsync(CreateADUserCommand command)
        {


          

            object[] paramRoleId = { command.RoleId };
            var roleResult = await _dbController.SQLFetchAsync(Sql.SelectRole, paramRoleId) ?? null;


            if (roleResult.Rows.Count == 0)
            {
                _logger.LogError("Error: There is no Role with the supplied roleid {roleid}|Caller:ADUserController/Create|| [CreateADUserHandler][Handle]", command.RoleId);
                throw new HandleGeneralException(422, "There is no Role with the supplied roleid");

            }
            //SelectOneBranch
            object[] paramBranchId = { command.BranchId };
            var branchResult = await _dbController.SQLFetchAsync(Sql.SelectOneBranch, paramBranchId) ?? null;


            if (roleResult.Rows.Count == 0)
            {
                _logger.LogError("Error: There is no Branch with the supplied branchId {branchId}|Caller:ADUserController/Create|| [CreateADUserHandler][Handle]", command.BranchId);
                throw new HandleGeneralException(422, "There is no  Branch with the supplied branchId");

            }
            //role_name
            Role role = new Role();
            role = _convertDataTableToObject.ConvertDataTable<Role>(roleResult).FirstOrDefault();
            //check if user name exists
            object[] paramUser = { command.UserName };
            var user = _dbController.SQLFetchAsync(Sql.SelectADUser, paramUser) ?? null;

            if (user.Result.Rows.Count>0)
            {
                _logger.LogError("Error: User already exists {Username}||Caller:ADUserController/Create  || [CreateADUserHandler][Handle]", command.UserName);
                throw new HandleGeneralException(400, "User already exists");

            }

            //get details from AD 
      var userDetail=   await    _authenticateRestClient.GetXtradotAdUserDetails(command.UserName, command.Domain);
            if (userDetail == null) {
                _logger.LogError("Error: User:{Username} does not  exists in Active Directory ||Caller:ADUserController/Create  || [CreateADUserHandler][Handle]", command.UserName);
                throw new HandleGeneralException(400, "User: "+command.UserName+ " does not exist in Active Directory");
            };
            var aDUserId = Guid.NewGuid().ToString();
            object[] paramADUser = { aDUserId  , command.UserName, userDetail.data.firstName, userDetail.data.lastName, command.Active = true, userDetail.data.email, command.BranchId,command.created_by};
            //INSERT INTO  inb_aduser (id,user_name,first_name,last_name,active,email,branch_id,created_by)VALUES(#,#,#,#,#,#,#,#)
            var adUser = _dbController.SQLExecuteAsync(Sql.InsertADUser, paramADUser) ?? null;

            if (adUser.Result == 0)
            {
                _logger.LogError("Server Error occured, user was not created ||Caller:ADUserController/Create  || [CreateADUserHandler][Handle]", command.UserName);
                throw new HandleGeneralException(400, "Server Error occured");

            }
            object[] paramRole = {Guid.NewGuid().ToString(), aDUserId, command.RoleId };

            var userRole1 = _dbController.SQLExecuteAsync(Sql.InsertUserRole, paramRole);
           
            if (userRole1.Result == 0)
            {
                _logger.LogError("Server Error occured role was not created||Caller:ADUserController/Create  || [CreateADUserHandler][Handle]", command.UserName);
                throw new HandleGeneralException(400, "Server Error occured");

            }


            var spanContext = _tracer.ActiveSpan.Context.ToString();
            command.id = aDUserId;
            var @event = new ADUserCreated(command.id, command.RoleId, command.RoleId);

            //if (_outbox.Enabled)
            //{
            //    await _outbox.SendAsync(@event, spanContext: spanContext);
            //    return;
            //}
           //await _publisher.PublishAsync(@event, spanContext: spanContext);





        }


    }
    }
 
