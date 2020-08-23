using CargoSupport.Enums;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Helpers
{
    public static class DataConversionHelper
    {
        public static List<AnalyzeModel> ConvertPinRouteModelToAnalyzeModel(List<DataModel> dataModel)
        {
            var analyzeModels = new List<AnalyzeModel>();

            foreach (var pinRouteModel in dataModel)
            {
                if (pinRouteModel.Driver != null)
                {
                    var existingRecordToGroupWith = analyzeModels.FirstOrDefault(prm => prm.Driver.BadgeNo.Equals(pinRouteModel.Driver.BadgeNo));

                    if (existingRecordToGroupWith == null)
                    {
                        analyzeModels.Add(new AnalyzeModel(pinRouteModel));
                    }
                    else
                    {
                        existingRecordToGroupWith.AddPinRouteModelIfItHasNotBeenAdded(pinRouteModel);
                    }
                }
            }

            foreach (var analyzeModel in analyzeModels)
            {
                analyzeModel.PupulatePublicProperties();
            }
            return analyzeModels;
        }

        public static QuinyxRole GetQuinyxEnum(int categoryId)
        {
            /*
             * 226245 .Eftermiddag
             * 226233 - .Förmiddag
             */

            if (categoryId.Equals(226245) ||
                categoryId.Equals(226233))
            {
                return QuinyxRole.Driver;
            }
            else
            {
                return QuinyxRole.Other;
            }
        }
    }
}