using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.Domain
{
    public class ObjectResponse
    {
        public bool Success { get; set; }
        public string[] Message { get; set; }
        public string[] Error { get; set; }
        public object Data { get; set; }
    }
}
