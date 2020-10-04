namespace CargoSupport.ViewModels.Analyze
{
    public class AllBossesViewModel
    {
        public string LabelTitle { get; set; }
        public int StaffCatId { get; set; }
        public int SectionId { get; set; }
        public int DriverId { get; set; } = 0;
        public double NumberOfValidDeliveries { get; set; }
        public double NumberOfValidDeliveriesLeft { get; set; }
        public double CustomersWithinTimeSlot { get; set; }
        public double CustomersWithinPrognosis { get; set; }
        public double CustomersBeforeTimeSlot { get; set; }
        public double CustomersBeforeEstimatedTime { get; set; }
        public double percentageWithin5MinOfTimeSlot { get; set; }
        public double percentageWithin15MinOfCustomerEstimatedTime { get; set; }
        public double CustomersDividedByWorkHours { get; set; }
    }
}