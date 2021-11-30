using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Services
{
    public static class BaseUrlService
    {
       public static string BaseUrlLink()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var _baseUrl = config.GetSection("ClientCallUrl").GetSection("BaseUrl").Value;
            return _baseUrl;
        }

        public static string BaseUrlLinkForActiveDirectory()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var _baseUrl = config.GetSection("ClientCallUrlForActiveDirectory").GetSection("BaseUrl").Value;
            return _baseUrl;
        }
    }
}
