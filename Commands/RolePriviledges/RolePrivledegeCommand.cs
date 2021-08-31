using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.RolePriviledges
{
    public class RolePrivledegeCommand:ICommand
    {
        public string id { get; set; }
       
        public string priviledge_id { get; set; }
        public string role_id { get; set; }

        public string permission_id { get; set; }
    }
}
