using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.UserRole
{
    public class ADUserAndRold:ICommand
    {
        public string AdUserId { get; set; }
        public string RoleId { get; set; }
        public bool Active { get; set; }
    }
}
