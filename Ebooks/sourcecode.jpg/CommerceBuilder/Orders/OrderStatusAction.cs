namespace CommerceBuilder.Orders
{
    /// <summary>
    /// This class represents a OrderStatusAction object in the database.
    /// </summary>
    public partial class OrderStatusAction
    {
        /// <summary>
        /// Order Action
        /// </summary>
        public OrderAction OrderAction
        {
            get
            {
                return (OrderAction)this.OrderActionId;
            }
            set
            {
                this.OrderActionId = (short)value;
            }
        }
    }
}
