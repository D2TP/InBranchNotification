using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Permissions
{
    public class PermissionQueries : IQuery<PagedList<Permission>>
    {
        public QueryStringParameters _queryStringParameters;

        public PermissionQueries()
        {
        }

        public PermissionQueries(QueryStringParameters queryStringParameters)
        {
            _queryStringParameters = queryStringParameters;
        }
        public string id { get; set; }
        public string permission_name { get; set; }
    }
}
