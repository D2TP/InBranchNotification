using Convey.CQRS.Queries;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.ADUser.queries
{
    public class GetOneADUserQuery : IQuery<ADCreateCommandDTO>
    {
        public GetOneADUserQuery(int aDUserId)
        {
            ADUserId = aDUserId;
        }

        public int ADUserId { get; set; }
    }
}
