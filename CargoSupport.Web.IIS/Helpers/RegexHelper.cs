namespace CargoSupport.Helpers
{
    /// <summary>
    /// Regex helper class
    /// </summary>
    public static class RegexHelper
    {
        /// <summary>
        /// Returns the prefix of the route in order to group routes by prefix
        /// </summary>
        /// <param name="sampleOrder">The current order to determine the prefix of</param>
        /// <param name="number">The number to add in the end of the prefix</param>
        /// <returns>A constructed string of prefix and <paramref name="number"/></returns>
        public static string GetPrefixOfRoute(string sampleOrder, int number)
        {
            string numberAsString;
            if (number < 10)
            {
                numberAsString = $"0{number}";
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