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
    /// This class represents a CustomField object in the database.
    /// </summary>
    public partial class CustomField : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CustomField() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="customFieldId">Value of CustomFieldId.</param>
        /// </summary>
        public CustomField(Int32 customFieldId)
        {
            this.CustomFieldId = customFieldId;
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
          columnNames.Add(prefix + "CustomFieldId");
          columnNames.Add(prefix + "StoreId");
          columnNames.Add(prefix + "TableName");
          columnNames.Add(prefix + "ForeignKeyId");
          columnNames.Add(prefix + "FieldName");
          columnNames.Add(prefix + "FieldValue");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given CustomField object from the given database data reader.
        /// </summary>
        /// <param name="customField">The CustomField object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(CustomField customField, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            customField.CustomFieldId = dr.GetInt32(0);
            customField.StoreId = dr.GetInt32(1);
            customField.TableName = dr.GetString(2);
            customField.ForeignKeyId = dr.GetInt32(3);
            customField.FieldName = dr.GetString(4);
            customField.FieldValue = dr.GetString(5);
            customField.IsDirty = false;
        }

#endregion

        private Int32 _CustomFieldId;
        private Int32 _StoreId;
        private String _TableName = string.Empty;
        private Int32 _ForeignKeyId;
        private String _FieldName = string.Empty;
        private String _FieldValue = string.Empty;
        private bool _IsDirty;

        /// <summary>
        /// CustomFieldId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 CustomFieldId
        {
            get { return this._CustomFieldId; }
            set
            {
                if (this._CustomFieldId != value)
                {
                    this._CustomFieldId = value;
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
        /// TableName
        /// </summary>
        public String TableName
        {
            get { return this._TableName; }
            set
            {
                if (this._TableName != value)
                {
                    this._TableName = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// ForeignKeyId
        /// </summary>
        public Int32 ForeignKeyId
        {
            get { return this._ForeignKeyId; }
            set
            {
                if (this._ForeignKeyId != value)
                {
                    this._ForeignKeyId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// FieldName
        /// </summary>
        public String FieldName
        {
            get { return this._FieldName; }
            set
            {
                if (this._FieldName != value)
                {
                    this._FieldName = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// FieldValue
        /// </summary>
        public String FieldValue
        {
            get { return this._FieldValue; }
            set
            {
                if (this._FieldValue != value)
                {
                    this._FieldValue = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this CustomField object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private Store _Store;

        /// <summary>
        /// The Store object that this CustomField object is associated with
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
        /// Deletes this CustomField object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_CustomFields");
            deleteQuery.Append(" WHERE CustomFieldId = @CustomFieldId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@CustomFieldId", System.Data.DbType.Int32, this.CustomFieldId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this CustomField object from the database for the given primary key.
        /// </summary>
        /// <param name="customFieldId">Value of CustomFieldId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 customFieldId)
        {
            bool result = false;
            this.CustomFieldId = customFieldId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_CustomFields");
            selectQuery.Append(" WHERE CustomFieldId = @customFieldId");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@customFieldId", System.Data.DbType.Int32, customFieldId);
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
        /// Saves this CustomField object to the database.
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
                if (this.CustomFieldId == 0) recordExists = false;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_CustomFields");
                    selectQuery.Append(" WHERE CustomFieldId = @CustomFieldId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@CustomFieldId", System.Data.DbType.Int32, this.CustomFieldId);
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
                    updateQuery.Append("UPDATE ac_CustomFields SET ");
                    updateQuery.Append("StoreId = @StoreId");
                    updateQuery.Append(", TableName = @TableName");
                    updateQuery.Append(", ForeignKeyId = @ForeignKeyId");
                    updateQuery.Append(", FieldName = @FieldName");
                    updateQuery.Append(", FieldValue = @FieldValue");
                    updateQuery.Append(" WHERE CustomFieldId = @CustomFieldId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@CustomFieldId", System.Data.DbType.Int32, this.CustomFieldId);
                        database.AddInParameter(updateCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(updateCommand, "@TableName", System.Data.DbType.String, this.TableName);
                        database.AddInParameter(updateCommand, "@ForeignKeyId", System.Data.DbType.Int32, this.ForeignKeyId);
                        database.AddInParameter(updateCommand, "@FieldName", System.Data.DbType.String, this.FieldName);
                        database.AddInParameter(updateCommand, "@FieldValue", System.Data.DbType.String, this.FieldValue);
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_CustomFields (StoreId, TableName, ForeignKeyId, FieldName, FieldValue)");
                    insertQuery.Append(" VALUES (@StoreId, @TableName, @ForeignKeyId, @FieldName, @FieldValue)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@CustomFieldId", System.Data.DbType.Int32, this.CustomFieldId);
                        database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(insertCommand, "@TableName", System.Data.DbType.String, this.TableName);
                        database.AddInParameter(insertCommand, "@ForeignKeyId", System.Data.DbType.Int32, this.ForeignKeyId);
                        database.AddInParameter(insertCommand, "@FieldName", System.Data.DbType.String, this.FieldName);
                        database.AddInParameter(insertCommand, "@FieldValue", System.Data.DbType.String, this.FieldValue);
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._CustomFieldId = result;
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