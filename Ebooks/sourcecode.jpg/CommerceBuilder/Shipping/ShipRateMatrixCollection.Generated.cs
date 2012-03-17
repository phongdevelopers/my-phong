namespace CommerceBuilder.Shipping
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ShipRateMatrix objects.
    /// </summary>
    public partial class ShipRateMatrixCollection : PersistentCollection<ShipRateMatrix>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="shipRateMatrixId">Value of ShipRateMatrixId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 shipRateMatrixId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (shipRateMatrixId == this[i].ShipRateMatrixId) return i;
            }
            return -1;
        }
    }
}
