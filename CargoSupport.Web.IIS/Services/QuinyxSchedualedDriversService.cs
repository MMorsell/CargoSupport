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
    public class QuinyxSchedualedDriversService : IHostedService
    {
        private readonly QuinyxHelper _qh;
        private readonly IAppCache _cache;
        private Timer _timer;

        public QuinyxSchedualedDriversService(
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
                UpdateRetrieveSchedualDriversFromQuinyxCache,
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(10)
            );

            return Task.CompletedTask;
        }

        // Call the Stop async method if required from within the app.
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void UpdateRetrieveSchedualDriversFromQuinyxCache(object state)
        {
            Log.Logger.Debug($"Start updating schedualdrivers cache {DateTime.Now.ToShortTimeString()}");
            string tomorrow = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            UpdateSpecificDate(today);
            UpdateSpecificDate(tomorrow);
            Log.Logger.Debug($"Finished updating schedualdrivers cache at {DateTime.Now.ToShortTimeString()}");
        }

        private void UpdateSpecificDate(string targetDate)
        {
            try
            {
                var result = _qh.RetrieveSchedualDriversFromQuinyx(targetDate, targetDate).Result;

                if (result == null)
                {
                    Log.Logger.Error($"Unsuccesful retrieval of RetrieveAllDriversFromQuinyx when updating schedualdrivers cache for date '{targetDate}',");

                    //CacheHelper.ReaddCache<XDocument>(_cache, $"{Constants.Cache.SchedualedDrivers}-{targetDate}", new TimeSpan(3, 0, 0));
                }
                else
                {
                    CacheHelper.UpdateCache(_cache, $"{Constants.Cache.SchedualedDrivers}-{targetDate}-{targetDate}", new TimeSpan(168, 0, 0), result);
                }
                Log.Logger.Debug($"Finished updating schedualdrivers cache for date '{targetDate}' at {DateTime.Now.ToShortTimeString()}");
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"Exception when updating schedualdrivers cache, will prolong lifetime of cache for date '{targetDate}'");

                //CacheHelper.ReaddCache<XDocument>(_cache, $"{Constants.Cache.SchedualedDrivers}-{targetDate}", new TimeSpan(3, 0, 0));
            }
        }
    }
}