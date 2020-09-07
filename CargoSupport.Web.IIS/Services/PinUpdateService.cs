using CargoSupport.Helpers;
using CargoSupport.Models.PinModels;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace CargoSupport.Services
{
    public class PinUpdateService : IHostedService
    {
        private readonly PinHelper _ph;
        private Timer _timer;

        public PinUpdateService()
        {
            this._ph = new PinHelper();
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
            }
            catch (Exception ex)
            {
                //TODO: add logging for exception
            }
        }
    }
}