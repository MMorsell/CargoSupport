using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Web.Extensions
{
    public static class Basic
    {
        public static DateTime SetHour(this DateTime dateTime, int hour)
        {
            return dateTime.SetPart(null, null, null, hour, null, null);
        }

        public static DateTime SetMinute(this DateTime dateTime, int minute)
        {
            return dateTime.SetPart(null, null, null, null, minute, null);
        }

        public static DateTime SetPart(this DateTime dateTime, int? year, int? month, int? day, int? hour, int? minute, int? second)
        {
            return new DateTime(
                year ?? dateTime.Year,
                month ?? dateTime.Month,
                day ?? dateTime.Day,
                hour ?? dateTime.Hour,
                minute ?? dateTime.Minute,
                second ?? dateTime.Second
            );
        }
    }
}