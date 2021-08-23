using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
//using OnboardingService.Domain;
//using OnboardingService.Models;
//using OnboardingService.Services;
using DbFactory;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
 

namespace DbFactory
{
    public class SystemSettings
    {
        // MemoryCacheOptions _memoryCacheOptions;
        // public DatabaseConnectionDetatils(IOptions<MemoryCacheOptions> optionsAccessor)
        // {
        //     _memoryCacheOptions = optionsAccessor.Value;
        // }
        private IMemoryCache _cache;
        private CommonBaseUrl  _baseUrl;
        private ApiCommonUtilityService _apiCommon;
        public SystemSettings(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _apiCommon = new ApiCommonUtilityService();
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _baseUrl = config.GetSection("CommonEndpoint").Get<CommonBaseUrl>();
        }
        private int ValidateNumber(string text, int defaultValue)
        {
            if(int.TryParse(text, out int number))
            {
                return number;
            }
            else
            {
                return defaultValue;
            }
        }
        public async Task<DbConnectionParam> GetConnectionDetatilsAsync()
        {
            try
            {
                DbConnectionParam dbConnectionCache = new DbConnectionParam();

                // Look for cache key.
                if (!_cache.TryGetValue(CacheKeys.DbConnectionDetails, out dbConnectionCache))
                {
                     DbConnectionParam dbConnection = new DbConnectionParam();
                    // Key not in cache, so get data.
                    var settings = await _apiCommon.GetSettingGroupAsync("CHANNEL_AUTH_DB", _baseUrl.BaseUrl);
                    if(settings.Count > 0)
                    {
                        dbConnection.CommandTimeout = ValidateNumber(settings.Where(m=>m.settingId.Equals("CHANNEL_AUTH_CMD_TIME_OUT")).Select(m=>m.fieldValue).FirstOrDefault(), 60);
                        dbConnection.ConnectionTimeout = ValidateNumber(settings.Where(m=>m.settingId.Equals("CHANNEL_AUTH_CONN_TIME_OUT")).Select(m=>m.fieldValue).FirstOrDefault(), 60);
                        dbConnection.MaxPoolSize = ValidateNumber(settings.Where(m=>m.settingId.Equals("CHANNEL_AUTH_CONN_POOL")).Select(m=>m.fieldValue).FirstOrDefault(), 100);
                        dbConnection.Port = ValidateNumber(settings.Where(m=>m.settingId.Equals("CHANNEL_AUTH_PORT")).Select(m=>m.fieldValue).FirstOrDefault(), 1433);

                        dbConnection.ProviderName = settings.Where(m=>m.settingId.Equals("CHANNEL_AUTH_DATA_PROVIDER")).Select(m=>m.fieldValue).FirstOrDefault();
                        dbConnection.DatabaseType = settings.Where(m=>m.settingId.Equals("CHANNEL_AUTH_DATA_TYPE")).Select(m=>m.fieldValue).FirstOrDefault();
                        dbConnection.DatabaseName = settings.Where(m=>m.settingId.Equals("CHANNEL_AUTH_DATABASE")).Select(m=>m.fieldValue).FirstOrDefault();
                        dbConnection.Password = settings.Where(m=>m.settingId.Equals("CHANNEL_AUTH_PASSWORD")).Select(m=>m.fieldValue).FirstOrDefault();
                        dbConnection.Server = settings.Where(m=>m.settingId.Equals("CHANNEL_AUTH_SERVER")).Select(m=>m.fieldValue).FirstOrDefault();
                        dbConnection.UserId = settings.Where(m=>m.settingId.Equals("CHANNEL_AUTH_USERNAME")).Select(m=>m.fieldValue).FirstOrDefault();

                        dbConnectionCache = dbConnection;

                        // Set cache options.
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            // Keep in cache for this time, reset time if accessed.
                            .SetSlidingExpiration(TimeSpan.FromDays(1));

                        // Save data in cache.
                        _cache.Set(CacheKeys.DbConnectionDetails, dbConnectionCache, cacheEntryOptions);
                    }
                }
               
                return dbConnectionCache;
            }
           catch
           {
               throw;
           }
        }


        public    DbConnectionParam  GetConnectionDetatils()
        {
            try
            {
                DbConnectionParam dbConnectionCache = new DbConnectionParam();

                // Look for cache key.
                if (!_cache.TryGetValue(CacheKeys.DbConnectionDetails, out dbConnectionCache))
                {
                    DbConnectionParam dbConnection = new DbConnectionParam();
                    // Key not in cache, so get data.
                    var settings =   _apiCommon.GetSettingGroup("CHANNEL_AUTH_DB", _baseUrl.BaseUrl);
                    if (settings.Count > 0)
                    {
                        dbConnection.CommandTimeout = ValidateNumber(settings.Where(m => m.settingId.Equals("CHANNEL_AUTH_CMD_TIME_OUT")).Select(m => m.fieldValue).FirstOrDefault(), 60);
                        dbConnection.ConnectionTimeout = ValidateNumber(settings.Where(m => m.settingId.Equals("CHANNEL_AUTH_CONN_TIME_OUT")).Select(m => m.fieldValue).FirstOrDefault(), 60);
                        dbConnection.MaxPoolSize = ValidateNumber(settings.Where(m => m.settingId.Equals("CHANNEL_AUTH_CONN_POOL")).Select(m => m.fieldValue).FirstOrDefault(), 100);
                        dbConnection.Port = ValidateNumber(settings.Where(m => m.settingId.Equals("CHANNEL_AUTH_PORT")).Select(m => m.fieldValue).FirstOrDefault(), 1433);

                        dbConnection.ProviderName = settings.Where(m => m.settingId.Equals("CHANNEL_AUTH_DATA_PROVIDER")).Select(m => m.fieldValue).FirstOrDefault();
                        dbConnection.DatabaseType = settings.Where(m => m.settingId.Equals("CHANNEL_AUTH_DATA_TYPE")).Select(m => m.fieldValue).FirstOrDefault();
                        dbConnection.DatabaseName = settings.Where(m => m.settingId.Equals("CHANNEL_AUTH_DATABASE")).Select(m => m.fieldValue).FirstOrDefault();
                        dbConnection.Password = settings.Where(m => m.settingId.Equals("CHANNEL_AUTH_PASSWORD")).Select(m => m.fieldValue).FirstOrDefault();
                        dbConnection.Server = settings.Where(m => m.settingId.Equals("CHANNEL_AUTH_SERVER")).Select(m => m.fieldValue).FirstOrDefault();
                        dbConnection.UserId = settings.Where(m => m.settingId.Equals("CHANNEL_AUTH_USERNAME")).Select(m => m.fieldValue).FirstOrDefault();

                        dbConnectionCache = dbConnection;

                        // Set cache options.
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            // Keep in cache for this time, reset time if accessed.
                            .SetSlidingExpiration(TimeSpan.FromDays(1));

                        // Save data in cache.
                        _cache.Set(CacheKeys.DbConnectionDetails, dbConnectionCache, cacheEntryOptions);
                    }
                }

                return dbConnectionCache;
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> GetBaseUrlAsync(string settingId)
        {
            try
            {
                string baseUrl = string.Empty;

                // Look for cache key.
                if (!_cache.TryGetValue(settingId, out baseUrl))
                {
                     DbConnectionParam dbConnection = new DbConnectionParam();
                    // Key not in cache, so get data.
                     baseUrl = await _apiCommon.GetSettingValueAsync(settingId, _baseUrl.BaseUrl);
                    if(string.IsNullOrEmpty(baseUrl))
                    {
                        // Set cache options.
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            // Keep in cache for this time, reset time if accessed.
                            .SetSlidingExpiration(TimeSpan.FromDays(1));

                        
                        if(!string.IsNullOrEmpty(settingId)) 
                        {
                            // Save data in cache.
                            _cache.Set(settingId, baseUrl, cacheEntryOptions);
                        }
                    }
                }
               
                return baseUrl;
            }
           catch
           {
               throw;
           }
        }
    }
}