using Convey.CQRS.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.ADLogin.queries
{
    public class LoginWithAdQuery:IQuery<bool>
    {
        public LoginWithAdQuery(string userName, string password)
        {
            UserName = userName;
            Password = password;

        }

        public String UserName { get; set; }
        public String Password { get; set; }
    }
}
