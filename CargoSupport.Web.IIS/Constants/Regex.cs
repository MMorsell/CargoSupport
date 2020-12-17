namespace CargoSupport.Constants
{
    /// <summary>
    /// Application wide used regex
    /// </summary>
    public static class Regex
    {
        /// <summary>
        /// Checks if route is a morning route
        /// </summary>
        public static System.Text.RegularExpressions.Regex AfternoonRegex => new System.Text.RegularExpressions.Regex("^[Kk]\\d\\d\\s");

        /// <summary>
        /// Checks if route is a morning route
        /// </summary>
        public static System.Text.RegularExpressions.Regex MorningRegex => new System.Text.RegularExpressions.Regex("^[Mm]\\d\\d\\s");

        /// <summary>
        /// Checks if route is a "Hämtas" route
        /// </summary>
        public static System.Text.RegularExpressions.Regex HamtasRegex => new System.Text.RegularExpressions.Regex("^[Hh][Ää][Mm][Tt][Aa][Ss]");

        /// <summary>
        /// Checks if route is a "Retur" route
        /// </summary>
        public static System.Text.RegularExpressions.Regex ReturRegex => new System.Text.RegularExpressions.Regex("^[Rr]\\d\\d\\s");
    }
}