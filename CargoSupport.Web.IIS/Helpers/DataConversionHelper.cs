using CargoSupport.Enums;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.ViewModels.Analyze;
using System.Collections.Concurrent;
using CargoSupport.Models.PinModels;

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

        public static TodayGraphsViewModel ConvertTodaysDataToGraphModels(List<DataModel> routesOfToday)
        {
            /*
             * Get Valid data:
             */

            var allCustomerWhereDeliveryHasBeenDone = new List<PinCustomerModel>();
            var todayGraphsModel = new TodayGraphsViewModel();
            foreach (var route in routesOfToday)
            {
                allCustomerWhereDeliveryHasBeenDone.AddRange(route.PinRouteModel.Customers.Where(customer => customer.PinCustomerDeliveryInfo.time_handled != null));
            }
            if (allCustomerWhereDeliveryHasBeenDone.Count > 0)
            {
                //Number of deliveries validated and done
                todayGraphsModel.NumberOfValidDeliveries = allCustomerWhereDeliveryHasBeenDone.Count;
                //Number left to be delivered
                todayGraphsModel.NumberOfValidDeliveriesLeft = routesOfToday.Sum(route => route.PinRouteModel.NumberOfCustomers) - todayGraphsModel.NumberOfValidDeliveries;

                //Number of deliveries made within 5 minutes of each customer time slot
                todayGraphsModel.CustomersWithinTimeSlot = allCustomerWhereDeliveryHasBeenDone.Where(customer => CustomerIsInTimeWindowPlusMinus5(customer)).Count();

                //Number of deliveries made withing 15 minutes of each customers estimated time
                todayGraphsModel.CustomersWithinPrognosis = allCustomerWhereDeliveryHasBeenDone.Where(customer => CustomerIsInPhasePlusMinus15Minutes(customer)).Count();

                //Number of customer deliveries made before time slot - 5 minutes
                todayGraphsModel.CustomersBeforeTimeSlot = allCustomerWhereDeliveryHasBeenDone.Where(customer => DeliveryHasBeenMadeBeforeTimeSlotMinus5Minutes(customer)).Count();

                //Number of deliveries made before estimated time +-0 minutes
                todayGraphsModel.CustomersBeforeEstimatedTime = allCustomerWhereDeliveryHasBeenDone.Where(customer => DeliveryHasBeenMadeBeforeEstimatedTimeMinus15Minutes(customer)).Count();

                if (todayGraphsModel.NumberOfValidDeliveries > 0)
                {
                    //Percentages deliveries withing 5 minutes of each customer time slot
                    todayGraphsModel.PercentageWithing5MinOfTimeSlot = Math.Round((todayGraphsModel.CustomersWithinTimeSlot / todayGraphsModel.NumberOfValidDeliveries), 4) * 100;
                    //Percentages deliveries withing 15 minutes of each customers estimated time
                    todayGraphsModel.PercentageWithing15MinOfCustomerEstimatedTime = Math.Round((todayGraphsModel.CustomersWithinPrognosis / todayGraphsModel.NumberOfValidDeliveries), 4) * 100;
                }

                todayGraphsModel.LabelTitle = routesOfToday[0].DateOfRoute.ToString(@"yyyy-MM-dd");
            }
            else
            {
                todayGraphsModel.LabelTitle = "No data on this day";
            }
            return todayGraphsModel;
        }

        private static bool CustomerIsInTimeWindowPlusMinus5(PinCustomerModel customerModel)
        {
            //Inom Tidsfönser -Hur många procent kunder inom tidsfönster - lägg på 5 minuter - time_handled ska vara inom detta
            var timeSpan_ActualDeliveryTime = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_handled.Split(' ')[1]);
            var timeSpan_WindowsStart = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.timewindow_start).Add(new TimeSpan(0, 0, -5, 0, 0));
            var timeSpan_WindowsEnd = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.timewindow_end).Add(new TimeSpan(0, 0, 5, 0, 0));

            var result = (timeSpan_ActualDeliveryTime > timeSpan_WindowsStart && timeSpan_ActualDeliveryTime < timeSpan_WindowsEnd);
            return result;
        }

        private static bool CustomerIsInPhasePlusMinus15Minutes(PinCustomerModel customerModel)
        {
            var timeSpan_ActualDeliveryTime = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_handled.Split(' ')[1]);
            var timeSpan_WindowsStart = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_estimated.Split(' ')[1]).Add(new TimeSpan(0, 0, -15, 0, 0));
            var timeSpan_WindowsEnd = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_estimated.Split(' ')[1]).Add(new TimeSpan(0, 0, 15, 0, 0));

            var result = (timeSpan_ActualDeliveryTime > timeSpan_WindowsStart && timeSpan_ActualDeliveryTime < timeSpan_WindowsEnd);
            return result;
        }

        private static bool DeliveryHasBeenMadeBeforeTimeSlotMinus5Minutes(PinCustomerModel customerModel)
        {
            var timeSpan_ActualDeliveryTime = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_handled.Split(' ')[1]);
            var timeSpan_WindowsStart = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.timewindow_start).Add(new TimeSpan(0, 0, -5, 0, 0));

            var result = (timeSpan_ActualDeliveryTime < timeSpan_WindowsStart);
            return result;
        }

        private static bool DeliveryHasBeenMadeBeforeEstimatedTimeMinus15Minutes(PinCustomerModel customerModel)
        {
            var timeSpan_ActualDeliveryTime = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_handled.Split(' ')[1]);
            var timeSpan_WindowsStart = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_estimated.Split(' ')[1]).Add(new TimeSpan(0, 0, -15, 0, 0));

            var result = (timeSpan_ActualDeliveryTime < timeSpan_WindowsStart);
            return result;
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