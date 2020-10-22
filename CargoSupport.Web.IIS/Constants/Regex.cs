using System.Text.RegularExpressions;

namespace CargoSupport.Constants
{
    public static class Regex
    {
        public static System.Text.RegularExpressions.Regex AfternoonRegex => new System.Text.RegularExpressions.Regex("^[Kk]\\d\\d\\s");
        public static System.Text.RegularExpressions.Regex MorningRegex => new System.Text.RegularExpressions.Regex("^[Mm]\\d\\d\\s");
        public static System.Text.RegularExpressions.Regex HamtasRegex => new System.Text.RegularExpressions.Regex("^[Hh][Ää][Mm][Tt][Aa][Ss]");
        public static System.Text.RegularExpressions.Regex ReturRegex => new System.Text.RegularExpressions.Regex("^[Rr]\\d\\d\\s");
    }
}