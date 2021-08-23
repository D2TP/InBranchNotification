
using Convey.CQRS.Commands;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;
using DbFactory;
using InBranchDashboard.Commands.CreatUserRole;
using InBranchDashboard.DbFactory;
using InBranchDashboard.Domain;
using InBranchDashboard.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InBranchMgt.Commands.RegisterAdUser.Handlers
{
    public class CreateADUserHandler : ICommandHandler<CreateUserRoleComand>
    {
        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<CreateADUserHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
        private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
        private readonly IMessageOutbox _outbox;
        public CreateADUserHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<CreateADUserHandler   > logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer, IMessageOutbox outbox)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task<ADCreateCommandDTO> HandleAsync(CreateADUserCommand command)
        {


            //G. OMONI check if role is valid 

            object[] paramRoleId = { command.RoleId };
            var roleResult = await _dbController.SQLFetchAsync(Sql.SelectRole, paramRoleId) ?? null;


            if (roleResult == null)
            {
                _logger.LogError("Error: There is no Role with the supplied roleid {roleid}|Caller:ADUserController/Create|| [CreateADUserHandler][Handle]", request.RoleId);
                throw new HandleGeneralException(422, "There is no Role with the supplied roleid");

            }
            //role_name
            Role role = new Role();
            role = _convertDataTableToObject.ConvertDataTable<Role>(roleResult).FirstOrDefault();
            //check if user name exists
            object[] paramUser = { command.UserName };
            var user = _dbController.SQLFetchAsync(Sql.SelectADUser, paramUser) ?? null;

            if (user != null)
            {
                _logger.LogError("Error: User already exists {Username}||Caller:ADUserController/Create  || [CreateADUserHandler][Handle]", command.UserName);
                throw new HandleGeneralException(422, "User already exists");

            }
         
            object[] paramADUser = { command.UserName, command.FirstName, command.LastName, command.Active = true };

            var adUser = _dbController.SQLExecuteAsync(Sql.InsertADUser, paramADUser) ?? null;

            if (adUser == null)
            {
                _logger.LogError("Server Error occured ||Caller:ADUserController/Create  || [CreateADUserHandler][Handle]", command.UserName);
                throw new HandleGeneralException(500, "Server Error occured");

            }
            object[] paramRole = { adUser, command.RoleId };
            var userRole1 = _dbController.SQLExecuteAsync(Sql.InsertUserRole, paramRole);
            // //G. OMONI I need to return aDCreateCommandDTO below
            var aDCreateCommandDTO = new ADCreateCommandDTO
            {
                Active = command.Active,
                ADUserId = adUser.Result,
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Role = role.role_name,
            };

            return aDCreateCommandDTO;
                }
        }
    }
}
