using Convey.CQRS.Queries;
using DbFactory;
using InBranchAuditTrail.DTOs;
using InBranchNotification.DbFactory;
using InBranchNotification.Exceptions;
using InBranchNotification.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
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

            var getItems = await _dbController.SQLFetchAsync(Sql.StoredProcSearcNotification, param);

            var entity = getItems.AsEnumerable().OrderBy(on => on.Field<string>("sender")).ToList();
            //  entity.Select(x=>x.Field<string>("recipents")).
            
            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:NotificationController/SearchAllNotification-Get|| [GetAllNotificationSearcHandler][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            var steList = new List<NotificationSearchDTO>();
            foreach (var item in entity)
            {
                var notificationSearchDTO = new NotificationSearchDTO();
                notificationSearchDTO.title = item.Field<string>("id") != null ? Convert.ToString(item.Field<string>("id")) : null;
                notificationSearchDTO.title = item.Field<string>("title") != null ? Convert.ToString(item.Field<string>("title")) : null;
                notificationSearchDTO.notification_date =   Convert.ToDateTime(item.Field<DateTime>("notification_date"))  ;
                notificationSearchDTO.sender = item.Field<string>("sender") != null ? Convert.ToString(item.Field<string>("sender")) : "";
                notificationSearchDTO.completed =   Convert.ToBoolean(item.Field<Boolean>("completed")) ;
                notificationSearchDTO.recipents = item.Field<string>("recipents") != null ? JsonSerializer.Deserialize<List<string>>(item.Field<string>("recipents")) : null;
                notificationSearchDTO.type = item.Field<string>("type") != null ? Convert.ToString(item.Field<string>("type")) : null;
                notificationSearchDTO.body = item.Field<string>("body") != null ? Convert.ToString(item.Field<string>("body")) : null;
                steList.Add(notificationSearchDTO);
                //JsonSerializer.Deserialize(item.Field<string>("sender"),);
            }

            // var aDCreateCommandDTO = _convertDataTableToObject.ConvertDataRowList<NotificationSearchDTO>(entity).AsQueryable();

            // aDCreateCommandDTO.FirstOrDefault().AppRoles = new List<AppRole>();
            var pagelist = PagedList<NotificationSearchDTO>.ToPagedList(steList.AsQueryable(),
            query._notificationSearchParameters.PageNumber,
            query._notificationSearchParameters.PageSize);

            return pagelist;
        }
    }
}

