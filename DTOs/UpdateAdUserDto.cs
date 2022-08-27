using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.DTOs
{
    public class UpdateAdUserDto
    {

        public string id { get; set; }

        public string user_name { get; set; }
 

        public string role_id { get; set; }
        public bool active { get; set; }

        public string branch_Id { get; set; }

        public string Domain { get; set; }
  
        
    }
}
