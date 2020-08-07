using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Constants
{
    public class MongoDb
    {
        public static string DatabaseName => "ICDB";
        public static string OutputScreenTableName => "ICOutputScreen";
        public static string QuinyxWorkerTableName => "ICWorkerLog";
    }
}