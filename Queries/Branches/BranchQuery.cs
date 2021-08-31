﻿using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Branches
{
    public class BranchQuery : IQuery<Branch>
    {
        public BranchQuery(string id)
        {
            this.id = id;
        }

        public string id { get; set; }

        public string branch_name { get; set; }

        public string region_id { get; set; }
    }
}
