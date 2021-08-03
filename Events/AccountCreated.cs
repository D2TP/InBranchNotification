using InBranchDashboard.Domain;

namespace InBranchDashboard.Commands.Events
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