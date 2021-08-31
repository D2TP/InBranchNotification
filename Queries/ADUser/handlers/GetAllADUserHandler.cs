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
    public class GetAllADUserHandler : IQueryHandler<GetAllADUserQuery, List<ADCreateCommandDTO>>
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
        public async Task<List<ADCreateCommandDTO>> HandleAsync(GetAllADUserQuery query)
        {
             
            var entity = await _dbController.SQLFetchAsync(Sql.SelectAllADUserAndRoleName);
            if (entity.Rows.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:ADUserController/GetAllADUsers-Get|| [GetAllADUserHandler][Handle]");
                throw new HandleGeneralException(500, "Server returned no result");
            }
            List<ADCreateCommandDTO> aDCreateCommandDTO = new List<ADCreateCommandDTO>();
            aDCreateCommandDTO = _convertDataTableToObject.ConvertDataTable<ADCreateCommandDTO>(entity);

            return aDCreateCommandDTO;
        }
    }
}
