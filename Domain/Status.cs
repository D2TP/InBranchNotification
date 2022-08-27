using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.Domain
{
    public enum Status
    {
        Unknown,
        Valid,
        Incomplete,
        Suspicious,
        Close,
        Dormant
    }
}
