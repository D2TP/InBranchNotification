using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace InBranchDashboard.Services
{
    public interface IValidateService
    {
        bool IsString(string textValue);
       
    }
}