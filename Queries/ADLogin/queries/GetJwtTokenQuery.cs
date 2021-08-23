using Convey.CQRS.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.ADLogin.queries
{
    public class GetJwtTokenQuery : IQuery<string>
    {

        public GetJwtTokenQuery(string userName, string displayName, string email)
        {
            UserName = userName;
            DisplayName = displayName;
            Email = email;
        }

        public string UserName { get; set; }

        public string DisplayName { get; set; }
        public string Email { get; set; }

    }
}
