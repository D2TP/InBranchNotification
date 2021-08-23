using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  DbFactory
{
    public class ApiInvokeModel
    {
        public bool Status { get; set; }
        public String ResponseObject { get; set; }
        public String RequestPayload { get; set; }
        public String Message { get; set; }
    }
}
