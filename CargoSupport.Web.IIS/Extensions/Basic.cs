using CargoSupport.Models.QuinyxModels;
using System;
using System.Collections.Generic;

namespace CargoSupport.Extensions
{
    /// <summary>
    /// Application extensions
    /// </summary>
    public static class ApplicationExtensions
    {
        public static List<BasicQuinyxModel> ConvertQuinyxModelToBasic(this List<QuinyxModel> inputList)
        {
            var returnList = new List<BasicQuinyxModel>();

            for (int i = 0; i < inputList.Count; i++)
            {
                returnList.Add(new BasicQuinyxModel
                {
                    Id = inputList[i].Id,
                    Active = inputList[i].ExtendedInformationModel.Active,
                    begTime = inputList[i].begTime,
                    begTimeString = inputList[i].begTimeString,
                    endTime = inputList[i].endTime,
                    endTimeString = inputList[i].endTimeString,
                    GivenName = inputList[i].ExtendedInformationModel.GivenName,
                    FamilyName = inputList[i].ExtendedInformationModel.FamilyName
                });
            }
            return returnList;
        }

        /// <summary>
        /// Sets the <see cref="DateTime"/> object to a specific hour
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> object to update</param>
        /// <param name="hour">The hour to set</param>
        /// <returns>An updated <see cref="DateTime"/> with the correct hour</returns>
        public static DateTime SetHour(this DateTime dateTime, int hour)
        {
            return dateTime.SetPart(null, null, null, hour, null, null);
        }

        /// <summary>
        /// Sets the <see cref="DateTime"/> object to a specific minute
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> object to update</param>
        /// <param name="minute">The minute to set</param>
        /// <returns>An updated <see cref="DateTime"/> with the correct minute</returns>
        public static DateTime SetMinute(this DateTime dateTime, int minute)
        {
            return dateTime.SetPart(null, null, null, null, minute, null);
        }

        /// <summary>
        /// Transforms the <see cref="DateTime"/> object to a specific <see cref="DateTime"/>
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> object to update</param>
        /// <param name="year">The year to set</param>
        /// <param name="month">The month to set</param>
        /// <param name="day">The day to set</param>
        /// <param name="hour">The hour to set</param>
        /// <param name="minute">The minute to set</param>
        /// <param name="second">The second to set</param>
        /// <returns></returns>
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