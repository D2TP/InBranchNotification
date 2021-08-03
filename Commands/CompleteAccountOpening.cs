using System;
using Convey.CQRS.Commands;

namespace Convey.Test.Accounts.Commands
{

    public class CompleteAccountOpening : ICommand
    {
        public Guid CustomerId { get; }
        public String AccountNo { get; }      

        public CompleteAccountOpening(Guid customerId, String accountNo, string status)
        {
            CustomerId = customerId;
            AccountNo = accountNo;           
        }
    }
}
