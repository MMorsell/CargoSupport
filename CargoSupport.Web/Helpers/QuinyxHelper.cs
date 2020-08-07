using CargoSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Helpers
{
    public class QuinyxHelper
    {
        private List<QuinyxWorkerModel> _currentWorkers;
        private MongoDbHelper _dbHelper;

        public QuinyxHelper()
        {
            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
            var nowDateTime = DateTime.Now.TimeOfDay;
            _currentWorkers = new List<QuinyxWorkerModel> {
                new QuinyxWorkerModel
            {
                FirstName = "Jack",
                LastName = "Hes",
                StartShiftTime = nowDateTime,
                EndShiftTime = DateTime.Now.AddHours(2).TimeOfDay,
                TotalWeightThisWeek = 200
            },
              new QuinyxWorkerModel
            {
                FirstName = "Angel",
                LastName = "ff",
                StartShiftTime = nowDateTime,
                EndShiftTime = DateTime.Now.AddHours(3).TimeOfDay,
                TotalWeightThisWeek = 55
            },
              new QuinyxWorkerModel
            {
                FirstName = "Jonas",
                LastName = "Borg",
                StartShiftTime = DateTime.Now.AddHours(2).TimeOfDay,
                EndShiftTime = DateTime.Now.AddHours(4).TimeOfDay,
                TotalWeightThisWeek = 600
            },
              new QuinyxWorkerModel
            {
                FirstName = "Roger",
                LastName = "Hes",
                StartShiftTime = DateTime.Now.AddHours(1).TimeOfDay,
                EndShiftTime = DateTime.Now.AddHours(3).TimeOfDay,
                TotalWeightThisWeek = 200
            },
              new QuinyxWorkerModel
            {
                FirstName = "Marr",
                LastName = "ff",
                StartShiftTime = DateTime.Now.AddHours(7).TimeOfDay,
                EndShiftTime = DateTime.Now.AddHours(7).TimeOfDay,
                TotalWeightThisWeek = 55
            },
              new QuinyxWorkerModel
            {
                FirstName = "Wussa",
                LastName = "Borg",
                StartShiftTime = DateTime.Now.AddHours(1).TimeOfDay,
                EndShiftTime = DateTime.Now.AddHours(1).TimeOfDay,
                TotalWeightThisWeek = 600
            },
            };

            _dbHelper.InsertMultipleRecords(Constants.MongoDb.QuinyxWorkerTableName, _currentWorkers).Wait();
        }

        public async Task<List<QuinyxWorkerModel>> GetAllWorkersWorkingTodaySorted()
        {
            return await _dbHelper.GetAllDriversForTodaySorted(Constants.MongoDb.QuinyxWorkerTableName);
        }

        public async Task<List<QuinyxWorkerModel>> GetAllWorkers()
        {
            return await _dbHelper.GetAllRecords<QuinyxWorkerModel>(Constants.MongoDb.QuinyxWorkerTableName);
        }

        public async Task ManuallyAddWorker(QuinyxWorkerModel workerModel)
        {
            await _dbHelper.InsertRecord(Constants.MongoDb.QuinyxWorkerTableName, workerModel);
        }
    }
}