namespace CommerceBuilder.Shipping
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ShipMethodGroup objects.
    /// </summary>
    public partial class ShipMethodGroupCollection : PersistentCollection<ShipMethodGroup>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="shipMethodId">Value of ShipMethodId of the required object.</param>
        /// <param name="groupId">Value of GroupId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 shipMethodId, Int32 groupId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((shipMethodId == this[i].ShipMethodId) && (groupId == this[i].GroupId)) return i;
            }
            return -1;
        }
    }
}
