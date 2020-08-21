using CargoSupport.Web.Models.QuinyxModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using CargoSupport.Enums;

namespace CargoSupport.Helpers
{
    public class QuinyxHelper
    {
        internal List<QuinyxModel> GetAllDriversSorted(DateTime now)
        {
            return GetDrivers(DateTime.Now, DateTime.Now).OrderBy(e => e.begTime).ThenBy(e => e.endTime).ToList();
        }

        public List<QuinyxModel> GetDrivers(DateTime from, DateTime to)
        {
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(from, to);
            HttpWebRequest webRequest = CreateWebRequest(CargoSupport.Constants.SoapApi.Connection);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();
            // get the response from the completed web request.
            var result = new List<QuinyxModel>();
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    XDocument doc = XDocument.Load(rd);

                    result = doc.Descendants().Where(x => x.Name.LocalName == "item").Select(y => new QuinyxModel
                    {
                        PersonId = (int)y.Elements().Where(z => z.Name.LocalName == "persId").FirstOrDefault(),
                        BadgeNo = (string)y.Elements().Where(z => z.Name.LocalName == "badgeNo").FirstOrDefault(),
                        begTimeString = (string)y.Elements().Where(z => z.Name.LocalName == "begTime").FirstOrDefault(),
                        endTimeString = (string)y.Elements().Where(z => z.Name.LocalName == "endTime").FirstOrDefault(),
                        categoryName = (string)y.Elements().Where(z => z.Name.LocalName == "categoryName").FirstOrDefault(),
                        hours = (decimal)y.Elements().Where(z => z.Name.LocalName == "hours").FirstOrDefault(),
                        costCentre = (int)y.Elements().Where(z => z.Name.LocalName == "costCentre").FirstOrDefault(),
                    }).ToList();
                }

                //Remove undefined result/Empty drivers
                result = result.Select(res => res).Where(res => res.BadgeNo != "").ToList();

                //Remove non-drivers
                for (int i = result.Count - 1; i >= 0; i--)
                {
                    if (CargoSupport.Helpers.DataConversionHelper.GetQuinyxEnum(result[i].categoryName) != QuinyxRole.Driver)
                    {
                        result.RemoveAt(i);
                    }
                }

                for (int i = 0; i < result.Count; i++)
                {
                    TimeSpan.TryParse(result[i].begTimeString, out TimeSpan begTime);
                    TimeSpan.TryParse(result[i].endTimeString, out TimeSpan endTime);

                    result[i].begTime = begTime;
                    result[i].endTime = endTime;
                }
                return result;
            }
        }

        public List<QuinyxModel> GetExtraInformationForDrivers(List<QuinyxModel> Drivers)
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
                        BadgeNo = (string)y.Elements().Where(z => z.Name.LocalName == "badgeNo").FirstOrDefault(),
                        GivenName = (string)y.Elements().Where(z => z.Name.LocalName == "givenName").FirstOrDefault(),
                        FamilyName = (string)y.Elements().Where(z => z.Name.LocalName == "familyName").FirstOrDefault(),
                        StaffCat = (int)y.Elements().Where(z => z.Name.LocalName == "staffCat").FirstOrDefault(),
                        StaffCatName = (string)y.Elements().Where(z => z.Name.LocalName == "staffCatName").FirstOrDefault(),
                        Section = (int)y.Elements().Where(z => z.Name.LocalName == "section").FirstOrDefault(),
                        CostCentre = (int)y.Elements().Where(z => z.Name.LocalName == "costCentre").FirstOrDefault(),
                        ReportingTo = (string)y.Elements().Where(z => z.Name.LocalName == "reportingTo").FirstOrDefault()
                    }).ToList();
                }
            }

            foreach (var driver in Drivers)
            {
                var extendedInfo = extendedInformation.FirstOrDefault(info => info.BadgeNo.Equals(driver.BadgeNo));

                if (extendedInfo != null)
                {
                    driver.ExtendedInformationModel = extendedInfo;
                }
            }
            return Drivers;
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
            string key = CargoSupport.Constants.SoapApi.GetApiKey();

            var fromDate = "2019-08-25";
            var toDate = "2019-08-25";

            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(@$"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:uri=""uri:FlexForce""><soapenv:Header/><soapenv:Body><uri:wsdlGetSchedulesV2 soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><apiKey>{key}</apiKey><getSchedulesV2Request xsi:type=""flex:getSchedulesV2Request"" xmlns:flex=""http://qwfm/soap/FlexForce""><fromDate xsi:type=""xsd:string"">{fromDate}</fromDate><fromTime xsi:type=""xsd:string"">00:00:00</fromTime><toDate xsi:type=""xsd:string"">{toDate}</toDate><toTime xsi:type=""xsd:string"">23:59:59</toTime><scheduledShifts xsi:type=""xsd:boolean"">true</scheduledShifts><absenceShifts xsi:type=""xsd:boolean"">false</absenceShifts><allUnits xsi:type=""xsd:boolean"">false</allUnits><includeCosts xsi:type=""xsd:boolean"">false</includeCosts></getSchedulesV2Request></uri:wsdlGetSchedulesV2></soapenv:Body></soapenv:Envelope>");
            return soapEnvelopeDocument;
        }

        private static XmlDocument CreateSoapGetAllDriversEnvelope()
        {
            string key = CargoSupport.Constants.SoapApi.GetApiKey();

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