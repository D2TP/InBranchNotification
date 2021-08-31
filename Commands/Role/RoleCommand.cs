using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.Roles
{
    public class RoleCommand : ICommand
    {

        public string id { get; set; }
        public string role_name { get; set; }
        public string category_id { get; set; }
    }
}
