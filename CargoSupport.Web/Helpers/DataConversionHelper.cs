using CargoSupport.Enums;
using CargoSupport.Models;
using CargoSupport.Web.Models.DatabaseModels;
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

        public static QuinyxRole GetQuinyxEnum(string input)
        {
            if (input.Trim().Equals("OFO", StringComparison.CurrentCultureIgnoreCase))
            {
                return QuinyxRole.OFO;
            }
            if (input.Trim().Equals("Transportledare", StringComparison.CurrentCultureIgnoreCase))
            {
                return QuinyxRole.Transportledare;
            }
            if (input.Trim().Equals("AM Eftermiddag", StringComparison.CurrentCultureIgnoreCase))
            {
                return QuinyxRole.AM_Eftermiddag;
            }
            if (input.Trim().Equals("AM Förmiddag", StringComparison.CurrentCultureIgnoreCase))
            {
                return QuinyxRole.AM_Förmiddag;
            }

            return QuinyxRole.Driver;
        }
    }
}