using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Priviledges
{
    public class PriviledgeQueries : IQuery<PagedList<Priviledge>>
    {
        public QueryStringParameters _queryStringParameters;

        public PriviledgeQueries()
        {
        }

        public PriviledgeQueries(QueryStringParameters queryStringParameters)
        {
            _queryStringParameters = queryStringParameters;
        }
        public string id { get; set; }
        public string priviledge_name { get; set; }
    }
}
