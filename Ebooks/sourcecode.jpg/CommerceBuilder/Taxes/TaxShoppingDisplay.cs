namespace CommerceBuilder.Taxes
{
    /// <summary>
    /// Enumeration that represents the shopping display options for taxes
    /// </summary>
    public enum TaxShoppingDisplay
    {
        /// <summary>
        /// Hide taxes while shopping
        /// </summary>
        Hide, 

        /// <summary>
        /// Show taxes included in the line item prices
        /// </summary>
        Included,

        /// <summary>
        /// Show taxes as separate line items
        /// </summary>
        LineItem,

        /// <summary>
        /// Show taxes included in the line item prices for registered users only
        /// </summary>
        IncludedRegistered,

        /// <summary>
        /// Show taxes as separate line items for registered users only
        /// </summary>
        LineItemRegistered
    }
}
