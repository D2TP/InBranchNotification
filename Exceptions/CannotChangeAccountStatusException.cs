using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.Test.Accounts.Domain;

namespace Convey.Test.Accounts.Exceptions
{
    

    public class CannotChangeAccountStatusException : DomainException
    {
        public override string Code { get; } = "cannot_change_account_status";
        public String AccountNo { get; }
        public Guid CustomerId { get; }
        public Status Status { get; }

        public CannotChangeAccountStatusException(Guid customerId , String accountNo, Status status) : base(
            $"Cannot change account: {accountNo} state to: {status} for customerId : {customerId}.")
        {
            CustomerId = customerId;
            AccountNo = accountNo;
            Status = status;
        }
    }
}
