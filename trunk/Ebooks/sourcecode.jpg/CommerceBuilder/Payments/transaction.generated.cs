//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// This class represents a Transaction object in the database.
    /// </summary>
    public partial class Transaction : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Transaction() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="transactionId">Value of TransactionId.</param>
        /// </summary>
        public Transaction(Int32 transactionId)
        {
            this.TransactionId = transactionId;
        }

        /// <summary>
        /// Returns a coma separated list of column names in this database object.
        /// </summary>
        /// <param name="prefix">Prefix to use with column names. Leave null or empty for no prefix.</param>
        /// <returns>A coman separated list of column names for this database object.</returns>
        public static string GetColumnNames(string prefix)
        {
          if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
          else prefix = prefix + ".";
          List<string> columnNames = new List<string>();
          columnNames.Add(prefix + "TransactionId");
          columnNames.Add(prefix + "TransactionTypeId");
          columnNames.Add(prefix + "PaymentId");
          columnNames.Add(prefix + "PaymentGatewayId");
          columnNames.Add(prefix + "ProviderTransactionId");
          columnNames.Add(prefix + "TransactionDate");
          columnNames.Add(prefix + "Amount");
          columnNames.Add(prefix + "TransactionStatusId");
          columnNames.Add(prefix + "ResponseCode");
          columnNames.Add(prefix + "ResponseMessage");
          columnNames.Add(prefix + "AuthorizationCode");
          columnNames.Add(prefix + "AVSResultCode");
          columnNames.Add(prefix + "CVVResultCode");
          columnNames.Add(prefix + "CAVResultCode");
          columnNames.Add(prefix + "RemoteIP");
          columnNames.Add(prefix + "Referrer");
          columnNames.Add(prefix + "AdditionalData");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given Transaction object from the given database data reader.
        /// </summary>
        /// <param name="transaction">The Transaction object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(Transaction transaction, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            transaction.TransactionId = dr.GetInt32(0);
            transaction.TransactionTypeId = dr.GetInt16(1);
            transaction.PaymentId = dr.GetInt32(2);
            transaction.PaymentGatewayId = NullableData.GetInt32(dr, 3);
            transaction.ProviderTransactionId = NullableData.GetString(dr, 4);
            transaction.TransactionDate = LocaleHelper.ToLocalTime(dr.GetDateTime(5));
            transaction.Amount = dr.GetDecimal(6);
            transaction.TransactionStatusId = dr.GetInt16(7);
            transaction.ResponseCode = NullableData.GetString(dr, 8);
            transaction.ResponseMessage = NullableData.GetString(dr, 9);
            transaction.AuthorizationCode = NullableData.GetString(dr, 10);
            transaction.AVSResultCode = NullableData.GetString(dr, 11);
            transaction.CVVResultCode = NullableData.GetString(dr, 12);
            transaction.CAVResultCode = NullableData.GetString(dr, 13);
            transaction.RemoteIP = NullableData.GetString(dr, 14);
            transaction.Referrer = NullableData.GetString(dr, 15);
            transaction.AdditionalData = NullableData.GetString(dr, 16);
            transaction.IsDirty = false;
        }

#endregion

        private Int32 _TransactionId;
        private Int16 _TransactionTypeId;
        private Int32 _PaymentId;
        private Int32 _PaymentGatewayId;
        private String _ProviderTransactionId = string.Empty;
        private DateTime _TransactionDate;
        private LSDecimal _Amount;
        private Int16 _TransactionStatusId;
        private String _ResponseCode = string.Empty;
        private String _ResponseMessage = string.Empty;
        private String _AuthorizationCode = string.Empty;
        private String _AVSResultCode = string.Empty;
        private String _CVVResultCode = string.Empty;
        private String _CAVResultCode = string.Empty;
        private String _RemoteIP = string.Empty;
        private String _Referrer = string.Empty;
        private String _AdditionalData = string.Empty;
        private bool _IsDirty;

        /// <summary>
        /// TransactionId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 TransactionId
        {
            get { return this._TransactionId; }
            set
            {
                if (this._TransactionId != value)
                {
                    this._TransactionId = value;
                    this.IsDirty = true;
                    this.EnsureChildProperties();
                }
            }
        }

        /// <summary>
        /// TransactionTypeId
        /// </summary>
        public Int16 TransactionTypeId
        {
            get { return this._TransactionTypeId; }
            set
            {
                if (this._TransactionTypeId != value)
                {
                    this._TransactionTypeId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// PaymentId
        /// </summary>
        public Int32 PaymentId
        {
            get { return this._PaymentId; }
            set
            {
                if (this._PaymentId != value)
                {
                    this._PaymentId = value;
                    this.IsDirty = true;
                    this._Payment = null;
                }
            }
        }

        /// <summary>
        /// PaymentGatewayId
        /// </summary>
        public Int32 PaymentGatewayId
        {
            get { return this._PaymentGatewayId; }
            set
            {
                if (this._PaymentGatewayId != value)
                {
                    this._PaymentGatewayId = value;
                    this.IsDirty = true;
                    this._PaymentGateway = null;
                }
            }
        }

        /// <summary>
        /// ProviderTransactionId
        /// </summary>
        public String ProviderTransactionId
        {
            get { return this._ProviderTransactionId; }
            set
            {
                if (this._ProviderTransactionId != value)
                {
                    this._ProviderTransactionId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// TransactionDate
        /// </summary>
        public DateTime TransactionDate
        {
            get { return this._TransactionDate; }
            set
            {
                if (this._TransactionDate != value)
                {
                    this._TransactionDate = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Amount
        /// </summary>
        public LSDecimal Amount
        {
            get { return this._Amount; }
            set
            {
                if (this._Amount != value)
                {
                    this._Amount = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// TransactionStatusId
        /// </summary>
        public Int16 TransactionStatusId
        {
            get { return this._TransactionStatusId; }
            set
            {
                if (this._TransactionStatusId != value)
                {
                    this._TransactionStatusId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// ResponseCode
        /// </summary>
        public String ResponseCode
        {
            get { return this._ResponseCode; }
            set
            {
                if (this._ResponseCode != value)
                {
                    this._ResponseCode = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// ResponseMessage
        /// </summary>
        public String ResponseMessage
        {
            get { return this._ResponseMessage; }
            set
            {
                if (this._ResponseMessage != value)
                {
                    this._ResponseMessage = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// AuthorizationCode
        /// </summary>
        public String AuthorizationCode
        {
            get { return this._AuthorizationCode; }
            set
            {
                if (this._AuthorizationCode != value)
                {
                    this._AuthorizationCode = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// AVSResultCode
        /// </summary>
        public String AVSResultCode
        {
            get { return this._AVSResultCode; }
            set
            {
                if (this._AVSResultCode != value)
                {
                    this._AVSResultCode = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// CVVResultCode
        /// </summary>
        public String CVVResultCode
        {
            get { return this._CVVResultCode; }
            set
            {
                if (this._CVVResultCode != value)
                {
                    this._CVVResultCode = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// CAVResultCode
        /// </summary>
        public String CAVResultCode
        {
            get { return this._CAVResultCode; }
            set
            {
                if (this._CAVResultCode != value)
                {
                    this._CAVResultCode = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// RemoteIP
        /// </summary>
        public String RemoteIP
        {
            get { return this._RemoteIP; }
            set
            {
                if (this._RemoteIP != value)
                {
                    this._RemoteIP = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Referrer
        /// </summary>
        public String Referrer
        {
            get { return this._Referrer; }
            set
            {
                if (this._Referrer != value)
                {
                    this._Referrer = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// AdditionalData
        /// </summary>
        public String AdditionalData
        {
            get { return this._AdditionalData; }
            set
            {
                if (this._AdditionalData != value)
                {
                    this._AdditionalData = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this Transaction object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                if (this._IsDirty) return true;
                if (this.SubscriptionsLoaded && this.Subscriptions.IsDirty) return true;
                return false;
            }
            set { this._IsDirty = value; }
        }

        /// <summary>
        /// Ensures that child objects of this Transaction are properly associated with this Transaction object.
        /// </summary>
        public virtual void EnsureChildProperties()
        {
            if (this.SubscriptionsLoaded) { foreach (Subscription subscription in this.Subscriptions) { subscription.TransactionId = this.TransactionId; } }
        }

#region Parents
        private PaymentGateway _PaymentGateway;
        private Payment _Payment;

        /// <summary>
        /// The PaymentGateway object that this Transaction object is associated with
        /// </summary>
        public PaymentGateway PaymentGateway
        {
            get
            {
                if (!this.PaymentGatewayLoaded)
                {
                    this._PaymentGateway = PaymentGatewayDataSource.Load(this.PaymentGatewayId);
                }
                return this._PaymentGateway;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool PaymentGatewayLoaded { get { return ((this._PaymentGateway != null) && (this._PaymentGateway.PaymentGatewayId == this.PaymentGatewayId)); } }

        /// <summary>
        /// The Payment object that this Transaction object is associated with
        /// </summary>
        public Payment Payment
        {
            get
            {
                if (!this.PaymentLoaded)
                {
                    this._Payment = PaymentDataSource.Load(this.PaymentId);
                }
                return this._Payment;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool PaymentLoaded { get { return ((this._Payment != null) && (this._Payment.PaymentId == this.PaymentId)); } }

#endregion

#region Children
        private SubscriptionCollection _Subscriptions;

        /// <summary>
        /// A collection of Subscription objects associated with this Transaction object.
        /// </summary>
        public SubscriptionCollection Subscriptions
        {
            get
            {
                if (!this.SubscriptionsLoaded)
                {
                    this._Subscriptions = SubscriptionDataSource.LoadForTransaction(this.TransactionId);
                }
                return this._Subscriptions;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool SubscriptionsLoaded { get { return (this._Subscriptions != null); } }

#endregion

        /// <summary>
        /// Deletes this Transaction object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        protected bool BaseDelete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_Transactions");
            deleteQuery.Append(" WHERE TransactionId = @TransactionId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@TransactionId", System.Data.DbType.Int32, this.TransactionId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this Transaction object from the database for the given primary key.
        /// </summary>
        /// <param name="transactionId">Value of TransactionId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 transactionId)
        {
            bool result = false;
            this.TransactionId = transactionId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Transactions");
            selectQuery.Append(" WHERE TransactionId = @transactionId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@transactionId", System.Data.DbType.Int32, transactionId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    result = true;
                    LoadDataReader(this, dr);;
                }
                dr.Close();
            }
            return result;
        }

        /// <summary>
        /// Saves this Transaction object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;
                
                if (this.TransactionId == 0) recordExists = false;

                //SET DEFAULT FOR DATE FIELD
                if (this.TransactionDate == System.DateTime.MinValue) this.TransactionDate = LocaleHelper.LocalNow;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_Transactions");
                    selectQuery.Append(" WHERE TransactionId = @TransactionId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@TransactionId", System.Data.DbType.Int32, this.TransactionId);
                        if ((int)database.ExecuteScalar(selectCommand) == 0)
                        {
                            recordExists = false;
                        }
                    }
                }

                int result = 0;
                if (recordExists)
                {
                    //UPDATE
                    StringBuilder updateQuery = new StringBuilder();
                    updateQuery.Append("UPDATE ac_Transactions SET ");
                    updateQuery.Append("TransactionTypeId = @TransactionTypeId");
                    updateQuery.Append(", PaymentId = @PaymentId");
                    updateQuery.Append(", PaymentGatewayId = @PaymentGatewayId");
                    updateQuery.Append(", ProviderTransactionId = @ProviderTransactionId");
                    updateQuery.Append(", TransactionDate = @TransactionDate");
                    updateQuery.Append(", Amount = @Amount");
                    updateQuery.Append(", TransactionStatusId = @TransactionStatusId");
                    updateQuery.Append(", ResponseCode = @ResponseCode");
                    updateQuery.Append(", ResponseMessage = @ResponseMessage");
                    updateQuery.Append(", AuthorizationCode = @AuthorizationCode");
                    updateQuery.Append(", AVSResultCode = @AVSResultCode");
                    updateQuery.Append(", CVVResultCode = @CVVResultCode");
                    updateQuery.Append(", CAVResultCode = @CAVResultCode");
                    updateQuery.Append(", RemoteIP = @RemoteIP");
                    updateQuery.Append(", Referrer = @Referrer");
                    updateQuery.Append(", AdditionalData = @AdditionalData");
                    updateQuery.Append(" WHERE TransactionId = @TransactionId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@TransactionId", System.Data.DbType.Int32, this.TransactionId);
                        database.AddInParameter(updateCommand, "@TransactionTypeId", System.Data.DbType.Int16, this.TransactionTypeId);
                        database.AddInParameter(updateCommand, "@PaymentId", System.Data.DbType.Int32, this.PaymentId);
                        database.AddInParameter(updateCommand, "@PaymentGatewayId", System.Data.DbType.Int32, NullableData.DbNullify(this.PaymentGatewayId));
                        database.AddInParameter(updateCommand, "@ProviderTransactionId", System.Data.DbType.String, NullableData.DbNullify(this.ProviderTransactionId));
                        database.AddInParameter(updateCommand, "@TransactionDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.TransactionDate));
                        database.AddInParameter(updateCommand, "@Amount", System.Data.DbType.Decimal, this.Amount);
                        database.AddInParameter(updateCommand, "@TransactionStatusId", System.Data.DbType.Int16, this.TransactionStatusId);
                        database.AddInParameter(updateCommand, "@ResponseCode", System.Data.DbType.String, NullableData.DbNullify(this.ResponseCode));
                        database.AddInParameter(updateCommand, "@ResponseMessage", System.Data.DbType.String, NullableData.DbNullify(this.ResponseMessage));
                        database.AddInParameter(updateCommand, "@AuthorizationCode", System.Data.DbType.String, NullableData.DbNullify(this.AuthorizationCode));
                        database.AddInParameter(updateCommand, "@AVSResultCode", System.Data.DbType.String, NullableData.DbNullify(this.AVSResultCode));
                        database.AddInParameter(updateCommand, "@CVVResultCode", System.Data.DbType.String, NullableData.DbNullify(this.CVVResultCode));
                        database.AddInParameter(updateCommand, "@CAVResultCode", System.Data.DbType.String, NullableData.DbNullify(this.CAVResultCode));
                        database.AddInParameter(updateCommand, "@RemoteIP", System.Data.DbType.String, NullableData.DbNullify(this.RemoteIP));
                        database.AddInParameter(updateCommand, "@Referrer", System.Data.DbType.String, NullableData.DbNullify(this.Referrer));
                        database.AddInParameter(updateCommand, "@AdditionalData", System.Data.DbType.String, NullableData.DbNullify(this.AdditionalData));
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_Transactions (TransactionTypeId, PaymentId, PaymentGatewayId, ProviderTransactionId, TransactionDate, Amount, TransactionStatusId, ResponseCode, ResponseMessage, AuthorizationCode, AVSResultCode, CVVResultCode, CAVResultCode, RemoteIP, Referrer, AdditionalData)");
                    insertQuery.Append(" VALUES (@TransactionTypeId, @PaymentId, @PaymentGatewayId, @ProviderTransactionId, @TransactionDate, @Amount, @TransactionStatusId, @ResponseCode, @ResponseMessage, @AuthorizationCode, @AVSResultCode, @CVVResultCode, @CAVResultCode, @RemoteIP, @Referrer, @AdditionalData)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@TransactionId", System.Data.DbType.Int32, this.TransactionId);
                        database.AddInParameter(insertCommand, "@TransactionTypeId", System.Data.DbType.Int16, this.TransactionTypeId);
                        database.AddInParameter(insertCommand, "@PaymentId", System.Data.DbType.Int32, this.PaymentId);
                        database.AddInParameter(insertCommand, "@PaymentGatewayId", System.Data.DbType.Int32, NullableData.DbNullify(this.PaymentGatewayId));
                        database.AddInParameter(insertCommand, "@ProviderTransactionId", System.Data.DbType.String, NullableData.DbNullify(this.ProviderTransactionId));
                        database.AddInParameter(insertCommand, "@TransactionDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.TransactionDate));
                        database.AddInParameter(insertCommand, "@Amount", System.Data.DbType.Decimal, this.Amount);
                        database.AddInParameter(insertCommand, "@TransactionStatusId", System.Data.DbType.Int16, this.TransactionStatusId);
                        database.AddInParameter(insertCommand, "@ResponseCode", System.Data.DbType.String, NullableData.DbNullify(this.ResponseCode));
                        database.AddInParameter(insertCommand, "@ResponseMessage", System.Data.DbType.String, NullableData.DbNullify(this.ResponseMessage));
                        database.AddInParameter(insertCommand, "@AuthorizationCode", System.Data.DbType.String, NullableData.DbNullify(this.AuthorizationCode));
                        database.AddInParameter(insertCommand, "@AVSResultCode", System.Data.DbType.String, NullableData.DbNullify(this.AVSResultCode));
                        database.AddInParameter(insertCommand, "@CVVResultCode", System.Data.DbType.String, NullableData.DbNullify(this.CVVResultCode));
                        database.AddInParameter(insertCommand, "@CAVResultCode", System.Data.DbType.String, NullableData.DbNullify(this.CAVResultCode));
                        database.AddInParameter(insertCommand, "@RemoteIP", System.Data.DbType.String, NullableData.DbNullify(this.RemoteIP));
                        database.AddInParameter(insertCommand, "@Referrer", System.Data.DbType.String, NullableData.DbNullify(this.Referrer));
                        database.AddInParameter(insertCommand, "@AdditionalData", System.Data.DbType.String, NullableData.DbNullify(this.AdditionalData));
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._TransactionId = result;
                    }
                }
                this.SaveChildren();

                //OBJECT IS DIRTY IF NO RECORDS WERE UPDATED OR INSERTED
                this.IsDirty = (result == 0);
                if (this.IsDirty) { return SaveResult.Failed; }
                else { return (recordExists ? SaveResult.RecordUpdated : SaveResult.RecordInserted); }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

        /// <summary>
        /// Saves that child objects associated with this Transaction object.
        /// </summary>
        public virtual void SaveChildren()
        {
            this.EnsureChildProperties();
            if (this.SubscriptionsLoaded) this.Subscriptions.Save();
        }

     }
}