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
                StartDate = new DateTime(),
                EndDate = new DateTime().AddHours(4),
                TotalWeightThisWeek = 200
            },
              new QuinyxWorkerModel
            {
                FirstName = "Angel",
                LastName = "ff",
                StartDate = new DateTime(),
                EndDate = new DateTime().AddHours(4),
                TotalWeightThisWeek = 55
            },
              new QuinyxWorkerModel
            {
                FirstName = "Jonas",
                LastName = "Borg",
                StartDate = new DateTime(),
                EndDate = new DateTime().AddHours(4),
                TotalWeightThisWeek = 600
            },
            };

            dbHelper.InsertMultipleRecords(Constants.MongoDb.QuinyxWorkerTableName, _currentWorkers).Wait();
        }

        public List<QuinyxWorkerModel> GetAllWorkers()
        {
            return _currentWorkers;
        }

        public void ManuallyAddWorker(QuinyxWorkerModel workerModel)
        {
            _currentWorkers.Add(workerModel);
        }
    }
}