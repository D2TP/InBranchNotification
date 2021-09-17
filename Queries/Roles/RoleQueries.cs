using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Roles
{
    public class RoleQueries : IQuery<PagedList<Role>>
    {
        public QueryStringParameters _queryStringParameters;
        public RoleQueries()
        {
        }

        public RoleQueries(QueryStringParameters queryStringParameters)
        {
            _queryStringParameters = queryStringParameters;
        }
        public string id { get; set; }

        public string role_name { get; set; }

        public string category_id { get; set; }
    }
}
