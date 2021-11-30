using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Domain
{
 

    public class StaffDomain
    {
        public string displayText { get; set; }
        public string code { get; set; }
    }

    public class FBNDomain
    {
        public List<StaffDomain> data { get; set; }
        public string code { get; set; }
        public string description { get; set; }
    }

}
