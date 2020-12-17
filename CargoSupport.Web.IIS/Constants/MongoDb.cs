namespace CargoSupport.Constants
{
    /// <summary>
    /// Application constants for database collections
    /// </summary>
    public static class MongoDb
    {
        /// <summary>
        /// Database collection key that holds application main output
        /// </summary>
        public const string OutputScreenCollectionName = "ICOutputScreen";

        /// <summary>
        /// Database collection key that holds the car informations
        /// </summary>
        public const string CarCollectionName = "ICCars";

        /// <summary>
        /// Database collection key that holds old backed up data
        /// </summary>
        public const string BackupCollectionName = "ICOutputScreen_Backup";
    }
}