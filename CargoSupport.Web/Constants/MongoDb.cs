using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Constants
{
    public class MongoDb
    {
#if DEBUG
        public static string ConnectionString => "mongodb://root:example@host.docker.internal:27017";
        public static string Host => "https://localhost:27017";
#else
        public static string ConnectionString => "mongodb://root:example@192.168.1.81:27017";
        public static string Host => "https://192.168.1.81:27017";

#endif
        public static string DatabaseName => "ICDB";
        public static string OutputScreenTableName => "ICOutputScreen";
        public static string BackupCollectionName => "ICOutputScreen_Backup";
        public static string QuinyxWorkerTableName => "ICWorkerLog";
    }
}