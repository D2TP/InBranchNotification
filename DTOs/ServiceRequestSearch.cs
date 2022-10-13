using InBranchNotification.Domain;
using InBranchNotification.DTOs;
using InBranchNotification.Helpers;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InBranchNotification.DTOs
{
    public class ServiceRequestSearch : QueryStringParameters
    {
       
        public string id { get; set; }  
        public string service_request_status_id { get; set; } 
        public string service_request_type_id { get; set; }
        public string client { get; set; }
        public string cif_id { get; set; }
        public string approver { get; set; }
        public string reviewer { get; set; }
        public DateTime? fromdate  { get; set; }
        public DateTime? todate { get; set; }
        
      
    }
}
 