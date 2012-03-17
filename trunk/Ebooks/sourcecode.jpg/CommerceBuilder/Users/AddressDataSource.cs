using System.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// DataSource class for Address objects
    /// </summary>
    [System.ComponentModel.DataObject(true)]
    public partial class AddressDataSource
    {
        /// <summary>
        /// Reassign addresses from one user to another.
        /// </summary>
        /// <param name="oldUserId">the old user id</param>
        /// <param name="newUserId">the new user id</param>
        public static void UpdateUser(int oldUserId, int newUserId)
        {
            string updateQuery = "UPDATE ac_Addresses SET UserId = @newUserId WHERE UserId = @oldUserId";
            Database database = Token.Instance.Database;
            using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery))
            {
                database.AddInParameter(updateCommand, "@newUserId", DbType.Int32, newUserId);
                database.AddInParameter(updateCommand, "@oldUserId", DbType.Int32, oldUserId);
                database.ExecuteNonQuery(updateCommand);
            }
            // ENSURE THE PRIMARY ADDRESS ID IS CORRECT FOR THE NEW USER
            int oldUserPrimaryAddressId = GetPrimaryAddressId(oldUserId);
            int newUserPrimaryAddressId = GetPrimaryAddressId(newUserId);

            // ALL ADDRESSES FROM OLD USER HAVE BEEN REASSIGNED, SO OLD USER NO LONGER HAS PRIMARY ADDRESS
            SetPrimaryAddressId(0, oldUserId);

            // IN MOST CASES, THE OLD USER'S PRIMARY ADDRESS ID SHOULD BE THE CORRECT ONE
            if (IsAddressIdValidForUser(oldUserPrimaryAddressId, newUserId))
            {
                SetPrimaryAddressId(oldUserPrimaryAddressId, newUserId);
            }
            else if (!IsAddressIdValidForUser(newUserPrimaryAddressId, newUserPrimaryAddressId))
            {
                // THE OLD PRIMARY ADDRESS ID IS WRONG, AND THE EXISTING PRIMARY ADDRESS IS WRONG
                // ATTEMPT TO SET WITH THE FIRST ADDRESS ASSIGNED TO THE USER
                SetPrimaryAddressId(GetFirstAddressId(newUserId), newUserId);
            }
        }

        private static int GetPrimaryAddressId(int userId)
        {
            Database database = Token.Instance.Database;
            using (DbCommand selectCommand = database.GetSqlStringCommand("SELECT PrimaryAddressId FROM ac_Users WHERE UserId = @userId"))
            {
                database.AddInParameter(selectCommand, "@userId", DbType.Int32, userId);
                return AlwaysConvert.ToInt(Token.Instance.Database.ExecuteScalar(selectCommand));
            }
        }

        private static int GetFirstAddressId(int userId)
        {
            Database database = Token.Instance.Database;
            using (DbCommand selectCommand = database.GetSqlStringCommand("SELECT MIN(AddressId) As FirstId FROM ac_Addresses WHERE UserId = @userId"))
            {
                database.AddInParameter(selectCommand, "@userId", DbType.Int32, userId);
                return AlwaysConvert.ToInt(Token.Instance.Database.ExecuteScalar(selectCommand));
            }
        }

        private static bool IsAddressIdValidForUser(int addressId, int userId)
        {
            Database database = Token.Instance.Database;
            using (DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS NumAddr FROM ac_Addresses WHERE AddressId = @addressId AND UserId = @userId"))
            {
                database.AddInParameter(selectCommand, "@addressId", DbType.Int32, addressId);
                database.AddInParameter(selectCommand, "@userId", DbType.Int32, userId);
                return AlwaysConvert.ToInt(Token.Instance.Database.ExecuteScalar(selectCommand)) > 0;
            }
        }

        private static void SetPrimaryAddressId(int addressId, int userId)
        {
            string updateQuery = "UPDATE ac_Users SET PrimaryAddressId = @addressId WHERE UserId = @userId";
            Database database = Token.Instance.Database;
            using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery))
            {
                database.AddInParameter(updateCommand, "@addressId", DbType.Int32, NullableData.DbNullify(addressId));
                database.AddInParameter(updateCommand, "@userId", DbType.Int32, userId);
                database.ExecuteNonQuery(updateCommand);
            }
        }
    }
}
