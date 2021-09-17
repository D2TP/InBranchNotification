using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Branches
{
    public class BranchQueries : IQuery<PagedList<Branch>>
    {
        public QueryStringParameters _queryStringParameters;

        public BranchQueries(QueryStringParameters queryStringParameters)
        {
            _queryStringParameters = queryStringParameters;
        }
        public BranchQueries()
        {
        }
        public string id { get; set; }

        public string branch_name { get; set; }

        public string region_id { get; set; }
    }
}
