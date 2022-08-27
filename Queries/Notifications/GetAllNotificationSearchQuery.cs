using Convey.CQRS.Queries;
using InBranchAuditTrail.DTOs;
using InBranchNotification.Domain;
using InBranchNotification.Helpers;
using System;

namespace InBranchNotification.Queries.Notifications
{
    public class GetAllNotificationSearchQuery : IQuery<PagedList<NotificationSearchDTO>>
    {
        public NotificationSearchParameters _notificationSearchParameters;
        public GetAllNotificationSearchQuery()
        {
        }
        public GetAllNotificationSearchQuery(NotificationSearchParameters notificationSearchParameters)
        {
            _notificationSearchParameters =  notificationSearchParameters;
        }
 
    }
}
