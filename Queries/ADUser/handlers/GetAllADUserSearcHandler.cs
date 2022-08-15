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
    public class GetAllADUserSearcHandler : IQueryHandler<GetAllADUserSearchQuery, PagedList<ADUserRoleBranchDTO>>
    {
        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<GetAllADUserSearcHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;

        public GetAllADUserSearcHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<GetAllADUserSearcHandler> logger, IConvertDataTableToObject convertDataTableToObject)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _convertDataTableToObject = convertDataTableToObject;
        }
 
        public Task<PagedList<ADUserRoleBranchDTO>> HandleAsync(GetAllADUserSearchQuery query)
        {
            var entity = _dbController.SQLFetchAsync(Sql.SelectAllADUserAndRoleNameBranch).Result.AsEnumerable().OrderBy(on => on.Field<string>("UserName"))
 .ToList();
             
            if (entity.Count == 0)
            {

                _logger.LogError("Error: Server returned no result |Caller:ADUserController/SearchAllADusers-Get|| [GetAllADUserSearcHandler][Handle]");
                throw new HandleGeneralException(400, "Server returned no result");
            }
            var aDCreateCommandDTO = _convertDataTableToObject.ConvertDataRowList<ADUserRoleBranchDTO>(entity).AsQueryable();
            if (!string.IsNullOrEmpty(query._aDUserSearchParameters.Name))
            {
                  aDCreateCommandDTO = aDCreateCommandDTO.Where(s => s.LastNmae.Contains(query._aDUserSearchParameters.Name)
                               || s.FirstName.Contains(query._aDUserSearchParameters.Name));
                
            }

            if (!string.IsNullOrEmpty(query._aDUserSearchParameters.Role))
            {
                aDCreateCommandDTO = aDCreateCommandDTO.Where(s => (s.RoleId).ToLower().Equals(query._aDUserSearchParameters.Role.ToLower()));

            }
            if (!string.IsNullOrEmpty(query._aDUserSearchParameters.Status))
            {
                aDCreateCommandDTO = aDCreateCommandDTO.Where(s => (s.Active).Equals(Convert.ToBoolean( Convert.ToInt32(query._aDUserSearchParameters.Status))));

            }
            // aDCreateCommandDTO.FirstOrDefault().AppRoles = new List<AppRole>();
            var pagelist = PagedList<ADUserRoleBranchDTO>.ToPagedList(aDCreateCommandDTO,
            query._aDUserSearchParameters.PageNumber,
            query._aDUserSearchParameters.PageSize);
              
            return Task.FromResult(pagelist);
        }
    }
}
