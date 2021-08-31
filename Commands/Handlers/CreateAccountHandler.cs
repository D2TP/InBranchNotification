using System;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;
using Convey.Persistence.MongoDB;
using InBranchDashboard.Commands.Events;
using InBranchDashboard.Domain;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace InBranchDashboard.Commands.Handlers
{

    public class CreateAccountHandler : ICommandHandler<CreateAccount>
    {
        //private readonly IMongoRepository<Account, Guid> _repository;
        private readonly IBusPublisher _publisher;
        private readonly IMessageOutbox _outbox;       
        private readonly ILogger<CreateAccountHandler> _logger;
        private readonly ITracer _tracer;

        public CreateAccountHandler(  IBusPublisher publisher,
            IMessageOutbox outbox, ITracer tracer,
            ILogger<CreateAccountHandler> logger)
        {
          //  _repository = repository;
            _publisher = publisher;
            _outbox = outbox;
            _tracer = tracer;
            _logger = logger;
        }

        public async Task HandleAsync(CreateAccount command)
        {
            //var exists = await _repository.ExistsAsync(o => o.AccountNo == command.AccountNo);
            //if (exists)
            //{
            //    throw new InvalidOperationException($"Account with given no: {command.AccountNo} for {command.CustomerId} already exists!");
            //}           
         
            //var account = new Account(command.AccountId,command.CustomerId, command.AccountNo, DateTime.Now );
            //await _repository.AddAsync(account);

            //_logger.LogInformation($"Created an Account with no: {command.AccountNo}, customer: {command.CustomerId}.");
            //var spanContext = _tracer.ActiveSpan.Context.ToString();
            //var @event = new AccountCreated(account);
            //if (_outbox.Enabled)
            //{
            //    await _outbox.SendAsync(@event, spanContext: spanContext);
            //    return;
            //}

            //await _publisher.PublishAsync(@event, spanContext: spanContext);
        }
    }
}
