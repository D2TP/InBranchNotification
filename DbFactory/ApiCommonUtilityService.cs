using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
 
using Microsoft.AspNetCore.Authentication;
using DbFactory;

namespace DbFactory
{
    public class ApiCommonUtilityService
    {
        private readonly ApiService _apiService;
        public ApiCommonUtilityService()
        {
            _apiService = new ApiService();
        }

        public async Task<List<SettingModel>> GetSettingGroupAsync(string settigGroup, string baseUrl)
        {
            try
            {
                List<SettingModel> settings = null;
                
                var apiResponse = await _apiService.InvokeApiGetAsync($@"{baseUrl}api/Settings/FieldValueGroup/{settigGroup}");
                if(apiResponse.Status)
                {
                    settings = new List<SettingModel>();

                    var jObject = JsonDocument.Parse(apiResponse.ResponseObject);
                    string settingValues = JsonDocument.Parse(jObject.RootElement.GetString("data")).RootElement.GetString("settingValues");
                    settings =  JsonSerializer.Deserialize<List<SettingModel>>(settingValues);
                }
                return settings;
            }
           catch
           {
               throw;
           }
        }

        public    List<SettingModel>  GetSettingGroup (string settigGroup, string baseUrl)
        {
            try
            {
                List<SettingModel> settings = null;

                var apiResponse =   _apiService.InvokeApiGet($@"{baseUrl}api/Settings/FieldValueGroup/{settigGroup}");
                if (apiResponse.Status)
                {
                    settings = new List<SettingModel>();

                    var jObject = JsonDocument.Parse(apiResponse.ResponseObject);
                    string settingValues = JsonDocument.Parse(jObject.RootElement.GetString("data")).RootElement.GetString("settingValues");
                    settings = JsonSerializer.Deserialize<List<SettingModel>>(settingValues);
                }
                return settings;
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> GetSettingValueAsync(string settingId, string baseUrl)
        {
            try
            {
                string settingValue = string.Empty;
                
                var apiResponse = await _apiService.InvokeApiGetAsync($@"{baseUrl}api/Settings/FieldValue/{settingId}");
                if(apiResponse.Status)
                {
                    var jObject = JsonDocument.Parse(apiResponse.ResponseObject);
                    settingValue = JsonDocument.Parse(jObject.RootElement.GetString("data")).RootElement.GetString("fieldValue");
                }
                return settingValue;
            }
           catch
           {
               throw;
           }
        }

    }
}