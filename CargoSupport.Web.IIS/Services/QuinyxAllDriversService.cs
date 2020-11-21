using CargoSupport.Helpers;
using LazyCache;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CargoSupport.Services
{
    public class QuinyxAllDriversService : IHostedService
    {
        private readonly QuinyxHelper _qh;
        private readonly IAppCache _cache;
        private Timer _timer;

        public QuinyxAllDriversService(
            IConfiguration configuration,
            IWebHostEnvironment env,
            IAppCache cache)
        {
            this._qh = new QuinyxHelper(configuration, env, cache);
            this._cache = cache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // timer repeates call to UpdateTodaysOrders every 10 minutes.
            _timer = new Timer(
                UpdateRetrieveAllDriversFromQuinyxCache,
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(60)
            );

            return Task.CompletedTask;
        }

        // Call the Stop async method if required from within the app.
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void UpdateRetrieveAllDriversFromQuinyxCache(object state)
        {
            try
            {
                Log.Logger.Debug($"Start updating all drivers cache {DateTime.Now.ToShortTimeString()}");
                var result = _qh.RetrieveAllDriversFromQuinyx().Result;

                if (result == null)
                {
                    Log.Logger.Error($"Unsuccesful retrieval of RetrieveAllDriversFromQuinyx when updating all drivers cache");

                    //CacheHelper.ReaddCache<XDocument>(_cache, Constants.Cache.AllDrivers, new TimeSpan(3, 0, 0));
                }
                else
                {
                    CacheHelper.UpdateCache(_cache, Constants.Cache.AllDrivers, new TimeSpan(168, 0, 0), result);
                }
                Log.Logger.Debug($"Finished updating all drivers cache at {DateTime.Now.ToShortTimeString()}");
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"Exception when updating all drivers cache");
                //CacheHelper.ReaddCache<XDocument>(_cache, Constants.Cache.AllDrivers, new TimeSpan(3, 0, 0));
            }
        }
    }
}