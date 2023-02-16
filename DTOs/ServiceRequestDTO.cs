using System;

namespace InBranchNotification.DTOs
{
    public class ServiceRequestDTO
    {
       
      
        public string service_request_type_id { get; set; }
        public string service_request_status_id { get; set; }
        public string client { get; set; }
        public string cif_id { get; set; }
        public string other_request_details { get; set; }
        public string comment { get; set; }

    }
}
