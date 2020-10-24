using CargoSupport.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.ViewModels.Analyze;
using System.Collections.Concurrent;
using CargoSupport.Models.PinModels;
using CargoSupport.Interfaces;
using Serilog;

namespace CargoSupport.Helpers
{
    public class DataConversionHelper : IDataConversionHelper
    {
        private readonly IQuinyxHelper _quinyxHelper;

        public DataConversionHelper(IQuinyxHelper quinyxHelper)
        {
            _quinyxHelper = quinyxHelper;
        }

        public class CarStatisticsModel
        {
            public string CarName { get; set; }
            public double DistanceInSwedishMiles { get; set; }
        }

        public CarStatisticsModel[] ConvertDataToCarStatisticsModel(List<DataModel> routesOfToday)
        {
            try
            {
                var resultModels = new List<CarStatisticsModel>();

                var groupedData = routesOfToday.GroupBy(data => data.CarModel);
                /*
                 * Get Valid data:
                 */

                foreach (var group in groupedData)
                {
                    var listedGroup = group.ToList();
                    if (listedGroup[0].CarModel != "Ej Satt" &&
                        listedGroup[0].CarModel != "")
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
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception in function ConvertDataToCarStatisticsModel");
                return new CarStatisticsModel[0];
            }
        }

        public TodayGraphsViewModel[] ConvertTodaysDataToGraphModelsAsParalell(List<DataModel> routesOfToday, bool splitRouteName)
        {
            try
            {
                var resultModels = new ConcurrentBag<TodayGraphsViewModel>();

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

                Parallel.ForEach(groupedData, group =>
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
                        todayGraphsModel.CustomersWithinTimeSlot = allCustomerWhereDeliveryHasBeenDone.Count(customer => CustomerIsInTimeWindowPlusMinus5(customer));

                        //Number of deliveries made withing 15 minutes of each customers estimated time
                        todayGraphsModel.CustomersWithinPrognosis = allCustomerWhereDeliveryHasBeenDone.Count(customer => CustomerIsInPhasePlusMinus15Minutes(customer));

                        //Number of customer deliveries made before time slot - 5 minutes
                        todayGraphsModel.CustomersBeforeTimeSlot = allCustomerWhereDeliveryHasBeenDone.Count(customer => DeliveryHasBeenMadeBeforeTimeSlotMinus5Minutes(customer));

                        //Number of deliveries made before estimated time -15 minutes
                        todayGraphsModel.CustomersBeforeEstimatedTime = allCustomerWhereDeliveryHasBeenDone.Count(customer => DeliveryHasBeenMadeBeforeEstimatedTimeMinus15Minutes(customer));

                        var allHoursDedicatedOnRoutes = group.Sum(route => (double)route.Driver.hours);
                        if (allHoursDedicatedOnRoutes <= 0)
                        {
                            //Failsafe if no driver exist on any route
                            todayGraphsModel.CustomersDividedByWorkHours = 0;
                        }
                        else
                        {
                            //Number of customers per work time of the drivers of all routes (does NOT include worktime for the whole force)
                            todayGraphsModel.CustomersDividedByWorkHours = Math.Round(todayGraphsModel.NumberOfValidDeliveries / allHoursDedicatedOnRoutes, 2);
                        }

                        if (todayGraphsModel.NumberOfValidDeliveries > 0)
                        {
                            //Percentages deliveries withing 5 minutes of each customer time slot
                            var conversion = (todayGraphsModel.CustomersWithinTimeSlot / todayGraphsModel.NumberOfValidDeliveries);
                            todayGraphsModel.PercentageWithin5MinOfTimeSlot = Math.Round(conversion, 4) * 100;
                            //Percentages deliveries withing 15 minutes of each customers estimated time
                            conversion = (todayGraphsModel.CustomersWithinPrognosis / todayGraphsModel.NumberOfValidDeliveries);
                            todayGraphsModel.PercentageWithin15MinOfCustomerEstimatedTime = Math.Round(conversion, 4) * 100;
                        }

                        if (splitRouteName)
                        {
                            todayGraphsModel.LabelTitle = listOfGroup[0].PinRouteModel.ParentOrderName;
                        }
                        else
                        {
                            todayGraphsModel.LabelTitle = listOfGroup[0].DateOfRoute.ToString(@"yyyy-MM-dd");
                        }

                        //Calculate percentages where route actual start is correct, or within 5 minutes of delay ON ROUTE LEVEL, NOT CUSTOMER LEVEL
                        var startedRoutesRightNow = listOfGroup.Where(route => route.PinRouteModel.RouteHasStarted);
                        double percentageOfStartedWithin5minutes = startedRoutesRightNow.Count((route => RouteHasBeenStartedBeforeOr5MinutesPlusOfEstimatedStart(route)));
                        var percentage = (percentageOfStartedWithin5minutes / startedRoutesRightNow.Count());
                        todayGraphsModel.PercentageWithin5MinutesOfStartTime = Math.Round(percentage, 4) * 100;

                        resultModels.Add(todayGraphsModel);
                    }
                });

                if (resultModels.Count == 0)
                {
                    resultModels.Add(new TodayGraphsViewModel
                    {
                        NumberOfValidDeliveries = 0,
                        NumberOfValidDeliveriesLeft = 0,
                        CustomersWithinTimeSlot = 0,
                        CustomersWithinPrognosis = 0,
                        CustomersBeforeTimeSlot = 0,
                        CustomersBeforeEstimatedTime = 0,
                        PercentageWithin5MinOfTimeSlot = 0,
                        PercentageWithin15MinOfCustomerEstimatedTime = 0,
                        PercentageWithin5MinutesOfStartTime = 0,
                        LabelTitle = "No data on this day, or 0 deliveries has been made"
                    });
                }

                return resultModels.OrderBy(d => d.LabelTitle).ToArray();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception in function ConvertTodaysDataToGraphModelsAsParalell");
                return new TodayGraphsViewModel[0];
            }
        }

        public AllBossesViewModel[] ConvertDatRowsToBossGroup(List<DataModel> routesOfToday)
        {
            try
            {
                var resultModels = new ConcurrentBag<AllBossesViewModel>();

                var dataGroupedByReportingTo = routesOfToday.GroupBy(data => data.Driver.ExtendedInformationModel.StaffCat);

                Parallel.ForEach(dataGroupedByReportingTo, driverGroup =>
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
                            todayGraphsModel.CustomersWithinTimeSlot = allCustomerWhereDeliveryHasBeenDone.Count(customer => CustomerIsInTimeWindowPlusMinus5(customer));

                            //Number of deliveries made withing 15 minutes of each customers estimated time
                            todayGraphsModel.CustomersWithinPrognosis = allCustomerWhereDeliveryHasBeenDone.Count(customer => CustomerIsInPhasePlusMinus15Minutes(customer));

                            //Number of customer deliveries made before time slot - 5 minutes
                            todayGraphsModel.CustomersBeforeTimeSlot = allCustomerWhereDeliveryHasBeenDone.Count(customer => DeliveryHasBeenMadeBeforeTimeSlotMinus5Minutes(customer));

                            //Number of deliveries made before estimated time +-0 minutes
                            todayGraphsModel.CustomersBeforeEstimatedTime = allCustomerWhereDeliveryHasBeenDone.Count(customer => DeliveryHasBeenMadeBeforeEstimatedTimeMinus15Minutes(customer));

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
                                todayGraphsModel.percentageWithin5MinOfTimeSlot = Math.Round(conversion, 4) * 100;
                                //Percentages deliveries withing 15 minutes of each customers estimated time
                                conversion = (todayGraphsModel.CustomersWithinPrognosis / todayGraphsModel.NumberOfValidDeliveries);
                                todayGraphsModel.percentageWithin15MinOfCustomerEstimatedTime = Math.Round(conversion, 4) * 100;
                            }

                            todayGraphsModel.LabelTitle = listOfGroup[0].Driver.ExtendedInformationModel.StaffCatName;
                            todayGraphsModel.StaffCatId = listOfGroup[0].Driver.ExtendedInformationModel.StaffCat;
                            todayGraphsModel.SectionId = listOfGroup[0].Driver.ExtendedInformationModel.Section;
                            resultModels.Add(todayGraphsModel);
                        }
                    }
                });

                return resultModels.OrderBy(d => d.LabelTitle).ToArray();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception in function ConvertDatRowsToBossGroup");
                return new AllBossesViewModel[0];
            }
        }

        private void ExtractDataByCompanyBosses(ConcurrentBag<AllBossesViewModel> resultModels, IGrouping<int, DataModel> driverGroup)
        {
            try
            {
                var innerGroupByBoss = driverGroup.GroupBy(data => data.Driver.ExtendedInformationModel.Section);
                Parallel.ForEach(innerGroupByBoss, innerDriverGroup =>
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
                        innerTodayGraphsModel.CustomersWithinTimeSlot = innerAllCustomerWhereDeliveryHasBeenDone.Count(customer => CustomerIsInTimeWindowPlusMinus5(customer));

                        //Number of deliveries made withing 15 minutes of each customers estimated time
                        innerTodayGraphsModel.CustomersWithinPrognosis = innerAllCustomerWhereDeliveryHasBeenDone.Count(customer => CustomerIsInPhasePlusMinus15Minutes(customer));

                        //Number of customer deliveries made before time slot - 5 minutes
                        innerTodayGraphsModel.CustomersBeforeTimeSlot = innerAllCustomerWhereDeliveryHasBeenDone.Count(customer => DeliveryHasBeenMadeBeforeTimeSlotMinus5Minutes(customer));

                        //Number of deliveries made before estimated time +-0 minutes
                        innerTodayGraphsModel.CustomersBeforeEstimatedTime = innerAllCustomerWhereDeliveryHasBeenDone.Count(customer => DeliveryHasBeenMadeBeforeEstimatedTimeMinus15Minutes(customer));

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
                            innerTodayGraphsModel.percentageWithin5MinOfTimeSlot = Math.Round(conversion, 4) * 100;
                            //Percentages deliveries withing 15 minutes of each customers estimated time
                            conversion = (innerTodayGraphsModel.CustomersWithinPrognosis / innerTodayGraphsModel.NumberOfValidDeliveries);
                            innerTodayGraphsModel.percentageWithin15MinOfCustomerEstimatedTime = Math.Round(conversion, 4) * 100;
                        }

                        innerTodayGraphsModel.LabelTitle = listOfGroup[0].Driver.ExtendedInformationModel.SectionName;
                        innerTodayGraphsModel.StaffCatId = listOfGroup[0].Driver.ExtendedInformationModel.StaffCat;
                        innerTodayGraphsModel.SectionId = listOfGroup[0].Driver.ExtendedInformationModel.Section;
                        resultModels.Add(innerTodayGraphsModel);
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception in function ExtractDataByCompanyBosses");
            }
        }

        public AllBossesViewModel[] ConvertDataModelsToMultipleDriverTableData(List<DataModel> routesOfToday)
        {
            try
            {
                var resultModels = new ConcurrentBag<AllBossesViewModel>();

                var groupedData = routesOfToday.GroupBy(data => data.Driver.Id);

                Parallel.ForEach(groupedData, group =>
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
                        todayGraphsModel.CustomersWithinTimeSlot = allCustomerWhereDeliveryHasBeenDone.Count(customer => CustomerIsInTimeWindowPlusMinus5(customer));

                        //Number of deliveries made withing 15 minutes of each customers estimated time
                        todayGraphsModel.CustomersWithinPrognosis = allCustomerWhereDeliveryHasBeenDone.Count(customer => CustomerIsInPhasePlusMinus15Minutes(customer));

                        //Number of customer deliveries made before time slot - 5 minutes
                        todayGraphsModel.CustomersBeforeTimeSlot = allCustomerWhereDeliveryHasBeenDone.Count(customer => DeliveryHasBeenMadeBeforeTimeSlotMinus5Minutes(customer));

                        //Number of deliveries made before estimated time +-0 minutes
                        todayGraphsModel.CustomersBeforeEstimatedTime = allCustomerWhereDeliveryHasBeenDone.Count(customer => DeliveryHasBeenMadeBeforeEstimatedTimeMinus15Minutes(customer));

                        if (todayGraphsModel.NumberOfValidDeliveries > 0)
                        {
                            //Percentages deliveries withing 5 minutes of each customer time slot
                            todayGraphsModel.percentageWithin5MinOfTimeSlot = Math.Round((todayGraphsModel.CustomersWithinTimeSlot / todayGraphsModel.NumberOfValidDeliveries), 4) * 100;
                            //Percentages deliveries withing 15 minutes of each customers estimated time
                            todayGraphsModel.percentageWithin15MinOfCustomerEstimatedTime = Math.Round((todayGraphsModel.CustomersWithinPrognosis / todayGraphsModel.NumberOfValidDeliveries), 4) * 100;
                        }

                        todayGraphsModel.LabelTitle = groupAsList[0].Driver.GetDriverName();
                        todayGraphsModel.DriverId = groupAsList[0].Driver.Id;
                        todayGraphsModel.StaffCatId = groupAsList[0].Driver.ExtendedInformationModel.StaffCat;
                        todayGraphsModel.SectionId = groupAsList[0].Driver.ExtendedInformationModel.Section;
                        resultModels.Add(todayGraphsModel);
                    }
                });

                if (resultModels.Count == 0)
                {
                    resultModels.Add(new AllBossesViewModel
                    {
                        NumberOfValidDeliveries = 0,
                        NumberOfValidDeliveriesLeft = 0,
                        CustomersWithinTimeSlot = 0,
                        CustomersWithinPrognosis = 0,
                        CustomersBeforeTimeSlot = 0,
                        CustomersBeforeEstimatedTime = 0,
                        percentageWithin5MinOfTimeSlot = 0,
                        percentageWithin15MinOfCustomerEstimatedTime = 0,
                        LabelTitle = "No data on this day"
                    });
                }

                return resultModels.ToArray();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception in function ConvertDataModelsToMultipleDriverTableData");
                return new AllBossesViewModel[0];
            }
        }

        public SimplifiedRecordsViewModel[] ConvertDataToSimplifiedRecordsAsParalell(List<DataModel> routes)
        {
            try
            {
                var resultModels = new ConcurrentBag<SimplifiedRecordsViewModel>();
                Parallel.ForEach(routes, route =>
                {
                    var allCommentsAsList = route.PinRouteModel.Customers.Select(customer => customer.CustomerReportModel);
                    var allServicesCommentsAsList = route.PinRouteModel.Customers.Select(customer => customer.CustomerServiceModel);
                    resultModels.Add(new SimplifiedRecordsViewModel()
                    {
                        RouteName = route.PinRouteModel.RouteName,
                        NumberOfCustomers = route.PinRouteModel.NumberOfCustomers,
                        Weight = route.PinRouteModel.Weight,
                        DistansInSwedishMiles = Math.Round((route.PinRouteModel.DistanceInMeters / 10000), 2),
                        CommentFromTransport = route.PostRideAnnotation,
                        ResourceRoute = route.IsResourceRoute,
                        DateOfRoute = route.DateOfRoute.ToString(@"yyyy-MM-dd"),
                        CustomerComments = allCommentsAsList.ToArray(),
                        ServiceComments = allServicesCommentsAsList.ToArray()
                    });
                });

                if (resultModels.Count == 0)
                {
                    resultModels.Add(new SimplifiedRecordsViewModel());
                }

                return resultModels.ToArray();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception in function ConvertDataToSimplifiedRecordsAsParalell");
                return new SimplifiedRecordsViewModel[0];
            }
        }

        private bool RouteHasBeenStartedBeforeOr5MinutesPlusOfEstimatedStart(DataModel route)
        {
            var timeSpan_Extra5MinutesOfEstimatedRouteStart = route.PinRouteModel.ScheduledRouteStart.TimeOfDay.Add(new TimeSpan(0, 0, 5, 0, 0));

            var result = (route.PinRouteModel.ActualRouteStartAsDate.TimeOfDay < timeSpan_Extra5MinutesOfEstimatedRouteStart);
            return result;
        }

        private bool CustomerIsInTimeWindowPlusMinus5(PinCustomerModel customerModel)
        {
            //Inom Tidsfönser -Hur många procent kunder inom tidsfönster - lägg på 5 minuter - time_handled ska vara inom detta
            var timeSpan_ActualDeliveryTime = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_handled.Split(' ')[1]);
            var timeSpan_WindowsStart = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.timewindow_start).Add(new TimeSpan(0, 0, -5, 0, 0));
            var timeSpan_WindowsEnd = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.timewindow_end).Add(new TimeSpan(0, 0, 5, 0, 0));

            var result = (timeSpan_ActualDeliveryTime > timeSpan_WindowsStart && timeSpan_ActualDeliveryTime < timeSpan_WindowsEnd);
            return result;
        }

        private bool CustomerIsInPhasePlusMinus15Minutes(PinCustomerModel customerModel)
        {
            var timeSpan_ActualDeliveryTime = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_handled.Split(' ')[1]);
            var timeSpan_WindowsStart = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_estimated.Split(' ')[1]).Add(new TimeSpan(0, 0, -15, 0, 0));
            var timeSpan_WindowsEnd = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_estimated.Split(' ')[1]).Add(new TimeSpan(0, 0, 15, 0, 0));

            var result = (timeSpan_ActualDeliveryTime > timeSpan_WindowsStart && timeSpan_ActualDeliveryTime < timeSpan_WindowsEnd);
            return result;
        }

        private bool DeliveryHasBeenMadeBeforeTimeSlotMinus5Minutes(PinCustomerModel customerModel)
        {
            var timeSpan_ActualDeliveryTime = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_handled.Split(' ')[1]);
            var timeSpan_WindowsStart = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.timewindow_start).Add(new TimeSpan(0, 0, -5, 0, 0));

            var result = (timeSpan_ActualDeliveryTime < timeSpan_WindowsStart);
            return result;
        }

        private bool DeliveryHasBeenMadeBeforeEstimatedTimeMinus15Minutes(PinCustomerModel customerModel)
        {
            var timeSpan_ActualDeliveryTime = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_handled.Split(' ')[1]);
            var timeSpan_WindowsStart = TimeSpan.Parse(customerModel.PinCustomerDeliveryInfo.time_estimated.Split(' ')[1]).Add(new TimeSpan(0, 0, -15, 0, 0));

            var result = (timeSpan_ActualDeliveryTime < timeSpan_WindowsStart);
            return result;
        }

        public List<SlimViewModel> ConvertDataModelsToSlimViewModels(List<DataModel> dataModels)
        {
            var returnList = new ConcurrentBag<SlimViewModel>();

            var groupedModels = dataModels.GroupBy(data => data.Driver.Id);

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
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, $"Exception when ConvertDataModelsToSlimViewModels");
                }
            });

            return returnList.ToList();
        }

        public async Task<FullViewModel> ConvertDataModelsToFullViewModel(List<DataModel> dataModels)
        {
            var returnModel = new FullViewModel();
            Task<SlimViewModel> convertSlimTask = GetSlimInformation(dataModels, _quinyxHelper);
            Task<Xyz[]> convertTask = ConvertToCustomerPositionData(dataModels);
            Task<Xy[]> convertKiloTask = ConvertToWeightByData(dataModels);
            Task<Xy[]> convertDistanceTask = ConvertToDistanceByData(dataModels);
            Task<Xy[]> convertCustomerTask = ConvertToCustomerByData(dataModels);

            await Task.WhenAll(convertTask, convertSlimTask, convertKiloTask, convertDistanceTask, convertCustomerTask);

            returnModel.SlimViewModel = convertSlimTask.Result;
            returnModel.CustomerPositionData = convertTask.Result;
            returnModel.KiloData = convertKiloTask.Result;
            returnModel.DistanceData = convertDistanceTask.Result;
            returnModel.CustomersData = convertCustomerTask.Result;
            return returnModel;
        }

        private async Task<Xy[]> ConvertToCustomerByData(List<DataModel> dataModels)
        {
            return await Task.Run(() =>
            {
                var customerDataTempList = new ConcurrentBag<Xy>();
                Parallel.ForEach(dataModels, dataRow =>
                {
                    customerDataTempList.Add(new Xy
                    {
                        x = dataRow.DateOfRoute.ToString(@"yyyy-MM-dd"),
                        y = dataRow.PinRouteModel.NumberOfCustomers
                    });
                });
                return customerDataTempList.OrderBy(row => row.x).ToArray();
            });
        }

        private async Task<Xy[]> ConvertToDistanceByData(List<DataModel> dataModels)
        {
            return await Task.Run(() =>
            {
                var customerDataTempList = new ConcurrentBag<Xy>();
                Parallel.ForEach(dataModels, dataRow =>
                {
                    customerDataTempList.Add(new Xy
                    {
                        x = dataRow.DateOfRoute.ToString(@"yyyy-MM-dd"),
                        y = Math.Round((dataRow.PinRouteModel.DistanceInMeters / 10000), 1),
                    });
                });
                return customerDataTempList.OrderBy(row => row.x).ToArray();
            });
        }

        private async Task<Xy[]> ConvertToWeightByData(List<DataModel> dataModels)
        {
            return await Task.Run(() =>
            {
                var customerDataTempList = new ConcurrentBag<Xy>();
                Parallel.ForEach(dataModels, dataRow =>
                {
                    customerDataTempList.Add(new Xy
                    {
                        x = dataRow.DateOfRoute.ToString(@"yyyy-MM-dd"),
                        y = dataRow.PinRouteModel.Weight,
                    });
                });
                return customerDataTempList.OrderBy(row => row.x).ToArray();
            });
        }

        private async Task<SlimViewModel> GetSlimInformation(List<DataModel> dataModels, IQuinyxHelper _quinyxHelper)
        {
            /*
             * Only sending one datarow since the name is the same for all rows
             */
            var singleDataModelWithDriverDetails = await _quinyxHelper.AddNamesToData(new List<DataModel>() { dataModels[0] });

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
                AvrWeight = dataModels.Sum(data => data.PinRouteModel.Weight) / dataModels.Count,
                DriverFullName = singleDataModelWithDriverDetails[0].Driver.GetDriverName(),
                QuinyxId = singleDataModelWithDriverDetails[0].Driver.Id
            };

            return returnModel;
        }

        private async Task<Xyz[]> ConvertToCustomerPositionData(List<DataModel> dataModels)
        {
            return await Task.Run(() =>
            {
                var customerDataTempList = new ConcurrentBag<Xyz>();
                Parallel.ForEach(dataModels, dataRow =>
                {
                    Parallel.ForEach(dataRow.PinRouteModel.Customers, customer =>
                    {
                        customerDataTempList.Add(new Xyz
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