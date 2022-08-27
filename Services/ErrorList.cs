using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.Services
{
    public class ErrorList : IErrorList
    {
      

    
        public List<string> AddError(string error )
        {
            List<string> ErrorLists = new List<string>();
            ErrorLists.Add(error);

            return ErrorLists;
        }
    }
}
