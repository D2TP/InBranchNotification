using InBranchDashboard.Exceptions;
using Microsoft.Extensions.Configuration;
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
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var setingStringId = config.GetSection("ClientCallUrl").GetSection("SetingStringId").Value;
            var getUrl = await GetEndPointUrl(setingStringId);
            return getUrl.data.fieldValue;


        }

        public async Task<string> BaseUrlLinkForActiveDirectory()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var setingStringId = config.GetSection("ClientCallUrlForActiveDirectory").GetSection("SetingStringId").Value;
            var getUrl = await GetEndPointUrl(setingStringId);
            return getUrl.data.fieldValue;


        }

        private async Task<BaseUrlObject> GetEndPointUrl(string settingId)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var _commnserviceBaseUrl = config.GetSection("CommonService").GetSection("BaseUrl").Value;


            var client = new RestClient(_commnserviceBaseUrl + "common/api/Settings/FieldValue/" + settingId);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);
            var baseUrlObject = response != null ? JsonConvert.DeserializeObject<BaseUrlObject>(response.Content) : null;

            if (baseUrlObject == null)
            {

                throw new HandleGeneralException(400, HttpStatusCode.BadRequest.ToString() + " : Unabale to Connect to Xtradot Service [#InBranchAUTH001-2-A] AuthenticateRestClient [GetXtradotAdUserDetails]");
            }
            return baseUrlObject;
        }
    }
}
