using InBranchNotification.Domain;
using System;
using System.Diagnostics;

namespace InBranchNotification.DTOs
{
    public class ServiceHistroySearchDto : QueryStringParameters
    {
        public string id { get; set; }
        public string actor { get; set; }
        public string status { get; set; }
        public DateTime? from_entry_date { get; set; }
        public DateTime? to_entry_date { get; set; }
        public string service_request_id { get; set; }  
        public string comment { get; set; }
        public string activity { get; set; }
    }
}
 