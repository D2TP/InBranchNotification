using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.DTOs
{
    public class ADUserRoleBranchDTO
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastNmae { get; set; }
        public string Email { get; set; }
        public string BranchName { get; set; }
        public string BranchId { get; set; }
        public string RoleId { get; set; }
        public bool Active { get; set; }
        public string RoleName { get; set; }
        public DateTime EntryDate { get; set; }
        // public List<UserAppRole> UserAppRoles { get; set; }
    }
    //public class UserAppRole
    //{
    //    public string RoleName { get; set; }

    //    public bool IsRoleActive { get; set; }

    //}
}
