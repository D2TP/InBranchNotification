using System;

namespace InBranchNotification.DTOs
{
    public class ServiceRequestDetail  
    {
        public string id { get; set; }
        public string approver { get; set; }
        public string reviewer { get; set; }
        public DateTime? review_date { get; set; }
        public DateTime? approval_date { get; set; }
        public string status { get; set; }
        public string request_type { get; set; }
        
        public string client { get; set; }
        public string cif_id { get; set; }
        public DateTime service_request_date { get; set; }



    }
}
