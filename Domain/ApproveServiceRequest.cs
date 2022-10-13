namespace InBranchNotification.Domain
{
    public class ApproveServiceRequest
    {
        public string service_request_status_id { get; set; }
        public string approver { get; set; }
      
        public string id { get; set; }
        public string comment { get; set; }
    }
}
