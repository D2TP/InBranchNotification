using Convey.Test.Accounts.Domain;
using Convey.Test.Accounts.Services;

namespace Convey.Test.Accounts.Events
{
    internal class AccountStatusChanged : IDomainEvent
    {     

        public Account Account { get; }
        public Status PreviousState { get; }

        public AccountStatusChanged(Account account, Status previousState)
        {
            Account = account;
            PreviousState = previousState;
        }
    }
}