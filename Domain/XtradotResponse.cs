using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.Domain
{
 

    public class Data
    {
        public bool isValid { get; set; }
    }

    public class XtradotResponse
    {
        public Data data { get; set; }
        public string code { get; set; }
        public string description { get; set; }
    }
}
