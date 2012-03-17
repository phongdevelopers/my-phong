namespace CommerceBuilder.Taxes
{
    /// <summary>
    /// Indicates the nexus used to calculate tax liability.
    /// </summary>
    public enum TaxNexus
    {
        /// <summary>
        /// The merchant address
        /// </summary>
        PointOfSale,
        /// <summary>
        /// The warehouse address
        /// </summary>
        PointOfOrigin,
        /// <summary>
        /// The customer billing address
        /// </summary>
        PointOfBilling,
        /// <summary>
        /// The customer shipping address
        /// </summary>
        PointOfDelivery
    }
}