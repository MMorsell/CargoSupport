using System;
using System.IO;

namespace CargoSupport.Constants
{
    public static class SoapApi
    {
        public static string GetApiKey()
        {
            string fileName = "soapKey.txt";
            var key = string.Empty;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullName = System.IO.Path.Combine(desktopPath, fileName);
            using (StreamReader steamReader = new StreamReader(fullName))
            {
                key = steamReader.ReadToEnd();
            }

            return key;
        }

        public static string Connection => "https://api.quinyx.com/FlexForceWebServices.php";
    }
}