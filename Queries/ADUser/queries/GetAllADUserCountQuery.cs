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
    public class GetAllADUserCountQuery : IQuery<CountDto>
    {
         
        public GetAllADUserCountQuery()
        {
        }

        //public GetAllADUserCountQuery(int? aDUserId)
        //{
        //    ADUserId = aDUserId;
        //}

        //public int? ADUserId { get; set; }
    }
}
