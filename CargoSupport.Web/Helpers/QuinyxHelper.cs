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
        private MongoDbHelper dbHelper;

        public QuinyxHelper()
        {
            dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);

            _currentWorkers = new List<QuinyxWorkerModel> {
                new QuinyxWorkerModel
            {
                FirstName = "Jack",
                LastName = "Hes",
                StartShiftTime = DateTime.Now.TimeOfDay,
                EndShiftTime = DateTime.Now.AddHours(4).TimeOfDay,
                TotalWeightThisWeek = 200
            },
              new QuinyxWorkerModel
            {
                FirstName = "Angel",
                LastName = "ff",
                StartShiftTime = DateTime.Now.TimeOfDay,
                EndShiftTime = DateTime.Now.AddHours(4).TimeOfDay,
                TotalWeightThisWeek = 55
            },
              new QuinyxWorkerModel
            {
                FirstName = "Jonas",
                LastName = "Borg",
                StartShiftTime = DateTime.Now.TimeOfDay,
                EndShiftTime = DateTime.Now.AddHours(4).TimeOfDay,
                TotalWeightThisWeek = 600
            },
            };

            dbHelper.InsertMultipleRecords(Constants.MongoDb.QuinyxWorkerTableName, _currentWorkers).Wait();
        }

        public async Task<List<QuinyxWorkerModel>> GetAllWorkersWorkingToday()
        {
            return await dbHelper.GetAllRecords<QuinyxWorkerModel>(Constants.MongoDb.QuinyxWorkerTableName);
        }

        public async Task<List<QuinyxWorkerModel>> GetAllWorkers()
        {
            return await dbHelper.GetAllRecords<QuinyxWorkerModel>(Constants.MongoDb.QuinyxWorkerTableName);
        }

        public async Task ManuallyAddWorker(QuinyxWorkerModel workerModel)
        {
            await dbHelper.InsertRecord(Constants.MongoDb.QuinyxWorkerTableName, workerModel);
        }
    }
}