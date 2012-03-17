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

namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// This class represents a CustomUrl object in the database.
    /// </summary>
    public partial class CustomUrl : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CustomUrl() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="customUrlId">Value of CustomUrlId.</param>
        /// </summary>
        public CustomUrl(Int32 customUrlId)
        {
            this.CustomUrlId = customUrlId;
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
          columnNames.Add(prefix + "CustomUrlId");
          columnNames.Add(prefix + "StoreId");
          columnNames.Add(prefix + "CatalogNodeId");
          columnNames.Add(prefix + "CatalogNodeTypeId");
          columnNames.Add(prefix + "Url");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given CustomUrl object from the given database data reader.
        /// </summary>
        /// <param name="customUrl">The CustomUrl object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(CustomUrl customUrl, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            customUrl.CustomUrlId = dr.GetInt32(0);
            customUrl.StoreId = dr.GetInt32(1);
            customUrl.CatalogNodeId = dr.GetInt32(2);
            customUrl.CatalogNodeTypeId = dr.GetByte(3);
            customUrl.Url = dr.GetString(4);
            customUrl.IsDirty = false;
        }

#endregion

        private Int32 _CustomUrlId;
        private Int32 _StoreId;
        private Int32 _CatalogNodeId;
        private Byte _CatalogNodeTypeId;
        private String _Url = string.Empty;
        private String _LoweredUrl = string.Empty;
        private bool _IsDirty;

        /// <summary>
        /// CustomUrlId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 CustomUrlId
        {
            get { return this._CustomUrlId; }
            set
            {
                if (this._CustomUrlId != value)
                {
                    this._CustomUrlId = value;
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
        /// CatalogNodeId
        /// </summary>
        public Int32 CatalogNodeId
        {
            get { return this._CatalogNodeId; }
            set
            {
                if (this._CatalogNodeId != value)
                {
                    this._CatalogNodeId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// CatalogNodeTypeId
        /// </summary>
        public Byte CatalogNodeTypeId
        {
            get { return this._CatalogNodeTypeId; }
            set
            {
                if (this._CatalogNodeTypeId != value)
                {
                    this._CatalogNodeTypeId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Url
        /// </summary>
        public String Url
        {
            get { return this._Url; }
            set
            {
                if (this._Url != value)
                {
                    this._Url = value;
                    this.IsDirty = true;
                    this._LoweredUrl = value.ToLowerInvariant();
                }
            }
        }

        /// <summary>
        /// LoweredUrl
        /// </summary>
        public String LoweredUrl
        {
            get { return this._LoweredUrl; }
        }

        /// <summary>
        /// Indicates whether this CustomUrl object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private Store _Store;

        /// <summary>
        /// The Store object that this CustomUrl object is associated with
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
        /// Deletes this CustomUrl object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        protected bool BaseDelete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_CustomUrls");
            deleteQuery.Append(" WHERE CustomUrlId = @customUrlId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@CustomUrlId", System.Data.DbType.Int32, this.CustomUrlId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this CustomUrl object from the database for the given primary key.
        /// </summary>
        /// <param name="customUrlId">Value of CustomUrlId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 customUrlId)
        {
            bool result = false;
            this.CustomUrlId = customUrlId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_CustomUrls");
            selectQuery.Append(" WHERE CustomUrlId = @customUrlId");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@customUrlId", System.Data.DbType.Int32, customUrlId);
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
        /// Saves this CustomUrl object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        protected SaveResult BaseSave()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;
                
                //SET EMPTY STOREID TO CURRENT CONTEXT
                if (this.StoreId == 0) this.StoreId = Token.Instance.StoreId;
                if (this.CustomUrlId == 0) recordExists = false;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_CustomUrls");
                    selectQuery.Append(" WHERE CustomUrlId = @customUrlId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@CustomUrlId", System.Data.DbType.Int32, this.CustomUrlId);
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
                    updateQuery.Append("UPDATE ac_CustomUrls SET ");
                    updateQuery.Append("StoreId = @StoreId");
                    updateQuery.Append(", CatalogNodeId = @CatalogNodeId");
                    updateQuery.Append(", CatalogNodeTypeId = @CatalogNodeTypeId");
                    updateQuery.Append(", Url = @Url");
                    updateQuery.Append(", LoweredUrl = @LoweredUrl");
                    updateQuery.Append(" WHERE CustomUrlId = @CustomUrlId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@CustomUrlId", System.Data.DbType.Int32, this.CustomUrlId);
                        database.AddInParameter(updateCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(updateCommand, "@CatalogNodeId", System.Data.DbType.Int32, this.CatalogNodeId);
                        database.AddInParameter(updateCommand, "@CatalogNodeTypeId", System.Data.DbType.Byte, this.CatalogNodeTypeId);
                        database.AddInParameter(updateCommand, "@Url", System.Data.DbType.String, this.Url);
                        database.AddInParameter(updateCommand, "@LoweredUrl", System.Data.DbType.String, this.LoweredUrl);
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_CustomUrls (StoreId, CatalogNodeId, CatalogNodeTypeId, Url, LoweredUrl)");
                    insertQuery.Append(" VALUES (@StoreId, @CatalogNodeId, @CatalogNodeTypeId, @Url, @LoweredUrl)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@CustomUrlId", System.Data.DbType.Int32, this.CustomUrlId);
                        database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(insertCommand, "@CatalogNodeId", System.Data.DbType.Int32, this.CatalogNodeId);
                        database.AddInParameter(insertCommand, "@CatalogNodeTypeId", System.Data.DbType.Byte, this.CatalogNodeTypeId);
                        database.AddInParameter(insertCommand, "@Url", System.Data.DbType.String, this.Url);
                        database.AddInParameter(insertCommand, "@LoweredUrl", System.Data.DbType.String, this.LoweredUrl);
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._CustomUrlId = result;
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