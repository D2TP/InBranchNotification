using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Commands;

namespace Convey.Test.Accounts.Commands
{
    public class CreateAccount : ICommand
    {
        public Guid AccountId { get; }
        public Guid CustomerId { get; }
        public String  AccountNo { get; set; }
        public CreateAccount(Guid accountId, Guid customerId, String accountNo)
        {
            AccountId = AccountId == Guid.Empty ? Guid.NewGuid() : accountId;
            CustomerId = customerId;
            AccountNo = accountNo;
        }
    }
}
