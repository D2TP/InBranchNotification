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
    public class RoleBasedOnPriviledgeQueries : IQuery<PagedList<RoleBaseOnPriviledgeDTO>>
    {

        public QueryStringParameters _queryStringParameters;

        public RoleBasedOnPriviledgeQueries()
        {
        }

        public RoleBasedOnPriviledgeQueries(QueryStringParameters queryStringParameters)
        {
            _queryStringParameters = queryStringParameters;
        }

        public string id { get; set; }

        public string priviledge_id { get; set; }
        public string role_id { get; set; }
        public List<Priviledge> Priviledges { get; set; }
     

       
    }


}
