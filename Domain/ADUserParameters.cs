using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Domain
{
    public class ADUserParameters : QueryStringParameters
    {

    }

    public class ADUserSearchParameters : QueryStringParameters
    {
        public string Role { get; set; }
        public string Name { get; set; }
        public string  Status { get; set; }
    }
}
