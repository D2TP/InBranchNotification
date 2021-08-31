using Convey.CQRS.Queries;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Roles
{
    public class AdUserRoleListQuery :IQuery<List<AdUserRoleDTO>>
    {
        public AdUserRoleListQuery(string adeUserId)
        {
            AdeUserId = adeUserId;
        }

        public string AdeUserId { get; set; }
    }
}
