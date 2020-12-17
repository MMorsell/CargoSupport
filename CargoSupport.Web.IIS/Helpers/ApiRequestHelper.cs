using CargoSupport.Models.PinModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CargoSupport.Helpers
{
    /// <summary>
    /// Api helper class to call PinDeliver API
    /// </summary>
    public class ApiRequestHelper
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Default constructor that should be used
        /// </summary>
        /// <param name="Configuration"><see cref="IConfiguration"/> object from program startup</param>
        /// <param name="env"><see cref="IWebHostEnvironment"/> object from program startup</param>
        public ApiRequestHelper(IConfiguration Configuration, IWebHostEnvironment env)
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
            if (env.IsDevelopment())
            {
                _client = new HttpClient();
            }
            else
            {
                _client = new HttpClient(handler: httpClientHandler);
            }
            _client.DefaultRequestHeaders.Add("X-PINDELIVER-API-KEY", Configuration.GetValue<string>("pinServer"));
            _client.DefaultRequestHeaders.Add("X-PINDELIVER-API-CLIENT-KEY", Configuration.GetValue<string>("pinClient"));
        }

        /// <summary>
        /// Returns a single API call of type <see cref="T"/>
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="url">The API url to call</param>
        /// <returns>Promise to a Task{<T>}</returns>
        public async Task<T> GetSingleResult<T>(string url)
        {
            var response = await _client.GetAsync(
                url)
                .ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());

            return result;
        }

        /// <summary>
        /// Returns a list of results of type <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="url">The API url to call</param>
        /// <returns>Promise to a Task{List{T}}</returns>
        public async Task<List<T>> GetMultipleResult<T>(string url)
        {
            var response = await _client.GetAsync(
                url)
                .ConfigureAwait(false);
            var results = JsonConvert.DeserializeObject<List<T>>(await response.Content.ReadAsStringAsync());

            return results;
        }

        /// <summary>
        /// This method batches <see cref="GetSingleResult"/> API calls to retrieve the resposes faster
        /// Batching calls is faster in this case since the throughput of the external API cant handle more than 100 calls at a given time
        /// </summary>
        /// <param name="routeIds">ID's of Routes to ask for in the API Calls</param>
        /// <returns>Promise to a Task{List{PinRouteModel}}</returns>
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