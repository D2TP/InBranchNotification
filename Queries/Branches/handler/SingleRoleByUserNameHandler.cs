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
using System.Data;
using InBranchDashboard.Domain;
using InBranchDashboard.Queries.Branches;

namespace InBranchDashboard.Queries.Roles.handler
{
    public class SingleRoleByUserNameHandler : IQueryHandler<UserRoleQuery, List<AdUserRoleDTO>>
    {
        private readonly IDbController _dbController;
        private readonly SystemSettings _systemSettings;
        // private readonly IMapper _mapper;
        private readonly ILogger<GetADUserAndassingedRoleHandler> _logger;
        private readonly IConvertDataTableToObject _convertDataTableToObject;

        public SingleRoleByUserNameHandler(IMemoryCache memoryCache, IDbController dbController, ILogger<GetADUserAndassingedRoleHandler> logger, IConvertDataTableToObject convertDataTableToObject)
        {
            _dbController = dbController;
            _systemSettings = new SystemSettings(memoryCache);
            _logger = logger;
            _convertDataTableToObject = convertDataTableToObject;
        }
        public async Task<List<AdUserRoleDTO>> HandleAsync(UserRoleQuery query)
        {
            object[] param = { query.AdUserName };
            var entity = await _dbController.SQLFetchAsync(Sql.SelectADUserByUserName, param);
            if (entity.Rows.Count == 0)
            {
                _logger.LogError("Error: There is no user with {User Id} |Caller:ADUserController/GetAnDUsers-Get|| [CreateOneADUserHandler][Handle]", query.AdUserName);
                throw new HandleGeneralException(404, "User does not exist");
            }
            List<AdUserRoleDTO> adUserRoleDTO = new List<AdUserRoleDTO>();
            adUserRoleDTO = entity.AsEnumerable()
      .Select(x => new AdUserRoleDTO
      {
        // assuming column 0's type is Nullable<long>
        id = (string)x["id"],
          active = (bool)x["active"] ? (bool)x["active"] : false,
          email = (string)x["email"],
          branch_name = (string)x["branch_name"],
          entry_date = (DateTime)x["entry_date"],
          first_name = (string)x["first_name"],
          last_name = (string)x["last_name"],
          user_name = (string)x["last_name"],
          Role = new RoleDTo
          {
              id = (string)x["role_id"],
              role_name = (string)x["role_name"],
              category_id= (string)x["category_id"],
              is_role_active= (bool)x["is_role_active"]

          }
      }).ToList();
          //  adUserRoleDTO = _convertDataTableToObject.ConvertDataTable<AdUserRoleDTO>(entity);

            return adUserRoleDTO;
        }
    }
}

