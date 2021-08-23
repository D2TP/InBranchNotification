 
using Convey.CQRS.Commands;
 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchMgt.Commands.RegisterAdUser.Handlers
{
    public class CreateADUserCommand : ICommand 
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        
        public int RoleId { get; set; }
        public bool Active { get; set; }


    }

}
