using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.Permissions
{
    public class PermissionCommand : ICommand
    {
     
        public string id { get; set; }
        public string permission_name { get; set; }
    }
}
