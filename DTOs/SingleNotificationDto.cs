using InBranchNotification.Domain;
using System.Collections.Generic;
using System;

namespace InBranchNotification.DTOs
{
    public class SingleNotificationDto
    {

        public string id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public DateTime? notification_date { get; set; }
        public string sender { get; set; }
        public string body { get; set; }
        public bool completed { get; set; }
        public bool approved { get; set; }
        public string approver { get; set; }
        public List<string> recipents { get; set; }

        private int? rcount;

        public int? recipent_count
        {
            get { return recipents != null ? recipents.Count : 0; }
            set { rcount = value; }
        }
    }
}
