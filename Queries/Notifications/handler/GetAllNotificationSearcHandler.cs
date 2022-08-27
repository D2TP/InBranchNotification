using Convey.CQRS.Queries;
using DbFactory;
using InBranchAuditTrail.DTOs;
using InBranchNotification.DbFactory;
using InBranchNotification.Exceptions;
using InBranchNotification.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.Queries.Notifications.handler
{
   
    public class GetAllNotificationSearcHandler : IQueryHandler<GetAllNotificationSearchQuery, PagedList<NotificationSearchDTO>>
    {
        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<GetAllNotificationSearcHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;

        public GetAllNotificationSearcHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<GetAllNotificationSearcHandler> logger, IConvertDataTableToObject convertDataTableToObject)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task<PagedList<NotificationSearchDTO>> HandleAsync(GetAllNotificationSearchQuery query)
        {

            //@id=#,@title=#,@type=#,@notification_date=#,@sender=#,@body=#,@completed=# ";
            object[] param = {
                    query._notificationSearchParameters.id!=null? query._notificationSearchParameters.id : DBNull.Value,
                    query._notificationSearchParameters.title!=null? query._notificationSearchParameters.title : DBNull.Value,
                    query._notificationSearchParameters.type!=null? query._notificationSearchParameters.type : DBNull.Value,
                    query._notificationSearchParameters.from_entry_date!=null? query._notificationSearchParameters.from_entry_date : DBNull.Value,
                    query._notificationSearchParameters.to_entry_date!=null? query._notificationSearchParameters.to_entry_date : DBNull.Value,
                    query._notificationSearchParameters.sender!=null? query._notificationSearchParameters.sender : DBNull.Value,
                    query._notificationSearchParameters.body!=null? query._notificationSearchParameters.body : DBNull.Value,
                    query._notificationSearchParameters.completed!=null? query._notificationSearchParameters.completed : DBNull.Value };


        //    public DateTime? from_entry_date { get; set; }
        //public DateTime? to_entry_date { get; set; }

        var getItems =await _dbController.SQLFetchAsync(Sql.StoredProcSearcNotification, param);
          // var entity=getItems.AsEnumerable().OrderBy(on => on.Field<string>("sender"))
                 var entity = getItems.AsEnumerable().OrderBy(on => on.Field<string>("sender")).ToList();
            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:NotificationController/SearchAllNotification-Get|| [GetAllNotificationSearcHandler][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            var aDCreateCommandDTO = _convertDataTableToObject.ConvertDataRowList<NotificationSearchDTO>(entity).AsQueryable();
            
            // aDCreateCommandDTO.FirstOrDefault().AppRoles = new List<AppRole>();
            var pagelist = PagedList<NotificationSearchDTO>.ToPagedList(aDCreateCommandDTO,
            query._notificationSearchParameters.PageNumber,
            query._notificationSearchParameters.PageSize);

            return  pagelist;
        }
    }
}
 
