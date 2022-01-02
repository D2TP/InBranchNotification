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
    public class UserRoleQuery : IQuery<List<AdUserRoleDTO>>
    {
        public QueryStringParameters _queryStringParameters;
        public UserRoleQuery(string adUserName, QueryStringParameters queryStringParameters)
        {
            AdUserName = adUserName;
            _queryStringParameters = queryStringParameters;
        }

        public UserRoleQuery(string adUserName)
        {
            AdUserName = adUserName;

        }
        public UserRoleQuery()
        {


        }

        public string AdUserName { get; set; }
    }
}