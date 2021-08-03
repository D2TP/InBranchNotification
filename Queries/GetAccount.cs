using System;
using Convey.CQRS.Queries;
using Convey.Test.Accounts.DTOs;

namespace Convey.Test.Accounts.Queries
{
    public class GetAccount : IQuery<AccountDto>
    {
        public String AccountNo { get; set; }
        public Guid CustomerId { get; set; }
    }
}
