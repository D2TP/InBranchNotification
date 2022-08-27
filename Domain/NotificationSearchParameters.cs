using System;

namespace InBranchNotification.Domain
{
    public class NotificationParameters : QueryStringParameters
    {

    }

    public class NotificationSearchParameters : QueryStringParameters
    {
        public string id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public DateTime? from_entry_date { get; set; }
        public DateTime? to_entry_date { get; set; }
        public string sender { get; set; }
        public string body { get; set; }
        public bool? completed { get; set; }
    }
    
}
