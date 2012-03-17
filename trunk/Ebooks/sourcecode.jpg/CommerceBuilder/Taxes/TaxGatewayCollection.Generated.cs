namespace CommerceBuilder.Taxes
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of TaxGateway objects.
    /// </summary>
    public partial class TaxGatewayCollection : PersistentCollection<TaxGateway>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="taxGatewayId">Value of TaxGatewayId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 taxGatewayId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (taxGatewayId == this[i].TaxGatewayId) return i;
            }
            return -1;
        }
    }
}
