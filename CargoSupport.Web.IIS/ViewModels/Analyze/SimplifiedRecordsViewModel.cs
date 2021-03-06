﻿using CargoSupport.Models;

namespace CargoSupport.ViewModels.Analyze
{
    public class SimplifiedRecordsViewModel
    {
        public string RouteName { get; set; }
        public string DateOfRoute { get; set; }
        public string StartTimeDiff { get; set; }
        public double NumberOfCustomers { get; set; } = 0;
        public double Weight { get; set; } = 0;
        public double DistansInSwedishMiles { get; set; } = 0;
        public string CommentFromTransport { get; set; }
        public CustomerReportModel[] CustomerComments { get; set; } = new CustomerReportModel[] { new CustomerReportModel { Comment = "Tomt" } };
        public CustomerServiceModel[] ServiceComments { get; set; } = new CustomerServiceModel[] { new CustomerServiceModel { CategoryLevelComment = "Tomt" } };
        public bool ResourceRoute { get; set; } = false;
    }
}