using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.RolePriviledges
{
    public class RolePriviledgeQueries : IQuery<PagedList<RolePriviledgeDTO>>
    {

        public QueryStringParameters _queryStringParameters;

        public RolePriviledgeQueries()
        {
        }

        public RolePriviledgeQueries(QueryStringParameters queryStringParameters)
        {
            _queryStringParameters = queryStringParameters;
        }

        public string id { get; set; }

        public string priviledge_id { get; set; }
        public string role_id { get; set; }

        public string permission_id { get; set; }
    }
}
