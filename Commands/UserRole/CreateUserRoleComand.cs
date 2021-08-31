using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.UserRole
{
    public class CreateUserRoleComand: ICommand 
    {
        public string ADUserId { get; set; }

        public string RoleId { get; set; }

    }
}
