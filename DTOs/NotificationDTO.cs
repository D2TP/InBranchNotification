using System;
using System.Collections.Generic;

namespace InBranchAuditTrail.DTOs
{
    public class NotificationDTO
    {
         
       
        public string title { get; set; }
        public string type { get; set; }
        public string sender { get; set; }
        public string body { get; set; }
        public List<string> recipents { get; set; }
        public int? recipent_count => recipents.Count;

    }
}
