﻿using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.RolePriviledges
{
    public class RolePriviledgeQueries : IQuery<List<RolePriviledgeDTO>>
    {
        public string id { get; set; }

        public string priviledge_id { get; set; }
        public string role_id { get; set; }

        public string permission_id { get; set; }
    }
}