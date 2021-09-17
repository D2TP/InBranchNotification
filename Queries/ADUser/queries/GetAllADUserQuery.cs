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
    public class GetAllADUserQuery : IQuery<PagedList<ADUserBranchDTO>>
    {
        public  ADUserParameters _aDUserParameters;
        public GetAllADUserQuery()
        {
        }
        public GetAllADUserQuery(ADUserParameters aDUserParameters)
        {
            _aDUserParameters = aDUserParameters;
        }
         
    }
}
