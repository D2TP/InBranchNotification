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
    public class GetAllADUserCountHandler : IQueryHandler<GetAllADUserCountQuery,CountDto> 
    {
        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<GetAllADUserCountHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;
        //GetAllADUserCountQuery : IQuery<CountDto>
        public GetAllADUserCountHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<GetAllADUserCountHandler> logger, IConvertDataTableToObject convertDataTableToObject)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _convertDataTableToObject = convertDataTableToObject;
        }
 
        public async Task<CountDto> HandleAsync(GetAllADUserCountQuery query)
        {
            //await _dbController.SQLFetchAsync(Sql.SelectADUserCount, param);
            var entity = await _dbController.SQLFetchAsync(Sql.SelectADUserCount);  
            var entityActive= await _dbController.SQLFetchAsync(Sql.SelectADUserActiveCount);
            var entityInactive = await _dbController.SQLFetchAsync(Sql.SelectADUserInactiveCount);
            //SelectADUserActiveCount SelectADUserInactiveCount
            if (entity.Rows.Count == 0)
            {
                _logger.LogError("Error: There is no user with {User Id} |Caller:ADUserController/GetADUserCount-Get|| [GetAllADUserCountHandler][Handle]");
                throw new HandleGeneralException(404, "User does not exist");
            }
            var countDto = new CountDto();
            countDto.Total = _convertDataTableToObject.GetItem<CountDto>(entity.Rows[0]).Total;
            countDto.ActiveUser = _convertDataTableToObject.GetItem<CountDto>(entityActive.Rows[0]).ActiveUser;
            countDto.Inactive = _convertDataTableToObject.GetItem<CountDto>(entityInactive.Rows[0]).Inactive;

            return countDto;
        }
    }
}
