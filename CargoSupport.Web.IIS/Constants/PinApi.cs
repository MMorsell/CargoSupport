using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Constants
{
    public static class PinApi
    {
        public static string ServerKeyFile => "pinServer.txt";
        public static string ClientKeyFile => "pinClient.txt";

        public static string GetOrder(int id)
        {
            return $"https://ica.pindeliver.com/api/v1_2/Order/get/{id}";
        }

        public static string GetRoute(int id)
        {
            return $"https://ica.pindeliver.com/api/v1_2/Route/get/{id}";
        }

        public static string GetApiKey(string keyName)
        {
            var key = string.Empty;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullName = System.IO.Path.Combine(desktopPath, keyName);
            using (StreamReader steamReader = new StreamReader(fullName))
            {
                key = steamReader.ReadToEnd();
            }

            return key;
        }
    }
}