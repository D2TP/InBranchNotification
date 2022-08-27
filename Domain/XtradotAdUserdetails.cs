using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.Domain
{
    

        public class XtradotAdUser
    {
        public string userName { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public List<string> roles { get; set; }
    }

    public class XtradotAdUserdetails

    {
        public XtradotAdUser data { get; set; }
        public string code { get; set; }
        public string description { get; set; }
    }


}
