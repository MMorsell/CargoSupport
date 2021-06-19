using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.QuinyxModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CargoSupport.Interfaces
{
    public interface IQuinyxHelper
    {
        Task<List<DataModel>> AddNamesToData(List<DataModel> Data);

        Task<List<DataModel>> AddNamesToData(Task<List<DataModel>> dataRetrievalTask);

        Task<List<QuinyxModel>> GetAllDriversSorted(DateTime date, bool clearNames = true);

        Task<QuinyxModel[]> GetAllDriversSortedToArray(DateTime date, bool clearNames = true);

        Task<List<int>> GetAllDriversWithReportingTo(string reportingTo);

        Task<List<QuinyxModel>> GetDrivers(DateTime from, DateTime to);

        Task<List<ExtendedInformationModel>> GetExtraInformationForDrivers();

        Task<List<BasicQuinyxModel>> GetNonSchedualedDrivers();

        Task<XDocument> RetrieveAllDriversFromQuinyx();
    }
}