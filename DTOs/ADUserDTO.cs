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
        public string BranchName { get; set; }
        //public int BranchId { get; set; }
        public List<AppRole> AppRoles { get; set; }
}
    public class AppRole
    {
        public string RoleName { get; set; }

        public bool IsRoleActive { get; set; }

    }
}
