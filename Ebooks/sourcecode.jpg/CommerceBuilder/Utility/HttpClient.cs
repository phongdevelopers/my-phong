using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace CommerceBuilder.Utility
{
    public static class HttpClient
    {
        /// <summary>
        /// Executs a GET request using the given URL and returns the response
        /// </summary>
        /// <param name="requestUrl">The URL to retrieve</param>
        /// <returns>Response from the server</returns>
        public static string DoGetRequest(string requestUrl)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            httpWebRequest.Method = "GET";
            using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()))
            {
                string responseData = responseStream.ReadToEnd();
                responseStream.Close();
                return responseData;
            }
        }
    }
}
