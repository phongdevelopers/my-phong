namespace CommerceBuilder.Shipping
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Warehouse objects.
    /// </summary>
    public partial class WarehouseCollection : PersistentCollection<Warehouse>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="warehouseId">Value of WarehouseId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 warehouseId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (warehouseId == this[i].WarehouseId) return i;
            }
            return -1;
        }
    }
}
