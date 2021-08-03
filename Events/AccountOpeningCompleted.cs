using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Events;
using Convey.Test.Accounts.Domain;
using Convey.Test.Accounts.Services;

namespace Convey.Test.Accounts.Events
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
