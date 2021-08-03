using InBranchDashboard.Domain;
using InBranchDashboard.Services;

namespace InBranchDashboard.Events
{
    public class AccountKYCCompleted :IDomainEvent
    {
        public Account Account { get; }

        public AccountKYCCompleted(Account account)
        {
            Account = account;
        }
    }
}