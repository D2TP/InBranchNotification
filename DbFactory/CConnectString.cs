using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbFactory
{
    internal struct CConnectString
    {
        public const string Oracle = "Data source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={2})));User Id={3};Password={4};Max Pool Size={5};Connection Timeout={6};";
        public const string SQLServer = "Server={0};Database={2};User ID={3};Password={4};Max Pool Size={5};Connection Timeout={6};";
        public const string PostgreSQL = "Host={0};Port={1};Database={2};Username={3};Password={4};";
    }
}
