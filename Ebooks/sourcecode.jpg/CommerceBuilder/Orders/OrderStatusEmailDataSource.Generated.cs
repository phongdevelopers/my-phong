//Generated by DataSourceBaseGenerator_Assn

using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
namespace CommerceBuilder.Orders
{

    /// <summary>
    /// DataSource class for OrderStatusEmail objects
    /// </summary>
    public partial class OrderStatusEmailDataSource
    {
        /// <summary>
        /// Deletes a OrderStatusEmail object from the database
        /// </summary>
        /// <param name="orderStatusEmail">The OrderStatusEmail object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(OrderStatusEmail orderStatusEmail)
        {
            return orderStatusEmail.Delete();
        }

        /// <summary>
        /// Deletes a OrderStatusEmail object with given id from the database
        /// </summary>
        /// <param name="orderStatusId">Value of OrderStatusId of the object to delete.</param>
        /// <param name="emailTemplateId">Value of EmailTemplateId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 orderStatusId, Int32 emailTemplateId)
        {
            OrderStatusEmail orderStatusEmail = new OrderStatusEmail();
            if (orderStatusEmail.Load(orderStatusId, emailTemplateId)) return orderStatusEmail.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a OrderStatusEmail object to the database.
        /// </summary>
        /// <param name="orderStatusEmail">The OrderStatusEmail object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(OrderStatusEmail orderStatusEmail) { return orderStatusEmail.Save(); }

        /// <summary>
        /// Load a OrderStatusEmail object for the given primary key from the database.
        /// </summary>
        /// <param name="orderStatusId">Value of OrderStatusId of the object to load.</param>
        /// <param name="emailTemplateId">Value of EmailTemplateId of the object to load.</param>
        /// <returns>The loaded OrderStatusEmail object.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderStatusEmail Load(Int32 orderStatusId, Int32 emailTemplateId)
        {
            OrderStatusEmail orderStatusEmail = new OrderStatusEmail();
            orderStatusEmail.OrderStatusId = orderStatusId;
            orderStatusEmail.EmailTemplateId = emailTemplateId;
            orderStatusEmail.IsDirty = false;
            return orderStatusEmail;
        }

        /// <summary>
        /// Loads a collection of OrderStatusEmail objects for the given criteria for EmailTemplate from the database.
        /// </summary>
        /// <param name="emailTemplateId">Value of EmailTemplateId of the object to load.</param>
        /// <returns>A collection of OrderStatusEmail objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderStatusEmailCollection LoadForEmailTemplate(Int32 emailTemplateId)
        {
            OrderStatusEmailCollection OrderStatusEmails = new OrderStatusEmailCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT OrderStatusId");
            selectQuery.Append(" FROM ac_OrderStatusEmails");
            selectQuery.Append(" WHERE EmailTemplateId = @emailTemplateId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@emailTemplateId", System.Data.DbType.Int32, emailTemplateId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    OrderStatusEmail orderStatusEmail = new OrderStatusEmail();
                    orderStatusEmail.EmailTemplateId = emailTemplateId;
                    orderStatusEmail.OrderStatusId = dr.GetInt32(0);
                    OrderStatusEmails.Add(orderStatusEmail);
                }
                dr.Close();
            }
            return OrderStatusEmails;
        }

        /// <summary>
        /// Loads a collection of OrderStatusEmail objects for the given criteria for OrderStatus from the database.
        /// </summary>
        /// <param name="orderStatusId">Value of OrderStatusId of the object to load.</param>
        /// <returns>A collection of OrderStatusEmail objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderStatusEmailCollection LoadForOrderStatus(Int32 orderStatusId)
        {
            OrderStatusEmailCollection OrderStatusEmails = new OrderStatusEmailCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT EmailTemplateId");
            selectQuery.Append(" FROM ac_OrderStatusEmails");
            selectQuery.Append(" WHERE OrderStatusId = @orderStatusId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@orderStatusId", System.Data.DbType.Int32, orderStatusId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    OrderStatusEmail orderStatusEmail = new OrderStatusEmail();
                    orderStatusEmail.OrderStatusId = orderStatusId;
                    orderStatusEmail.EmailTemplateId = dr.GetInt32(0);
                    OrderStatusEmails.Add(orderStatusEmail);
                }
                dr.Close();
            }
            return OrderStatusEmails;
        }

        /// <summary>
        /// Updates/Saves the given OrderStatusEmail object to the database.
        /// </summary>
        /// <param name="orderStatusEmail">The OrderStatusEmail object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(OrderStatusEmail orderStatusEmail) { return orderStatusEmail.Save(); }

    }
}
