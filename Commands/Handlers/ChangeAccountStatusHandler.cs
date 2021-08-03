using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;
using Convey.Persistence.MongoDB;
using InBranchDashboard.Commands.Events;
using InBranchDashboard.Domain;
using InBranchDashboard.Exceptions;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace InBranchDashboard.Commands.Handlers
{
    public class ChangeAccountStatusHandler : ICommandHandler<ChangeAccountStatus>
    {
        private readonly IMongoRepository<Account, Guid> _repository;
        private readonly IBusPublisher _publisher;
        private readonly IMessageOutbox _outbox;
        private readonly ILogger<ChangeAccountStatusHandler> _logger;
        private readonly ITracer _tracer;

        public ChangeAccountStatusHandler(IMongoRepository<Account, Guid> repository, IBusPublisher publisher,
            IMessageOutbox outbox, ITracer tracer,
            ILogger<ChangeAccountStatusHandler> logger)
        {
            _repository = repository;
            _publisher = publisher;
            _outbox = outbox;
            _tracer = tracer;
            _logger = logger;
        }

        public async Task HandleAsync(ChangeAccountStatus command)
        {
            var account = await _repository.GetAsync(o => o.AccountNo == command.AccountNo);
            if (account is null)
            {
                throw new InvalidOperationException($"Account with given no: {command.AccountNo}  for {command.CustomerId} not found!");
            }

            if (account.AccountBalance < 100)
            {
                throw new InvalidAccountBalanceException(command.CustomerId, command.AccountNo, account.AccountBalance );
            }

            if (!Enum.TryParse<Status>(command.Status, true, out var status))
            {
                throw new CannotChangeAccountStatusException(command.CustomerId, command.AccountNo, Status.Unknown);
            }

            switch (status)
            {
                case Status.Close:
                    account.Close();
                    break;
                case Status.Dormant:
                    account.MarkAsDormant();
                    break;
                case Status.Valid:
                    account.SetValid();
                    break;
                case Status.Incomplete:
                    account.SetIncomplete();
                    break;
                case Status.Suspicious:
                    account.MarkAsSuspicious();
                    break;
                default:
                    throw new CannotChangeAccountStatusException(command.CustomerId, command.AccountNo, status);
            }

            await _repository.UpdateAsync(account);

            _logger.LogInformation($"Updated account status to {command.Status} for account no: {command.AccountNo}, customer: {command.CustomerId}.");
            var spanContext = _tracer.ActiveSpan.Context.ToString();
            var @event = new AccountCreated(account);
            if (_outbox.Enabled)
            {
                await _outbox.SendAsync(@event, spanContext: spanContext);
                return;
            }

            await _publisher.PublishAsync(@event, spanContext: spanContext);
        }

        
    }
}
