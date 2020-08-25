﻿using CargoSupport.Web.Models.QuinyxModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using CargoSupport.Enums;
using CargoSupport.Web.Models.DatabaseModels;
using CargoSupport.Web.Extensions;

namespace CargoSupport.Helpers
{
    public class QuinyxHelper
    {
        public List<QuinyxModel> GetAllDriversSorted(DateTime date, bool clearNames = true)
        {
            return GetDrivers(date, date, clearNames).OrderBy(e => e.begTime).ThenBy(e => e.endTime).ToList();
        }

        public List<QuinyxModel> GetDrivers(DateTime from, DateTime to, bool clearNames = true)
        {
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(from, to);
            HttpWebRequest webRequest = CreateWebRequest(Constants.SoapApi.Connection);
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
                        Id = (int)y.Elements().Where(z => z.Name.LocalName == "persId").FirstOrDefault(),
                        BadgeNo = (string)y.Elements().Where(z => z.Name.LocalName == "badgeNo").FirstOrDefault(),
                        begTimeString = (string)y.Elements().Where(z => z.Name.LocalName == "begTime").FirstOrDefault(),
                        endTimeString = (string)y.Elements().Where(z => z.Name.LocalName == "endTime").FirstOrDefault(),
                        CategoryId = (int)y.Elements().Where(z => z.Name.LocalName == "categoryId").FirstOrDefault(),
                        Section = (int)y.Elements().Where(z => z.Name.LocalName == "section").FirstOrDefault(),
                        SectionName = (string)y.Elements().Where(z => z.Name.LocalName == "sectionName").FirstOrDefault(),
                        hours = (decimal)y.Elements().Where(z => z.Name.LocalName == "hours").FirstOrDefault(),
                        CostCentre = (int)y.Elements().Where(z => z.Name.LocalName == "costCentre").FirstOrDefault(),
                        ManagerId = (int)y.Elements().Where(z => z.Name.LocalName == "managerId").FirstOrDefault(),
                    }).ToList();
                }

                result = result.Select(res => res).Where(res => res.Id != 0).ToList();

                result = GetExtraInformationForDrivers(result);

                //Remove non-drivers
                for (int i = result.Count - 1; i >= 0; i--)
                {
                    if (result[i].ExtendedInformationModel.Active == 0 ||
                        CargoSupport.Helpers.DataConversionHelper.GetQuinyxEnum(result[i].CategoryId) != QuinyxRole.Driver)
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
                    if (clearNames)
                    {
                        result[i].ExtendedInformationModel.GivenName = "";
                        result[i].ExtendedInformationModel.FamilyName = "";
                    }
                }
                return result;
            }
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

            foreach (var driver in Drivers)
            {
                var extendedInfo = extendedInformation.FirstOrDefault(info => info.Id.Equals(driver.Id));

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