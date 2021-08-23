using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.CreatUserRole
{
    public class CreateUserRoleComand: ICommand 
    {
        public int ADUserId { get; set; }

        public int RoleId { get; set; }

    }
}
