using InBranchDashboard.Domain;
using InBranchDashboard.Services;

namespace InBranchDashboard.Events
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