using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace InBranchNotification.Services
{
    public class ValidateService : IValidateService
    {
        public bool IsString(string textValue)
        {
            var result = false; 
            result = textValue.All(c => Char.IsLetterOrDigit(c) || c == '_');
            if (textValue == string.Empty)
            {
                result = false;
            }
            if (textValue.Length < 2)
            {
                result = false;
            }
          
           
            return result;
        }


        
    }
}