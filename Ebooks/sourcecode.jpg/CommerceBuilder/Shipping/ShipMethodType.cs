using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Shipping
{
    /// <summary>
    /// Enumeration that represents the type of shipping method based on how its rate is calculated
    /// </summary>
    public enum ShipMethodType
    {
        /// <summary>
        /// This shipping method uses flat rate charges 
        /// </summary>
        FlatRate,

        /// <summary>
        /// This shipping method bases rate calculation on the weight
        /// </summary>
        WeightBased,

        /// <summary>
        /// This shipping method bases rate calculation on the price (cost)
        /// </summary>
        CostBased,

        /// <summary>
        /// This shipping method bases rate calculation on the quantity of items
        /// </summary>
        QuantityBased,

        /// <summary>
        /// This shipping method gets rate calculation from an external shipping provider
        /// </summary>
        IntegratedProvider
    }
}
