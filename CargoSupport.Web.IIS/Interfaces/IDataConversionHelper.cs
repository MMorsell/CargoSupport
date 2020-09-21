using CargoSupport.Helpers;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.ViewModels.Analyze;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CargoSupport.Interfaces
{
    public interface IDataConversionHelper
    {
        Task<FullViewModel> ConvertDataModelsToFullViewModel(List<DataModel> dataModels);

        AllBossesViewModel[] ConvertDataModelsToMultipleDriverTableData(List<DataModel> routesOfToday);

        Task<List<SlimViewModel>> ConvertDataModelsToSlimViewModels(List<DataModel> dataModels);

        DataConversionHelper.CarStatisticsModel[] ConvertDataToCarStatisticsModel(List<DataModel> routesOfToday);

        SimplifiedRecordsViewModel[] ConvertDataToSimplifiedRecordsAsParalell(List<DataModel> routes);

        AllBossesViewModel[] ConvertDatRowsToBossGroup(List<DataModel> routesOfToday);

        TodayGraphsViewModel[] ConvertTodaysDataToGraphModelsAsParalell(List<DataModel> routesOfToday, bool splitRouteName);
    }
}