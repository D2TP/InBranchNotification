using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchMgt.Commands.RegisterAdUser.Handlers
{
    public class ADCreateCommandDTO:ICommand
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ADUserId { get; set; }
        public string Role { get; set; }
        public bool Active { get; set; }

    }
}
