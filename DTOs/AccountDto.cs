using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.DTOs
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public String AccountNo { get; set; }
        public decimal AccountBalance { get; internal set; }
    }
}
