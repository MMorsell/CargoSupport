using CargoSupport.Helpers;
using CargoSupport.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestingConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var _dbHelper = new MongoDbHelper(CargoSupport.Constants.MongoDb.DatabaseName);

            //DateTime.TryParse("2020-08-29", out DateTime from);

            //List<DataModel> analyzeModels = _dbHelper.GetAllRecordsByDate(CargoSupport.Constants.MongoDb.OutputScreenTableName, from).Result;

            //CargoSupport.Helpers.DataConversionHelper.ConvertTodaysDataToGraphModels(analyzeModels);
            //var qh = new CargoSupport.Helpers.QuinyxHelper();

            //var res = qh.GetAllDriversFromADate(DateTime.Now).ToList();

            //foreach (var r in res)
            //{
            //    Console.WriteLine($"{r.GivenName} - {r.begTimeString}");
            //}

            //var ph = new PinHelper();
            //var _qnHelper = new QuinyxHelper();

            //var sw = new Stopwatch();
            //sw.Start();
            ////List<QuinyxModel> allDriversForToday = _qnHelper.GetAllDriversSorted(DateTime.Now.AddDays(-2), false);

            //List<PinRouteModel> routes = ph.RetrieveRoutesFromActualPin(15668).Result;
            //ph.PopulateRoutesWithDriversAndSaveResultToDatabase(routes).Wait();
            ////allDriversForToday = _qnHelper.GetExtraInformationForDrivers(allDriversForToday);
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed);

            //var drivers = _qnHelper.GetExtraInformationForDrivers(_qnHelper.GetDrivers(DateTime.Now.AddDays(-1), DateTime.Now));

            //foreach (var driver in drivers)
            //{
            //    Console.WriteLine($"{driver.ExtendedInformationModel.GivenName} {driver.ExtendedInformationModel.FamilyName} - {driver.ExtendedInformationModel.StaffCat} - {driver.ExtendedInformationModel.StaffCatName} - {driver.ExtendedInformationModel.Section} - {driver.ExtendedInformationModel.CostCentre} - {driver.ExtendedInformationModel.ReportingTo}");
            //}

            ////_dbhelper.BackupData<PinRouteModel>(CargoSupport.Constants.MongoDb.OutputScreenTableName, CargoSupport.Constants.MongoDb.BackupCollectionName).Wait();
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