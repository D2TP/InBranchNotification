using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.ADUser.queries
{
    public class GetOneADUserQuery : IQuery<ObjectResponse>
    {
        public GetOneADUserQuery(string aDUserId)
        {
            ADUserId = aDUserId;
        }

        public string ADUserId { get; set; }
    }
}
