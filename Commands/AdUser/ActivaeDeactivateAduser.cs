using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.Commands.AdUser
{
    public class ActivaeDeactivateAduser : ICommand
    {
        public string AdUserId { get; set; }
        public bool Active { get; set; }
    }
}
