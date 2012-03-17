using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Stores.ForexProviders.BankOfCanada
{
    public class ForexProvider : ForexProviderBase
    {
        private const string BOC_URL = "http://www.bankofcanada.ca/rss/fx/noon/fx-noon-all.xml";

        public override string Name
        {
            get { return "Bank of Canada"; }
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
                Decimal sourceFactor = GetCADRate(forexData, sourceCurrency);
                if (sourceFactor == 0)
                    throw new ArgumentException("Cannot convert from " + sourceCurrency + " to CAD.", "sourceCurrency");
                Decimal targetFactor = GetCADRate(forexData, targetCurrency);
                if (targetFactor == 0)
                    throw new ArgumentException("Cannot convert from " + targetCurrency + " to CAD.", "targetCurrency");
                return (LSDecimal)(targetFactor / sourceFactor);
            }
            return 0;
        }

        private static XmlDocument GetForexFile()
        {
            bool downloadForexData = true;
            string forexFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\forex-bankofcanada.xml");
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
                    HttpWebRequest rssRequest = (HttpWebRequest)WebRequest.Create(BOC_URL);
                    rssRequest.Timeout = 45000;

                    using (HttpWebResponse rssResponse = (HttpWebResponse)rssRequest.GetResponse())
                    {
                        Encoding enc = System.Text.Encoding.UTF8;
                        using (StreamReader sr = new StreamReader(rssResponse.GetResponseStream(), enc))
                        {
                            forexDataXml = sr.ReadToEnd();
                            sr.Close();
                        }
                        rssResponse.Close();
                    }

                    //SAVE THE FILE DATA
                    if (!string.IsNullOrEmpty(forexDataXml))
                    {
                        string parsedXml = ParseRssData(forexDataXml);
                        File.WriteAllText(forexFileName, parsedXml);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Could not download forex data from " + BOC_URL, ex);
                }
            }
            XmlDocument forexData = new XmlDocument();
            forexData.Load(forexFileName);
            return forexData;
        }

        /// <summary>
        /// Convert the RSS data into something simpler for use
        /// </summary>
        /// <param name="rssData">A string containing the xml data returned from the bank of canada rss feed</param>
        /// <returns>A string with simplified XML data for use in returning exchange rates</returns>
        private static string ParseRssData(string rssData)
        {
            StringBuilder forexData = new StringBuilder();
            forexData.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<rateItems baseCurrency=\"CAD\">\n");
            XmlDocument rssXml= new XmlDocument();
            rssXml.LoadXml(rssData);
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(rssXml.NameTable);
            nsMgr.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
            nsMgr.AddNamespace("cb", "http://centralbanks.org/cb/1.0/");
            nsMgr.AddNamespace("rss", "http://purl.org/rss/1.0/");

            XmlNodeList itemNodes = rssXml.SelectNodes("/rdf:RDF//rss:item", nsMgr);
            foreach (XmlNode item in itemNodes)
            {
                string baseCurrency = GetElementValue(item, "cb:baseCurrency", nsMgr);
                if (baseCurrency == "CAD")
                {
                    string targetCurrency = GetElementValue(item, "cb:targetCurrency", nsMgr);
                    string rate = GetElementValue(item, "cb:value", nsMgr);
                    if (!string.IsNullOrEmpty(targetCurrency) && !string.IsNullOrEmpty(rate))
                    {
                        forexData.Append("<rateItem targetCurrency=\"" + targetCurrency + "\" rate=\"" + rate + "\" />\n");
                    }
                }
            }
            forexData.Append("</rateItems>\n");
            return forexData.ToString();
        }

        private static string GetElementValue(XmlNode parentNode, string xPath, XmlNamespaceManager nsMgr)
        {
            XmlNode childNode = parentNode.SelectSingleNode(xPath, nsMgr);
            if (childNode != null) return childNode.InnerText;
            return string.Empty;
        }

        /// <summary>
        /// Gets the rate to convert from EUR to the target currency (1 EUR = ? target)
        /// </summary>
        /// <param name="forexData">XML document containing NY Federal Reserve noon rates.</param>
        /// <param name="targetCurrency">3 letter ISO code to get rate for</param>
        /// <returns>Exchange rate from EUR to the target currency (1 EUR = ? target)</returns>
        private static Decimal GetCADRate(XmlDocument forexData, string targetCurrency)
        {
            //SHORTCUT FOR EUR
            if (targetCurrency == "CAD") return 1;
            //LOOK FOR THE CURRENCY
            XmlNode rateNode = forexData.DocumentElement.SelectSingleNode("rateItem[@targetCurrency='" + targetCurrency + "']");
            if (rateNode != null)
            {
                //CONVERSION RATE FOUND
                return (decimal)AlwaysConvert.ToDecimal(rateNode.Attributes["rate"].Value);
            }
            //RATE NOT FOUND, RETURN 0
            return 0;
        }

    }
}
