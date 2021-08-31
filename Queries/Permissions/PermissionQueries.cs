using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Permissions
{
    public class PermissionQueries : IQuery<List<Permission>>
    {
        public string id { get; set; }
        public string permission_name { get; set; }
    }
}
