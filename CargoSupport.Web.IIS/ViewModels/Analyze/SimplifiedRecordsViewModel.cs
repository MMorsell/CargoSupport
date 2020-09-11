namespace CargoSupport.ViewModels.Analyze
{
    public class SimplifiedRecordsViewModel
    {
        public string RouteName { get; set; }
        public string DateOfRoute { get; set; }
        public double NumberOfCustomers { get; set; } = 0;
        public double Weight { get; set; } = 0;
        public double DistansInSwedishMiles { get; set; } = 0;
        public string CommentFromTransport { get; set; }
        public string[] CustomerComments { get; set; } = { "Inte någon kundkommentar" };
        public bool ResourceRoute { get; set; } = false;
    }
}