using Convey.Test.Accounts.Domain;

namespace Convey.Test.Accounts.Commands.Events
{
    public class AccountCreated
    {
        private Account account;

        public AccountCreated(Account account)
        {
            this.account = account;
        }
    }
}