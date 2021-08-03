using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Events;
using InBranchDashboard.Domain;
using InBranchDashboard.Services;

namespace InBranchDashboard.Events
{
    public class AccountOpeningCompleted : IDomainEvent
    {
        public Account Account { get; }        
        public AccountOpeningCompleted(Account account)
        {
            Account = account;
        }
    }
}
