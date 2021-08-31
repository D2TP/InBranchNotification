using Convey.CQRS.Queries;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Regions
{
    public class RegionQuery : IQuery<RegionDTO>
    {
        public RegionQuery(string id)
        {
            this.id = id;
        }

        public string id { get; set; }
        public string region_name { get; set; }
    }
}
