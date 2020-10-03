using CargoSupport.Models.DatabaseModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CargoSupport.Services
{
    public class DBBackupService : IHostedService
    {
        private readonly MongoDbService _db;

        private Timer _timer;

        public DBBackupService(IConfiguration configuration)
        {
            this._db = new MongoDbService(configuration["mongoDatabaseName"], configuration);
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
                _db.BackupData<DataModel>(Constants.MongoDb.OutputScreenCollectionName, Constants.MongoDb.BackupCollectionName).Wait();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"Exception when backing up database");
            }
        }
    }
}