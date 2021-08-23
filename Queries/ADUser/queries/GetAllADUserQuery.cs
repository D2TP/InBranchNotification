using Convey.CQRS.Queries;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.ADUser.queries
{
    public class GetAllADUserQuery: IQuery<List<ADCreateCommandDTO>>
    {
    }
}
