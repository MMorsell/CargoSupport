using CargoSupport.Helpers;
using CargoSupport.Models.DatabaseModels;
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
    public class DBBackupService : IHostedService
    {
        private readonly MongoDbHelper _db;
        private Timer _timer;

        public DBBackupService()
        {
            this._db = new MongoDbHelper(Constants.MongoDb.DatabaseName);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            TimeSpan interval = TimeSpan.FromHours(24);
            //calculate time to run the first time & delay to set the timer
            //DateTime.Today gives time of midnight 00.00
            var nextRunTime = DateTime.Today.AddDays(1).AddHours(1);
            var curTime = DateTime.Now;
            var firstInterval = nextRunTime.Subtract(curTime);

            Action action = () =>
            {
                var t1 = Task.Delay(firstInterval);
                t1.Wait();
                //Backup database at expected time
                BackupDatabase(null);
                //now schedule it to be called every 24 hours for future
                // timer repeates call to BackupDatabase every 24 hours.
                _timer = new Timer(
                    BackupDatabase,
                    null,
                    TimeSpan.Zero,
                    interval
                );
            };

            // no need to await this call here because this task is scheduled to run much much later.
            Task.Run(action);
            return Task.CompletedTask;
        }

        // Call the Stop async method if required from within the app.
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void BackupDatabase(object state)
        {
            try
            {
                //TODO:Verify that this is working and running at 24:00
                _db.BackupData<DataModel>(Constants.MongoDb.OutputScreenTableName, Constants.MongoDb.BackupCollectionName).Wait();
            }
            catch (Exception ex)
            {
                //TODO: add logging for exception
            }
        }
    }
}