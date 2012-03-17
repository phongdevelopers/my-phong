using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace CommerceBuilder.Utility
{
    public class UrlHelper
    {
        /// <summary>
        /// Gets the url relative to the application root
        /// </summary>
        /// <param name="url">The absolute url</param>
        /// <param name="appPath">The application path</param>
        /// <returns>The url relative to the application root</returns>
        public static string GetAppRelativeUrl(string url, string appPath)
        {
            if (!appPath.EndsWith("/")) appPath += "/";
            if (url.StartsWith(appPath)) url = url.Substring(appPath.Length);
            int qIndex = url.IndexOf("?");
            if (qIndex > -1) url = url.Substring(0, qIndex);
            return url;
        }

        /// <summary>
        /// Gets the query portion of the url
        /// </summary>
        /// <param name="url">The url</param>
        /// <returns>The query portion of the url if it exists</returns>
        public static string GetQueryString(string url)
        {
            if (url == null) return string.Empty;
            int qIndex = url.IndexOf("?");
            if (qIndex > -1) return url.Substring(qIndex + 1);
            return string.Empty;
        }

        /// <summary>
        /// Given a url, extracts the domain
        /// </summary>
        /// <param name="url">The url to parse</param>
        /// <returns>The domain name</returns>
        public static string GetDomainFromUrl(string url)
        {
            try
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                    url = "http://" + url;
                Uri uri = new Uri(url);
                return uri.Authority;
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// Given a url or domain, return the second (and first) level domain.
        /// </summary>
        /// <param name="url">The url (or domain name) to parse.</param>
        /// <returns>The second and top level domain portion.</returns>
        public static string GetSecondLevelDomainFromUrl(string url)
        {
            string tempDomain = GetDomainFromUrl(url);
            if (string.IsNullOrEmpty(tempDomain)) return string.Empty;
            bool isIP = Regex.IsMatch(tempDomain, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");
            if (isIP) return tempDomain;
            string[] domainParts = tempDomain.Split('.');
            if (domainParts.Length < 2) return tempDomain;
            return domainParts[domainParts.Length - 2] + "." + domainParts[domainParts.Length - 1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webConfigContent"></param>
        /// <param name="cookieDomain"></param>
        /// <returns></returns>
        public static void UpdateWebConfigCookieDomain(string cookieDomain)
        {
            // LOCATE WEB CONFIG
            string webConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "web.config");
            if (File.Exists(webConfigPath))
            {
                // TRACK REPLACEMENTS
                Dictionary<string, string> configReplacements = new Dictionary<string, string>();

                // READ CONTENT
                XmlDocument webConfigXmlDocument = new XmlDocument();
                webConfigXmlDocument.Load(webConfigPath);

                // LOOP NODES LOOKING FOR DOMAINS
                string[] xPathNodes = new string[] { "system.web/authentication/forms", "system.web/anonymousIdentification" };
                foreach (string xPath in xPathNodes)
                {
                    // LOCATE THIS ELEMENT
                    XmlElement element = XmlUtility.GetElement(webConfigXmlDocument.DocumentElement, xPath, false);
                    if (element != null)
                    {
                        // CHECK THE CURRENT COOKIE DOMAIN
                        string currentDomain = XmlUtility.GetAttributeValue(element, "domain", string.Empty);
                        if (currentDomain != cookieDomain)
                        {
                            // DOMAIN MISMATCH, WE MUST UPDATE THIS NODE
                            string xmlBefore = element.OuterXml;
                            if (string.IsNullOrEmpty(cookieDomain))
                            {
                                // WE MUST REMOVE THE DOMAIN
                                element.RemoveAttribute("domain");
                            }
                            else
                            {
                                // WE MUST UPDATE THE NODE
                                element.SetAttribute("domain", cookieDomain);
                            }
                            string xmlAfter = element.OuterXml;
                            if (xmlBefore != xmlAfter) configReplacements[xmlBefore] = xmlAfter;
                        }
                    }
                }

                // ONLY SAVE CHANGES IF WE FOUND SOMETHING TO FIX
                if (configReplacements.Count > 0)
                {
                    // GET RAW WEB.CONFIG CONTENT
                    string webConfigContent = File.ReadAllText(webConfigPath);
                    foreach (string findValue in configReplacements.Keys)
                    {
                        // MAKE REPLACEMENTS, MINIFYING ANY CHANGES TO WHITESPACE ETC.
                        webConfigContent = webConfigContent.Replace(findValue, configReplacements[findValue]);
                    }

                    // SAVE THE WEB.CONFIG
                    File.WriteAllText(webConfigPath, webConfigContent);
                }
            }
        }
    }
}