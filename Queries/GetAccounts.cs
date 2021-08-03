using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Queries;
using InBranchDashboard.DTOs;

namespace InBranchDashboard.Queries
{
    public class GetAccounts : IQuery<IEnumerable<AccountDto>>
    {
        public Guid CustomerId { get; internal set; }
    }
}
