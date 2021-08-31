using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Events
{
    public class GenericCreatedEvent
    {
        private string _created;
        private string _objectId;
        private string _UserRoleId;
        public GenericCreatedEvent(string created, string objectId)
        {
            _created = created;
            _objectId =  objectId;
             
        }
    }
}
