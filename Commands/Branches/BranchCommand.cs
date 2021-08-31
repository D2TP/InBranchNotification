using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.Branches
{
    public class BranchCommand : ICommand
    {

        public string id { get; set; }
        public string branch_name { get; set; }
        public string region_id { get; set; }
    }
}
