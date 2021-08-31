 
using Convey.CQRS.Commands;
 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.AdUser
{
    public class CreateADUserCommand : ICommand
    {
        //public CreateADUserCommand()
        //{
        //}

        //public CreateADUserCommand(string userName, string firstName, string lastName, string roleId, bool active)
        //{
        //    UserName = userName;
        //    FirstName = firstName;
        //    LastName = lastName;
        //    RoleId = roleId;
        //    Active = active;

        //}
        public string id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        
        public string RoleId { get; set; }
        public bool Active { get; set; }

        public string BranchId { get; set; }

        public string Email { get; set; }

    }

    // check and thow and array of exception. 
}
