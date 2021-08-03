using Convey.Test.Accounts.Domain;
using Convey.Test.Accounts.Services;

namespace Convey.Test.Accounts.Events
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