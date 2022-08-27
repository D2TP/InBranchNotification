using System.Collections.Generic;

namespace InBranchNotification.Services
{
    public interface IErrorList
    {
         
        public List<string> AddError(string error);
    }
}