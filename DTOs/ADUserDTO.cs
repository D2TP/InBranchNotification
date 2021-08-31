using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.DTOs
{
    public class ADUserDTO
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastNmae { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }

        public int BranchId { get; set; }
    }
}
