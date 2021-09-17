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
    public class AdUserRoleQuery :IQuery<List<AdUserRoleDTO>>
    {   
        public QueryStringParameters _queryStringParameters;
        public AdUserRoleQuery(string adeUserId, QueryStringParameters queryStringParameters)
        {
            AdeUserId = adeUserId;
            _queryStringParameters = queryStringParameters;
        }

        public AdUserRoleQuery(string adeUserId )
        {
            AdeUserId = adeUserId;
           
        }
        public AdUserRoleQuery()
        {
             

        }

        public string AdeUserId { get; set; }
    }
}
