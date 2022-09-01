using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.Commands.Audit
{
    public class NotificationCommand : ICommand
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
