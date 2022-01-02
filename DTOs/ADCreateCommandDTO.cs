using InBranchDashboard.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.DTOs
{
    public class ADCreateCommandDTO
    {
        [Display(Name = "ADUserId")]
        public string id { get; set; }
        [Display(Name = "UserName")]
        public string user_name { get; set; }
        [Display(Name = "FirstName")]
        public string first_name { get; set; }
        [Display(Name = "LastName")]
        public string last_name { get; set; }
        [Display(Name = "Role")]
        public string role_name { get; set; }
    
        [Display(Name = "Active")]
        public bool  active{ get; set; }

        [Display(Name = "Is_Role_Active")]
        public bool is_role_active { get; set; }
        [Display(Name = "EntryDate")]
        public DateTime  entry_date   { get; set; }
        [Display(Name = "BranchName")]
        public string branch_name { get; set; }
        [Display(Name = "Email")]
        public string email { get; set; }
        public StaffRoles Roles { get; set; }
        private string displayName;
        public string DisplayName
        {
            get { return fristNameLastName(); }
            set { displayName = value; }
        }


        private string fristNameLastName()
        {
            return first_name + " " + last_name;
        }
        public class StaffRoles
        {
            public string RoleName { get; set; }

            public bool Is_Role_Active { get; set; }

        }
    }


}
