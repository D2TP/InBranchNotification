using InBranchNotification.DTOs;
using System;

namespace InBranchNotification.Domain
{
    public class ServiceRequest
    {
        public string id { get; set; }
        public string service_request_type_id { get; set; }
        public string service_request_status_id { get; set; }
        public string client { get; set; }
        public string cif_id { get; set; }
        public DateTime? service_request_date { get; set; }
        public string approver { get; set; }
        public string reviewer { get; set; }
        public DateTime? review_date { get; set; }
        public DateTime? approval_date { get; set; }

    }
}
