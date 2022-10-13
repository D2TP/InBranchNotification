using System;

namespace InBranchNotification.DTOs
{
    public class ServiceRequestHistoryDto
    {
        public string id { get; set; }
        public string actor { get; set; }
        public string activity { get; set; }
        public DateTime? activity_date { get; set; }
        public string comment { get; set; }
        public string service_request_id { get; set; }
        public string status { get; set; }

        public string service_status { get; set; }

        public string request_type { get; set; }
    }
}
