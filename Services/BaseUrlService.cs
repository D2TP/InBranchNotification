using InBranchNotification.Domain;
using InBranchNotification.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UAParser;

namespace InBranchNotification.Services
{
    public class BaseUrlService : IBaseUrlService
    {
        private readonly ILogger<BaseUrlService> _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly IBaseUrlService _baseUrlService;
        public BaseUrlService(IBaseUrlService baseUrlService, IHttpContextAccessor accessor, ILogger<BaseUrlService> logger)
        {
            _logger = logger;
            _accessor = accessor;
            _baseUrlService = baseUrlService;
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


        public async Task<ObjectResponse> AddAuditItem(Audit audit, StringValues userAgent)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var _commnserviceBaseUrl = config.GetSection("CommonEndpoint").GetSection("BaseUrl").Value;

            _logger.LogCritical("This is the common endpoint:" + _commnserviceBaseUrl);
            var fullUlr = _commnserviceBaseUrl + "api/Audit/AuditItem";
            //http://196.6.186.100:8085/common/api/Audit/AuditItem
            _logger.LogCritical("This is the full Ulr:" + fullUlr);
            var client = new RestClient("http://196.6.186.100:8085/common/api/Audit/AuditItem");

            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            //set browser details
            string uaString = Convert.ToString(userAgent[0]);
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(uaString);
            var browsr = c.UserAgent.Family; //IP Address from the request.
                                             // Note in localhost you will get ::1 but when it hosted you will get IP Address
            var ipaddrs = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            audit.client_browser = browsr;
            audit.ip_address = ipaddrs;
            audit.mac_address = "";
            var tojson = JsonConvert.SerializeObject(audit);
            request.AddParameter("application/json", tojson, ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);
            var objectResponse = response != null ? JsonConvert.DeserializeObject<ObjectResponse>(response.Content) : null;

            if (objectResponse == null)
            {

                throw new HandleGeneralException(503, HttpStatusCode.ServiceUnavailable.ToString() + " : Unabale to Connect to Xtradot Service [#InBranchAUTH001-2-A] AuthenticateRestClient [GetXtradotAdUserDetails]");
            }
            return objectResponse;
        }

    }
}
