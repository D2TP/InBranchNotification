using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Roles
{
    public class RoleQuery : IQuery<Role>
    {
        public RoleQuery(string id)
        {
            this.id = id;
        }

        public string id { get; set; }
        public string category_name { get; set; }
    }
}
