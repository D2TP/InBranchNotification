using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace InBranchNotification.Services
{
    public interface IValidateService
    {
        bool IsString(string textValue);
       
    }
}