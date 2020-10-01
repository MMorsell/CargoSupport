using CargoSupport.Models.QuinyxModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CargoSupport.Enums;
using CargoSupport.Models.DatabaseModels;
using System.Threading.Tasks;
using RestSharp;
using CargoSupport.Interfaces;

namespace CargoSupport.Helpers
{
    public class QuinyxHelper : IQuinyxHelper
    {
        public async Task<List<QuinyxModel>> GetAllDriversSorted(DateTime date, bool clearNames = true)
        {
            var getDriversTask = GetDrivers(date, date);

            var getExtrainformationTask = GetExtraInformationForDrivers();

            await Task.WhenAll(getDriversTask, getExtrainformationTask);

            var combinedResult = CombineQuinyxModelWithExtendedModel(getDriversTask.Result, getExtrainformationTask.Result, clearNames);
            return combinedResult.OrderBy(e => e.ExtendedInformationModel.GivenName).ToList();
        }

        public async Task<QuinyxModel[]> GetAllDriversSortedToArray(DateTime date, bool clearNames = true)
        {
            var getDriversTask = GetDrivers(date, date);

            var getExtrainformationTask = GetExtraInformationForDrivers();

            await Task.WhenAll(getDriversTask, getExtrainformationTask);

            var combinedResult = CombineQuinyxModelWithExtendedModel(getDriversTask.Result, getExtrainformationTask.Result, clearNames);
            return combinedResult.OrderBy(e => e.ExtendedInformationModel.GivenName).ToArray();
        }

        public async Task<List<int>> GetAllDriversWithReportingTo(string reportingTo)
        {
            var allDriversUnsorted = GetNonSchedualedDrivers();

            var validDriver = await allDriversUnsorted;

            return validDriver.Where(driver => driver.ReportingTo == reportingTo && driver.Active == 1).Select(driver => driver.Id).ToList();
        }

        public async Task<List<QuinyxModel>> GetDrivers(DateTime from, DateTime to)
        {
            var fromDate = from.ToString(@"yyyy-MM-dd");
            var toDate = to.ToString(@"yyyy-MM-dd");
            var client = new RestClient(Constants.SoapApi.Connection)
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "text/xml");
            request.AddHeader("Cookie", "QWFMSESSION=B3sAtHUIsYKEGfzcSW98Lsbqu4jAxdfy");
            request.AddParameter("text/xml", $"<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:uri=\"uri:FlexForce\"><soapenv:Header/><soapenv:Body><uri:wsdlGetSchedulesV2 soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><apiKey>{Constants.SoapApi.GetApiKey()}</apiKey><getSchedulesV2Request xsi:type=\"flex:getSchedulesV2Request\" xmlns:flex=\"http://qwfm/soap/FlexForce\"><fromDate xsi:type=\"xsd:string\">{fromDate}</fromDate><fromTime xsi:type=\"xsd:string\">00:00:00</fromTime><toDate xsi:type=\"xsd:string\">{toDate}</toDate><toTime xsi:type=\"xsd:string\">23:59:59</toTime><scheduledShifts xsi:type=\"xsd:boolean\">true</scheduledShifts><absenceShifts xsi:type=\"xsd:boolean\">false</absenceShifts><allUnits xsi:type=\"xsd:boolean\">false</allUnits><includeCosts xsi:type=\"xsd:boolean\">false</includeCosts></getSchedulesV2Request></uri:wsdlGetSchedulesV2></soapenv:Body></soapenv:Envelope>", ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);

            var result = new List<QuinyxModel>();
            XDocument doc2 = XDocument.Parse(response.Content);
            var allItems2 = doc2.Descendants().Where(x => x.Name.LocalName == "item");

            foreach (var item in allItems2)
            {
                var quinyxModel = new QuinyxModel();

                var rawId = item.Elements().FirstOrDefault(z => z.Name.LocalName == "persId");
                if (rawId == null)
                {
                    quinyxModel.Id = 0;
                }
                else
                {
                    quinyxModel.Id = (int)rawId;
                }

                if (quinyxModel.Id != 0)
                {
                    quinyxModel.BadgeNo = (string)item.Elements().FirstOrDefault(z => z.Name.LocalName == "badgeNo");
                    quinyxModel.begTimeString = (string)item.Elements().FirstOrDefault(z => z.Name.LocalName == "begTime");
                    quinyxModel.endTimeString = (string)item.Elements().FirstOrDefault(z => z.Name.LocalName == "endTime");
                    quinyxModel.CategoryId = (int)item.Elements().FirstOrDefault(z => z.Name.LocalName == "categoryId");
                    quinyxModel.Section = (int)item.Elements().FirstOrDefault(z => z.Name.LocalName == "section");
                    quinyxModel.SectionName = (string)item.Elements().FirstOrDefault(z => z.Name.LocalName == "sectionName");
                    quinyxModel.hours = (decimal)item.Elements().FirstOrDefault(z => z.Name.LocalName == "hours");
                    quinyxModel.CostCentre = (int)item.Elements().FirstOrDefault(z => z.Name.LocalName == "costCentre");
                    quinyxModel.ManagerId = (int)item.Elements().FirstOrDefault(z => z.Name.LocalName == "managerId");
                    result.Add(quinyxModel);
                }
            }
            return result.Select(res => res).Where(res => res.Id != 0).ToList();
        }

        private List<QuinyxModel> CombineQuinyxModelWithExtendedModel(List<QuinyxModel> quinyxResult, List<ExtendedInformationModel> extendedResult, bool clearNames = true)
        {
            //Attatch correct extendedModel with quinyxResult
            foreach (var driver in quinyxResult)
            {
                var extendedInfo = extendedResult.FirstOrDefault(info => info.Id.Equals(driver.Id));

                if (extendedInfo != null)
                {
                    driver.ExtendedInformationModel = extendedInfo;
                    if (clearNames)
                    {
                        driver.ExtendedInformationModel.GivenName = "";
                        driver.ExtendedInformationModel.FamilyName = "";
                    }
                }
            }

            //Remove non-drivers
            for (int i = quinyxResult.Count - 1; i >= 0; i--)
            {
                if (quinyxResult[i].ExtendedInformationModel.Active == 0 ||
                    GetQuinyxEnum(quinyxResult[i].CategoryId) != QuinyxRole.Driver)
                {
                    quinyxResult.RemoveAt(i);
                }

                TimeSpan.TryParse(quinyxResult[i].begTimeString, out TimeSpan begTime);
                TimeSpan.TryParse(quinyxResult[i].endTimeString, out TimeSpan endTime);

                quinyxResult[i].begTime = begTime;
                quinyxResult[i].endTime = endTime;
            }
            return quinyxResult;
        }

        public QuinyxRole GetQuinyxEnum(int categoryId)
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

        public async Task<List<BasicQuinyxModel>> GetNonSchedualedDrivers()
        {
            var client = new RestClient(Constants.SoapApi.Connection)
            {
                Timeout = -1
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "text/xml");
            request.AddHeader("Cookie", "QWFMSESSION=8K1nfQjkE56AmcKVN9dQdEhPCqsH0IhY");
            request.AddParameter("text/xml", $"<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:uri=\"uri:FlexForce\"> <soapenv:Header/> <soapenv:Body> <uri:wsdlGetEmployeesV2 soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"> <apiKey>{Constants.SoapApi.GetApiKey()}</apiKey> </uri:wsdlGetEmployeesV2> </soapenv:Body> </soapenv:Envelope>", ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);

            XDocument doc = XDocument.Parse(response.Content);

            var quinyxBasicModels = doc.Descendants().Where(x => x.Name.LocalName == "item").Select(y => new BasicQuinyxModel
            {
                Id = (int)y.Elements().FirstOrDefault(z => z.Name.LocalName == "id"),
                GivenName = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "givenName"),
                FamilyName = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "familyName"),
                Active = (int)y.Elements().FirstOrDefault(z => z.Name.LocalName == "active"),
                StaffCat = (int)y.Elements().FirstOrDefault(z => z.Name.LocalName == "staffCat"),
                StaffCatName = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "staffCatName"),
                Section = (int)y.Elements().FirstOrDefault(z => z.Name.LocalName == "section"),
                SectionName = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "sectionName"),
                ReportingTo = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "reportingTo"),
            }).ToList();
            return quinyxBasicModels.Where(mod => mod.Active == 1).ToList();
        }

        public async Task<List<DataModel>> AddNamesToData(List<DataModel> Data)
        {
            var client = new RestClient(Constants.SoapApi.Connection)
            {
                Timeout = -1
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "text/xml");
            request.AddHeader("Cookie", "QWFMSESSION=8K1nfQjkE56AmcKVN9dQdEhPCqsH0IhY");
            request.AddParameter("text/xml", $"<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:uri=\"uri:FlexForce\"> <soapenv:Header/> <soapenv:Body> <uri:wsdlGetEmployeesV2 soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"> <apiKey>{Constants.SoapApi.GetApiKey()}</apiKey> </uri:wsdlGetEmployeesV2> </soapenv:Body> </soapenv:Envelope>", ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);

            XDocument doc = XDocument.Parse(response.Content);

            var extendedInformation = doc.Descendants().Where(x => x.Name.LocalName == "item").Select(y => new ExtendedInformationModel
            {
                Id = (int)y.Elements().FirstOrDefault(z => z.Name.LocalName == "id"),
                GivenName = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "givenName"),
                FamilyName = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "familyName"),
                StaffCat = (int)y.Elements().FirstOrDefault(z => z.Name.LocalName == "staffCat"),
                StaffCatName = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "staffCatName"),
                Section = (int)y.Elements().FirstOrDefault(z => z.Name.LocalName == "section"),
                SectionName = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "sectionName"),
                ReportingTo = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "reportingTo"),
                Active = (int)y.Elements().FirstOrDefault(z => z.Name.LocalName == "active"),
            }).ToList();

            foreach (var data in Data)
            {
                var extendedInfo = extendedInformation.FirstOrDefault(info => info.Id.Equals(data.Driver.Id));

                if (extendedInfo != null)
                {
                    data.Driver.ExtendedInformationModel = extendedInfo;
                }
            }
            return Data;
        }

        public async Task<List<ExtendedInformationModel>> GetExtraInformationForDrivers()
        {
            var client = new RestClient(Constants.SoapApi.Connection)
            {
                Timeout = -1
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "text/xml");
            request.AddHeader("Cookie", "QWFMSESSION=8K1nfQjkE56AmcKVN9dQdEhPCqsH0IhY");
            request.AddParameter("text/xml", $"<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:uri=\"uri:FlexForce\"> <soapenv:Header/> <soapenv:Body> <uri:wsdlGetEmployeesV2 soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"> <apiKey>{Constants.SoapApi.GetApiKey()}</apiKey> </uri:wsdlGetEmployeesV2> </soapenv:Body> </soapenv:Envelope>", ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);

            XDocument doc = XDocument.Parse(response.Content);
            var extendedInformation = doc.Descendants().Where(x => x.Name.LocalName == "item").Select(y => new ExtendedInformationModel
            {
                Id = (int)y.Elements().FirstOrDefault(z => z.Name.LocalName == "id"),
                GivenName = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "givenName"),
                FamilyName = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "familyName"),
                StaffCat = (int)y.Elements().FirstOrDefault(z => z.Name.LocalName == "staffCat"),
                StaffCatName = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "staffCatName"),
                ReportingTo = (string)y.Elements().FirstOrDefault(z => z.Name.LocalName == "reportingTo"),
                Active = (int)y.Elements().FirstOrDefault(z => z.Name.LocalName == "active"),
            }).ToList();

            return extendedInformation;
        }
    }
}