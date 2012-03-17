using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Utility class to support tracing
    /// </summary>
    public static class WebTrace
    {
        /// <summary>
        /// Writes a trace message
        /// </summary>
        /// <param name="message">Message to write</param>
        public static void Write(string message)
        {
            Write(String.Empty, message);
        }

        /// <summary>
        /// Writes a trace message
        /// </summary>
        /// <param name="category">Message category</param>
        /// <param name="message">Message to write</param>
        public static void Write(string category, string message)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Trace.Write(category, message);
            }
        }

        /// <summary>
        /// Gets TraceContext object of the current HttpContext 
        /// </summary>
        /// <returns>TraceContext object</returns>
        public static TraceContext GetTraceContext()
        {
            HttpContext context = HttpContext.Current;
            if (context != null) return context.Trace;
            return TraceSimulator.Get();
        }

        /// <summary>
        /// Allows us to get a dummy trace context
        /// From http://www.trainedchimpanzeeband.com/explodedclown/2007/05/01/simulating-httpcontext-with-sessions-and-posts-for-aspnet
        /// </summary>
        private static class TraceSimulator
        {
            public static TraceContext Get()
            {
                SimulatedHttpRequest req = new SimulatedHttpRequest("GET", "localhost", string.Empty, string.Empty);
                HttpContext context = new HttpContext(req);
                return context.Trace;
            }

            /// <summary>
            /// Used to simulate an HttpRequest.
            /// </summary>
            private class SimulatedHttpRequest : SimpleWorkerRequest
            {
                private string verb;
                private string host;
                private string body = string.Empty;
                public string Body { get { return body; } set { body = value; } }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="verb"></param>
                /// <param name="host"></param>
                /// <param name="page"></param>
                /// <param name="query"></param>
                public SimulatedHttpRequest(
                    string verb,
                    string host,
                    string page,
                    string query)
                    : this("/", AppDomain.CurrentDomain.BaseDirectory, verb, host, page, query, null)
                { }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="appVirtualDir"></param>
                /// <param name="appPhysicalDir"></param>
                /// <param name="verb"></param>
                /// <param name="host"></param>
                /// <param name="page"></param>
                /// <param name="query"></param>
                /// <param name="output"></param>
                public SimulatedHttpRequest(
                    string appVirtualDir,
                    string appPhysicalDir,
                    string verb,
                    string host,
                    string page,
                    string query,
                    TextWriter output)
                    : base(appVirtualDir, appPhysicalDir, page, query, output)
                {
                    if (string.IsNullOrEmpty(verb))
                        throw new ArgumentNullException("verb", "Verb cannot be null nor empty.");
                    if (string.IsNullOrEmpty(host))
                        throw new ArgumentNullException("host", "Host cannot be null nor empty.");

                    this.verb = verb;
                    this.host = host;
                }

                /// <summary>
                /// Gets the name of the server.
                /// </summary>
                /// <returns></returns>
                public override string GetServerName()
                {
                    return host;
                }

                /// <summary>
                /// Maps the path to a filesystem path.
                /// </summary>
                /// <param name="virtualPath">Virtual path.</param>
                /// <returns></returns>
                public override string MapPath(string virtualPath)
                {
                    return Path.Combine(this.GetAppPath(), virtualPath);
                }

                /// <summary>
                /// 
                /// </summary>
                /// <returns></returns>
                public override string GetHttpVerbName()
                {
                    return verb;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <returns></returns>
                public override byte[] GetPreloadedEntityBody()
                {
                    return new UTF8Encoding().GetBytes(Body);
                }

                /// <summary>
                /// 
                /// </summary>
                /// <returns></returns>
                public override int GetPreloadedEntityBodyLength()
                {
                    return GetPreloadedEntityBody().Length;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="index"></param>
                /// <returns></returns>
                public override string GetKnownRequestHeader(int index)
                {
                    bool isPost = verb == "POST";
                    if (!isPost) return base.GetKnownRequestHeader(index);

                    if (index == HeaderContentType)
                    {
                        return "application/x-www-form-urlencoded; charset=utf-8";
                    }
                    else if (index == HeaderContentLength)
                    {
                        return GetPreloadedEntityBodyLength().ToString();
                    }

                    return base.GetKnownRequestHeader(index);
                }
            }
        }
    }
}
