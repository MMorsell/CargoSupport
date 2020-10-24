using System;

namespace CargoSupport.Models
{
    public class CustomerServiceModel
    {
        public string Number { get; set; } = "";
        public string OrderNumber { get; set; } = "";
        public DateTime Opened { get; set; } = DateTime.MinValue;
        public DateTime Closed { get; set; } = DateTime.MaxValue;
        public string CategoryLevelComment { get; set; } = "";

        public void SetNumber(string input)
        {
            if (input != null)
            {
                Number = input;
            }
        }

        public void SetOrderNumber(string input)
        {
            if (input != null)
            {
                OrderNumber = input;
            }
        }

        public void SetOpened(string input)
        {
            if (input != null)
            {
                DateTime.TryParse(input, out DateTime res);
                Opened = res;
            }
        }

        public void SetClosed(string input)
        {
            if (input != null)
            {
                DateTime.TryParse(input, out DateTime res);
                Closed = res;
            }
        }

        public void SetCategoryLevelComment(string input)
        {
            if (input != null)
            {
                CategoryLevelComment = input;
            }
        }
    }
}