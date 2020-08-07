using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Constants
{
    public class MongoDb
    {
        public string DatabaseName => "ICDB";
        public string OutputScreenTableName => "ICOutputScreen";
        public string QuinyxWorkerTableName => "ICWorkerLog";
    }
}