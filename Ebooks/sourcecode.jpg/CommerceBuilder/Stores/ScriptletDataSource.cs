using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Caching;
using CommerceBuilder.Common;
using System.ComponentModel;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// DataSource class for Scriptlet objects
    /// </summary>
    [DataObject(true)]
    public partial class ScriptletDataSource
    {
        private const string _CacheKey = "25210089AAAF4DB383230227EA1EA627";

        /// <summary>
        /// Clears the cache for loaded scriptlets for given theme
        /// </summary>
        /// <param name="themeId">Id of the theme for which to clear the loaded scriptlets</param>
        public static void ClearCache(string themeId)
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                context.Cache.Remove(_CacheKey + themeId);
            }
        }

        /// <summary>
        /// Deletes the given Scriptlet object from the database
        /// </summary>
        /// <param name="scriptlet">The Scriptlet object to delete</param>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(Scriptlet scriptlet)
        {
            return scriptlet.Delete();
        }

        /// <summary>
        /// Deletes a Scriptlet object from the database with given identifier and type
        /// </summary>
        /// <param name="themeId">Name/Identifier of the theme</param>
        /// <param name="identifier">Identifier of Scriptlet to delete</param>
        /// <param name="scriptletType">Type of of Scriptlet to delete</param>
        /// <param name="isCustom">Is the Scriptlet to be deleted a custom scriptlet?</param>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(string themeId, string identifier, ScriptletType scriptletType, bool isCustom)
        {
            Scriptlet s = Load(themeId, identifier, scriptletType, (isCustom ? BitFieldState.True : BitFieldState.False), false);
            if (s != null) return s.Delete();
            return false;
        }
                
        /// <summary>
        /// Loads a Scriptlet object with given identifier and type from the database
        /// </summary>
        /// <param name="themeId">Name/Identifier of the theme</param>
        /// <param name="identifier">Identifier of Scriptlet to load</param>
        /// <param name="scriptletType">Type of of Scriptlet to load</param>
        /// <returns>The Scriptlet object loaded</returns>
        public static Scriptlet Load(string themeId, string identifier, ScriptletType scriptletType)
        {
            return Load(themeId, identifier, scriptletType, BitFieldState.Any, true);
        }

        /// <summary>
        /// Loads a Scriptlet object with given identifier and type from the database
        /// </summary>
        /// <param name="themeId">Name/Identifier of the theme</param>
        /// <param name="identifier">Identifier of Scriptlet to load</param>
        /// <param name="scriptletType">Type of of Scriptlet to load</param>
        /// <param name="useCache">If <b>true</b> tries to load the object from cache first</param>
        /// <returns>The Scriptlet object loaded</returns>
        public static Scriptlet Load(string themeId, string identifier, ScriptletType scriptletType, bool useCache)
        {
            return Load(themeId, identifier, scriptletType, BitFieldState.Any, useCache);
        }

        /// <summary>
        /// Loads a Scriptlet object with given identifier and type from the database
        /// </summary>
        /// <param name="themeId">Name/Identifier of the theme</param>
        /// <param name="identifier">Identifier of Scriptlet to load</param>
        /// <param name="scriptletType">Type of of Scriptlet to load</param>
        /// <param name="custom">Is the Scriptlet to be loaded a custom scriptlet?</param>
        /// <param name="useCache">If <b>true</b> tries to load the object from cache first</param>
        /// <returns>The Scriptlet object loaded</returns>
        public static Scriptlet Load(string themeId, string identifier, ScriptletType scriptletType, BitFieldState custom, bool useCache)
        {
            if (useCache)
            {
                ScriptletCollection scriptlets = CacheLoad(themeId);
                int index = scriptlets.IndexOf(identifier, scriptletType, custom);
                if (index > -1) return scriptlets[index];
                return null;
            }
            //(DO NOT USE CACHE, LOAD FOR THE NAME)
            Scriptlet s = new Scriptlet();
            if (s.Load(themeId, identifier, scriptletType, custom)) return s;
            return null;
        }

        /// <summary>
        /// Retrieves all scriptlets for the current store form the application cache.
        /// </summary>
        /// <returns>A collection of scriplets for the current store.</returns>
        public static ScriptletCollection CacheLoad(string themeId)
        {
            return GetCachedScriptlets(themeId);
        }

        /// <summary>
        /// Loads a collection of Scriptlet Objects for the given theme and given type from the cache
        /// </summary>
        /// <param name="scriptletType">Type of scriptlets to load</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <param name="themeId">Theme for which to load the scriptlets</param>
        /// <returns>A collection of Scriptlet objects</returns>
        public static ScriptletCollection CacheLoad(string themeId, ScriptletType scriptletType, string sortExpression)
        {
            ScriptletCollection scriptlets = GetCachedScriptlets(themeId);
            ScriptletCollection subset = new ScriptletCollection();
            foreach (Scriptlet item in scriptlets)
            {
                if ((scriptletType == ScriptletType.Unspecified) || (item.ScriptletType == scriptletType)) subset.Add(item);
            }
            if (sortExpression != string.Empty)
            {
                subset.Sort(sortExpression);
            }
            return subset;
        }

        private static string[] GetScriptletDirectories(string rootDirectory)
        {
            List<string> directories = new List<string>();
            directories.Add(rootDirectory);
            DirectoryInfo di = new DirectoryInfo(rootDirectory);
            if (di.Exists)
            {
                foreach (DirectoryInfo sdi in di.GetDirectories())
                {
                    if (sdi.Name != ".svn")
                    {
                        string[] subdirs = GetScriptletDirectories(sdi.FullName);
                        if ((subdirs != null) && (subdirs.Length > 0)) directories.AddRange(subdirs);
                    }
                }
            }
            return directories.ToArray();
        }

        private static ScriptletCollection GetCachedScriptlets()
        {
            return GetCachedScriptlets(string.Empty);
        }

        private static ScriptletCollection GetCachedScriptlets(string themeId)
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            HttpContext context = HttpContext.Current;
            ScriptletCollection scriptlets;
            if (themeId == null) themeId = string.Empty;

            string pathPart;
            if (string.IsNullOrEmpty(themeId))
            {
                pathPart = "App_Data\\Scriptlets";
            }
            else
            {
                pathPart = "App_Themes\\" + themeId + "\\Scriptlets";
                if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pathPart)))
                {
                    pathPart = "App_Data\\Scriptlets";
                }                
            }

            if (context != null)
            {
                scriptlets = context.Cache[_CacheKey + themeId] as ScriptletCollection;
                if (scriptlets == null)
                {
                    scriptlets = ScriptletDataSource.LoadAll(themeId);
                    if (scriptlets.Count > 0)
                    {
                        scriptlets.Sort("Identifier", GenericComparer.SortDirection.ASC);
                        CacheDependency dep = new CacheDependency(GetScriptletDirectories(context.Server.MapPath("~/" + pathPart.Replace('\\', '/'))));
                        CacheItemPriority priority = scriptlets.Count < 400 ? CacheItemPriority.NotRemovable : CacheItemPriority.High;
                        context.Cache.Insert(_CacheKey + themeId, scriptlets, dep, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, priority, null);
                    }
                }
                return scriptlets;
            }

            //CALLED FROM NON-WEB CONTEXT, LOAD STORE SETTINGS
            scriptlets = ScriptletDataSource.LoadAll(themeId);
            scriptlets.Sort("Identifier", GenericComparer.SortDirection.ASC);
            return scriptlets;
        }

        /// <summary>
        /// Loads all Scriptlet objects for the given theme
        /// </summary>
        /// <returns>A collection of all Scriptlet objects in the store</returns>
        public static ScriptletCollection LoadAll(string themeId)
        {
            StoreSettingCollection settings = Token.Instance.Store.Settings;
            Hashtable ht = new Hashtable();
            ScriptletCollection results = new ScriptletCollection();
            //LOAD CUSTOM SCRIPTLETS FIRST            
            string pathPart;
            if (string.IsNullOrEmpty(themeId))
            {
                pathPart = "App_Data\\Scriptlets";
            }
            else
            {
                pathPart = "App_Themes\\" + themeId + "\\Scriptlets";
                if(!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pathPart))) 
                {
                    pathPart = "App_Data\\Scriptlets";
                }
            }

            DirectoryInfo di = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pathPart + "\\Custom"));
            if (di.Exists)
            {
                DirectoryInfo[] folders = di.GetDirectories();
                foreach (DirectoryInfo folder in folders)
                {
                    ScriptletType scriptletType;
                    try
                    {
                        scriptletType = (ScriptletType)Enum.Parse(typeof(ScriptletType), folder.Name, true);
                    }
                    catch (ArgumentException)
                    {
                        //FOLDER IS NOT A RECOGNIZED SCRIPTLET TYPE
                        scriptletType = ScriptletType.Unspecified;
                    }
                    if (scriptletType != ScriptletType.Unspecified)
                    {
                        FileInfo[] files = folder.GetFiles("*.htm");
                        foreach (FileInfo file in files)
                        {
                            string identifier = Path.GetFileNameWithoutExtension(file.Name);
                            string hashkey = scriptletType.ToString() + "_" + identifier;
                            Scriptlet s = ScriptletDataSource.Load(themeId, identifier, scriptletType, BitFieldState.True, false);
                            if (s != null)
                            {
                                ht.Add(s.ScriptletType + "_" + s.Identifier, true);
                                results.Add(s);
                            }
                        }
                    }
                }
            }
            //LOAD DEFAULT SCRIPTLETS NEXT
            di = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pathPart + "\\Default"));
            if (di.Exists)
            {
                DirectoryInfo[] folders = di.GetDirectories();
                foreach (DirectoryInfo folder in folders)
                {
                    ScriptletType scriptletType;
                    try
                    {
                        scriptletType = (ScriptletType)Enum.Parse(typeof(ScriptletType), folder.Name, true);
                    }
                    catch (ArgumentException)
                    {
                        //FOLDER IS NOT A RECOGNIZED SCRIPTLET TYPE
                        scriptletType = ScriptletType.Unspecified;
                    }
                    if (scriptletType != ScriptletType.Unspecified)
                    {
                        FileInfo[] files = folder.GetFiles("*.htm");
                        foreach (FileInfo file in files)
                        {
                            string identifier = Path.GetFileNameWithoutExtension(file.Name);
                            string hashkey = scriptletType.ToString() + "_" + identifier;
                            if (!ht.ContainsKey(hashkey))
                            {
                                Scriptlet s = ScriptletDataSource.Load(themeId, identifier, scriptletType, BitFieldState.False, false);
                                if (s != null)
                                {
                                    ht.Add(hashkey, true);
                                    results.Add(s);
                                }
                            }
                        }
                    }
                }
            }
            return results;
        }

    }
}
