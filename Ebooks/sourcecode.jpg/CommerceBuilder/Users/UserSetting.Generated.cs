//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// This class represents a UserSetting object in the database.
    /// </summary>
    public partial class UserSetting : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public UserSetting() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="userSettingId">Value of UserSettingId.</param>
        /// </summary>
        public UserSetting(Int32 userSettingId)
        {
            this.UserSettingId = userSettingId;
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
          columnNames.Add(prefix + "UserSettingId");
          columnNames.Add(prefix + "UserId");
          columnNames.Add(prefix + "FieldName");
          columnNames.Add(prefix + "FieldValue");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given UserSetting object from the given database data reader.
        /// </summary>
        /// <param name="userSetting">The UserSetting object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(UserSetting userSetting, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            userSetting.UserSettingId = dr.GetInt32(0);
            userSetting.UserId = dr.GetInt32(1);
            userSetting.FieldName = dr.GetString(2);
            userSetting.FieldValue = dr.GetString(3);
            userSetting.IsDirty = false;
        }

#endregion

        private Int32 _UserSettingId;
        private Int32 _UserId;
        private String _FieldName = string.Empty;
        private String _FieldValue = string.Empty;
        private bool _IsDirty;

        /// <summary>
        /// UserSettingId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 UserSettingId
        {
            get { return this._UserSettingId; }
            set
            {
                if (this._UserSettingId != value)
                {
                    this._UserSettingId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// UserId
        /// </summary>
        public Int32 UserId
        {
            get { return this._UserId; }
            set
            {
                if (this._UserId != value)
                {
                    this._UserId = value;
                    this.IsDirty = true;
                    this._User = null;
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
        /// Indicates whether this UserSetting object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private User _User;

        /// <summary>
        /// The User object that this UserSetting object is associated with
        /// </summary>
        public User User
        {
            get
            {
                if (!this.UserLoaded)
                {
                    this._User = UserDataSource.Load(this.UserId);
                }
                return this._User;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool UserLoaded { get { return ((this._User != null) && (this._User.UserId == this.UserId)); } }

#endregion

        /// <summary>
        /// Deletes this UserSetting object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_UserSettings");
            deleteQuery.Append(" WHERE UserSettingId = @UserSettingId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@UserSettingId", System.Data.DbType.Int32, this.UserSettingId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this UserSetting object from the database for the given primary key.
        /// </summary>
        /// <param name="userSettingId">Value of UserSettingId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 userSettingId)
        {
            bool result = false;
            this.UserSettingId = userSettingId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_UserSettings");
            selectQuery.Append(" WHERE UserSettingId = @userSettingId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@userSettingId", System.Data.DbType.Int32, userSettingId);
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
        /// Saves this UserSetting object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;
                
                if (this.UserSettingId == 0) recordExists = false;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_UserSettings");
                    selectQuery.Append(" WHERE UserSettingId = @UserSettingId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@UserSettingId", System.Data.DbType.Int32, this.UserSettingId);
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
                    updateQuery.Append("UPDATE ac_UserSettings SET ");
                    updateQuery.Append("UserId = @UserId");
                    updateQuery.Append(", FieldName = @FieldName");
                    updateQuery.Append(", FieldValue = @FieldValue");
                    updateQuery.Append(" WHERE UserSettingId = @UserSettingId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@UserSettingId", System.Data.DbType.Int32, this.UserSettingId);
                        database.AddInParameter(updateCommand, "@UserId", System.Data.DbType.Int32, this.UserId);
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
                    insertQuery.Append("INSERT INTO ac_UserSettings (UserId, FieldName, FieldValue)");
                    insertQuery.Append(" VALUES (@UserId, @FieldName, @FieldValue)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@UserSettingId", System.Data.DbType.Int32, this.UserSettingId);
                        database.AddInParameter(insertCommand, "@UserId", System.Data.DbType.Int32, this.UserId);
                        database.AddInParameter(insertCommand, "@FieldName", System.Data.DbType.String, this.FieldName);
                        database.AddInParameter(insertCommand, "@FieldValue", System.Data.DbType.String, this.FieldValue);
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._UserSettingId = result;
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
