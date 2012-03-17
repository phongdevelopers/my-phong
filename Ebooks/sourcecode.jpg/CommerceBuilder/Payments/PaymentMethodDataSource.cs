using System;
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Users;

namespace CommerceBuilder.Payments
{
    [DataObject(true)]
    public partial class PaymentMethodDataSource
    {
        /// <summary>
        /// Loads the payment methods that are available to the specified user, taking into account the
        /// role restrictions on the payment methods.
        /// </summary>
        /// <param name="userId">The user requesting available payment methods.</param>
        /// <returns>A list of users that are available to the user.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentMethodCollection LoadForUser(int userId)
        {
            return LoadForUser(userId, string.Empty);
        }

        /// <summary>
        /// Loads the payment methods that are available to the specified user, taking into account the
        /// role restrictions on the payment methods.
        /// </summary>
        /// <param name="userId">The user requesting available payment methods.</param>
        /// <param name="sortExpression">The sort order of the returned methods.  Defaults to OrderBy field.</param>
        /// <returns>A list of users that are available to the user.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentMethodCollection LoadForUser(int userId, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "OrderBy";
            //LOAD THE PAYMENT METHODS FOR THE STORE
            PaymentMethodCollection allMethods = PaymentMethodDataSource.LoadForStore(sortExpression);
            //LOAD THE USER TO OBTAIN ROLES
            User user = UserDataSource.Load(userId);
            //CREATE FILTERED LIST OF PAYMENT METHODS
            PaymentMethodCollection filteredMethods = new PaymentMethodCollection();
            foreach (PaymentMethod method in allMethods)
            {
                if (method.UserHasAccess(user)) filteredMethods.Add(method);
            }
            return filteredMethods;
        }

    }
}
