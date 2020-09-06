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

        public class CarStatisticsModel
        {
            public string CarName { get; set; }
            public double DistanceInSwedishMiles { get; set; }
        }

        public static CarStatisticsModel[] ConvertDataToCarStatisticsModel(List<DataModel> routesOfToday)
        {
            var resultModels = new List<CarStatisticsModel>();

            var groupedData = routesOfToday.GroupBy(data => data.CarModel);
            /*
             * Get Valid data:
             */

            foreach (var group in groupedData)
            {
                var allCustomerWhereDeliveryHasBeenDone = new List<PinCustomerModel>();
                var listedGroup = group.ToList();
                if (listedGroup[0].CarModel != "Ej Satt")
                {
                    var distanceInSwedishMeters = group.Sum(
                            dataRow => dataRow.PinRouteModel.DistanceInMeters);

                    var carModel = new CarStatisticsModel
                    {
                        CarName = group.ToList()[0].CarModel,
                        DistanceInSwedishMiles = distanceInSwedishMeters / 10000
                    };

                    resultModels.Add(carModel);
                }
            }

            return resultModels.ToArray();
        }

        public static TodayGraphsViewModel[] ConvertTodaysDataToGraphModels(List<DataModel> routesOfToday, bool splitRouteName)
        {
            var resultModels = new List<TodayGraphsViewModel>();

            IEnumerable<IGrouping<object, DataModel>> groupedData;
            if (splitRouteName)
            {
                groupedData = routesOfToday.GroupBy(data => data.PinRouteModel.ParentOrderId);
            }
            else
            {
                groupedData = routesOfToday.GroupBy(data => data.DateOfRoute.Date.ToLongDateString());
            }
            /*
             * Get Valid data:
             */

            foreach (var group in groupedData)
            {
                var allCustomerWhereDeliveryHasBeenDone = new List<PinCustomerModel>();
                var todayGraphsModel = new TodayGraphsViewModel();

                foreach (var route in group)
                {
                    allCustomerWhereDeliveryHasBeenDone.AddRange(route.PinRouteModel.Customers.Where(customer => customer.PinCustomerDeliveryInfo.time_handled != null));
                }
                if (allCustomerWhereDeliveryHasBeenDone.Count > 0)
                {
                    var listOfGroup = group.ToList();
                    //Number of deliveries validated and done
                    todayGraphsModel.NumberOfValidDeliveries = allCustomerWhereDeliveryHasBeenDone.Count;
                    //Number left to be delivered
                    todayGraphsModel.NumberOfValidDeliveriesLeft = group.Sum(route => route.PinRouteModel.NumberOfCustomers) - todayGraphsModel.NumberOfValidDeliveries;

                    //Number of deliveries made within 5 minutes of each customer time slot
                    todayGraphsModel.CustomersWithinTimeSlot = allCustomerWhereDeliveryHasBeenDone.Where(customer => CustomerIsInTimeWindowPlusMinus5(customer)).Count();

                    //Number of deliveries made withing 15 minutes of each customers estimated time
                    todayGraphsModel.CustomersWithinPrognosis = allCustomerWhereDeliveryHasBeenDone.Where(customer => CustomerIsInPhasePlusMinus15Minutes(customer)).Count();

                    //Number of customer deliveries made before time slot - 5 minutes
                    todayGraphsModel.CustomersBeforeTimeSlot = allCustomerWhereDeliveryHasBeenDone.Where(customer => DeliveryHasBeenMadeBeforeTimeSlotMinus5Minutes(customer)).Count();

                    //Number of deliveries made before estimated time +-0 minutes
                    todayGraphsModel.CustomersBeforeEstimatedTime = allCustomerWhereDeliveryHasBeenDone.Where(customer => DeliveryHasBeenMadeBeforeEstimatedTimeMinus15Minutes(customer)).Count();

                    var allHoursDedicatedOnRoutes = group.Sum(route => (double)route.Driver.hours);
                    if (allHoursDedicatedOnRoutes <= 0)
                    {
                        //Failsafe if no driver exist on any route
                        todayGraphsModel.CustomersDividedByWorkHours = 0;
                    }
                    else
                    {
                        //Number of customers per work time of the drivers of all routes (does NOT include worktime for the whole force)
                        todayGraphsModel.CustomersDividedByWorkHours = Math.Round(todayGraphsModel.NumberOfValidDeliveries / allHoursDedicatedOnRoutes);
                    }

                    if (todayGraphsModel.NumberOfValidDeliveries > 0)
                    {
                        //Percentages deliveries withing 5 minutes of each customer time slot
                        var conversion = (todayGraphsModel.CustomersWithinTimeSlot / todayGraphsModel.NumberOfValidDeliveries);
                        todayGraphsModel.PercentageWithing5MinOfTimeSlot = Math.Round(conversion, 4) * 100;
                        //Percentages deliveries withing 15 minutes of each customers estimated time
                        conversion = (todayGraphsModel.CustomersWithinPrognosis / todayGraphsModel.NumberOfValidDeliveries);
                        todayGraphsModel.PercentageWithing15MinOfCustomerEstimatedTime = Math.Round(conversion, 4) * 100;
                    }

                    if (splitRouteName)
                    {
                        todayGraphsModel.LabelTitle = listOfGroup[0].PinRouteModel.ParentOrderName;
                    }
                    else
                    {
                        todayGraphsModel.LabelTitle = listOfGroup[0].DateOfRoute.ToString(@"yyyy-MM-dd");
                    }
                }
                else
                {
                    todayGraphsModel.NumberOfValidDeliveries = 0;
                    todayGraphsModel.NumberOfValidDeliveriesLeft = 0;
                    todayGraphsModel.CustomersWithinTimeSlot = 0;
                    todayGraphsModel.CustomersWithinPrognosis = 0;
                    todayGraphsModel.CustomersBeforeTimeSlot = 0;
                    todayGraphsModel.CustomersBeforeEstimatedTime = 0;
                    todayGraphsModel.PercentageWithing5MinOfTimeSlot = 0;
                    todayGraphsModel.PercentageWithing15MinOfCustomerEstimatedTime = 0;
                    todayGraphsModel.LabelTitle = "No data on this day";
                }

                resultModels.Add(todayGraphsModel);
            }

            return resultModels.OrderBy(d => d.LabelTitle).ToArray();
        }

        public static AllBossesViewModel[] ConvertDatRowsToBossGroup(List<DataModel> routesOfToday)
        {
            var resultModels = new List<AllBossesViewModel>();

            var dataGroupedByReportingTo = routesOfToday.GroupBy(data => data.Driver.ExtendedInformationModel.StaffCat);

            foreach (var driverGroup in dataGroupedByReportingTo)
            {
                var allCustomerWhereDeliveryHasBeenDone = new List<PinCustomerModel>();
                var todayGraphsModel = new AllBossesViewModel();

                if (driverGroup.Key == 28899)
                {
                    /*
                     * Since all internal drivers are grouped by same id, we need extra grouping to seperate by correct boss
                     */
                    ExtractDataByCompanyBosses(resultModels, driverGroup);
                }
                else
                {
                    foreach (var route in driverGroup)
                    {
                        allCustomerWhereDeliveryHasBeenDone.AddRange(route.PinRouteModel.Customers.Where(customer => customer.PinCustomerDeliveryInfo.time_handled != null));
                    }
                    if (allCustomerWhereDeliveryHasBeenDone.Count > 0)
                    {
                        var listOfGroup = driverGroup.ToList();
                        //Number of deliveries validated and done
                        todayGraphsModel.NumberOfValidDeliveries = allCustomerWhereDeliveryHasBeenDone.Count;
                        //Number left to be delivered
                        todayGraphsModel.NumberOfValidDeliveriesLeft = driverGroup.Sum(route => route.PinRouteModel.NumberOfCustomers) - todayGraphsModel.NumberOfValidDeliveries;

                        //Number of deliveries made within 5 minutes of each customer time slot
                        todayGraphsModel.CustomersWithinTimeSlot = allCustomerWhereDeliveryHasBeenDone.Where(customer => CustomerIsInTimeWindowPlusMinus5(customer)).Count();

                        //Number of deliveries made withing 15 minutes of each customers estimated time
                        todayGraphsModel.CustomersWithinPrognosis = allCustomerWhereDeliveryHasBeenDone.Where(customer => CustomerIsInPhasePlusMinus15Minutes(customer)).Count();

                        //Number of customer deliveries made before time slot - 5 minutes
                        todayGraphsModel.CustomersBeforeTimeSlot = allCustomerWhereDeliveryHasBeenDone.Where(customer => DeliveryHasBeenMadeBeforeTimeSlotMinus5Minutes(customer)).Count();

                        //Number of deliveries made before estimated time +-0 minutes
                        todayGraphsModel.CustomersBeforeEstimatedTime = allCustomerWhereDeliveryHasBeenDone.Where(customer => DeliveryHasBeenMadeBeforeEstimatedTimeMinus15Minutes(customer)).Count();

                        var allHoursDedicatedOnRoutes = driverGroup.Sum(route => (double)route.Driver.hours);
                        if (allHoursDedicatedOnRoutes <= 0)
                        {
                            //Failsafe if no driver exist on any route
                            todayGraphsModel.CustomersDividedByWorkHours = 0;
                        }
                        else
                        {
                            //Number of customers per work time of the drivers of all routes (does NOT include worktime for the whole force)
                            todayGraphsModel.CustomersDividedByWorkHours = Math.Round(todayGraphsModel.NumberOfValidDeliveries / allHoursDedicatedOnRoutes);
                        }

                        if (todayGraphsModel.NumberOfValidDeliveries > 0)
                        {
                            //Percentages deliveries withing 5 minutes of each customer time slot
                            var conversion = (todayGraphsModel.CustomersWithinTimeSlot / todayGraphsModel.NumberOfValidDeliveries);
                            todayGraphsModel.PercentageWithing5MinOfTimeSlot = Math.Round(conversion, 4) * 100;
                            //Percentages deliveries withing 15 minutes of each customers estimated time
                            conversion = (todayGraphsModel.CustomersWithinPrognosis / todayGraphsModel.NumberOfValidDeliveries);
                            todayGraphsModel.PercentageWithing15MinOfCustomerEstimatedTime = Math.Round(conversion, 4) * 100;
                        }

                        todayGraphsModel.LabelTitle = listOfGroup[0].Driver.ExtendedInformationModel.StaffCatName;
                        todayGraphsModel.StaffCatId = listOfGroup[0].Driver.ExtendedInformationModel.StaffCat;
                        todayGraphsModel.SectionId = listOfGroup[0].Driver.ExtendedInformationModel.Section;
                        resultModels.Add(todayGraphsModel);
                    }
                }
            }

            return resultModels.OrderBy(d => d.LabelTitle).ToArray();
        }

        private static void ExtractDataByCompanyBosses(List<AllBossesViewModel> resultModels, IGrouping<int, DataModel> driverGroup)
        {
            var innerGroupByBoss = driverGroup.GroupBy(data => data.Driver.ExtendedInformationModel.Section);

            foreach (var innerDriverGroup in innerGroupByBoss)
            {
                var innerAllCustomerWhereDeliveryHasBeenDone = new List<PinCustomerModel>();
                var innerTodayGraphsModel = new AllBossesViewModel();

                foreach (var route in innerDriverGroup)
                {
                    innerAllCustomerWhereDeliveryHasBeenDone.AddRange(route.PinRouteModel.Customers.Where(customer => customer.PinCustomerDeliveryInfo.time_handled != null));
                }
                if (innerAllCustomerWhereDeliveryHasBeenDone.Count > 0)
                {
                    var listOfGroup = innerDriverGroup.ToList();
                    //Number of deliveries validated and done
                    innerTodayGraphsModel.NumberOfValidDeliveries = innerAllCustomerWhereDeliveryHasBeenDone.Count;
                    //Number left to be delivered
                    innerTodayGraphsModel.NumberOfValidDeliveriesLeft = innerDriverGroup.Sum(route => route.PinRouteModel.NumberOfCustomers) - innerTodayGraphsModel.NumberOfValidDeliveries;

                    //Number of deliveries made within 5 minutes of each customer time slot
                    innerTodayGraphsModel.CustomersWithinTimeSlot = innerAllCustomerWhereDeliveryHasBeenDone.Where(customer => CustomerIsInTimeWindowPlusMinus5(customer)).Count();

                    //Number of deliveries made withing 15 minutes of each customers estimated time
                    innerTodayGraphsModel.CustomersWithinPrognosis = innerAllCustomerWhereDeliveryHasBeenDone.Where(customer => CustomerIsInPhasePlusMinus15Minutes(customer)).Count();

                    //Number of customer deliveries made before time slot - 5 minutes
                    innerTodayGraphsModel.CustomersBeforeTimeSlot = innerAllCustomerWhereDeliveryHasBeenDone.Where(customer => DeliveryHasBeenMadeBeforeTimeSlotMinus5Minutes(customer)).Count();

                    //Number of deliveries made before estimated time +-0 minutes
                    innerTodayGraphsModel.CustomersBeforeEstimatedTime = innerAllCustomerWhereDeliveryHasBeenDone.Where(customer => DeliveryHasBeenMadeBeforeEstimatedTimeMinus15Minutes(customer)).Count();

                    var allHoursDedicatedOnRoutes = innerDriverGroup.Sum(route => (double)route.Driver.hours);
                    if (allHoursDedicatedOnRoutes <= 0)
                    {
                        //Failsafe if no driver exist on any route
                        innerTodayGraphsModel.CustomersDividedByWorkHours = 0;
                    }
                    else
                    {
                        //Number of customers per work time of the drivers of all routes (does NOT include worktime for the whole force)
                        innerTodayGraphsModel.CustomersDividedByWorkHours = Math.Round(innerTodayGraphsModel.NumberOfValidDeliveries / allHoursDedicatedOnRoutes);
                    }

                    if (innerTodayGraphsModel.NumberOfValidDeliveries > 0)
                    {
                        //Percentages deliveries withing 5 minutes of each customer time slot
                        var conversion = (innerTodayGraphsModel.CustomersWithinTimeSlot / innerTodayGraphsModel.NumberOfValidDeliveries);
                        innerTodayGraphsModel.PercentageWithing5MinOfTimeSlot = Math.Round(conversion, 4) * 100;
                        //Percentages deliveries withing 15 minutes of each customers estimated time
                        conversion = (innerTodayGraphsModel.CustomersWithinPrognosis / innerTodayGraphsModel.NumberOfValidDeliveries);
                        innerTodayGraphsModel.PercentageWithing15MinOfCustomerEstimatedTime = Math.Round(conversion, 4) * 100;
                    }

                    innerTodayGraphsModel.LabelTitle = listOfGroup[0].Driver.ExtendedInformationModel.SectionName;
                    innerTodayGraphsModel.StaffCatId = listOfGroup[0].Driver.ExtendedInformationModel.StaffCat;
                    innerTodayGraphsModel.SectionId = listOfGroup[0].Driver.ExtendedInformationModel.Section;
                    resultModels.Add(innerTodayGraphsModel);
                }
            }
        }

        public static AllBossesViewModel[] ConvertDataModelsToMultipleDriverTableData(List<DataModel> routesOfToday)
        {
            var resultModels = new List<AllBossesViewModel>();

            var groupedData = routesOfToday.GroupBy(data => data.Driver.Id);
            /*
             * Get Valid data:
             */

            foreach (var group in groupedData)
            {
                var allCustomerWhereDeliveryHasBeenDone = new List<PinCustomerModel>();
                var todayGraphsModel = new AllBossesViewModel();

                foreach (var route in group)
                {
                    allCustomerWhereDeliveryHasBeenDone.AddRange(route.PinRouteModel.Customers.Where(customer => customer.PinCustomerDeliveryInfo.time_handled != null));
                }
                if (allCustomerWhereDeliveryHasBeenDone.Count > 0)
                {
                    var groupAsList = group.ToList();
                    //Number of deliveries validated and done
                    todayGraphsModel.NumberOfValidDeliveries = allCustomerWhereDeliveryHasBeenDone.Count;
                    //Number left to be delivered
                    todayGraphsModel.NumberOfValidDeliveriesLeft = group.Sum(route => route.PinRouteModel.NumberOfCustomers) - todayGraphsModel.NumberOfValidDeliveries;

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

                    todayGraphsModel.LabelTitle = groupAsList[0].Driver.GetDriverName();
                    todayGraphsModel.StaffCatId = groupAsList[0].Driver.ExtendedInformationModel.StaffCat;
                    todayGraphsModel.SectionId = groupAsList[0].Driver.ExtendedInformationModel.Section;
                }
                else
                {
                    todayGraphsModel.NumberOfValidDeliveries = 0;
                    todayGraphsModel.NumberOfValidDeliveriesLeft = 0;
                    todayGraphsModel.CustomersWithinTimeSlot = 0;
                    todayGraphsModel.CustomersWithinPrognosis = 0;
                    todayGraphsModel.CustomersBeforeTimeSlot = 0;
                    todayGraphsModel.CustomersBeforeEstimatedTime = 0;
                    todayGraphsModel.PercentageWithing5MinOfTimeSlot = 0;
                    todayGraphsModel.PercentageWithing15MinOfCustomerEstimatedTime = 0;
                    todayGraphsModel.LabelTitle = "No data on this driver";
                }

                resultModels.Add(todayGraphsModel);
            }

            return resultModels.ToArray();
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

        public static async Task<List<SlimViewModel>> ConvertDataModelsToSlimViewModels(List<DataModel> dataModels)
        {
            var returnList = new ConcurrentBag<SlimViewModel>();
            var qh = new QuinyxHelper();
            var dataModelsWithNames = await qh.AddNamesToData(dataModels);

            var groupedModels = dataModelsWithNames.GroupBy(data => data.Driver.Id);

            Parallel.ForEach(groupedModels, group =>
            {
                try
                {
                    var groupToList = group.ToList();
                    if (groupToList[0].Driver.Id != -1)
                    {
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
                }
                catch (Exception)
                {
                    //TODO: Handle exeption here
                }
            });

            return returnList.ToList();
        }

        public static async Task<FullViewModel> ConvertDataModelsToFullViewModel(List<DataModel> dataModels)
        {
            var returnModel = new FullViewModel();
            Task<SlimViewModel> convertSlimTask = GetSlimInformation(dataModels);
            Task<xyz[]> convertTask = ConvertToCustomerPositionData(dataModels);
            Task<xy[]> convertKiloTask = ConvertToWeightByData(dataModels);
            Task<xy[]> convertDistanceTask = ConvertToDistanceByData(dataModels);
            Task<xy[]> convertCustomerTask = ConvertToCustomerByData(dataModels);

            await Task.WhenAll(convertTask, convertSlimTask, convertKiloTask, convertDistanceTask, convertCustomerTask);

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
                        y = Math.Round((dataRow.PinRouteModel.DistanceInMeters / 10000), 1),
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
            var qh = new QuinyxHelper();
            /*
             * Only sending one datarow since the name is the same for all rows
             */
            var singleDataModelWithDriverDetails = await qh.AddNamesToData(new List<DataModel>() { dataModels[0] });

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