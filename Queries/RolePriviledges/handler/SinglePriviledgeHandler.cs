
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
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.RolePriviledges.handler
{
    public class SingleRolePriviledgeHandler : IQueryHandler<RolePriviledgeQuery, List<RolePriviledgeDTO>>
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<SingleRolePriviledgeHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
      //  private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
      //  private readonly IMessageOutbox _outbox;
        public SingleRolePriviledgeHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<SingleRolePriviledgeHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox)//, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
          //  _publisher = publisher;
          //  _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task<List<RolePriviledgeDTO>> HandleAsync(RolePriviledgeQuery query)
        {

            object[] param = { query.id };
  //          var getEntity = await _dbController.SQLFetchAsync(Sql.SelectRolePriviledge);
  //          var entity = getEntity.AsEnumerable().OrderBy(on => on.Field<string>("priviledge_name"))
  //.ToList();

            var getEntity = await _dbController.SQLFetchAsync(Sql.SelectOneRolePriviledge, param);
            var entity = getEntity.AsEnumerable().OrderBy(on => on.Field<string>("priviledge_name")).ToList();
            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:RolePriviledgeController/GetCatigoriesById -Get|| [SinglePermissionHandler][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            //var rolePriviledgeDTO = new List<RolePriviledgeDTO>();
            //rolePriviledgeDTO = _convertDataTableToObject.ConvertDataRowList<List<RolePriviledgeDTO>>(entity).AsQueryable();

            var _rolePriviledge = _convertDataTableToObject.ConvertDataRowList<RolePriviledgeDTO>(entity);
            return _rolePriviledge;
        }
    }
}