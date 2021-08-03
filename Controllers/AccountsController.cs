using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using Convey.Test.Accounts.Commands;
using Convey.Test.Accounts.DTOs;
using Convey.Test.Accounts.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Convey.Test.Accounts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public AccountsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet("{accountNo}")]
        public async Task<ActionResult<AccountDto>> Get([FromRoute] GetAccount query)
        {
            var account = await _queryDispatcher.QueryAsync(query);
            if (account is null)
            {
                return NotFound();
            }

            return account;
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreateAccount command)
        {
            await _commandDispatcher.SendAsync(command);
            return CreatedAtAction(nameof(Get), new { accountNo = command.AccountNo }, null);
        }
        [HttpPut]
        public async Task<ActionResult> Put(CreateAccount command)
        {
            await _commandDispatcher.SendAsync(command);
            return CreatedAtAction(nameof(Get), new { accountNo = command.AccountNo }, null);
        }
        [HttpDelete]
        public async Task<ActionResult> Delete(CreateAccount command)
        {
            await _commandDispatcher.SendAsync(command);
            return CreatedAtAction(nameof(Get), new { accountNo = command.AccountNo }, null);
        }
    }
}
