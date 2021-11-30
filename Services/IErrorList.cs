using System.Collections.Generic;

namespace InBranchDashboard.Services
{
    public interface IErrorList
    {
         
        public List<string> AddError(string error);
    }
}