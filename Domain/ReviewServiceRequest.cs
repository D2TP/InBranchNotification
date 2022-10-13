namespace InBranchNotification.Domain
{
    public class ReviewServiceRequest
    {
        public string service_request_status_id { get; set; }
        public string reviewer { get; set; }
        public string id { get; set; }

        public string comment { get; set; }

    }
}
