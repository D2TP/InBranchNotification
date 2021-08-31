using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Priviledges
{
    public class PriviledgeQueries : IQuery<List<Priviledge>>
    {
        public string id { get; set; }
        public string priviledge_name { get; set; }
    }
}
