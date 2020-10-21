﻿using CargoSupport.Models.PinModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CargoSupport.Helpers
{
    public class ApiRequestHelper
    {
        private readonly HttpClient client;

        public ApiRequestHelper(IConfiguration Configuration)
        {
            var proxy = new WebProxy
            {
                Address = new Uri($"http://proxy02.ica.se:8080"),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,

                Credentials = System.Net.CredentialCache.DefaultCredentials
        };

            var httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
            };

            client = new HttpClient(handler: httpClientHandler);
            client.DefaultRequestHeaders.Add("X-PINDELIVER-API-KEY", Configuration.GetValue<string>("pinServer"));
            client.DefaultRequestHeaders.Add("X-PINDELIVER-API-CLIENT-KEY", Configuration.GetValue<string>("pinClient"));
        }

        public async Task<T> GetSingleResult<T>(string url)
        {
            var response = await client.GetAsync(
                url)
                .ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());

            return result;
        }

        public async Task<List<T>> GetMultipleResult<T>(string url)
        {
            var response = await client.GetAsync(
                url)
                .ConfigureAwait(false);
            var results = JsonConvert.DeserializeObject<List<T>>(await response.Content.ReadAsStringAsync());

            return results;
        }

        public async Task<List<PinRouteModel>> GetRoutesBatchParalellAsync(IEnumerable<int> routeIds)
        {
            var routes = new List<PinRouteModel>();
            var batchSize = 100;
            int numberOfBatches = (int)Math.Ceiling((double)routeIds.Count() / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var currentIds = routeIds.Skip(i * batchSize).Take(batchSize);
                var tasks = currentIds.Select(id => GetSingleResult<PinRouteModel>(Constants.PinApi.GetRoute(id)));
                routes.AddRange(await Task.WhenAll(tasks));
            }

            return routes;
        }
    }
}