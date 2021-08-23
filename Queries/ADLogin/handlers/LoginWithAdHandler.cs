using Convey.CQRS.Queries;
using InBranchDashboard.Queries.ADLogin.queries;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.ADLogin.handlers
{
    public class LoginWithAdHandler : IQueryHandler<LoginWithAdQuery, bool>
    {
        private readonly ILogger<LoginWithAdHandler> _logger;

        public LoginWithAdHandler(ILogger<LoginWithAdHandler> logger)
        {
            _logger = logger;
        }

        public Task<bool> HandleAsync(LoginWithAdQuery query)
        {
            var adPassed = false;

            string userNamePass = "springo";

            if (query.UserName == userNamePass)
            {
                //Check AD first here
                adPassed = true;
                return Task.FromResult(adPassed);
            }
            else
            {

                _logger.LogWarning("Failed User name and Password --> username: {request.UserName} || [LoginWithAdHandler][Handle]", query.UserName);

                return Task.FromResult(false);
            }
        }
    }
}
