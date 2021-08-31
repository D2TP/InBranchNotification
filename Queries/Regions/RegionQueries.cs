using Convey.CQRS.Queries;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Regions
{
    public class RegionQueries: IQuery<List<RegionDTO>>
    {
        public string id { get; set; }
        public string region_name { get; set; }
    }
}
