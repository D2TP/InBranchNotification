using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Commands.Category
{
    public class CategoryCommand: ICommand
    {
     
        public string id { get; set; }
        public string category_name { get; set; }
    }
}
