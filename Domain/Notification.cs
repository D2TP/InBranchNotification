using System;

namespace InBranchNotification.Domain
{
    public class Notification
    {
        public string id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public DateTime notification_date { get; set; }
        public string sender { get; set; }
        public string body { get; set; }
        public bool completed { get; set; }
    }
}
