using CargoSupport.Helpers;
using CargoSupport.Constants;
using System;
using CargoSupport.Models;
using System.Collections.Generic;

namespace TestingConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var _dbhelper = new MongoDbHelper(CargoSupport.Constants.MongoDb.DatabaseName);

            for (int i = 0; i < 30; i++)
            {
                var random = new Random();
                var val1 = random.Next(1, 30);
                var random2 = new Random();
                var val2 = random2.Next(1, 100);
                Console.WriteLine(@$"{{ x: ""2020-08-{val1}"", y: {val2} }},");
            }

            ////_dbhelper.BackupData<PinRouteModel>(CargoSupport.Constants.MongoDb.OutputScreenTableName, CargoSupport.Constants.MongoDb.BackupCollectionName).Wait();
            //var ph = new PinHelper();
            //var _qnHelper = new QuinyxHelper();
            //ph.RetrieveRoutesForToday().Wait();
            //List<PinRouteModel> todaysRoutes = _dbhelper.GetAllRecords<PinRouteModel>(CargoSupport.Constants.MongoDb.OutputScreenTableName).Result;

            //var h = new PinHelper();
            //h.RetrieveRoutesForToday();

            //var res = _qnHelper.GetAllWorkersWorkingTodaySorted().Result;

            //foreach (var re in res)
            //{
            //    Console.WriteLine($"{re.FirstName} {re.LastName} - {re.StartShiftTime.ToString()}/{re.EndShiftTime.ToString()}");
            //}
            //List<PinRouteModel> todaysRoutes = _dbhelper.GetAllRecords<PinRouteModel>(CargoSupport.Constants.MongoDb.OutputScreenTableName).Result;

            ////var allRecords = dbhelper.GetAllRecords<PinRouteModel>(CargoSupport.Constants.MongoDb.OutputScreenTableName).Result;
            //var r = _dbhelper.GetRecordById<PinRouteModel>(CargoSupport.Constants.MongoDb.OutputScreenTableName, new Guid("e46b2b7f-0d96-4716-9a73-f58242660561")).Result;
            //Console.WriteLine(r.RouteName);
            //Console.WriteLine("Hello World!");
        }
    }
}