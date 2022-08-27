using Convey.CQRS.Queries;
 
using InBranchNotification.Domain;
using InBranchNotification.DTOs;
using InBranchNotification.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.Queries.Branches
{
    public class NotificationQueries : IQuery<PagedList<Notification>>
    {
        public QueryStringParameters _queryStringParameters;

        public NotificationQueries(QueryStringParameters queryStringParameters)
        {
            _queryStringParameters = queryStringParameters;
        }
        public NotificationQueries()
        {
        }
        public string id { get; set; }

        public string title { get; set; }
        public string type { get; set; }
        public DateTime notification_date { get; set; }
        public string sender { get; set; }
        public string body { get; set; }
        public bool completed { get; set; }
    }
}
