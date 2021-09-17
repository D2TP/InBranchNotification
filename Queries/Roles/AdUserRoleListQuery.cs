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
    public class AdUserRoleListQuery :IQuery<PagedList<AdUserRoleDTO>>
    {   
        public QueryStringParameters _queryStringParameters;
        public AdUserRoleListQuery(string adeUserId, QueryStringParameters queryStringParameters)
        {
            AdeUserId = adeUserId;
            _queryStringParameters = queryStringParameters;
        }

        public AdUserRoleListQuery(string adeUserId )
        {
            AdeUserId = adeUserId;
           
        }
        public AdUserRoleListQuery ()
        {
             

        }

        public string AdeUserId { get; set; }
    }
}
