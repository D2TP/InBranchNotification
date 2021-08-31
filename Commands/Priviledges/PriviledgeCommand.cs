using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.Priviledges
{
    public class PriviledgeCommand : ICommand
    {
     
        public string id { get; set; }
        public string priviledge_name { get; set; }
    }
}
