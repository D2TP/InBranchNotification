﻿using Convey.CQRS.Queries;
using InBranchNotification.Domain;
using InBranchNotification.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.Queries.Branches
{
    public class NotificationQuery : IQuery<SingleNotificationDto>
    {
        public NotificationQuery(string id)
        {
            this.id = id;
        }

        public string id { get; set; }

        public string branch_name { get; set; }

        public string region_id { get; set; }
    }
}
