//-----------------------------------------------------------------------
// <copyright file="AddressValidatorServiceLocator.cs" company="Able Solutions Corporation">
//     Copyright (c) 2010 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Shipping.Providers
{
    using CommerceBuilder.Common;
    using CommerceBuilder.Taxes;

    /// <summary>
    /// Provides a location service for address validation providers
    /// </summary>
    public static class AddressValidatorServiceLocator
    {
        /// <summary>
        /// Locates a configured address validator service
        /// </summary>
        /// <returns>A configured address validator service, if available.  Null is returned if no 
        /// address validators are configured.</returns>
        public static IAddressValidatorService Locate()
        {
            // RIGHT NOW ONLY AVATAX HAS ADDRESS SERVICES, SO SEE IF IT IS CONFIGURED
            TaxGateway avaTaxGateway = LocateAvaTaxGateway();
            if (avaTaxGateway != null)
            {
                // MAKE SURE THE EXISTING SERVICE SUPPORTS THE ADDRESS VALIDATION INTERFACE
                object avaTaxProviderInstance = avaTaxGateway.GetProviderInstance();
                IAddressValidatorService addressValidatorService = avaTaxProviderInstance as IAddressValidatorService;
                return addressValidatorService;
            }
            return null;
        }

        /// <summary>
        /// Gets the configured avatax gateway
        /// </summary>
        /// <returns>The configured avatax gateway, or null if it is not configured</returns>
        private static TaxGateway LocateAvaTaxGateway()
        {
            TaxGatewayCollection taxGateways = Token.Instance.Store.TaxGateways;
            foreach(TaxGateway taxGateway in taxGateways)
            {
                if (taxGateway.Name == "Avalara AvaTax") return taxGateway;
            }
            return null;
        }
    }
}