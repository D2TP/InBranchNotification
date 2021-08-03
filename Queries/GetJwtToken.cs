﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Queries;

namespace InBranchDashboard.Queries.handlers
{
    public class GetJwtToken : IQuery<string>
    {
        public String UserName { get; set; }
        public String Password { get; set; }
    }
}
