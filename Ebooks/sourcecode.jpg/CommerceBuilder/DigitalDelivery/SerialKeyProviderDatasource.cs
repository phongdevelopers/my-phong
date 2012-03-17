using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CommerceBuilder.DigitalDelivery
{
    /// <summary>
    /// DataSource class for SerialKeyProvider objects
    /// </summary>
    public class SerialKeyProviderDataSource
    {
        /// <summary>
        /// Gets a list of all available objects that implement ISerialKeyProvider interface
        /// </summary>
        /// <returns>A list of all available objects that implement ISerialKeyProvider interface</returns>
        public static List<ISerialKeyProvider> GetSerialKeyProviders()
        {
            List<ISerialKeyProvider> providers = new List<ISerialKeyProvider>();
            List<string> providerNames = new List<string>();
            if (HttpContext.Current != null)
            {
                HttpServerUtility server = HttpContext.Current.Server;
                string[] files = System.IO.Directory.GetFiles(server.MapPath("~/bin"), "*.DLL");
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
                                    ISerialKeyProvider instance = null;
                                    if ((!string.IsNullOrEmpty(thisInterface.FullName) && thisInterface.FullName.Equals(typeof(ISerialKeyProvider).FullName)))
                                    {
                                        string classId = Utility.Misc.GetClassId(thisType);
                                        string loweredClassId = classId.ToLowerInvariant();
                                        if (!providerNames.Contains(loweredClassId))
                                        {
                                            instance = Activator.CreateInstance(Type.GetType(classId)) as ISerialKeyProvider;
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
