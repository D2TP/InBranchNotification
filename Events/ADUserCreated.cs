using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Events
{
    public class ADUserCreated
    {
        private int _ADUserId;

        public ADUserCreated(int ADUserId)
        {
            _ADUserId = ADUserId;
        }
    }
}
