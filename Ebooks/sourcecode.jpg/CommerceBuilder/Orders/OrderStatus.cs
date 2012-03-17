using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Class that represents OrderStatus object in database
    /// </summary>
    public partial class OrderStatus
    {
        /// <summary>
        /// Inventory action associated with this order status
        /// </summary>
        public InventoryAction InventoryAction
        {
            get { return (InventoryAction)this.InventoryActionId; }
            set { this.InventoryActionId = (short)value; }
        }

        /// <summary>
        /// Deletes an order status from the database
        /// </summary>
        /// <returns>True if the order status is deleted, false otherwise.</returns>
        public bool Delete()
        {
            //Delete OrderStatusEmail associations first
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_OrderStatusEmails");
            deleteQuery.Append(" WHERE OrderStatusId = @orderStatusId");

            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@orderStatusId", System.Data.DbType.Int32, this.OrderStatusId);
                database.ExecuteNonQuery(deleteCommand);
            }

            return this.BaseDelete();
        }

        /// <summary>
        /// Deletes an Order Status, reassociating any orders with the specified order status.
        /// </summary>
        /// <param name="newOrderStatusId">The order status that associated orders should be switched to</param>
        /// <returns>True if the order status is deleted, false otherwise.</returns>
        public virtual bool Delete(int newOrderStatusId)
        {
            OrderStatusDataSource.MoveOrders(this.OrderStatusId, newOrderStatusId);
            return this.Delete();
        }
    }
}
