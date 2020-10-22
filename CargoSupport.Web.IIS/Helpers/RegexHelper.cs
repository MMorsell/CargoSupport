namespace CargoSupport.Helpers
{
    public static class RegexHelper
    {
        public static string GetPrefixOfRoute(string sampleOrder, int number)
        {
            string numberAsString;
            if (number < 10)
            {
                numberAsString = $"0{number.ToString()}";
            }
            else
            {
                numberAsString = number.ToString();
            }

            if (Constants.Regex.AfternoonRegex.IsMatch(sampleOrder))
            {
                return $"K{numberAsString}";
            }
            else if (Constants.Regex.MorningRegex.IsMatch(sampleOrder))
            {
                return $"M{numberAsString}";
            }
            else if (Constants.Regex.HamtasRegex.IsMatch(sampleOrder))
            {
                return $"Hämtas{numberAsString}";
            }
            else if (Constants.Regex.ReturRegex.IsMatch(sampleOrder))
            {
                return $"R{numberAsString}";
            }
            return null;
        }
    }
}