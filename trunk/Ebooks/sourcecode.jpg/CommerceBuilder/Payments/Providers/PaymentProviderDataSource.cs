using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Payments.Providers
{
    /// <summary>
    /// DataSource class for PaymentProvider objects
    /// </summary>
    public static class PaymentProviderDataSource
    {
        /// <summary>
        /// Gets instances of all classes available that implement the IPaymentProvider interface
        /// </summary>
        /// <returns>List of instances of classes implementing IPaymentProvider</returns>
        public static List<IPaymentProvider> GetProviders()
        {
            List<IPaymentProvider> providers = new List<IPaymentProvider>();
            List<string> providerNames = new List<string>();
            PaymentGatewayCollection configuredGateways = PaymentGatewayDataSource.LoadForStore();
            //LOOP THROUGH ALL THE ASSEMBLIES IN THE CURRENT DOMAIN
            foreach (System.Reflection.Assembly assemblyInstance in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    //LOOP THROUGH ALL TYPES IN THE ASSEMBLY
                    foreach (Type thisType in assemblyInstance.GetTypes())
                    {
                        //ONLY CHECK CLASSES THAT ARE NOT ABSTRACT 
                        if ((thisType.IsClass && !thisType.IsAbstract))
                        {
                            //LOOP THROUGH ALL INTERFACES THIS TYPE IMPLMEMENTS
                            foreach (Type thisInterface in thisType.GetInterfaces())
                            {
                                //ONLY PROCEED IF THE CLASS IMPLEMENTS PAYMENT PROVIDER INTERFACE
                                string interfaceFullName = thisInterface.FullName;
                                if (!string.IsNullOrEmpty(interfaceFullName) && (interfaceFullName == "CommerceBuilder.Payments.Providers.IPaymentProvider"))
                                {
                                    //GET THE CLASS ID WITHOUT VERSION
                                    string classId = Utility.Misc.GetClassId(thisType);
                                    //MAKE SURE WE ONLY LIST THIS PROVIDER ONE TIME
                                    string loweredClassId = classId.ToLowerInvariant();
                                    if (!providerNames.Contains(loweredClassId))
                                    {
                                        //DO NOT INCLUDE GIFT CERTIFICATE PROVIDER
                                        if (classId != Utility.Misc.GetClassId(typeof(GiftCertificatePaymentProvider)))
                                        {
                                            //ONLY INCLUDE GATEWAYS NOT CONFIGURED ALREADY
                                            if (!IsConfigured(configuredGateways, classId))
                                            {
                                                try
                                                {
                                                    //ONLY INCLUDE GATEWAYS THAT WE CAN CREATE INSTANCES OF
                                                    IPaymentProvider instance = Activator.CreateInstance(Type.GetType(classId)) as IPaymentProvider;
                                                    if (instance != null)
                                                    {
                                                        providers.Add(instance);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.Warn("Could not create an instance of the payment provider: " + classId, ex);
                                                    throw;
                                                }
                                            }
                                        }
                                        //DO NOT LIST THIS PROVIDER AGAIN
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

            //SORT BY NAME
            providers.Sort(CompareProvidersByName);
            return providers;
        }

        private static bool IsConfigured(PaymentGatewayCollection gateways, string classId)
        {
            foreach (PaymentGateway gateway in gateways)
            {
                if (gateway.ClassId.Equals(classId)) return true;
            }
            return false;
        }

        private static int CompareProvidersByName(IPaymentProvider x, IPaymentProvider y)
        {
            return (x.Name.CompareTo(y.Name));
        }
    }
}