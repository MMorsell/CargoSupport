using CargoSupport.Enums;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.ViewModels.Analyze;
using System.Collections.Concurrent;

namespace CargoSupport.Helpers
{
    public static class DataConversionHelper
    {
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

        public static List<SlimViewModel> ConvertDataModelsToSlimViewModels(List<DataModel> dataModels)
        {
            var returnList = new ConcurrentBag<SlimViewModel>();
            var qh = new QuinyxHelper();
            var dataModelsWithNames = qh.AddNamesToData(dataModels);

            var groupedModels = dataModelsWithNames.GroupBy(data => data.Driver.Id);

            Parallel.ForEach(groupedModels, group =>
            {
                try
                {
                    var groupToList = group.ToList();
                    returnList.Add(new SlimViewModel()
                    {
                        AvrCustomers = Math.Round(group.Sum(data => data.PinRouteModel.NumberOfCustomers) / group.Count(), 0),
                        AvrDrivingDistance = Math.Round(
                                (
                                (group.Sum(data => data.PinRouteModel.DistanceInMeters)
                                / group.Count())
                                / 10000 /*To get result in swedish miles*/
                                )
                                , 0),
                        AvrWeight = Math.Round(group.Sum(data => data.PinRouteModel.Weight) / group.Count(), 0),
                        DriverFullName = groupToList[0].Driver.GetDriverName(),
                        QuinyxId = groupToList[0].Driver.Id
                    });
                }
                catch (Exception)
                {
                    //TODO: Handle exeption here
                }
            });

            return returnList.ToList();
        }

        public static FullViewModel ConvertDataModelsToFullViewModel(List<DataModel> dataModels)
        {
            var returnModel = new FullViewModel();
            Task<SlimViewModel> convertSlimTask = GetSlimInformation(dataModels);
            Task<xyz[]> convertTask = ConvertToCustomerPositionData(dataModels);
            Task<xy[]> convertKiloTask = ConvertToWeightByData(dataModels);
            Task<xy[]> convertDistanceTask = ConvertToDistanceByData(dataModels);
            Task<xy[]> convertCustomerTask = ConvertToCustomerByData(dataModels);

            Task.WaitAll(convertTask, convertSlimTask, convertKiloTask, convertDistanceTask, convertCustomerTask);

            returnModel.SlimViewModel = convertSlimTask.Result;
            returnModel.CustomerPositionData = convertTask.Result;
            returnModel.KiloData = convertKiloTask.Result;
            returnModel.DistanceData = convertDistanceTask.Result;
            returnModel.CustomersData = convertCustomerTask.Result;
            return returnModel;
        }

        private static async Task<xy[]> ConvertToCustomerByData(List<DataModel> dataModels)
        {
            return await Task.Run(() =>
            {
                var customerDataTempList = new ConcurrentBag<xy>();
                Parallel.ForEach(dataModels, dataRow =>
                {
                    customerDataTempList.Add(new xy
                    {
                        x = dataRow.DateOfRoute.ToString(@"yyyy-MM-dd"),
                        y = dataRow.PinRouteModel.NumberOfCustomers
                    });
                });
                return customerDataTempList.OrderBy(row => row.x).ToArray();
            });
        }

        private static async Task<xy[]> ConvertToDistanceByData(List<DataModel> dataModels)
        {
            return await Task.Run(() =>
            {
                var customerDataTempList = new ConcurrentBag<xy>();
                Parallel.ForEach(dataModels, dataRow =>
                {
                    customerDataTempList.Add(new xy
                    {
                        x = dataRow.DateOfRoute.ToString(@"yyyy-MM-dd"),
                        y = Math.Round((dataRow.PinRouteModel.DistanceInMeters / 1000), 1),
                    });
                });
                return customerDataTempList.OrderBy(row => row.x).ToArray();
            });
        }

        private static async Task<xy[]> ConvertToWeightByData(List<DataModel> dataModels)
        {
            return await Task.Run(() =>
            {
                var customerDataTempList = new ConcurrentBag<xy>();
                Parallel.ForEach(dataModels, dataRow =>
                {
                    customerDataTempList.Add(new xy
                    {
                        x = dataRow.DateOfRoute.ToString(@"yyyy-MM-dd"),
                        y = dataRow.PinRouteModel.Weight,
                    });
                });
                return customerDataTempList.OrderBy(row => row.x).ToArray();
            });
        }

        private static async Task<SlimViewModel> GetSlimInformation(List<DataModel> dataModels)
        {
            return await Task.Run(() =>
            {
                var qh = new QuinyxHelper();
                /*
                 * Only sending one datarow since the name is the same for all rows
                 */
                var singleDataModelWithDriverDetails = qh.AddNamesToData(new List<DataModel>() { dataModels[0] });

                var returnModel = new SlimViewModel()
                {
                    AvrCustomers = dataModels.Sum(data => data.PinRouteModel.NumberOfCustomers) / dataModels.Count,
                    AvrDrivingDistance = Math.Round(
                            (
                            (dataModels.Sum(data => data.PinRouteModel.DistanceInMeters)
                            / dataModels.Count)
                            / 10000 /*To get result in swedish miles*/
                            )
                            , 1),
                    AvrWeight = dataModels.Sum(data => data.PinRouteModel.Weight) / dataModels.Count(),
                    DriverFullName = singleDataModelWithDriverDetails[0].Driver.GetDriverName(),
                    QuinyxId = singleDataModelWithDriverDetails[0].Driver.Id
                };

                return returnModel;
            });
        }

        private static async Task<xyz[]> ConvertToCustomerPositionData(List<DataModel> dataModels)
        {
            return await Task.Run(() =>
            {
                var customerDataTempList = new ConcurrentBag<xyz>();
                Parallel.ForEach(dataModels, dataRow =>
                {
                    Parallel.ForEach(dataRow.PinRouteModel.Customers, customer =>
                    {
                        customerDataTempList.Add(new xyz
                        {
                            x = dataRow.DateOfRoute.ToString(@"yyyy-MM-dd"),
                            y = customer.position_lat,
                            z = customer.position_lng,
                        });
                    });
                });
                return customerDataTempList.OrderBy(row => row.x).ToArray();
            });
        }
    }
}