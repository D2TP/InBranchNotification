using InBranchDashboard.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace InBranchDashboard.Services
{
    public class BaseUrlService : IBaseUrlService
    {
        private readonly ILogger<BaseUrlService> _logger;
        public BaseUrlService(ILogger<BaseUrlService> logger)
        {
            _logger = logger;
        }

        public class Status
        {
            public bool success { get; set; }
            public object message { get; set; }
            public object error { get; set; }
            public object data { get; set; }
        }

        public class Data
        {
            public Status status { get; set; }
            public string settingId { get; set; }
            public string fieldValue { get; set; }
        }

        public class BaseUrlObject
        {
            public Data data { get; set; }
        }


        public async Task<string> BaseUrlLink()
        {

            var setingStringId = "API_BASE_URL_DOCUMENT";
            var getUrl = await GetEndPointUrl(setingStringId);
            return getUrl.data.fieldValue;


        }

        public async Task<string> BaseUrlLinkForActiveDirectory()
        {

            var setingStringId = "API_BASE_URL_AUTHENTICATION";
            var getUrl = await GetEndPointUrl(setingStringId);
            return getUrl.data.fieldValue;


        }

        private async Task<BaseUrlObject> GetEndPointUrl(string settingId)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var _commnserviceBaseUrl = config.GetSection("CommonEndpoint").GetSection("BaseUrl").Value;

            _logger.LogCritical("This is the common endpoint:" + _commnserviceBaseUrl);
            var fullUlr = _commnserviceBaseUrl + "api/Settings/FieldValue/" + settingId;

            _logger.LogCritical("This is the full Ulr:" + fullUlr);
            var client = new RestClient(fullUlr);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);
            var baseUrlObject = response != null ? JsonConvert.DeserializeObject<BaseUrlObject>(response.Content) : null;

            if (baseUrlObject == null)
            {

                throw new HandleGeneralException(503, HttpStatusCode.ServiceUnavailable.ToString() + " : Unabale to Connect to Xtradot Service [#InBranchAUTH001-2-A] AuthenticateRestClient [GetXtradotAdUserDetails]");
            }
            return baseUrlObject;
        }
    }
}
