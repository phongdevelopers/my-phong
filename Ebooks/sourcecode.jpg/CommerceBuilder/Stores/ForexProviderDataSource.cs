using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// DataSource class for ForexProvider objects
    /// </summary>
    [DataObject(true)]
    public class ForexProviderDataSource
    {
        /// <summary>
        /// Gets a list of all ForexProvider implementations in available assemblies
        /// </summary>
        /// <returns>A List of objects implementing IForexProvider interface</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<IForexProvider> GetProviders()
        {
            List<IForexProvider> providers = new List<IForexProvider>();
            List<string> providerNames = new List<string>();
            foreach (System.Reflection.Assembly assemblyInstance in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type thisType in assemblyInstance.GetTypes())
                    {
                        if ((thisType.IsClass && !thisType.IsAbstract))
                        {
                            foreach (Type thisInterface in thisType.GetInterfaces())
                            {
                                IForexProvider instance = null;
                                if ((!string.IsNullOrEmpty(thisInterface.FullName) && thisInterface.FullName.Equals("CommerceBuilder.Stores.IForexProvider")))
                                {
                                    string classId = Utility.Misc.GetClassId(thisType);
                                    string loweredClassId = classId.ToLowerInvariant();
                                    if (!providerNames.Contains(loweredClassId))
                                    {
                                        instance = Activator.CreateInstance(Type.GetType(classId)) as IForexProvider;
                                        if (instance != null)
                                        {
                                            providers.Add(instance);
                                        }
                                        providerNames.Add(loweredClassId);
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    //ignore error
                }
            }
            return providers;
        }

        /// <summary>
        /// Gets the current ForexProvider 
        /// </summary>
        /// <returns>The current ForexProvider</returns>
        public static IForexProvider GetCurrentProvider()
        {
            IForexProvider instance = null;
            Store store = Token.Instance.Store;
            if (store != null)
            {
                string classId = store.Settings.ForexProviderClassId;
                if (!string.IsNullOrEmpty(classId))
                {
                    //TRY TO INSTANTIATE THE CLASS
                    try
                    {
                        instance = Activator.CreateInstance(Type.GetType(classId)) as IForexProvider;
                    }
                    catch
                    {
                        Logger.Error("Could not create Forex provider " + classId);
                        instance = null;
                    }
                }
            }
            return instance;
        }
    }
}
