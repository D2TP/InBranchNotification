using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.DTOs
{
    public class AddRoleDTO
    {
        [Required]
        public string role_name { get; set; }
         
        [Required]
        public string category_id { get; set; }
    }
}
