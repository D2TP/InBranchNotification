using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InBranchDashboard.Services;
using Convey.Types;

namespace InBranchDashboard.Domain
{
    public abstract class AggregateRoot  : IIdentifiable<Guid>
    {
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();
        public IEnumerable<IDomainEvent> Events => _events;
        public Guid Id { get; protected set; }
        public int Version { get; protected set; }

        protected void AddEvent(IDomainEvent @event)
        {
            _events.Add(@event);
        }

        public void ClearEvents() => _events.Clear();
    }
}
