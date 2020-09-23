namespace CargoSupport.ViewModels.Analyze
{
    public class FullViewModel
    {
        public SlimViewModel SlimViewModel { get; set; }
        /*
         * Static startingValues
         */
        public Xyz[] CustomerPositionData { get; set; }
        public Xy[] KiloData { get; set; }
        public Xy[] DistanceData { get; set; }
        public Xy[] CustomersData { get; set; }
    }

    public class Xyz
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Xy
    {
        public string x { get; set; }
        public double y { get; set; }
    }
}