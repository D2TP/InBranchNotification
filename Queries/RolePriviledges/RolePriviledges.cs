using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.RolePriviledges
{
    public class RolePriviledgeQuery : IQuery<List<RolePriviledgeDTO>>
    {
        public RolePriviledgeQuery(string id)
        {
            this.id = id;
        }

        public string id { get; set; }

        public string priviledge_id { get; set; }
        public string role_id { get; set; }

     //   public string permission_id { get; set; }
    }
}
