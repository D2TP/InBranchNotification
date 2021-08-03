using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Queries;
using Convey.Test.Accounts.DTOs;

namespace Convey.Test.Accounts.Queries
{
    public class GetAccounts : IQuery<IEnumerable<AccountDto>>
    {
        public Guid CustomerId { get; internal set; }
    }
}
