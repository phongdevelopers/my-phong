using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Stores.ForexProviders.ECB
{
    public class ForexProvider : ForexProviderBase
    {
        private const string ECB_URL = "http://www.ecb.int/stats/eurofxref/eurofxref-daily.xml";

        public override string Name
        {
            get { return "European Central Bank"; }
        }

        public override LSDecimal GetExchangeRate(string sourceCurrency, string targetCurrency)
        {
            //VALIDATE INPUTS
            if (sourceCurrency == null) throw new ArgumentNullException("sourceCurrency");
            if (targetCurrency == null) throw new ArgumentNullException("targetCurrency");
            sourceCurrency = sourceCurrency.Trim().ToUpperInvariant();
            targetCurrency = targetCurrency.Trim().ToUpperInvariant();
            if (sourceCurrency.Length != 3) throw new ArgumentException("sourceCurrency must be 3 letter ISO code");
            if (targetCurrency.Length != 3) throw new ArgumentException("targetCurrency must be 3 letter ISO code");
            //MAKE SURE LATEST FOREX DATA IS AVAILABLE
            XmlDocument forexData = GetForexFile();
            if (forexData != null)
            {
                //GET CONVERSION RATES
                Decimal sourceFactor = GetEuroRate(forexData, sourceCurrency);
                if (sourceFactor == 0)
                    throw new ArgumentException("Cannot convert from " + sourceCurrency + " to EUR.", "sourceCurrency");
                Decimal targetFactor = GetEuroRate(forexData, targetCurrency);
                if (targetFactor == 0)
                    throw new ArgumentException("Cannot convert from " + targetCurrency + " to EUR.", "targetCurrency");
                return (LSDecimal)(targetFactor / sourceFactor);
            }
            return 0;
        }

        private static XmlDocument GetForexFile()
        {
            bool downloadForexData = true;
            string forexFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\forex-ecb.xml");
            if (File.Exists(forexFileName))
            {
                FileInfo forexFileInfo = new FileInfo(forexFileName);
                TimeSpan elapsedTime = DateTime.UtcNow.Subtract(forexFileInfo.LastWriteTimeUtc);
                //UPDATE IF FOREX DATA IS AT LEAST 12 HOURS OLD
                downloadForexData = (elapsedTime.Hours >= 12);
            }
            if (downloadForexData)
            {
                //OBTAIN LATEST FOREX FILE
                try
                {
                    string forexDataXml = string.Empty;

                    //USE HTTPWEBREQUEST INSTEAD OF XMLDOCUMENT TO FETCH THE FILE
                    //SO WE CAN HANDLE TIMEOUT
                    HttpWebRequest ecbRequest = (HttpWebRequest)WebRequest.Create(ECB_URL);
                    ecbRequest.Timeout = 45000;

                    using (HttpWebResponse ecbResponse = (HttpWebResponse)ecbRequest.GetResponse())
                    {
                        Encoding enc = System.Text.Encoding.UTF8;
                        using (StreamReader sr = new StreamReader(ecbResponse.GetResponseStream(), enc))
                        {
                            forexDataXml = sr.ReadToEnd();
                            sr.Close();
                        }
                        ecbResponse.Close();
                    }

                    //SAVE THE FILE DATA
                    if (!string.IsNullOrEmpty(forexDataXml))
                    {
                        //STRIP OUT JUST THE CUBE DATA
                        int startAt = forexDataXml.IndexOf("<Cube>");
                        int endAt = forexDataXml.LastIndexOf("</Cube>");
                        int length = (endAt - startAt) + 7;
                        string newXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + forexDataXml.Substring(startAt, length);
                        File.WriteAllText(forexFileName, newXml);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Could not download forex data from " + ECB_URL, ex);
                }
            }
            XmlDocument forexData = new XmlDocument();
            forexData.Load(forexFileName);
            return forexData;
        }

        /// <summary>
        /// Gets the rate to convert from EUR to the target currency (1 EUR = ? target)
        /// </summary>
        /// <param name="forexData">XML document containing NY Federal Reserve noon rates.</param>
        /// <param name="targetCurrency">3 letter ISO code to get rate for</param>
        /// <returns>Exchange rate from EUR to the target currency (1 EUR = ? target)</returns>
        private static Decimal GetEuroRate(XmlDocument forexData, string targetCurrency)
        {
            //SHORTCUT FOR EUR
            if (targetCurrency == "EUR") return 1;
            //LOOK FOR THE CURRENCY
            XmlNode cubeNode = forexData.DocumentElement.SelectSingleNode("Cube/Cube[@currency='" + targetCurrency + "']");
            if (cubeNode != null)
            {
                //CONVERSION RATE FOUND
                return (decimal)AlwaysConvert.ToDecimal(cubeNode.Attributes["rate"].Value);
            }
            //RATE NOT FOUND, RETURN 0
            return 0;
        }

    }
}
