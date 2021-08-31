using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.AdUser
{
    public class DeleteAdUser:ICommand
    {
        public string AdUserId { get; set; }
    }
}
