using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Exceptions
{
    public class HandleGeneralException : Exception
    {
        public string ErrorDescription { get; set; }
        public int ErrorCode { get; set; }
        public HandleGeneralException(int errorCode, string error)
        {
            ErrorDescription = error;
            ErrorCode = errorCode;
        }
    }
}
