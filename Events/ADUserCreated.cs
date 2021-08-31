using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Events
{
    public class ADUserCreated
    {
        private string _ADUserId;
        private readonly string _RoleId;
        private readonly string _UserRoleId;
        public ADUserCreated(string ADUserId,string UserRoleId, string RoleId )
        {
            _ADUserId = ADUserId;
            _RoleId= RoleId   ;
            _UserRoleId = UserRoleId;
        }
    }
}
