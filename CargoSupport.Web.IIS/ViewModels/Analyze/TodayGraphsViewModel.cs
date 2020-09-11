﻿namespace CargoSupport.ViewModels.Analyze
{
    public class TodayGraphsViewModel
    {
        public string LabelTitle { get; set; }
        public double NumberOfValidDeliveries { get; set; }
        public double NumberOfValidDeliveriesLeft { get; set; }
        public double CustomersWithinTimeSlot { get; set; }
        public double CustomersWithinPrognosis { get; set; }
        public double CustomersBeforeTimeSlot { get; set; }
        public double CustomersBeforeEstimatedTime { get; set; }
        public double PercentageWithing5MinOfTimeSlot { get; set; }
        public double PercentageWithing15MinOfCustomerEstimatedTime { get; set; }
        public double CustomersDividedByWorkHours { get; set; }
    }
}