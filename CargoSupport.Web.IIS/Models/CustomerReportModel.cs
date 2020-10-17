using System;
using CargoSupport.Extensions;

namespace CargoSupport.Models
{
    public class CustomerReportModel
    {
        public DateTime DateOfRoute { get; set; }
        public string POnumber { get; set; } = "Tomt";

        //Jag är nöjd med min leverans idag.
        public int SatisfactionNumber { get; set; } = -1;

        //Leveransen kom i tid.
        public int TimingNumber { get; set; } = -1;

        //Bemötandet från chauffören var bra.
        public int DriverNumber { get; set; } = -1;

        //Varorna höll hög kvalitet.
        public int ProduceNumber { get; set; } = -1;

        public string Comment { get; set; } = "Tomt";

        public void SetSatisfactionNumber(string input)
        {
            int.TryParse(input, out int res);
            if (res != 0)
            {
                SatisfactionNumber = res;
            }
        }

        public void SetTimingNumber(string input)
        {
            int.TryParse(input, out int res);
            if (res != 0)
            {
                TimingNumber = res;
            }
        }

        public void SetDriverNumber(string input)
        {
            int.TryParse(input, out int res);

            if (res != 0)
            {
                DriverNumber = res;
            }
        }

        public void SetProduceNumber(string input)
        {
            int.TryParse(input, out int res);
            if (res != 0)
            {
                ProduceNumber = res;
            }
        }

        public void SetComment(string input)
        {
            if (input != null)
            {
                Comment = input;
            }
        }

        public void SetDateOfRoute(string input)
        {
            DateTime.TryParse(input, out DateTime res);
            DateOfRoute = res.SetHour(6);
        }
    }
}