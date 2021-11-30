
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using Convey.MessageBrokers;
////using Convey.MessageBrokers.Outbox;
using DbFactory;
 
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

namespace InBranchDashboard.Commands.Branches.handler
{
    public class UpdateBranchHandler : ICommandHandler<UpdateBranchCommand>
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<UpdateBranchHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
      //  private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
      //  private readonly IMessageOutbox _outbox;

        public UpdateBranchHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<UpdateBranchHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
         //   _publisher = publisher;
          //  _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task HandleAsync(UpdateBranchCommand command)
        {


            object[] param = { command.id };

            var BranchSearch = await _dbController.SQLFetchAsync(Sql.SelectOneBranch, param);
            if (BranchSearch.Rows.Count == 0)
            {
                _logger.LogError("Error: Server returned no result |Caller:BranchsController/UpdateBranch || [UpdateBranchHandler][Handle]");
                throw new HandleGeneralException(400, "The BranchId not valid");
            }
            object[] paramRegionId = { command.region_id };
            var regionSearch = await _dbController.SQLFetchAsync(Sql.SelectOneRegion, paramRegionId);
            if (regionSearch.Rows.Count == 0)
            {
                _logger.LogError("Error: Server returned no result |Caller:BranchsController/UpdateBranch || [UpdateBranchHandler][Handle]");
                throw new HandleGeneralException(400, "The region_id not valid");
            }
            int entity;
            try
            {
                object[] param1 = { command.branch_name,command.region_id,command.id  };
                entity = await _dbController.SQLExecuteAsync(Sql.UpdateBranch, param1);
            }
            catch (Exception ex)
            {

                _logger.LogError("ex syetem error stack: {ex}Error: Server returned no result |Caller:BranchController/UpdateBranch|| [UpdateBranchHandler][Handle]", ex);
                throw new HandleGeneralException(400, "Update failed");
            }

            var spanContext = _tracer.ActiveSpan.Context.ToString();

            var @event = new GenericCreatedEvent(" Branch Updated", command.id);

            //if (_outbox.Enabled)
            //{
            //    await _outbox.SendAsync(@event, spanContext: spanContext);
            //    return;
            //}
         //   await _publisher.PublishAsync(@event, spanContext: spanContext);

        }


    }
}
