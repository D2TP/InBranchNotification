﻿
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;
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
 
namespace InBranchDashboard.Commands.Regions.handler
{
    public class AddRegionHandler : ICommandHandler<RegionCommand>
    {

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<AddRegionHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
        private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
        private readonly IMessageOutbox _outbox;

        public AddRegionHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<AddRegionHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer, IMessageOutbox outbox, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
            _publisher = publisher;
            _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task HandleAsync(RegionCommand command)
        {


            command.id = Guid.NewGuid().ToString(); 
             


            object[] param = { command.id, command.region_name };
            int entity;
            try
            {
                entity = await _dbController.SQLExecuteAsync(Sql.InsertRegion, param);
            }
            catch (Exception ex)
            {

                _logger.LogError("ex syetem error stack: {ex}Error: Server returned no result |Caller:RegionController/CreateRegion|| [AddRegionHandler][Handle]", ex);
                throw new HandleGeneralException(500, "Creation failed");
            }

            var spanContext = _tracer.ActiveSpan.Context.ToString();

            var @event = new GenericCreatedEvent("New Region created", command.id);

            if (_outbox.Enabled)
            {
                await _outbox.SendAsync(@event, spanContext: spanContext);
                return;
            }
            await _publisher.PublishAsync(@event, spanContext: spanContext);

        }


    }
}