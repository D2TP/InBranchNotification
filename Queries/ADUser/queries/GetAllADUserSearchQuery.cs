using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.ADUser.queries
{
    public class GetAllADUserSearchQuery : IQuery<PagedList<ADUserRoleBranchDTO>>
    {
        public  ADUserSearchParameters _aDUserSearchParameters;
        public GetAllADUserSearchQuery()
        {
        }
        public GetAllADUserSearchQuery(ADUserSearchParameters aDUserSearchParameters)
        {
            _aDUserSearchParameters = aDUserSearchParameters;
        }
         
    }
}
