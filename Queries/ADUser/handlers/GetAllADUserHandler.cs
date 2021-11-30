using Convey.CQRS.Queries;
using DbFactory;
using InBranchDashboard.DbFactory;
using InBranchDashboard.DTOs;
using InBranchDashboard.Exceptions;
using InBranchDashboard.Helpers;
using InBranchDashboard.Queries.ADUser.queries;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.ADUser.handlers
{
    public class GetAllADUserHandler : IQueryHandler<GetAllADUserQuery, PagedList<ADUserBranchDTO>>
    {
        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<GetOneADUserHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;

        public GetAllADUserHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<GetOneADUserHandler> logger, IConvertDataTableToObject convertDataTableToObject)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _convertDataTableToObject = convertDataTableToObject;
        }
 
        public Task<PagedList<ADUserBranchDTO>> HandleAsync(GetAllADUserQuery query)
        {
            var entity = _dbController.SQLFetchAsync(Sql.SelectADUserAndBranch).Result.AsEnumerable().OrderBy(on => on.Field<string>("user_name"))
 .ToList();
            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:ADUserController/GetAllADUsers-Get|| [GetAllADUserHandler][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            var aDCreateCommandDTO = _convertDataTableToObject.ConvertDataRowList<ADUserBranchDTO>(entity).AsQueryable();
            var pagelist = PagedList<ADUserBranchDTO>.ToPagedList(aDCreateCommandDTO,
            query._aDUserParameters.PageNumber,
            query._aDUserParameters.PageSize);
             
            return Task.FromResult(pagelist);
        }
    }
}
