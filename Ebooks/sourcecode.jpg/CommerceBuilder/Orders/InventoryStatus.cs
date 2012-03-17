using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Enumeration that represents the status of inventory.
    /// </summary>
    public enum InventoryStatus
    {
        /// <summary>
        /// Inventory status is none
        /// </summary>
        None, 
        
        /// <summary>
        /// Inventory is destocked
        /// </summary>
        Destocked
    }
}
