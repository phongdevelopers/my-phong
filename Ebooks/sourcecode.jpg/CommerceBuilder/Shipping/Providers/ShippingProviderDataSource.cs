using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CommerceBuilder.Shipping.Providers
{
    /// <summary>
    /// DataSource class for ShippingProvider objects
    /// </summary>
    public static class ShippingProviderDataSource
    {
        /// <summary>
        /// Gets instances of all classes available that implement the IShippingProvider interface
        /// </summary>
        /// <returns>List of instances of classes implementing IShippingProvider</returns>
        public static List<IShippingProvider> GetShippingProviders()
        {
            List<IShippingProvider> providers = new List<IShippingProvider>();
            if (HttpContext.Current != null)
            {
                HttpServerUtility server = HttpContext.Current.Server;
                string[] files = System.IO.Directory.GetFiles(server.MapPath("~/bin"), "*.DLL");
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
                                    IShippingProvider instance = null;
                                    if ((!string.IsNullOrEmpty(thisInterface.FullName) && thisInterface.FullName.Equals("CommerceBuilder.Shipping.Providers.IShippingProvider")))
                                    {
                                        string classId = Utility.Misc.GetClassId(thisType);
                                        string loweredClassId = classId.ToLowerInvariant();
                                        if (!providerNames.Contains(loweredClassId))
                                        {
                                            instance = Activator.CreateInstance(Type.GetType(classId)) as IShippingProvider;
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
            }
            return providers;
        }
    }
}
