using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// Enumeration that represents how the weight is modified by a kit item
    /// </summary>
    public enum KitWeightMode
    {
        //TODO ??
        /// <summary>
        /// 
        /// </summary>
        Inherit, 
        
        /// <summary>
        /// The weight of the base product will be modified using the weight of the kit item
        /// </summary>
        Modify, 
        
        /// <summary>
        /// The weight of the base product will be overridden by the weight of the kit item
        /// </summary>
        Override
    }
}
