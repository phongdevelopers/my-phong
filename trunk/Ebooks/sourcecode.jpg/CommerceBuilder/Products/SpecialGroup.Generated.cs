//Generated by PersistableBaseGenerator

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
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// This class represents a SpecialGroup object in the database.
    /// </summary>
    public partial class SpecialGroup : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SpecialGroup() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="specialId">Value of SpecialId.</param>
        /// <param name="groupId">Value of GroupId.</param>
        /// </summary>
        public SpecialGroup(Int32 specialId, Int32 groupId)
        {
            this.SpecialId = specialId;
            this.GroupId = groupId;
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
          columnNames.Add(prefix + "SpecialId");
          columnNames.Add(prefix + "GroupId");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given SpecialGroup object from the given database data reader.
        /// </summary>
        /// <param name="specialGroup">The SpecialGroup object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(SpecialGroup specialGroup, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            specialGroup.SpecialId = dr.GetInt32(0);
            specialGroup.GroupId = dr.GetInt32(1);
            specialGroup.IsDirty = false;
        }

#endregion

        private Int32 _SpecialId;
        private Int32 _GroupId;
        private bool _IsDirty;

        /// <summary>
        /// SpecialId
        /// </summary>
        [DataObjectField(true, false, false)]
        public Int32 SpecialId
        {
            get { return this._SpecialId; }
            set
            {
                if (this._SpecialId != value)
                {
                    this._SpecialId = value;
                    this.IsDirty = true;
                    this._Special = null;
                }
            }
        }

        /// <summary>
        /// GroupId
        /// </summary>
        [DataObjectField(true, false, false)]
        public Int32 GroupId
        {
            get { return this._GroupId; }
            set
            {
                if (this._GroupId != value)
                {
                    this._GroupId = value;
                    this.IsDirty = true;
                    this._Group = null;
                }
            }
        }

        /// <summary>
        /// Indicates whether this SpecialGroup object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private Group _Group;
        private Special _Special;

        /// <summary>
        /// The Group object that this SpecialGroup object is associated with
        /// </summary>
        public Group Group
        {
            get
            {
                if (!this.GroupLoaded)
                {
                    this._Group = GroupDataSource.Load(this.GroupId);
                }
                return this._Group;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool GroupLoaded { get { return ((this._Group != null) && (this._Group.GroupId == this.GroupId)); } }

        /// <summary>
        /// The Special object that this SpecialGroup object is associated with
        /// </summary>
        public Special Special
        {
            get
            {
                if (!this.SpecialLoaded)
                {
                    this._Special = SpecialDataSource.Load(this.SpecialId);
                }
                return this._Special;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool SpecialLoaded { get { return ((this._Special != null) && (this._Special.SpecialId == this.SpecialId)); } }

#endregion

        /// <summary>
        /// Deletes this SpecialGroup object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_SpecialGroups");
            deleteQuery.Append(" WHERE SpecialId = @SpecialId AND GroupId = @GroupId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@SpecialId", System.Data.DbType.Int32, this.SpecialId);
                database.AddInParameter(deleteCommand, "@GroupId", System.Data.DbType.Int32, this.GroupId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }

        /// <summary>
        /// Load this SpecialGroup object from the database for the given primary key.
        /// </summary>
        /// <param name="specialId">Value of SpecialId of the object to load.</param>
        /// <param name="groupId">Value of GroupId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 specialId, Int32 groupId)
        {
            this.SpecialId = specialId;
            this.GroupId = groupId;
            this.IsDirty = false;
            return true;
        }

        /// <summary>
        /// Saves this SpecialGroup object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;
                
                //generate key(s) if needed
                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_SpecialGroups");
                    selectQuery.Append(" WHERE SpecialId = @SpecialId AND GroupId = @GroupId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@SpecialId", System.Data.DbType.Int32, this.SpecialId);
                        database.AddInParameter(selectCommand, "@GroupId", System.Data.DbType.Int32, this.GroupId);
                        if ((int)database.ExecuteScalar(selectCommand) == 0)
                        {
                            recordExists = false;
                        }
                    }
                }

                if (recordExists)
                {
                    //RECORD ALREADY EXISTS IN DATABASE
                    this.IsDirty = false;
                    return SaveResult.RecordUpdated;
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_SpecialGroups (SpecialId, GroupId)");
                    insertQuery.Append(" VALUES (@SpecialId, @GroupId)");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@SpecialId", System.Data.DbType.Int32, this.SpecialId);
                        database.AddInParameter(insertCommand, "@GroupId", System.Data.DbType.Int32, this.GroupId);
                        int recordsAffected = database.ExecuteNonQuery(insertCommand);
                        //OBJECT IS NOT DIRTY IF RECORD WAS INSERTED
                        this.IsDirty = (recordsAffected == 0);
                        if (this.IsDirty) { return SaveResult.Failed; }
                        return SaveResult.RecordInserted;
                    }
                }

            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

     }
}
