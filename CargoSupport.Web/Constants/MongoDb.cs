﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Constants
{
    public class MongoDb
    {
        public static string ConnectionString => "mongodb://root:example@host.docker.internal:27017";
        public static string DatabaseName => "ICDB";
        public static string OutputScreenTableName => "ICOutputScreen";
        public static string BackupCollectionName => "ICOutputScreen_Backup";
        public static string QuinyxWorkerTableName => "ICWorkerLog";
    }
}