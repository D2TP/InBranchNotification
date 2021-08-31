using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.DTOs
{
    public class ADUserInsertDTO
    {

        [Required]
        [StringLength(maximumLength: 250, MinimumLength = 2)]
        public string user_name { get; set; }
        [Required]
        [StringLength(maximumLength: 250, MinimumLength = 2)]
        public string first_name { get; set; }
        [Required]
        [StringLength(maximumLength: 250, MinimumLength = 2)]
        public string last_name { get; set; }
        [Required]
        [StringLength(maximumLength: 250, MinimumLength = 10)]
        public string role_id { get; set; }
        [Required]

        public bool  active{ get; set; }
       
        public string branch_Id { get; set; }
        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        public string email { get; set; }
    }
}
