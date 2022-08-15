using InBranchDashboard.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.DTOs
{
    public class RoleBaseOnPriviledgeDTO 
    {
        public string id { get; set; }
        public string priviledge_name { get; set; }
        public string role_id { get; set; }
        public string priviledge_id { get; set; }
        public string role_name { get; set; }
        public int? RoleCount { get; set; }
        public List<Priviledge> Priviledges { get; set; }
        
    }
}
