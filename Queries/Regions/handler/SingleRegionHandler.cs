
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Regions.handler
{
    public class SingleRegionHandler : IQueryHandler<RegionQuery, RegionDTO>
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<SingleRegionHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
     //   private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
      //  private readonly IMessageOutbox _outbox;
        public SingleRegionHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<SingleRegionHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox)//, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
        //    _publisher = publisher;
            //_outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task<RegionDTO> HandleAsync(RegionQuery query)
        {

            object[] param = { query.id };
            var entity = await _dbController.SQLFetchAsync(Sql.SelectOneRegion, param);
            if (entity.Rows.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:RegionController/GetCatigoriesById -Get|| [SingleRegionHandler][Handle]");
                throw new HandleGeneralException(500, "Server returned no result");
            }
            RegionDTO RegionDTO = new RegionDTO();
            RegionDTO = _convertDataTableToObject.ConvertDataTable<RegionDTO>(entity).FirstOrDefault();

            return RegionDTO;
        }
    }
}