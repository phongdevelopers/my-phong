using System;
using CommerceBuilder.Data;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Products;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// DataSource class for Store objects
    /// </summary>
    [DataObject(true)]
    public partial class StoreDataSource
    {
        /// <summary>
        /// Creates a new store object
        /// </summary>
        /// <param name="storeName">Name of the new store</param>
        /// <param name="adminEmail">Email address of initial super user account</param>
        /// <param name="adminPassword">Password for initial super user account</param>
        /// <param name="switchContext">If true, the token context is switched to the new store.  If false, the token 
        /// context remains the same as it was before the method is called.</param>
        /// <returns>The created store object</returns>
        public static Store CreateStore(string storeName, string adminEmail, string adminPassword, bool switchContext)
        {
            //NEED TO SAVE THE CURRENT STORE CONTEXT
            Store masterStore = Token.Instance.Store;
            //CREATE THE STORE
            Store newStore = new Store();
            newStore.Name = storeName;
            newStore.NextOrderId = 1;
            newStore.OrderIdIncrement = 1;
            newStore.WeightUnit = CommerceBuilder.Shipping.WeightUnit.Pounds;
            newStore.MeasurementUnit = CommerceBuilder.Shipping.MeasurementUnit.Inches;
            newStore.Save();
            //NEED TO SWITCH OUR TOKEN CONTEXT TO THE NEW STORE
            Token.Instance.InitStoreContext(newStore);
            //INITIALIZE THE AUDIT LOGS
            Logger.Audit(AuditEventType.ApplicationStarted, true, string.Empty);
            //INITIALIZE ROLES AND GROUPS
            RoleDataSource.EnsureDefaultRoles();
            GroupDataSource.EnsureDefaultGroups();
            //CREATE THE SUPER USER
            User user = UserDataSource.CreateUser(adminEmail, adminPassword);
            //ASSIGN USER TO APPROPRIATE GROUP
            CommerceBuilder.Users.Group suGroup = GroupDataSource.LoadForName("Super Users");
            user.UserGroups.Add(new UserGroup(user.UserId, suGroup.GroupId));
            user.Save();
            //RESET THE ORIGINAL STORE CONTEXT
            if (!switchContext) Token.Instance.InitStoreContext(masterStore);
            //RETURN THE NEW STORE
            return newStore;
        }

        /// <summary>
        /// Loads the current store
        /// </summary>
        /// <returns>The current store</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static Store Load()
        {
            return StoreDataSource.Load(Token.Instance.StoreId);
        }

        /// <summary>
        /// Gets the highest order number assigned for a store.
        /// </summary>
        /// <returns>The highest order number assigned</returns>
        public static int GetMaxOrderNumber()
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT MAX(OrderNumber) FROM ac_Orders WHERE StoreId = @storeId");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Gets the next order number for a store
        /// </summary>
        /// <returns>The next order number to be assigned</returns>
        private static int InternalGetNextOrderNumber()
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT NextOrderId FROM ac_Stores WHERE StoreId = @storeId");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Gets the next order number for a store
        /// </summary>
        /// <param name="increment">If true, the order number returned is 'assigned' and the next number will be advanced by the increment.</param>
        /// <returns>The next order number for a store</returns>
        public static int GetNextOrderNumber(bool increment)
        {
            if (!increment) return InternalGetNextOrderNumber();
            Database database = Token.Instance.Database;
            int nextOrderNumber = 0;
            //PREPARE THE COMMANDS TO SELECT, THEN UPDATE NEXT ORDER NUMBER
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT NextOrderId FROM ac_Stores WHERE StoreId = @storeId");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            DbCommand updateCommand = database.GetSqlStringCommand("UPDATE ac_Stores SET NextOrderId = NextOrderId + OrderIdIncrement WHERE StoreId = @storeId");
            database.AddInParameter(updateCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //WE MAY NOT NEED TO OPEN A CONNECTION IF ONE IS ALREADY IN PROGRESS
            bool localTransaction = (database.TransactionCount == 0);
            try
            {
                if (localTransaction) database.BeginTransaction(IsolationLevel.Serializable);
                //EXECUTE THE QUERIES
                nextOrderNumber = (int)database.ExecuteScalar(selectCommand);
                database.ExecuteNonQuery(updateCommand);
                //COMMIT THE TRANSACTION
                if (localTransaction) database.CommitTransaction();
            }
            catch
            {
                if (localTransaction)
                {
                    try
                    {
                        database.RollbackTransaction();
                    }
                    catch { }
                }
                throw;
            }
            return nextOrderNumber;
        }

        /// <summary>
        /// Sets the next order number for the store
        /// </summary>
        /// <returns>The value set for next order number</returns>
        /// <remarks>This method will calculate the next available number.</remarks>
        public static int SetNextOrderNumber()
        {
            return SetNextOrderNumber(0);
        }

        /// <summary>
        /// Sets the next order number for the store
        /// </summary>
        /// <param name="nextOrderNumber">The order number to set; pass 0 to have the next available number calculated</param>
        /// <returns>The value set for next order number</returns>
        public static int SetNextOrderNumber(int nextOrderNumber)
        {
            Database database = Token.Instance.Database;
            //VALIDATE INPUT
            if (nextOrderNumber < 1)
            {
                //CALCULATE APPROPRIATE ORDER NUMBER
                nextOrderNumber = GetMaxOrderNumber() + 1;
            }
            //UPDATE NEXT ORDER NUMBER
            DbCommand updateCommand = database.GetSqlStringCommand("UPDATE ac_Stores SET NextOrderId = @nextOrderNumber WHERE StoreId = @storeId");
            database.AddInParameter(updateCommand, "@nextOrderNumber", System.Data.DbType.Int32, nextOrderNumber);
            database.AddInParameter(updateCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            database.ExecuteNonQuery(updateCommand);
            return nextOrderNumber;
        }

        /// <summary>
        /// Counts all stores defined in the database
        /// </summary>
        /// <returns>Number of stores defined in the database</returns>
        public static int CountAll()
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalCount FROM ac_Stores");
            return (int)database.ExecuteScalar(selectCommand);
        }

        
    }
    
}
