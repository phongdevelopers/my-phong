using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Products;
using CommerceBuilder.Users;

namespace CommerceBuilder.Orders
{

    /// <summary>
    /// This class represents a Subscription in database
    /// </summary>
    public partial class Subscription
    {
        /// <summary>
        /// Activates this subscription
        /// </summary>
        public void Activate()
        {
            SubscriptionPlan sp = this.SubscriptionPlan;
            this.ExpirationDate = sp.CalculateExpiration();
            this.IsActive = true;
            this.GroupId = sp.GroupId;
            this.Save();
        }

        /// <summary>
        /// Deletes this subscription
        /// </summary>
        /// <returns>true if delete successful, false otherwise</returns>
        public bool Delete()
        {
            if (this.SubscriptionId > 0)
            {
                //DELETE ANY USER ROLES ASSOCIATED WITH THIS SUBSCRIPTION, WE HAVE TO RECALCULATE BUT AFTER DELETING THIS SUBSCRIPTION
                string sql = "DELETE FROM ac_UserGroups WHERE SubscriptionId = @subscriptionId";
                Database database = Token.Instance.Database;
                DbCommand selectCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(selectCommand, "@subscriptionId", System.Data.DbType.Int32, this.SubscriptionId);
                database.ExecuteNonQuery(selectCommand);

                //REMOVE REFERENCE FOR ANY PAYMENTS ASSOCIATED WITH THIS SUBSCRIPTION
                sql = "UPDATE ac_Payments SET SubscriptionId = NULL WHERE SubscriptionId = @subscriptionId";
                database = Token.Instance.Database;
                selectCommand = database.GetSqlStringCommand(sql);
                database.AddInParameter(selectCommand, "@subscriptionId", System.Data.DbType.Int32, this.SubscriptionId);
                database.ExecuteNonQuery(selectCommand);
            }
            bool result = this.BaseDelete();
            if (result)
            {
                UserGroup.RecalculateExpiration(this.UserId, this.GroupId);
            }
            return result;
        }

        public SaveResult Save()
        {
            // UPDATE THE USER GROUP EXPIRATION IF NEEDED
            SaveResult result = this.BaseSave();
            if (this.GroupId > 0) UserGroup.RecalculateExpiration(this.UserId, this.GroupId);
            return result;
        }
    }
}
