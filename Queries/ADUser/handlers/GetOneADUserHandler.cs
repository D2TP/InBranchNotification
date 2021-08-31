using Convey.CQRS.Queries;
using DbFactory;
using InBranchDashboard.DbFactory;
using InBranchDashboard.DTOs;
using InBranchDashboard.Exceptions;
using InBranchDashboard.Queries.ADUser.queries;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.ADUser.handlers
{
    public class GetOneADUserHandler : IQueryHandler<GetOneADUserQuery, ADCreateCommandDTO>
    {
        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<GetOneADUserHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;

        public GetOneADUserHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<GetOneADUserHandler> logger, IConvertDataTableToObject convertDataTableToObject)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _convertDataTableToObject = convertDataTableToObject;
        }
        public async Task<ADCreateCommandDTO> HandleAsync(GetOneADUserQuery query)
        {
            object[] param = { query.ADUserId };
            var entity = await _dbController.SQLFetchAsync(Sql.SelectADUserAndRoleName, param);
            if (entity.Rows.Count ==0)
            {
                _logger.LogError("Error: There is no user with {User Id} |Caller:ADUserController/GetAnDUsers-Get|| [CreateOneADUserHandler][Handle]", query.ADUserId);
                throw new HandleGeneralException(404, "User does not exist");
            }
            List<ADCreateCommandDTO> aDCreateCommandDTO = new List<ADCreateCommandDTO>();
            aDCreateCommandDTO = _convertDataTableToObject.ConvertDataTable<ADCreateCommandDTO>(entity);

            return aDCreateCommandDTO.FirstOrDefault();
        }
    }
}
 
