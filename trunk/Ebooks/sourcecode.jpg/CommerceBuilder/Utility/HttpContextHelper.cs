//-----------------------------------------------------------------------
// <copyright file="HttpContextHelper.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using System.Web;
    using System.Web.Configuration;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper class to access common elements of the HttpContext
    /// </summary>
    public static class HttpContextHelper
    {
        /// <summary>
        /// Gets the HttpRequest associated with the current HttpContext.
        /// </summary>
        /// <returns>The HttpRequest object, or null if it is not available in this context.</returns>
        /// <remarks>In II7 in integrated pipeline mode, accessing request will throw an exception 
        /// during application level events (like AbleCommerceHttpModule.Init).</remarks>
        public static HttpRequest SafeGetRequest()
        {
            return SafeGetRequest(HttpContext.Current);
        }

        /// <summary>
        /// Gets the HttpRequest associated with the given context.
        /// </summary>
        /// <param name="context">The HttpContext to retreive the request from.</param>
        /// <returns>The HttpRequest object, or null if it is not available in this context.</returns>
        /// <remarks>In II7 in integrated pipeline mode, accessing request will throw an exception 
        /// during application level events (like AbleCommerceHttpModule.Init).</remarks>
        public static HttpRequest SafeGetRequest(HttpContext context)
        {
            if (context == null) return null;
            try
            {
                HttpRequest request = context.Request;
                return request;
            }
            catch
            {
                // IGNORE ANY ERRORS
            }

            // RETURN NULL IF REFERENCE CANNOT BE OBTAINED
            return null;
        }

        /// <summary>
        /// Gets the HttpResponse associated with the given context.
        /// </summary>
        /// <param name="context">The HttpContext to retreive the response from.</param>
        /// <returns>The HttpResponse object, or null if it is not available in this context.</returns>
        /// <remarks>In II7 in integrated pipeline mode, accessing response will throw an exception 
        /// during application level events (like AbleCommerceHttpModule.Init).</remarks>
        public static HttpResponse SafeGetResponse(HttpContext context)
        {
            if (context == null) return null;
            try
            {
                HttpResponse response = context.Response;
                return response;
            }
            catch
            {
                // IGNORE ANY ERRORS
            }

            // RETURN NULL IF REFERENCE CANNOT BE OBTAINED
            return null;
        }

        /// <summary>
        /// Gets the System.Web.Caching.Cache object associated with the current context.
        /// </summary>
        /// <returns>The System.Web.Caching.Cache object associated with the current context.</returns>
        public static System.Web.Caching.Cache SafeGetCache()
        {
            HttpContext context = HttpContext.Current;
            if (context == null) return null;
            try
            {
                System.Web.Caching.Cache cache = context.Cache;
                return cache;
            }
            catch
            {
                // IGNORE ANY ERRORS
            }

            // RETURN NULL IF REFERENCE CANNOT BE OBTAINED
            return null;
        }

        /// <summary>
        /// Gets the System.Web.SessionState.HttpSessionState object associated with the current context.
        /// </summary>
        /// <returns>The System.Web.SessionState.HttpSessionState object associated with the current context.</returns>
        public static System.Web.SessionState.HttpSessionState SafeGetSession()
        {
            HttpContext context = HttpContext.Current;
            if (context == null) return null;
            return context.Session;
        }

        /// <summary>
        /// Attempts to get the name of the currently executing web script
        /// </summary>
        /// <returns>The current script name, or string.Empty if script cannot be determined</returns>
        public static string GetCurrentScriptName()
        {
            // GET THE REQUEST, IGNORE ANY FAILURE
            HttpRequest request = SafeGetRequest();

            // SEE IF WE FOUND A REQUEST OBJECT
            if (request != null)
            {
                // GET THE SCRIPTNAME FROM SERVER
                string scriptName = request.ServerVariables["SCRIPT_NAME"];
                if (scriptName != null)
                {
                    return scriptName;
                }
            }

            // IF SCRIPT NAME IS NOT FOUND, RETURN EMPTY
            return string.Empty;
        }

        /// <summary>
        /// Indicates whether the current request can be identified as an installation script
        /// </summary>
        /// <returns>True if the request is being made to the install directory, false otherwise.</returns>
        public static bool IsInstallRequest()
        {
            HttpRequest request = HttpContextHelper.SafeGetRequest();
            if (request != null)
            {
                // OBTAIN THE SCRIPT PATH FROM THE URL
                string absolutePath = request.Url.AbsolutePath.ToLowerInvariant();
                string relativePath;
                if (request.ApplicationPath.Length > 1) relativePath = absolutePath.Substring(request.ApplicationPath.Length);
                else relativePath = absolutePath;

                // SEE IF THIS IS AN INSTALL SCRIPT
                if (relativePath.StartsWith("/install/")) return true;
            }

            // AN INSTALL REQUEST WAS NOT DETECTED
            return false;
        }

        /// <summary>
        /// Returns the current ASPNET trust level
        /// </summary>
        /// <returns>The current ASPNET trust level</returns>
        public static AspNetHostingPermissionLevel AspNetTrustLevel
        {
            get
            {
                AspNetHostingPermissionLevel[] trustLevels = new AspNetHostingPermissionLevel[] {
                        AspNetHostingPermissionLevel.Unrestricted,
                        AspNetHostingPermissionLevel.High,
                        AspNetHostingPermissionLevel.Medium,
                        AspNetHostingPermissionLevel.Low,
                        AspNetHostingPermissionLevel.Minimal 
                    };
                foreach (AspNetHostingPermissionLevel trustLevel in trustLevels)
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                    }
                    catch (System.Security.SecurityException)
                    {
                        continue;
                    }
                    return trustLevel;
                }
                return AspNetHostingPermissionLevel.None;
            }
        }

        /// <summary>
        /// Parse the server software identifier and attempt to determine the major/minor version.
        /// </summary>
        /// <param name="serverSoftware">The server reported value of HttpRequest.ServerVariables["SERVER_SOFTWARE"].</param>
        /// <returns>Returns IIS version.</returns>
        public static double GetIISVersion(string serverSoftware)
        {
            if (string.IsNullOrEmpty(serverSoftware))
                return 0;
            Match match = IISServerExpression.Match(serverSoftware);
            if (match.Success)
            {
                return AlwaysConvert.ToDouble(match.Groups[1].Value);
            }
            return 0d;
        }

        private static Regex IISServerExpression = new Regex("^Microsoft-IIS/([0-9]\\.[0-9])$", RegexOptions.Compiled);
    }
}