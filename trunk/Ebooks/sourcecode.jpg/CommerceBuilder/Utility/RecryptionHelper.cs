//-----------------------------------------------------------------------
// <copyright file="RecryptionHelper.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Text;
    using System.Web;
    using CommerceBuilder.Common;
    using CommerceBuilder.Configuration;
    using CommerceBuilder.Data;
    using CommerceBuilder.Payments;
    using CommerceBuilder.Stores;
    using CommerceBuilder.Utility;

    /// <summary>
    /// Service class to assist with re-encrypting database fields due to an encryption 
    /// key change.
    /// </summary>
    public static class RecryptionHelper
    {
        /// <summary>
        /// Delegate for executing the recryption process on a separate thread
        /// </summary>
        /// <param name="storeId">ID of the store context</param>
        /// <param name="oldKey">Existing key used to encrypt data</param>
        /// <param name="newKey">New key to be applied to encrypted data</param>
        private delegate void RecryptDatabaseDelegate(int storeId, byte[] oldKey, byte[] newKey);

        /// <summary>
        /// Counts the total number of records in the database that are marked for recrypt
        /// </summary>
        /// <returns>The total number of records in the database that are marked for recrypt</returns>
        /// <remarks>Unless a recryption is in progress, this will always return 0. Use EstimateRecryptionWorkload to 
        /// count the records that would need recryption in the event of a key change.</remarks>
        public static int GetRecryptionWorkload()
        {
            Database database = Token.Instance.Database;

            // COUNT FOR PAYMENTS TABLE
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_Payments WHERE ReCrypt = 1");
            int totalCount = AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));

            // COUNT FOR PAYMENTGATEWAYS TABLE
            selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_PaymentGateways WHERE ReCrypt = 1");
            totalCount += AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));

            // COUNT FOR PAYMENTGATEWAYS TABLE
            selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_ShipGateways WHERE ReCrypt = 1");
            totalCount += AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));

            // COUNT FOR PAYMENTGATEWAYS TABLE
            selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_TaxGateways WHERE ReCrypt = 1");
            totalCount += AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
            return totalCount;
        }

        /// <summary>
        /// Estimates the number of records that may need to be updated with a key change
        /// </summary>
        /// <returns>Number of records that may need to be updated with a key change</returns>
        public static int EstimateRecryptionWorkload()
        {
            Database database = Token.Instance.Database;

            // COUNT ENCRYPTED DATA IN PAYMENTS TABLE
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_Payments WHERE EncryptedAccountData IS NOT NULL");
            int totalCount = AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));

            // COUNT CONFIGDATA IN PAYMENTGATEWAYS TABLE
            selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_PaymentGateways WHERE ConfigData IS NOT NULL");
            totalCount += AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));

            // COUNT CONFIGDATA IN SHIPGATEWAYS TABLE
            selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_ShipGateways WHERE ConfigData IS NOT NULL");
            totalCount += AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));

            // COUNT CONFIGDATA IN TAXGATEWAYS TABLE
            selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_TaxGateways WHERE ConfigData IS NOT NULL");
            totalCount += AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
            return totalCount;
        }

        /// <summary>
        /// Sets the recryption flag for records in the database
        /// </summary>
        /// <param name="recrypt">The value of to set for the recrypt field; pass true to mark database for recryption</param>
        public static void SetRecryptionFlag(bool recrypt)
        {
            Database database = Token.Instance.Database;

            // SET FLAG FOR PAYMENTS TABLE
            DbCommand updateCommand = database.GetSqlStringCommand("UPDATE ac_Payments SET ReCrypt = " + (recrypt ? "1" : "0") + " WHERE EncryptedAccountData IS NOT NULL");
            database.ExecuteNonQuery(updateCommand);

            // SET FLAG FOR PAYMENTGATEWAYS TABLE
            updateCommand = database.GetSqlStringCommand("UPDATE ac_PaymentGateways SET ReCrypt = " + (recrypt ? "1" : "0") + " WHERE ConfigData IS NOT NULL");
            database.ExecuteNonQuery(updateCommand);

            // SET FLAG FOR SHIPGATEWAYS TABLE
            updateCommand = database.GetSqlStringCommand("UPDATE ac_ShipGateways SET ReCrypt = " + (recrypt ? "1" : "0") + " WHERE ConfigData IS NOT NULL");
            database.ExecuteNonQuery(updateCommand);

            // SET FLAG FOR TAXGATEWAYS TABLE
            updateCommand = database.GetSqlStringCommand("UPDATE ac_TaxGateways SET ReCrypt = " + (recrypt ? "1" : "0") + " WHERE ConfigData IS NOT NULL");
            database.ExecuteNonQuery(updateCommand);
        }

        /// <summary>
        /// Decrypts all encrypted data in the database, then re-encrypts it using the new encryption key
        /// </summary>
        /// <param name="context">The HttpApplication context to look for the encryption keys</param>
        public static void RecryptDatabase(HttpApplication context)
        {
            // GET OLD AND NEW KEY FROM CONFIG
            AbleCommerceEncryptionSection encryptionConfig = AbleCommerceEncryptionSection.GetSection(context);
            byte[] oldKey = encryptionConfig.OldEncryptionKey.GetKey();
            byte[] newKey = encryptionConfig.EncryptionKey.GetKey();
            RecryptionHelper.RecryptDatabase(oldKey, newKey);
        }

        /// <summary>
        /// Decrypts all encrypted data in the database, then re-encrypts it using the new encryption key
        /// </summary>
        /// <param name="oldKey">The old encryption key</param>
        /// <param name="newKey">The new encryption key</param>
        public static void RecryptDatabase(byte[] oldKey, byte[] newKey)
        {
            // IF EITHER KEY IS VALID WE SHOULD CONTINUE TO PROCESS
            // ONE INVALID KEY LIKELY SIGNIFIES UNENCRYPTED DATA
            if (EncryptionHelper.IsKeyValid(oldKey) || EncryptionHelper.IsKeyValid(newKey))
            {
                RecryptionHelper.LaunchRecryptionOnNewThread(oldKey, newKey);
            }
            else
            {
                Logger.Error("Error recrypting data, could not find valid keys; Recryption cancelled.");
                RecryptionHelper.SetRecryptionFlag(false);
            }
        }

        /// <summary>
        /// Decrypts the payment data using old key and Re-Encryptes it using the new key.
        /// </summary>
        /// <param name="oldKey">The old encryption key</param>
        /// <param name="newKey">The new encryption key</param>
        private static void LaunchRecryptionOnNewThread(byte[] oldKey, byte[] newKey)
        {
            RecryptDatabaseDelegate del = new RecryptDatabaseDelegate(InternalRecryptDatabase);
            AsyncCallback cb = new AsyncCallback(RecryptDatabaseCallback);
            IAsyncResult ar = del.BeginInvoke(Token.Instance.StoreId, oldKey, newKey, cb, null);
        }

        /// <summary>
        /// Callback method for recryption thread
        /// </summary>
        /// <param name="ar">Results from the execution of the recrypt thread</param>
        private static void RecryptDatabaseCallback(IAsyncResult ar)
        {
            RecryptDatabaseDelegate del = (RecryptDatabaseDelegate)((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate;
            try
            {
                del.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                Logger.Error("Unhandled exception occured during the recryption process.", ex);
            }
        }

        /// <summary>
        /// Implementation of data recryption
        /// </summary>
        /// <param name="storeId">ID of the store context</param>
        /// <param name="oldKey">Existing key used to encrypt data</param>
        /// <param name="newKey">New key to be applied to encrypted data</param>
        private static void InternalRecryptDatabase(int storeId, byte[] oldKey, byte[] newKey)
        {
            // WE MUST INITIALIZE THE STORE CONTEXT AS THIS THREAD HAS NO HTTPCONTEXT
            Store store = StoreDataSource.Load(storeId);
            if (store != null)
            {
                // INITIALIZE THE TOKEN WITH THE STORE CONTEXT
                Token.Instance.InitStoreContext(store);

                // PROCESS RECORDS IN BATCHES OF 100
                int lastCount = 0;
                int count = RecryptionHelper.GetRecryptionWorkload();
                while ((count > 0) && (count != lastCount))
                {
                    List<RecryptRecord> records = RecryptionHelper.LoadForRecrypt(100);
                    foreach (RecryptRecord record in records)
                    {
                        record.DoRecrypt(oldKey, newKey);
                    }

                    // if lastCount and count ever match, it means nothing changed in the last iteration
                    // keep track of this to prevent endless looping in case a recrypt operation fails
                    lastCount = count;
                    count = RecryptionHelper.GetRecryptionWorkload();
                }

                // REMOVE RECRYPT FLAG
                RecryptionHelper.SetRecryptionFlag(false);
            }
        }

        /// <summary>
        /// Loads records to be recrypted
        /// </summary>
        /// <param name="maximumRows">The maximum number of records to load</param>
        /// <returns>A list of records with data to be recrypted</returns>
        private static List<RecryptRecord> LoadForRecrypt(int maximumRows)
        {
            // INITIALIZE VARIABLES FOR LOADING DATA
            List<RecryptRecord> results = new List<RecryptRecord>();
            Database database = Token.Instance.Database;
            int storeId = Token.Instance.StoreId;

            // LOAD ANY PAYMENTS MARKED FOR RECRYPTION
            string sql = "SELECT TOP " + maximumRows + " P.PaymentId,P.EncryptedAccountData FROM ac_Payments P, ac_Orders O WHERE P.OrderId=O.OrderId AND O.StoreId=" + storeId + " AND P.ReCrypt=1";
            DbCommand selectCommand = database.GetSqlStringCommand(sql);
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    results.Add(new RecryptRecord("ac_Payments", dr.GetInt32(0), NullableData.GetString(dr, 1)));
                }

                dr.Close();
            }

            // LOAD ANY PAYMENT GATEWAYS MARKED FOR RECRYPTION
            int remainingRows = maximumRows - results.Count;
            if (remainingRows > 0)
            {
                sql = "SELECT TOP " + remainingRows + " PaymentGatewayId,ConfigData FROM ac_PaymentGateways WHERE StoreId=" + storeId + " AND ReCrypt=1";
                selectCommand = database.GetSqlStringCommand(sql);
                using (IDataReader dr = database.ExecuteReader(selectCommand))
                {
                    while (dr.Read())
                    {
                        results.Add(new RecryptRecord("ac_PaymentGateways", dr.GetInt32(0), NullableData.GetString(dr, 1)));
                    }

                    dr.Close();
                }
            }

            // LOAD ANY SHIPGATEWAYS MARKED FOR RECRYPTION
            remainingRows = maximumRows - results.Count;
            if (remainingRows > 0)
            {
                sql = "SELECT TOP " + remainingRows + " ShipGatewayId,ConfigData FROM ac_ShipGateways WHERE StoreId=" + storeId + " AND ReCrypt=1";
                selectCommand = database.GetSqlStringCommand(sql);
                using (IDataReader dr = database.ExecuteReader(selectCommand))
                {
                    while (dr.Read())
                    {
                        results.Add(new RecryptRecord("ac_ShipGateways", dr.GetInt32(0), NullableData.GetString(dr, 1)));
                    }

                    dr.Close();
                }
            }

            // LOAD ANY TAXGATEWAYS MARKED FOR RECRYPTION
            remainingRows = maximumRows - results.Count;
            if (remainingRows > 0)
            {
                sql = "SELECT TOP " + remainingRows + " TaxGatewayId,ConfigData FROM ac_TaxGateways WHERE StoreId=" + storeId + " AND ReCrypt=1";
                selectCommand = database.GetSqlStringCommand(sql);
                using (IDataReader dr = database.ExecuteReader(selectCommand))
                {
                    while (dr.Read())
                    {
                        results.Add(new RecryptRecord("ac_TaxGateways", dr.GetInt32(0), NullableData.GetString(dr, 1)));
                    }

                    dr.Close();
                }
            }

            // RETURN ALL LOADED RECORSD
            return results;
        }

        /// <summary>
        /// Contains an element of data to be recrypted
        /// </summary>
        private class RecryptRecord
        {
            /// <summary>
            /// Private storage for TableName property
            /// </summary>
            private string tableName;

            /// <summary>
            /// Private storate for PrimaryKey property
            /// </summary>
            private int primaryKey;

            /// <summary>
            /// Private storage for EncryptedData property
            /// </summary>
            private string encryptedData;

            /// <summary>
            /// Initializes a new instance of the RecryptRecord class
            /// </summary>
            /// <param name="tableName">Name of the table the data is from</param>
            /// <param name="primaryKey">Value for the primary key</param>
            /// <param name="encryptedData">Data to be encrypted / recrypted</param>
            public RecryptRecord(string tableName, int primaryKey, string encryptedData)
            {
                this.tableName = tableName;
                this.primaryKey = primaryKey;
                this.encryptedData = encryptedData;
            }

            /// <summary>
            /// Gets the table name
            /// </summary>
            public string TableName
            {
                get { return this.tableName; }
            }

            /// <summary>
            /// Gets the primary key value
            /// </summary>
            public int PrimaryKey
            {
                get { return this.primaryKey; }
            }

            /// <summary>
            /// Gets the encrypted data
            /// </summary>
            public string EncryptedData
            {
                get { return this.encryptedData; }
            }

            /// <summary>
            /// Re-encrypt the data
            /// </summary>
            /// <param name="oldKey">The old encryption key</param>
            /// <param name="newKey">The new encryption key</param>
            public void DoRecrypt(byte[] oldKey, byte[] newKey)
            {
                string recryptedValue = RecryptRecord.RecryptValue(this.EncryptedData, oldKey, newKey);
                if (this.EncryptedData != recryptedValue)
                {
                    if (RecryptRecord.SaveRecryptedData(this.TableName, this.PrimaryKey, recryptedValue))
                    {
                        this.encryptedData = recryptedValue;
                    }
                }
            }

            /// <summary>
            /// Recrypts the given value
            /// </summary>
            /// <param name="encryptedData">The original value to be recrypted</param>
            /// <param name="oldKey">The old key that the original value is encrypted with</param>
            /// <param name="newKey">The new key to use for encrypting the value</param>
            /// <returns>The recrypted value</returns>
            /// <remarks>If the input encryptedData value is not already encrypted (with oldKey), it will 
            /// still be properly encrypted with newKey</remarks>
            private static string RecryptValue(string encryptedData, byte[] oldKey, byte[] newKey)
            {
                string recryptedValue = string.Empty;
                if (!string.IsNullOrEmpty(encryptedData))
                {
                    string decryptedValue = EncryptionHelper.DecryptAES(encryptedData, oldKey);
                    recryptedValue = EncryptionHelper.EncryptAES(decryptedValue, newKey);
                }

                return recryptedValue;
            }

            /// <summary>
            /// Updates the encrypted data for a record being re-encrypted
            /// </summary>
            /// <param name="tableName">Name of the table to update</param>
            /// <param name="primaryKey">Primary key ID for the record to be updated</param>
            /// <param name="encryptedData">Re-encrypted data to save for the specfied record</param>
            /// <returns>True if the update is successful</returns>
            private static bool SaveRecryptedData(string tableName, int primaryKey, string encryptedData)
            {
                // DETERMINE APPROPRIATE FIELD NAMES
                string dataFieldName, keyFieldName;
                switch (tableName)
                {
                    case "ac_Payments":
                        dataFieldName = "EncryptedAccountData";
                        keyFieldName = "PaymentId";
                        break;
                    case "ac_PaymentGateways":
                        dataFieldName = "ConfigData";
                        keyFieldName = "PaymentGatewayId";
                        break;
                    case "ac_ShipGateways":
                        dataFieldName = "ConfigData";
                        keyFieldName = "ShipGatewayId";
                        break;
                    case "ac_TaxGateways":
                        dataFieldName = "ConfigData";
                        keyFieldName = "TaxGatewayId";
                        break;
                    default:
                        throw new ArgumentException("tableName must be one of these values: ac_Payments, ac_PaymentGateways, ac_ShipGateways, ac_TaxGateways", "tableName");
                }

                // EXECUTE THE UPDATE QUERY
                Database database = Token.Instance.Database;
                DbCommand updateCommand = database.GetSqlStringCommand("UPDATE " + tableName + " SET " + dataFieldName + " = @encryptedData, ReCrypt = 0 WHERE " + keyFieldName + " = @primaryKey");
                database.AddInParameter(updateCommand, "@encryptedData", DbType.String, NullableData.DbNullify(encryptedData));
                database.AddInParameter(updateCommand, "@primaryKey", DbType.Int32, primaryKey);
                int affected = AlwaysConvert.ToInt(database.ExecuteNonQuery(updateCommand));
                return affected == 1;
            }
        }
    }
}