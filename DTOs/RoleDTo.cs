using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.DTOs
{
    public class RoleDTo
    {
        //  id,role_name,category_id
        [Display(Name = "RoleId")]
        public string id { get; set; }
        [Display(Name = "RoleName")]
        public string role_name { get; set; }
        [Display(Name = "CategoryId")]
        public string category_id { get; set; }

        public bool is_role_active { get; set; }
    }
}
