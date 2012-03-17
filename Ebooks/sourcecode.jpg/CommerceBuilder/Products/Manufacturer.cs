namespace CommerceBuilder.Products
{
    public partial class Manufacturer
    {
        /// <summary>
        /// Deletes a manufacturer, reassociating any products with the specified manufacturer.
        /// </summary>
        /// <param name="newManufacturerId">The manufacturer that associated products should be switched to</param>
        /// <returns>True if the manufacturer is deleted, false otherwise.</returns>
        public virtual bool Delete(int newManufacturerId)
        {
            ManufacturerDataSource.MoveProducts(this.ManufacturerId, newManufacturerId);
            return this.Delete();
        }
    }
}
