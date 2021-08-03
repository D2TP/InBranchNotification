using System;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;
using Convey.Persistence.MongoDB;
using InBranchDashboard.Domain;
using InBranchDashboard.Events;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace InBranchDashboard.Commands.Handlers
{
    public class CompleteAccountOpeningHandler : ICommandHandler<CompleteAccountOpening>
    {
        private readonly IMongoRepository<Account, Guid> _repository;
        private readonly IBusPublisher _publisher;
        private readonly IMessageOutbox _outbox;
        private readonly ILogger<CompleteAccountOpeningHandler> _logger;
        private readonly ITracer _tracer;

        public CompleteAccountOpeningHandler(IMongoRepository<Account, Guid> repository, IBusPublisher publisher,
            IMessageOutbox outbox, ITracer tracer,
            ILogger<CompleteAccountOpeningHandler> logger)
        {
            _repository = repository;
            _publisher = publisher;
            _outbox = outbox;
            _tracer = tracer;
            _logger = logger;
        }

        public async Task HandleAsync(CompleteAccountOpening command)
        {
            var account = await _repository.GetAsync(o => o.AccountNo == command.AccountNo);
            if (account is null)
            {
                throw new InvalidOperationException($"Account with given no: {command.AccountNo} for {command.CustomerId} cannot be found!");
            }

            account.CompleteAccountOpening(command.CustomerId, command.AccountNo);
            await _repository.UpdateAsync(account);

            _logger.LogInformation($"Completed accoutn opening for Account with no: {command.AccountNo}, customer: {command.CustomerId}.");
            var spanContext = _tracer.ActiveSpan.Context.ToString();
            
            var @event = new AccountOpeningCompleted(account);
            if (_outbox.Enabled)
            {
                await _outbox.SendAsync(@event, spanContext: spanContext);
                return;
            }

            await _publisher.PublishAsync(@event, spanContext: spanContext);
        }
    }
}
