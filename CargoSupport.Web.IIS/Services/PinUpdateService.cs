using CargoSupport.Helpers;
using CargoSupport.Interfaces;
using CargoSupport.Models.PinModels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CargoSupport.Services
{
    public class PinUpdateService : IHostedService
    {
        private readonly PinHelper _ph;
        private Timer _timer;
        private readonly ILogger _logger;

        public PinUpdateService(ILoggerFactory logger, IMongoDbService dbService)
        {
            _logger = logger.CreateLogger("PinUpdateService");
            this._ph = new PinHelper(dbService);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // timer repeates call to UpdateTodaysOrders every 10 minutes.
            _timer = new Timer(
                UpdateTodaysOrders,
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

        private void UpdateTodaysOrders(object state)
        {
            try
            {
                _logger.LogInformation($"Start updating todays orders at {DateTime.Now.ToShortTimeString()}");
                var existingIds = _ph.GetAllOrderIdsAsStringForThisDay(DateTime.Now).Result;

                foreach (var existingId in existingIds)
                {
                    int.TryParse(existingId, out int result);

                    if (result != 0)
                    {
                        List<PinRouteModel> routes = _ph.RetrieveRoutesFromActualPin(result).Result;
                        _ph.UpdateExistingRecordsIfThereIsOne(routes).Wait();
                    }
                }
                _logger.LogInformation($"Finished updating todays orders at {DateTime.Now.ToShortTimeString()}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception when updating existing orders");
            }
        }
    }
}