using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InBranchDashboard.DTOs;
using InBranchDashboard.Events;
using InBranchDashboard.Exceptions;
using InBranchDashboard.Services;
using Convey.Types;

namespace InBranchDashboard.Domain
{
    public class Account : AggregateRoot 
    {
        //public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public String AccountNo { get; private set; }
        public decimal AccountBalance { get; private set; }
        public Status Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool KYCDone { get; private set; }
        public Account(Guid id, Guid customerId, String accountNo, DateTime createdAt, decimal accountBalance,Status status, bool kycDone)
        {
            Id = id;
            CustomerId = customerId;
            AccountNo = accountNo;
            CreatedAt = createdAt;
            AccountBalance = accountBalance;
            Status = status;           
            KYCDone = kycDone;
        }

        internal AccountDto AsDto()
        {
            return new AccountDto
            {
                Id = Id,
                AccountBalance = AccountBalance,
                AccountNo = AccountNo,
            };
        }




        /*public static AccountDetailsDto AsDetailsDto(this Account document)
            => new AccountDetailsDto
            {
                Id = document.Id,
                Email = document.Email,
                FullName = document.FullName,
                PhoneNo = document.PhoneNo,
                Status = document.Status.ToString().ToLowerInvariant(),
                CreatedAt = document.CreatedAt,
                CustomerAccounts = document.CustomerAccounts
            };
        */

        public Account(Guid id, Guid customerId, String accountNo, DateTime createdAt): this (id, customerId, accountNo, createdAt, decimal.Zero, Status.Incomplete, false)
        {
            Id = id;
            CustomerId = customerId;
            AccountNo = accountNo;
        }


        public void CompleteAccountOpening(Guid customerId, string accountNo)
        {           
            if (Status != Status.Incomplete)
            {
                throw new CannotChangeAccountStatusException(customerId, accountNo, Status);
            }

            if (AccountBalance < 100)
            {
                throw new InvalidAccountBalanceException(customerId, accountNo, AccountBalance);
            }
           
            Status= Status.Valid;
            AddEvent(new AccountOpeningCompleted(this));
        }

        public void SetValid() => SetState(Status.Valid);

        public void SetIncomplete() => SetState(Status.Incomplete);

        public void Close() => SetState(Status.Close);

        public void MarkAsSuspicious() => SetState(Status.Suspicious);
        public void MarkAsDormant() => SetState(Status.Dormant);

        private void SetState(Status status)
        {
            var previousState = Status;
            Status = status;
            AddEvent(new AccountStatusChanged(this, previousState));
        }

        public void SetKYCDone()
        {
            if (KYCDone)
            {
                return;
            }

            KYCDone = true;
            AddEvent(new AccountKYCCompleted(this));
        }

        
    }
 
}
