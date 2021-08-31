using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Domain
{
    //table name: inb_aduser
    public class ADUser
    {
        public string id { get; set; }
        public string user_name { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public bool active { get; set; }

        public DateTime entry_date { get; set; }

        public string branch_id { get; set; }

        public string email { get; set; }




    }
}
