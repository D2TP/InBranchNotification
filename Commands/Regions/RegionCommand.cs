using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.Regions
{
    public class RegionCommand : ICommand
    {
     
        public string id { get; set; }
        public string region_name { get; set; }
    }
}
