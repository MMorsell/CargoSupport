namespace CargoSupport.Constants
{
    public static class MongoDb
    {
        public static string ConnectionString => "mongodb://localhost:27017";
        public static string DatabaseName => "ICDB";
        public static string OutputScreenTableName => "ICOutputScreen";
        public static string CarTableName => "ICCars";
        public static string BackupCollectionName => "ICOutputScreen_Backup";
        public static string QuinyxWorkerTableName => "ICWorkerLog";
        public static string WhitelistTable => "ICPolicy";
    }
}