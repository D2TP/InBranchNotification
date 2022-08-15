
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using Convey.MessageBrokers;
//using Convey.MessageBrokers.Outbox;
using DbFactory;
using InBranchDashboard.Commands.UserRole;
using InBranchDashboard.DbFactory;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Events;
using InBranchDashboard.Exceptions;
using InBranchDashboard.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OpenTracing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.RolePriviledges.handler
{ 

    public class RoleBasedOnPriviledgeQueryHandler : IQueryHandler<RoleBasedOnPriviledgeQueries, PagedList<RoleBaseOnPriviledgeDTO>>
{

        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<RoleBasedOnPriviledgeQueryHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
       // private readonly IBusPublisher _publisher;
        private readonly ITracer _tracer;
       // private readonly IMessageOutbox _outbox;
        public RoleBasedOnPriviledgeQueryHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<RoleBasedOnPriviledgeQueryHandler> logger, IConvertDataTableToObject convertDataTableToObject, ITracer tracer)//, IMessageOutbox outbox)//, IBusPublisher publisher)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _tracer = tracer;
        //    _publisher = publisher;
   //         _outbox = outbox;
            _convertDataTableToObject = convertDataTableToObject;
        }

        public async Task<PagedList<RoleBaseOnPriviledgeDTO>> HandleAsync(RoleBasedOnPriviledgeQueries query)
    {
            var getEntityRoles = await _dbController.SQLFetchAsync(Sql.SelectRoles);
            var entityRoles = getEntityRoles.AsEnumerable().OrderBy(on => on.Field<string>("role_name")).ToList();
            var getEntity =await   _dbController.SQLFetchAsync(Sql.RoleBasedPriviledge);
            var entity = getEntity.AsEnumerable().OrderBy(on => on.Field<string>("priviledge_name"))
  .ToList();
            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:RolePriviledgeController/GetAllCatigories-Get|| [RolePriviledgeQueryHandler][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            
            var roleBaseOnPriviledgeDTO = new List<RoleBaseOnPriviledgeDTO>();
            foreach (var item in entityRoles)
            {  
                var roleBaseOnPriviledgeDTOItem = new RoleBaseOnPriviledgeDTO();
                roleBaseOnPriviledgeDTOItem.id = item.Field<string>("id");
                roleBaseOnPriviledgeDTOItem.role_name = item.Field<string>("role_name");
                roleBaseOnPriviledgeDTOItem.role_id = item.Field<string>("id"); 
                roleBaseOnPriviledgeDTOItem.Priviledges = new List<Priviledge>();
                foreach (var itemEnity in entity.Where(x=>x.Field<string>("role_id") ==item.Field<string>("id")))
                {
                 

                  

                    var priviledgeItem = new Priviledge();
                    priviledgeItem.id = itemEnity.Field<string>("priviledge_id");
                    priviledgeItem.priviledge_name = itemEnity.Field<string>("priviledge_name");

                  
                    roleBaseOnPriviledgeDTOItem.Priviledges.Add(priviledgeItem);
                   
                }  
                roleBaseOnPriviledgeDTO.Add(roleBaseOnPriviledgeDTOItem);
               
                
                 
            }
            
        //  var  _roleBasedPriviledge = _convertDataTableToObject.ConvertDataRowList<RoleBaseOnPriviledgeDTO>(entity).AsQueryable();

            
            var roleBasedOnPriviledge = PagedList<RoleBaseOnPriviledgeDTO>.ToPagedList(roleBaseOnPriviledgeDTO.AsQueryable(),
               query._queryStringParameters.PageNumber,
                query._queryStringParameters.PageSize);

            return roleBasedOnPriviledge;
        }
}
}
