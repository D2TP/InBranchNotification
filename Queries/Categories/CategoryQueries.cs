using Convey.CQRS.Queries;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchDashboard.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Queries.Categories
{
    public class CategoryQueries: IQuery<PagedList<CategoryDTO>>
    {
        public QueryStringParameters _queryStringParameters;

        public CategoryQueries()
        {
        }

        public CategoryQueries(QueryStringParameters queryStringParameters)
        {
            _queryStringParameters = queryStringParameters;
        }
        public string id { get; set; }
        public string category_name { get; set; }
    }
}
