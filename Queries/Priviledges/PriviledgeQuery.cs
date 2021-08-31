using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Priviledges
{
    public class PriviledgeQuery : IQuery<Priviledge>
    {
        public PriviledgeQuery(string id)
        {
            this.id = id;
        }

        public string id { get; set; }
        public string priviledge_name { get; set; }
    }
}
