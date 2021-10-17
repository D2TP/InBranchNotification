
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
using InBranchDashboard.Helpers;
using InBranchDashboard.Queries.Categories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Branches.handler
{ 

    public class BranchQueryHandler : IQueryHandler<BranchQueries, PagedList<Branch>>
{

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<BranchQueryHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
       // private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
    //    private readonly IMessageOutbox _outbox;
        public BranchQueryHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<BranchQueryHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox)//, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
       //     _publisher = publisher;
        //    _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task<PagedList<Branch>> HandleAsync(BranchQueries query)
    {
           // var entity = await _dbController.SQLFetchAsync(Sql.SelectBranches);
            var entity = _dbController.SQLFetchAsync(Sql.SelectBranches).Result.AsEnumerable().OrderBy(on => on.Field<string>("branch_name"))
 .ToList();
            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:RegionController/GetAllCatigories-Get|| [CategoryQueryHandler][Handle]");
                throw new HandleGeneralException(500, "Server returned no result");
            }
           
            var branches = _convertDataTableToObject.ConvertDataRowList<Branch>(entity).AsQueryable();
          var brancheItems = PagedList<Branch>.ToPagedList(branches,
             query._queryStringParameters.PageNumber,
             query._queryStringParameters.PageSize);


            return brancheItems;
        }
}
}
