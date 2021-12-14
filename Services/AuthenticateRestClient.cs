using InBranchDashboard.Domain;
using InBranchDashboard.Exceptions;
using InBranchDashboard.Queries.ADLogin.queries;
using InBranchDashboard.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Services
{
    public class AuthenticateRestClient : IAuthenticateRestClient
    {
        private readonly IBaseUrlService _baseUrlService;
        private readonly ILogger<AuthenticateRestClient> _logger;
        public AuthenticateRestClient(ILogger<AuthenticateRestClient> logger, IBaseUrlService baseUrlService)
        {
            _baseUrlService = baseUrlService;
            _logger = logger;
        }

        public bool CheckXtradotAdUser(LoginWithAdQuery loginWithAdQuery)
        {
            var baseUrl = _baseUrlService.BaseUrlLinkForActiveDirectory();
            bool validateUser = false;
            var client = new RestClient(baseUrl + "api/Authentication/ad-validate");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = JsonConvert.SerializeObject(loginWithAdQuery);
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var xtradotResponse = JsonConvert.DeserializeObject<XtradotResponse>(response.Content);
            if (xtradotResponse == null)
            {
                _logger.LogError("[#AuthenticateRestClient-1-C] Connection to Xtradot api/Authentication/ad-validate failed Method: loginWithAdQuery");
                throw new HandleGeneralException(401, "[#AuthenticateRestClient-1-C] Connection to Xtradot api/Authentication/ad-validate failed Method: loginWithAdQuery");
            }
            if (xtradotResponse.data.isValid)
            {
                validateUser = true;
            }


            return validateUser;

        }

        public XtradotAdUserdetails GetXtradotAdUserDetails(string username, string domain)
        {
            var baseUrl = _baseUrlService.BaseUrlLinkForActiveDirectory();
            var client = new RestClient(baseUrl + "api/Authentication/ad/" + username + "/" + domain);

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var xtradotAdUserdetails = JsonConvert.DeserializeObject<XtradotAdUserdetails>(response.Content);
            return xtradotAdUserdetails;
        }

        public FBNDomain XtradotGetDomains()
        {
            var baseUrl = _baseUrlService.BaseUrlLinkForActiveDirectory();
            var client = new RestClient(baseUrl + "api/Authentication/lookup/domains");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var fBNDomain = JsonConvert.DeserializeObject<FBNDomain>(response.Content);
            return fBNDomain;
        }
    }
}
