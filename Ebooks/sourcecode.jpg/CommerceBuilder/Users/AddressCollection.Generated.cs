namespace CommerceBuilder.Users
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Address objects.
    /// </summary>
    public partial class AddressCollection : PersistentCollection<Address>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="addressId">Value of AddressId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 addressId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (addressId == this[i].AddressId) return i;
            }
            return -1;
        }
    }
}
