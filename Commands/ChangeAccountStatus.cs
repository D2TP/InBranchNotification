using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Commands;

namespace Convey.Test.Accounts.Commands
{
    public class ChangeAccountStatus : ICommand
    {
        public Guid CustomerId { get;   }
        public String AccountNo { get; }
        public String Status { get; }

        public ChangeAccountStatus(Guid customerId, String accountNo, string status)
        {
            CustomerId = customerId;
            AccountNo = accountNo;
            Status = status;
        }
    }
}
