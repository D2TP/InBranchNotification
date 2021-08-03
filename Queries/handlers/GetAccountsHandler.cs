using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Queries;
using Convey.Persistence.MongoDB;
using Convey.Test.Accounts.Domain;
using Convey.Test.Accounts.DTOs;

namespace Convey.Test.Accounts.Queries.handlers
{
    public class GetAccountsHandler : IQueryHandler<GetAccounts,IEnumerable <AccountDto>>    
    {
        private readonly IMongoRepository<Account, Guid> _accountsRepository;

        public GetAccountsHandler(IMongoRepository<Account, Guid> accountsRepository)
        {
            _accountsRepository = accountsRepository;
        }

        public async Task<IEnumerable<AccountDto>> HandleAsync(GetAccounts query)
        {
            var accounts = await _accountsRepository.FindAsync(accts => accts.CustomerId == query.CustomerId);            

            return accounts?.Select(p => p.AsDto());
        }

    }
}
