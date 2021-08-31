using InBranchDashboard.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.DTOs
{
    public class AdUserRoleDTO
    {
        [Display(Name = "ADUserId")]
        public string id { get; set; }
        [Display(Name = "UserName")]
        public string user_name { get; set; }
        [Display(Name = "FirstName")]
        public string first_name { get; set; }
        [Display(Name = "LastName")]
        public string last_name { get; set; }
   
        [Display(Name = "Active")]
        public bool active { get; set; }
        [Display(Name = "EntryDate")]
        public DateTime entry_date { get; set; }
        [Display(Name = "BranchName")]
        public string branch_name { get; set; }
        [Display(Name = "Email")]
        public string email { get; set; }
        public Role Role { get; set; }
    }
}
