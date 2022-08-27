using System;
using Convey.CQRS.Events;
using Convey.MessageBrokers;

namespace InBranchNotification.Events.External
{ 
    [Message("payments")]
    public class PaymentMade : IEvent
    {
        public Guid PaymentId { get; }

        public PaymentMade(Guid paymentId)
        {
            PaymentId = paymentId;
        }
    }
}
