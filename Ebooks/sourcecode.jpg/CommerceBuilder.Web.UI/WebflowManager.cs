//-----------------------------------------------------------------------
// <copyright file="WebflowManager.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Caching;
    using CommerceBuilder.Exceptions;
    using CommerceBuilder.Personalization;
    using CommerceBuilder.Stores;
    using CommerceBuilder.Utility;

    /// <summary>
    /// Methods to help determine appropriate web flow or resource location based
    /// on AbleCommerce business rules and configuration of active web application.
    /// </summary>
    public static class WebflowManager
    {
        /// <summary>
        /// Object used to synchronize updates to the shared application cache
        /// </summary>
        private static readonly object syncLock = new object();

        /// <summary>
        /// Key that should be used for cache of one page checkout indicator.
        /// </summary>
        private static string onePageCheckoutIndicatorCacheKey = "OnePageCheckoutIndicator";

        /// <summary>
        /// Regular expression used to parse content attribute from scriptlet part tag
        /// </summary>
        private static Regex parseContentAttributeExpression = new Regex(@"<cb:ScriptletPart [^>]*Content=""([^""]*)""", RegexOptions.IgnoreCase);
        
        /// <summary>
        /// Determines whether the configured web application is using one page checkout.
        /// </summary>
        /// <returns>True if one page checkout can be identified, false otherwise.</returns>
        public static bool IsUsingOnePageCheckout()
        {
            // THIS METHOD CAN ONLY WORK IF WE HAVE A VALID HTTP CONTEXT
            HttpContext context = HttpContext.Current;
            if (context == null)
            {
                throw new CommerceBuilderException("This method can only be called within an HTTP context.");
            }

            // CHECK FOR CACHED SETTING
            if (context.Cache[onePageCheckoutIndicatorCacheKey] == null)
            {
                // DETERMINE THE PRESENT VALUE FOR THE ONE PAGE CHECKOUT INDICATOR
                bool indicator;

                // IF WE IDENTIFY A SCRIPTLET IN USE, TRACK IT SO WE CAN SET UP CACHE DEPENDENCIES
                string scriptletName = string.Empty;
                string themeName = string.Empty;

                // SEE IF THE EXPECTED CHECKOUT SCRIPT FILE CAN BE FOUND
                string checkoutFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Checkout\\Default.aspx");
                if (File.Exists(checkoutFilePath))
                {
                    SharedPersonalization sp = SharedPersonalizationDataSource.LoadForPath("~/Checkout/Default.aspx", false);
                    themeName = GetThemeFromPersonalization(sp);
                    
                    // GET THE CONTENT SCRIPTLET CURRENTLY IN USE
                    scriptletName = GetContentScriptletFromPersonalization(sp, themeName);
                    if (string.IsNullOrEmpty(scriptletName))
                    {
                        scriptletName = GetDefaultContentScriptlet(checkoutFilePath);
                    }

                    // WAS A CONTENT SCRIPTLET IDENTIFIED?
                    if (!string.IsNullOrEmpty(scriptletName))
                    {
                        // YES, CHECK THE CONFIGURED CONTENT SCRIPTLET FOR THE ONE PAGE CHECKOUT CONTROL
                        indicator = IsOnePageCheckoutSetInScriptlet(themeName, scriptletName);
                    }
                    else
                    {
                        // NO, SCRIPTLET NOT FOUND SO PRESUME ONE PAGE CHECKOUT IS NOT IN USE
                        indicator = false;
                    }
                }
                else
                {
                    // THE EXPECTED CHECKOUT SCRIPT IS NOT PRESENT, SO PRESUME ONE PAGE CHECKOUT IS NOT IN USE
                    indicator = false;
                }

                // BUILD A LIST OF FILE DEPENDENCIES FOR THE CACHE INDICATOR
                List<string> fileDependencies = new List<string>();

                // WE NEED TO KNOW IF THE EXPECTED CHECKOUT SCRIPT CHANGES
                fileDependencies.Add(checkoutFilePath);

                // IF A SCRIPTLET WAS IDENTIFIED, WE SHOULD MONITOR FOR CHANGES
                if (!string.IsNullOrEmpty(scriptletName))
                {
                    // OBTAIN BASE APPLICATION DIRECTORY
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    // BOTH DEFAULT AND CUSTOM SCRIPTLET SHOULD BE MONITORED
                    if (ThemeHasScriptlets(themeName))
                    {
                        fileDependencies.Add(Path.Combine(baseDirectory, "App_Themes\\" + themeName + "\\Scriptlets\\Default\\Content\\" + scriptletName + ".htm"));
                        fileDependencies.Add(Path.Combine(baseDirectory, "App_Themes\\" + themeName + "\\Scriptlets\\Custom\\Content\\" + scriptletName + ".htm"));
                    }
                    else
                    {
                        fileDependencies.Add(Path.Combine(baseDirectory, "App_Data\\Scriptlets\\Default\\Content\\" + scriptletName + ".htm"));
                        fileDependencies.Add(Path.Combine(baseDirectory, "App_Data\\Scriptlets\\Custom\\Content\\" + scriptletName + ".htm"));
                    }
                }

                // CREATE THE CACHE DEPENDENCY WITH LIST OF FILE PATHS
                CacheDependency fileDependency = new CacheDependency(fileDependencies.ToArray());

                // CACHE THE INDICATOR
                lock (syncLock)
                {
                    context.Cache.Remove(onePageCheckoutIndicatorCacheKey);
                    context.Cache.Add(onePageCheckoutIndicatorCacheKey, indicator, fileDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                }
            }

            // RETURN THE CACHED INDICATOR
            return AlwaysConvert.ToBool(context.Cache[onePageCheckoutIndicatorCacheKey], false);
        }

        private static bool ThemeHasScriptlets(string themeName)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrEmpty(themeName)) return false;
            if (Directory.Exists(Path.Combine(baseDirectory, "App_Themes\\" + themeName + "\\Scriptlets")))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses shared personalization for the given virtual path to obtain the applied content scriptlet
        /// </summary>
        /// <param name="virtualPath">The virtual path to check for personalization data</param>
        /// <returns>The name of the content scriptlet applied in personalization data, or string.Empty
        /// if the value could not be determined</returns>
        private static string GetContentScriptletFromPersonalization(SharedPersonalization sp, string themeName)
        {
            // LOOK FOR SHARED PERSONALIZATION SET ON THE GIVEN VIRTUAL PATH
            if (sp != null)
            {
                // DECODE THE PERSONALIZATION BLOB
                Dictionary<string, PersonalizationInfo> props = sp.DecodePageSettings();

                // LOOP THE KEYS FOR THE PERSONALIZED PROPERTIES
                foreach (string key in props.Keys)
                {
                    // SEE IF THIS PERSONALIZATION DATA APPLIES TO SCRIPTLETPART
                    PersonalizationInfo pinfo = props[key];
                    if (pinfo.ControlType.Name == "ScriptletPart")
                    {
                        // SEE IF THERE IS A VALUE SET FOR THE CONTENT SCRIPTLET
                        if (pinfo.Properties.ContainsKey("Content"))
                        {
                            // TRY TO LOAD THE SPECIFIED CONTENT SCRIPTLET
                            string contentScriptlet = pinfo.Properties["Content"].ToString();
                            //Scriptlet s = ScriptletDataSource.Load(contentScriptlet, ScriptletType.Content);                            
                            Scriptlet s = ScriptletDataSource.Load(themeName, contentScriptlet, ScriptletType.Content);
                            if (s != null)
                            {
                                return s.Identifier;
                            }
                        }
                    }
                }
            }

            // EITHER SHARED PERSONALIZATION WAS NOT SET, CONTENT SCRIPTLET SETTING IS UNAVAILABLE,
            // OR THE SPECIFIED CONTENT SCRIPTLET IS INVALID
            return string.Empty;
        }

        private static string GetThemeFromPersonalization(SharedPersonalization sp)
        {            
            string theme = string.Empty;
            
            //GET THE DEFAULT STORE THEME FIRST
            Store store = CommerceBuilder.Common.Token.Instance.Store;
            if (store != null)
            {
                theme = store.Settings.StoreTheme;
                if (!string.IsNullOrEmpty(theme) && !CommerceBuilder.UI.Styles.Theme.Exists(theme))
                {
                    //INVALID THEME SELECTED
                    theme = string.Empty;
                }
            }

            //TRY TO GET THE THEME FROM PERSONALIZATION
            if (sp != null)
            {
                if ((sp.Theme != string.Empty) && (CommerceBuilder.UI.Styles.Theme.Exists(sp.Theme)))
                {
                    theme = sp.Theme;
                }
            }
            return theme;
        }

        /// <summary>
        /// Parses the file for the content scriptlet attribute value
        /// </summary>
        /// <param name="scriptPath">The fully qualified physical path to the file to parse</param>
        /// <returns>The name of the default content scriptlet, or empty string if the value cannot be 
        /// determined</returns>
        private static string GetDefaultContentScriptlet(string scriptPath)
        {
            // ENSURE THE SPECIFIED SCRIPT FILE CAN BE FOUND
            if (File.Exists(scriptPath))
            {
                // NEED TO LOAD THE FILE CONTENT FOR REGEX PARSING
                string allText = File.ReadAllText(scriptPath);

                // APPLY REGULAR EXPRESSION TO LOOK FOR CONTENT ATTRIBUTE
                Match match = parseContentAttributeExpression.Match(allText);
                if (match.Success)
                {
                    // GROUP 0 IS THE ENTIRE MATCH, GROUP 1 IS THE VALUE OF THE CONTENT ATTRIBUTE
                    return match.Groups[1].Value;
                }
            }

            // IF WE GET THIS FAR, WE COULD NOT IDENTIFY THE DEFAULT CONTENT SCRIPTELT
            return string.Empty;
        }

        /// <summary>
        /// Scans a content scriptlet to determine if it contains reference to a OnePageCheckout control
        /// </summary>
        /// <param name="identifier">The identifier of the content scriptlet</param>
        /// <returns>True if the one page checkout control is discovered in the specified scriptlet</returns>
        private static bool IsOnePageCheckoutSetInScriptlet(string themeName, string identifier)
        {
            // TRY TO LOAD THE SPECIFIED SCRIPTLET

            Scriptlet s = ScriptletDataSource.Load(themeName, identifier, ScriptletType.Content);
            if (s != null)
            {
                // SEE IF SCRIPTLET CONTAINS A REFERENCE TO ONE PAGE CHECKOUT CONTROL
                // IF IT HAS ONE PAGE CHECKOUT, RETURN 1 (TRUE) ELSE 0 (FALSE)
                return s.ScriptletData.Contains("[[ConLib:OnePageCheckout");
            }
            else
            {
                // THE SCRIPTLET COULD NOT BE LOADED, ONE PAGE CHECKOUT IDENTIFIER NOT PRESENT
                return false;
            }
        }

        /// <summary>
        /// Forces any cached data to be refreshed
        /// </summary>
        public static void ClearCache()
        {
            // THIS METHOD CAN ONLY WORK IF WE HAVE A VALID HTTP CONTEXT
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                // REMOVE CACHED ONE PAGE INDICATOR
                context.Cache.Remove(onePageCheckoutIndicatorCacheKey);
            }
        }
    }
}