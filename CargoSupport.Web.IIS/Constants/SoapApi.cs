﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Constants
{
    public class SoapApi
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