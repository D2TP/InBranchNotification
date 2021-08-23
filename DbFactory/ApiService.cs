using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//using OnboardingService.Models;
//using Newtonsoft.Json;
using  DbFactory;

namespace DbFactory
{
    public class ApiService
    {
        public ApiService()
        {
        }

        public async Task<ApiInvokeModel> InvokeApiPostAsync(string requestPayload, string endpoint)
        {
            ApiInvokeModel apiResponse = new ApiInvokeModel();
            apiResponse.Status = false;
            apiResponse.Message = "Can not connect to API service at the moment, try later.";
            
            apiResponse.RequestPayload = endpoint;
            using (HttpClient client = new HttpClient())  
            {  
                //var jsonString = JsonConvert.SerializeObject());jsonString, Encoding.UTF8, "application/json"
                
                StringContent httpContent = new StringContent(requestPayload, Encoding.UTF8, "application/json");
                using (var Response = await client.PostAsync(endpoint,httpContent))  
                {  
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)  
                    {  
                        apiResponse.ResponseObject = await Response.Content.ReadAsStringAsync();
                        apiResponse.Status = true;
                        apiResponse.Message = "";
                        return apiResponse;  
                    }
                    else
                    {
                        apiResponse.Message = Response.StatusCode.ToString();
                    }  
                }
                return apiResponse; 
            }
        }
        public async Task<ApiInvokeModel> InvokeApiGetAsync(string endpoint)
        {
            ApiInvokeModel apiResponse = new ApiInvokeModel();
            apiResponse.Status = false;
            apiResponse.Message = "Can not connect to API service at the moment, try later.";
            
            apiResponse.RequestPayload = endpoint;
            using (HttpClient client = new HttpClient())  
            {  
                using (var Response = await client.GetAsync(endpoint))  
                {  
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)  
                    {  
                        apiResponse.ResponseObject = await Response.Content.ReadAsStringAsync();
                        apiResponse.Status = true;
                        apiResponse.Message = "";
                        return apiResponse;  
                    }  
                    else
                    {
                        apiResponse.Message = Response.StatusCode.ToString();
                    }  
                }
                return apiResponse;  
            }
        }


        public  ApiInvokeModel  InvokeApiGet (string endpoint)
        {
            ApiInvokeModel apiResponse = new ApiInvokeModel();
            apiResponse.Status = false;
            apiResponse.Message = "Can not connect to API service at the moment, try later.";

            apiResponse.RequestPayload = endpoint;
            using (HttpClient client = new HttpClient())
            {
                using (var Response =   client.GetAsync(endpoint).Result)
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        apiResponse.ResponseObject =   Response.Content.ReadAsStringAsync().Result;
                        apiResponse.Status = true;
                        apiResponse.Message = "";
                        return apiResponse;
                    }
                    else
                    {
                        apiResponse.Message = Response.StatusCode.ToString();
                    }
                }
                return apiResponse;
            }
        }
    }
}