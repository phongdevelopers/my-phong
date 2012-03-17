using CommerceBuilder.Utility;

namespace CommerceBuilder.Shipping
{
    public partial class Warehouse
    {
        /// <summary>
        /// Gets the formatted address for the warehouse.
        /// </summary>
        /// <param name="isHtml">Indicates whether the returned address should be formatted for HTML.</param>
        /// <returns>The formatted address for the warehouse.</returns>
        /// <remarks>The returned address does not include the "name" of the warehouse.</remarks>
        public string FormatAddress(bool isHtml)
        {
            return AddressFormatter.Format(string.Empty, string.Empty, this.Address1, this.Address2, this.City, this.Province, this.PostalCode, this.CountryCode, this.Phone, this.Fax, this.Email, isHtml);
        }

        /// <summary>
        /// Deletes a warehouse, reassociating any products with the specified warehouse.
        /// </summary>
        /// <param name="newWarehouseId">The warehouse that associated products should be switched to</param>
        /// <returns>True if the warehouse is deleted, false otherwise.</returns>
        public virtual bool Delete(int newWarehouseId)
        {
            WarehouseDataSource.MoveProducts(this.WarehouseId, newWarehouseId);
            return this.Delete();
        }
    }
}
