using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.Commands.AdUser
{
    public class UpdateAduser : ICommand
    {
     
        public string id { get; set; }
       
        public string user_name { get; set; }
      
        //public string first_name { get; set; }
        
        //public string last_name { get; set; }

        public string role_id { get; set; }
        public bool active { get; set; }

        public string branch_Id { get; set; }

        public string Domain { get; set; }
        //public string email { get; set; }
        public string modified_by { get; set; }


    }
}
