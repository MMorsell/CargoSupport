﻿using CargoSupport.Models.QuinyxModels;
using System;
using System.Collections.Generic;

namespace CargoSupport.Extensions
{
    public static class Basic
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