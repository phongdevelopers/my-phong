using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using CommerceBuilder.Common;
using CommerceBuilder.Orders;
using CommerceBuilder.Users;

namespace CommerceBuilder.Taxes.Providers
{
    /// <summary>
    /// Interface that must be implemented by tax providers
    /// </summary>
    public interface ITaxProvider
    {
        /// <summary>
        /// The gateway Id of the tax provider. This is passed at the time of initialization.
        /// </summary>
        int TaxGatewayId { get; }

        /// <summary>
        /// The name of the tax provider implementation
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// A short description of the tax provider implementation
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The version of the tax provider implementation
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Is the tax provider activated or not
        /// </summary>
        bool Activated { get; }
        
        /// <summary>
        /// Gets a Url for the logo of the tax provider implementation
        /// </summary>
        /// <param name="cs">ClientScriptManager</param>
        /// <returns>A Url for the logo of the tax provider implementation</returns>
        string GetLogoUrl(System.Web.UI.ClientScriptManager cs);

        /// <summary>
        /// Gets a Url for the configuration of the tax provider implementation
        /// </summary>
        /// <param name="cs">ClientScriptManager</param>
        /// <returns>A Url for the configuration of the tax provider implementation</returns>
        string GetConfigUrl(System.Web.UI.ClientScriptManager cs);

        /// <summary>
        /// Initialize the tax provider with the given configuration data. Called by AC at the time of initialization.
        /// </summary>
        /// <param name="taxGatewayId">The tax gateway id</param>
        /// <param name="configurationData">Configuration data in the form of name value paris</param>
        void Initialize(int taxGatewayId, Dictionary<String, String> configurationData);

        /// <summary>
        /// Get the configuration data in the form on name value pairs
        /// </summary>
        /// <returns>The configuration data as name value pairs</returns>
        Dictionary<string, string> GetConfigData();

        /// <summary>
        /// Cancel the taxes by this provider for the given basket
        /// </summary>
        /// <param name="basket">The basket for which to cancel the taxes</param>
        void Cancel(Basket basket);

        /// <summary>
        /// Cancel the taxes by this provider for the given order
        /// </summary>
        /// <param name="basket">The order for which to cancel the taxes</param>
        void Cancel(Order order);

        /// <summary>
        /// Commit the taxes by this provider for the given order
        /// </summary>
        /// <param name="order">The order for which to commit the taxes</param>
        void Commit(Order order);

        /// <summary>
        /// Calculate the taxes using this provider for the given basket
        /// </summary>
        /// <param name="basket">The basket to calculate tax for</param>
        /// <returns>Total tax calculated</returns>
        LSDecimal Calculate(Basket basket);

        /// <summary>
        /// Clear out any existing taxes and then recalculate new line items.
        /// </summary>
        /// <param name="order">The order to calculate tax for</param>
        /// <returns>Total tax calculated</returns>
        LSDecimal Recalculate(Order order);
    }
}
