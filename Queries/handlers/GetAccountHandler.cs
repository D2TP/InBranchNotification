using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Queries;
//using Convey.Persistence.MongoDB;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;

namespace InBranchDashboard.Queries.handlers
{
   

    public class GetAccountHandler : IQueryHandler<GetAccount, AccountDto>
    {
        //private readonly IMongoRepository<Account, Guid> _accountsRepository;

        //public GetAccountHandler(IMongoRepository<Account, Guid> accountsRepository)
        //{
        //    _accountsRepository = accountsRepository;
        //}

        public async Task<AccountDto> HandleAsync(GetAccount query)
        {
            //var account = await _accountsRepository.GetAsync(accnt => accnt.CustomerId == query.CustomerId);
            //return account is null
            //                ? null
            //                : new AccountDto { Id = account.Id, CustomerId = account.CustomerId, AccountBalance = account.AccountBalance };
            return null;
        }

    }
}
