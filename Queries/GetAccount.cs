using System;
using Convey.CQRS.Queries;
using InBranchDashboard.DTOs;

namespace InBranchDashboard.Queries
{
    public class GetAccount : IQuery<AccountDto>
    {
        public String AccountNo { get; set; }
        public Guid CustomerId { get; set; }
    }
}
