//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Utility;

namespace CommerceBuilder.DigitalDelivery
{
    /// <summary>
    /// This class represents a SerialKey object in the database.
    /// </summary>
    public partial class SerialKey : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SerialKey() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="serialKeyId">Value of SerialKeyId.</param>
        /// </summary>
        public SerialKey(Int32 serialKeyId)
        {
            this.SerialKeyId = serialKeyId;
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
          columnNames.Add(prefix + "SerialKeyId");
          columnNames.Add(prefix + "DigitalGoodId");
          columnNames.Add(prefix + "SerialKeyData");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given SerialKey object from the given database data reader.
        /// </summary>
        /// <param name="serialKey">The SerialKey object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(SerialKey serialKey, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            serialKey.SerialKeyId = dr.GetInt32(0);
            serialKey.DigitalGoodId = dr.GetInt32(1);
            serialKey.SerialKeyData = dr.GetString(2);
            serialKey.IsDirty = false;
        }

#endregion

        private Int32 _SerialKeyId;
        private Int32 _DigitalGoodId;
        private String _SerialKeyData = string.Empty;
        private bool _IsDirty;

        /// <summary>
        /// SerialKeyId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 SerialKeyId
        {
            get { return this._SerialKeyId; }
            set
            {
                if (this._SerialKeyId != value)
                {
                    this._SerialKeyId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// DigitalGoodId
        /// </summary>
        public Int32 DigitalGoodId
        {
            get { return this._DigitalGoodId; }
            set
            {
                if (this._DigitalGoodId != value)
                {
                    this._DigitalGoodId = value;
                    this.IsDirty = true;
                    this._DigitalGood = null;
                }
            }
        }

        /// <summary>
        /// SerialKeyData
        /// </summary>
        public String SerialKeyData
        {
            get { return this._SerialKeyData; }
            set
            {
                if (this._SerialKeyData != value)
                {
                    this._SerialKeyData = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this SerialKey object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private DigitalGood _DigitalGood;

        /// <summary>
        /// The DigitalGood object that this SerialKey object is associated with
        /// </summary>
        public DigitalGood DigitalGood
        {
            get
            {
                if (!this.DigitalGoodLoaded)
                {
                    this._DigitalGood = DigitalGoodDataSource.Load(this.DigitalGoodId);
                }
                return this._DigitalGood;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool DigitalGoodLoaded { get { return ((this._DigitalGood != null) && (this._DigitalGood.DigitalGoodId == this.DigitalGoodId)); } }

#endregion

        /// <summary>
        /// Deletes this SerialKey object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_SerialKeys");
            deleteQuery.Append(" WHERE SerialKeyId = @SerialKeyId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@SerialKeyId", System.Data.DbType.Int32, this.SerialKeyId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this SerialKey object from the database for the given primary key.
        /// </summary>
        /// <param name="serialKeyId">Value of SerialKeyId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 serialKeyId)
        {
            bool result = false;
            this.SerialKeyId = serialKeyId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_SerialKeys");
            selectQuery.Append(" WHERE SerialKeyId = @serialKeyId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@serialKeyId", System.Data.DbType.Int32, serialKeyId);
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
        /// Saves this SerialKey object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;
                
                if (this.SerialKeyId == 0) recordExists = false;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_SerialKeys");
                    selectQuery.Append(" WHERE SerialKeyId = @SerialKeyId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@SerialKeyId", System.Data.DbType.Int32, this.SerialKeyId);
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
                    updateQuery.Append("UPDATE ac_SerialKeys SET ");
                    updateQuery.Append("DigitalGoodId = @DigitalGoodId");
                    updateQuery.Append(", SerialKeyData = @SerialKeyData");
                    updateQuery.Append(" WHERE SerialKeyId = @SerialKeyId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@SerialKeyId", System.Data.DbType.Int32, this.SerialKeyId);
                        database.AddInParameter(updateCommand, "@DigitalGoodId", System.Data.DbType.Int32, this.DigitalGoodId);
                        database.AddInParameter(updateCommand, "@SerialKeyData", System.Data.DbType.String, this.SerialKeyData);
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_SerialKeys (DigitalGoodId, SerialKeyData)");
                    insertQuery.Append(" VALUES (@DigitalGoodId, @SerialKeyData)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@SerialKeyId", System.Data.DbType.Int32, this.SerialKeyId);
                        database.AddInParameter(insertCommand, "@DigitalGoodId", System.Data.DbType.Int32, this.DigitalGoodId);
                        database.AddInParameter(insertCommand, "@SerialKeyData", System.Data.DbType.String, this.SerialKeyData);
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._SerialKeyId = result;
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