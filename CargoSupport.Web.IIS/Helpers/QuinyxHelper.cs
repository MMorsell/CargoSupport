using CargoSupport.Models.QuinyxModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using CargoSupport.Enums;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.Extensions;
using CargoSupport.ViewModels;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using RestSharp;

namespace CargoSupport.Helpers
{
    public class QuinyxHelper
    {
        public async Task<List<QuinyxModel>> GetAllDriversSorted(DateTime date, bool clearNames = true)
        {
            var getDriversTask = GetDrivers(date, date);

            var getExtrainformationTask = GetExtraInformationForDrivers();

            await Task.WhenAll(getDriversTask, getExtrainformationTask);

            var combinedResult = CombineQuinyxModelWithExtendedModel(getDriversTask.Result, getExtrainformationTask.Result, false);
            return combinedResult.OrderBy(e => e.ExtendedInformationModel.GivenName).ToList();
        }

        public async Task<QuinyxModel[]> GetAllDriversSortedToArray(DateTime date, bool clearNames = true)
        {
            var getDriversTask = GetDrivers(date, date);

            var getExtrainformationTask = GetExtraInformationForDrivers();

            await Task.WhenAll(getDriversTask, getExtrainformationTask);

            var combinedResult = CombineQuinyxModelWithExtendedModel(getDriversTask.Result, getExtrainformationTask.Result, false);
            return combinedResult.OrderBy(e => e.ExtendedInformationModel.GivenName).ToArray();
        }

        public List<int> GetAllDriversWithReportingTo(string reportingTo)
        {
            var allDriversUnsorted = GetNonSchedualedDrivers();

            var validDriver = allDriversUnsorted.Where(driver => driver.ReportingTo == reportingTo && driver.Active == 1);

            return validDriver.Select(driver => driver.Id).ToList();
        }

        public async Task<List<QuinyxModel>> GetDrivers(DateTime from, DateTime to)
        {
            var client = new RestClient(Constants.SoapApi.Connection)
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "text/xml");
            request.AddHeader("Cookie", "QWFMSESSION=jORrEC6wSGb0GXeKtAnki0Hz93vnIpPk");
            request.AddParameter("text/xml", $"<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:uri=\"uri:FlexForce\"> <soapenv:Header/> <soapenv:Body> <uri:wsdlGetEmployeesV2 soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"> <apiKey>{Constants.SoapApi.GetApiKey()}</apiKey> </uri:wsdlGetEmployeesV2> </soapenv:Body> </soapenv:Envelope>", ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);

            var result = new List<QuinyxModel>();
            XDocument doc2 = XDocument.Parse(response.Content);
            var allItems2 = doc2.Descendants().Where(x => x.Name.LocalName == "item");

            foreach (var item in allItems2)
            {
                var quinyxModel = new QuinyxModel();

                var rawId = item.Elements().Where(z => z.Name.LocalName == "persId").FirstOrDefault();
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
                    quinyxModel.BadgeNo = (string)item.Elements().Where(z => z.Name.LocalName == "badgeNo").FirstOrDefault();
                    quinyxModel.begTimeString = (string)item.Elements().Where(z => z.Name.LocalName == "begTime").FirstOrDefault();
                    quinyxModel.endTimeString = (string)item.Elements().Where(z => z.Name.LocalName == "endTime").FirstOrDefault();
                    quinyxModel.CategoryId = (int)item.Elements().Where(z => z.Name.LocalName == "categoryId").FirstOrDefault();
                    quinyxModel.Section = (int)item.Elements().Where(z => z.Name.LocalName == "section").FirstOrDefault();
                    quinyxModel.SectionName = (string)item.Elements().Where(z => z.Name.LocalName == "sectionName").FirstOrDefault();
                    quinyxModel.hours = (decimal)item.Elements().Where(z => z.Name.LocalName == "hours").FirstOrDefault();
                    quinyxModel.CostCentre = (int)item.Elements().Where(z => z.Name.LocalName == "costCentre").FirstOrDefault();
                    quinyxModel.ManagerId = (int)item.Elements().Where(z => z.Name.LocalName == "managerId").FirstOrDefault();
                    result.Add(quinyxModel);
                }
            }
            return result.ToList().Select(res => res).Where(res => res.Id != 0).ToList();
        }

        private List<QuinyxModel> CombineQuinyxModelWithExtendedModel(List<QuinyxModel> quinyxResult, List<ExtendedInformationModel> extendedResult, bool clearNames = true)
        {
            //Remove non-drivers
            for (int i = quinyxResult.Count - 1; i >= 0; i--)
            {
                if (quinyxResult[i].ExtendedInformationModel.Active == 0 ||
                    CargoSupport.Helpers.DataConversionHelper.GetQuinyxEnum(quinyxResult[i].CategoryId) != QuinyxRole.Driver)
                {
                    quinyxResult.RemoveAt(i);
                }

                TimeSpan.TryParse(quinyxResult[i].begTimeString, out TimeSpan begTime);
                TimeSpan.TryParse(quinyxResult[i].endTimeString, out TimeSpan endTime);

                quinyxResult[i].begTime = begTime;
                quinyxResult[i].endTime = endTime;
            }

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

            return quinyxResult;
        }

        private static List<BasicQuinyxModel> GetNonSchedualedDrivers()
        {
            XmlDocument soapEnvelopeXml = CreateSoapGetAllDriversEnvelope();
            HttpWebRequest webRequest = CreateWebRequest(CargoSupport.Constants.SoapApi.Connection);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            var quinyxBasicModels = new List<BasicQuinyxModel>();
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    XDocument doc = XDocument.Load(rd);
                    quinyxBasicModels = doc.Descendants().Where(x => x.Name.LocalName == "item").Select(y => new BasicQuinyxModel
                    {
                        Id = (int)y.Elements().Where(z => z.Name.LocalName == "id").FirstOrDefault(),
                        GivenName = (string)y.Elements().Where(z => z.Name.LocalName == "givenName").FirstOrDefault(),
                        FamilyName = (string)y.Elements().Where(z => z.Name.LocalName == "familyName").FirstOrDefault(),
                        Active = (int)y.Elements().Where(z => z.Name.LocalName == "active").FirstOrDefault(),
                        ReportingTo = (string)y.Elements().Where(z => z.Name.LocalName == "reportingTo").FirstOrDefault(),
                    }).ToList();
                }
            }
            return quinyxBasicModels.Where(mod => mod.Active == 1).ToList();
        }

        public List<DataModel> AddNamesToData(List<DataModel> Data)
        {
            XmlDocument soapEnvelopeXml = CreateSoapGetAllDriversEnvelope();
            HttpWebRequest webRequest = CreateWebRequest(CargoSupport.Constants.SoapApi.Connection);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            var extendedInformation = new List<ExtendedInformationModel>();
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    XDocument doc = XDocument.Load(rd);
                    extendedInformation = doc.Descendants().Where(x => x.Name.LocalName == "item").Select(y => new ExtendedInformationModel
                    {
                        Id = (int)y.Elements().Where(z => z.Name.LocalName == "id").FirstOrDefault(),
                        GivenName = (string)y.Elements().Where(z => z.Name.LocalName == "givenName").FirstOrDefault(),
                        FamilyName = (string)y.Elements().Where(z => z.Name.LocalName == "familyName").FirstOrDefault(),
                        StaffCat = (int)y.Elements().Where(z => z.Name.LocalName == "staffCat").FirstOrDefault(),
                        StaffCatName = (string)y.Elements().Where(z => z.Name.LocalName == "staffCatName").FirstOrDefault(),
                        ReportingTo = (string)y.Elements().Where(z => z.Name.LocalName == "reportingTo").FirstOrDefault(),
                        Active = (int)y.Elements().Where(z => z.Name.LocalName == "active").FirstOrDefault(),
                    }).ToList();
                }
            }

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
            var extendedInformation = new List<ExtendedInformationModel>();

            extendedInformation = doc.Descendants().Where(x => x.Name.LocalName == "item").Select(y => new ExtendedInformationModel
            {
                Id = (int)y.Elements().Where(z => z.Name.LocalName == "id").FirstOrDefault(),
                GivenName = (string)y.Elements().Where(z => z.Name.LocalName == "givenName").FirstOrDefault(),
                FamilyName = (string)y.Elements().Where(z => z.Name.LocalName == "familyName").FirstOrDefault(),
                StaffCat = (int)y.Elements().Where(z => z.Name.LocalName == "staffCat").FirstOrDefault(),
                StaffCatName = (string)y.Elements().Where(z => z.Name.LocalName == "staffCatName").FirstOrDefault(),
                ReportingTo = (string)y.Elements().Where(z => z.Name.LocalName == "reportingTo").FirstOrDefault(),
                Active = (int)y.Elements().Where(z => z.Name.LocalName == "active").FirstOrDefault(),
            }).ToList();

            return extendedInformation;
        }

        public List<DriverViewModel> GetAllDrivers()
        {
            XmlDocument soapEnvelopeXml = CreateSoapGetAllDriversEnvelope();
            HttpWebRequest webRequest = CreateWebRequest(CargoSupport.Constants.SoapApi.Connection);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            var driverViewModel = new List<DriverViewModel>();
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    XDocument doc = XDocument.Load(rd);
                    driverViewModel = doc.Descendants().Where(x => x.Name.LocalName == "item").Select(y => new DriverViewModel
                    {
                        Id = (int)y.Elements().Where(z => z.Name.LocalName == "id").FirstOrDefault(),
                        FullName = $"{(string)y.Elements().Where(z => z.Name.LocalName == "givenName").FirstOrDefault()} {(string)y.Elements().Where(z => z.Name.LocalName == "familyName").FirstOrDefault()}",
                        Active = (int)y.Elements().Where(z => z.Name.LocalName == "active").FirstOrDefault(),
                    }).ToList();
                }
            }

            return driverViewModel.Select(dr => dr).Where(dr => dr.Active == 1).ToList();
        }

        private static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope(DateTime from, DateTime to)
        {
            string key = Constants.SoapApi.GetApiKey();
            var fromDate = from.ToString(@"yyyy-MM-dd");
            var toDate = to.ToString(@"yyyy-MM-dd");

            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(@$"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:uri=""uri:FlexForce""><soapenv:Header/><soapenv:Body><uri:wsdlGetSchedulesV2 soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><apiKey>{key}</apiKey><getSchedulesV2Request xsi:type=""flex:getSchedulesV2Request"" xmlns:flex=""http://qwfm/soap/FlexForce""><fromDate xsi:type=""xsd:string"">{fromDate}</fromDate><fromTime xsi:type=""xsd:string"">00:00:00</fromTime><toDate xsi:type=""xsd:string"">{toDate}</toDate><toTime xsi:type=""xsd:string"">23:59:59</toTime><scheduledShifts xsi:type=""xsd:boolean"">true</scheduledShifts><absenceShifts xsi:type=""xsd:boolean"">false</absenceShifts><allUnits xsi:type=""xsd:boolean"">false</allUnits><includeCosts xsi:type=""xsd:boolean"">false</includeCosts></getSchedulesV2Request></uri:wsdlGetSchedulesV2></soapenv:Body></soapenv:Envelope>");
            return soapEnvelopeDocument;
        }

        private static XmlDocument CreateSoapGetAllDriversEnvelope()
        {
            string key = Constants.SoapApi.GetApiKey();

            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(@$"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:uri=""uri:FlexForce""> <soapenv:Header/> <soapenv:Body> <uri:wsdlGetEmployeesV2 soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""> <apiKey>{key}</apiKey> </uri:wsdlGetEmployeesV2> </soapenv:Body> </soapenv:Envelope>");
            return soapEnvelopeDocument;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
    }
}