using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.AdUser
{
    public class UpdateAduser : ICommand
    {
     
        public string id { get; set; }
       
        public string UserName { get; set; }
      
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public string RoleId { get; set; }
        public bool Active { get; set; }

        public string BranchId { get; set; }

        public string Email { get; set; }

    }
}
