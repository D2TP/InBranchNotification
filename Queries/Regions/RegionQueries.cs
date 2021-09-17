using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Regions
{
    public class RegionQueries: IQuery<PagedList<RegionDTO>>
    {
        public QueryStringParameters _queryStringParameters;

        public RegionQueries()
        {
        }

        public RegionQueries(QueryStringParameters queryStringParameters)
        {
            _queryStringParameters = queryStringParameters;
        }

        public string id { get; set; }
        public string region_name { get; set; }
    }
}
