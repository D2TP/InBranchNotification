using Convey.CQRS.Queries;
using InBranchDashboard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Category
{
    public class CategoryQueries: IQuery<List<CategoryDTO>>
    {
        public string id { get; set; }
        public string category_name { get; set; }
    }
}
