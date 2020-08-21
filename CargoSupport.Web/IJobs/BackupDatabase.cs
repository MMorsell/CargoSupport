using CargoSupport.Helpers;
using CargoSupport.Web.Models.DatabaseModels;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Web.IJobs
{
    public class BackupDatabase : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await new MongoDbHelper(CargoSupport.Constants.MongoDb.DatabaseName).BackupData<DataModel>(CargoSupport.Constants.MongoDb.OutputScreenTableName, CargoSupport.Constants.MongoDb.BackupCollectionName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}