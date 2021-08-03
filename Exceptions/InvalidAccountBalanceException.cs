using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InBranchDashboard.Domain;

namespace InBranchDashboard.Exceptions
{
    

    public class InvalidAccountBalanceException : DomainException
    {
        public override string Code { get; } = "invalid)_account_balance";
        public String AccountNo { get; }
        public Guid CustomerId { get; }
        public Decimal AccountBalance { get; }

        public InvalidAccountBalanceException(Guid customerId , String accountNo, decimal accountBalance) : base(
            $"Invalid account balance for: {accountNo}, customerId : {customerId}. Account Balance : {accountBalance}")
        {
            CustomerId = customerId;
            AccountNo = accountNo;
            AccountBalance = accountBalance;
        }
    }
}
