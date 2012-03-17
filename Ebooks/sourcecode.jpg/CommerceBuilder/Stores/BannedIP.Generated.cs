//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// This class represents a BannedIP object in the database.
    /// </summary>
    public partial class BannedIP : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BannedIP() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="bannedIPId">Value of BannedIPId.</param>
        /// </summary>
        public BannedIP(Int32 bannedIPId)
        {
            this.BannedIPId = bannedIPId;
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
          columnNames.Add(prefix + "BannedIPId");
          columnNames.Add(prefix + "StoreId");
          columnNames.Add(prefix + "IPRangeStart");
          columnNames.Add(prefix + "IPRangeEnd");
          columnNames.Add(prefix + "Comment");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given BannedIP object from the given database data reader.
        /// </summary>
        /// <param name="bannedIP">The BannedIP object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(BannedIP bannedIP, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            bannedIP.BannedIPId = dr.GetInt32(0);
            bannedIP.StoreId = dr.GetInt32(1);
            bannedIP.IPRangeStart = dr.GetInt64(2);
            bannedIP.IPRangeEnd = dr.GetInt64(3);
            bannedIP.Comment = NullableData.GetString(dr, 4);
            bannedIP.IsDirty = false;
        }

#endregion

        private Int32 _BannedIPId;
        private Int32 _StoreId;
        private Int64 _IPRangeStart;
        private Int64 _IPRangeEnd;
        private String _Comment = string.Empty;
        private bool _IsDirty;

        /// <summary>
        /// BannedIPId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 BannedIPId
        {
            get { return this._BannedIPId; }
            set
            {
                if (this._BannedIPId != value)
                {
                    this._BannedIPId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// StoreId
        /// </summary>
        public Int32 StoreId
        {
            get { return this._StoreId; }
            set
            {
                if (this._StoreId != value)
                {
                    this._StoreId = value;
                    this.IsDirty = true;
                    this._Store = null;
                }
            }
        }

        /// <summary>
        /// IPRangeStart
        /// </summary>
        public Int64 IPRangeStart
        {
            get { return this._IPRangeStart; }
            set
            {
                if (this._IPRangeStart != value)
                {
                    this._IPRangeStart = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// IPRangeEnd
        /// </summary>
        public Int64 IPRangeEnd
        {
            get { return this._IPRangeEnd; }
            set
            {
                if (this._IPRangeEnd != value)
                {
                    this._IPRangeEnd = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Comment
        /// </summary>
        public String Comment
        {
            get { return this._Comment; }
            set
            {
                if (this._Comment != value)
                {
                    this._Comment = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this BannedIP object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private Store _Store;

        /// <summary>
        /// The Store object that this BannedIP object is associated with
        /// </summary>
        public Store Store
        {
            get
            {
                if (!this.StoreLoaded)
                {
                    this._Store = StoreDataSource.Load(this.StoreId);
                }
                return this._Store;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool StoreLoaded { get { return ((this._Store != null) && (this._Store.StoreId == this.StoreId)); } }

#endregion

        /// <summary>
        /// Deletes this BannedIP object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_BannedIPs");
            deleteQuery.Append(" WHERE BannedIPId = @BannedIPId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@BannedIPId", System.Data.DbType.Int32, this.BannedIPId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this BannedIP object from the database for the given primary key.
        /// </summary>
        /// <param name="bannedIPId">Value of BannedIPId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 bannedIPId)
        {
            bool result = false;
            this.BannedIPId = bannedIPId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_BannedIPs");
            selectQuery.Append(" WHERE BannedIPId = @bannedIPId");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@bannedIPId", System.Data.DbType.Int32, bannedIPId);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
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
        /// Saves this BannedIP object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;
                
                //SET EMPTY STOREID TO CURRENT CONTEXT
                if (this.StoreId == 0) this.StoreId = Token.Instance.StoreId;
                if (this.BannedIPId == 0) recordExists = false;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_BannedIPs");
                    selectQuery.Append(" WHERE BannedIPId = @BannedIPId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@BannedIPId", System.Data.DbType.Int32, this.BannedIPId);
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
                    updateQuery.Append("UPDATE ac_BannedIPs SET ");
                    updateQuery.Append("StoreId = @StoreId");
                    updateQuery.Append(", IPRangeStart = @IPRangeStart");
                    updateQuery.Append(", IPRangeEnd = @IPRangeEnd");
                    updateQuery.Append(", Comment = @Comment");
                    updateQuery.Append(" WHERE BannedIPId = @BannedIPId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@BannedIPId", System.Data.DbType.Int32, this.BannedIPId);
                        database.AddInParameter(updateCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(updateCommand, "@IPRangeStart", System.Data.DbType.Int64, this.IPRangeStart);
                        database.AddInParameter(updateCommand, "@IPRangeEnd", System.Data.DbType.Int64, this.IPRangeEnd);
                        database.AddInParameter(updateCommand, "@Comment", System.Data.DbType.String, NullableData.DbNullify(this.Comment));
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_BannedIPs (StoreId, IPRangeStart, IPRangeEnd, Comment)");
                    insertQuery.Append(" VALUES (@StoreId, @IPRangeStart, @IPRangeEnd, @Comment)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@BannedIPId", System.Data.DbType.Int32, this.BannedIPId);
                        database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(insertCommand, "@IPRangeStart", System.Data.DbType.Int64, this.IPRangeStart);
                        database.AddInParameter(insertCommand, "@IPRangeEnd", System.Data.DbType.Int64, this.IPRangeEnd);
                        database.AddInParameter(insertCommand, "@Comment", System.Data.DbType.String, NullableData.DbNullify(this.Comment));
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._BannedIPId = result;
                    }
                }

                //OBJECT IS DIRTY IF NO RECORDS WERE UPDATED OR INSERTED
                this.IsDirty = (result == 0);
                if (this.IsDirty) { return SaveResult.Failed; }
                else { return (recordExists ? SaveResult.RecordUpdated : SaveResult.RecordInserted); }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

     }
}
