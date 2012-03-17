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
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// This class represents a OrderNote object in the database.
    /// </summary>
    public partial class OrderNote : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderNote() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="orderNoteId">Value of OrderNoteId.</param>
        /// </summary>
        public OrderNote(Int32 orderNoteId)
        {
            this.OrderNoteId = orderNoteId;
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
          columnNames.Add(prefix + "OrderNoteId");
          columnNames.Add(prefix + "OrderId");
          columnNames.Add(prefix + "UserId");
          columnNames.Add(prefix + "CreatedDate");
          columnNames.Add(prefix + "Comment");
          columnNames.Add(prefix + "NoteTypeId");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given OrderNote object from the given database data reader.
        /// </summary>
        /// <param name="orderNote">The OrderNote object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(OrderNote orderNote, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            orderNote.OrderNoteId = dr.GetInt32(0);
            orderNote.OrderId = dr.GetInt32(1);
            orderNote.UserId = NullableData.GetInt32(dr, 2);
            orderNote.CreatedDate = LocaleHelper.ToLocalTime(dr.GetDateTime(3));
            orderNote.Comment = dr.GetString(4);
            orderNote.NoteTypeId = dr.GetByte(5);
            orderNote.IsDirty = false;
        }

#endregion

        private Int32 _OrderNoteId;
        private Int32 _OrderId;
        private Int32 _UserId;
        private DateTime _CreatedDate;
        private String _Comment = string.Empty;
        private Byte _NoteTypeId;
        private bool _IsDirty;

        /// <summary>
        /// OrderNoteId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 OrderNoteId
        {
            get { return this._OrderNoteId; }
            set
            {
                if (this._OrderNoteId != value)
                {
                    this._OrderNoteId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// OrderId
        /// </summary>
        public Int32 OrderId
        {
            get { return this._OrderId; }
            set
            {
                if (this._OrderId != value)
                {
                    this._OrderId = value;
                    this.IsDirty = true;
                    this._Order = null;
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
        /// CreatedDate
        /// </summary>
        public DateTime CreatedDate
        {
            get { return this._CreatedDate; }
            set
            {
                if (this._CreatedDate != value)
                {
                    this._CreatedDate = value;
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
        /// NoteTypeId
        /// </summary>
        public Byte NoteTypeId
        {
            get { return this._NoteTypeId; }
            set
            {
                if (this._NoteTypeId != value)
                {
                    this._NoteTypeId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this OrderNote object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private Order _Order;
        private User _User;

        /// <summary>
        /// The Order object that this OrderNote object is associated with
        /// </summary>
        public Order Order
        {
            get
            {
                if (!this.OrderLoaded)
                {
                    this._Order = OrderDataSource.Load(this.OrderId);
                }
                return this._Order;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool OrderLoaded { get { return ((this._Order != null) && (this._Order.OrderId == this.OrderId)); } }

        /// <summary>
        /// The User object that this OrderNote object is associated with
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
        /// Deletes this OrderNote object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_OrderNotes");
            deleteQuery.Append(" WHERE OrderNoteId = @OrderNoteId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@OrderNoteId", System.Data.DbType.Int32, this.OrderNoteId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this OrderNote object from the database for the given primary key.
        /// </summary>
        /// <param name="orderNoteId">Value of OrderNoteId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 orderNoteId)
        {
            bool result = false;
            this.OrderNoteId = orderNoteId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_OrderNotes");
            selectQuery.Append(" WHERE OrderNoteId = @orderNoteId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@orderNoteId", System.Data.DbType.Int32, orderNoteId);
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
        /// Saves this OrderNote object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        protected SaveResult BaseSave()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;
                
                if (this.OrderNoteId == 0) recordExists = false;

                //SET DEFAULT FOR DATE FIELD
                if (this.CreatedDate == System.DateTime.MinValue) this.CreatedDate = LocaleHelper.LocalNow;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_OrderNotes");
                    selectQuery.Append(" WHERE OrderNoteId = @OrderNoteId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@OrderNoteId", System.Data.DbType.Int32, this.OrderNoteId);
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
                    updateQuery.Append("UPDATE ac_OrderNotes SET ");
                    updateQuery.Append("OrderId = @OrderId");
                    updateQuery.Append(", UserId = @UserId");
                    updateQuery.Append(", CreatedDate = @CreatedDate");
                    updateQuery.Append(", Comment = @Comment");
                    updateQuery.Append(", NoteTypeId = @NoteTypeId");
                    updateQuery.Append(" WHERE OrderNoteId = @OrderNoteId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@OrderNoteId", System.Data.DbType.Int32, this.OrderNoteId);
                        database.AddInParameter(updateCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);
                        database.AddInParameter(updateCommand, "@UserId", System.Data.DbType.Int32, NullableData.DbNullify(this.UserId));
                        database.AddInParameter(updateCommand, "@CreatedDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.CreatedDate));
                        database.AddInParameter(updateCommand, "@Comment", System.Data.DbType.String, this.Comment);
                        database.AddInParameter(updateCommand, "@NoteTypeId", System.Data.DbType.Byte, this.NoteTypeId);
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_OrderNotes (OrderId, UserId, CreatedDate, Comment, NoteTypeId)");
                    insertQuery.Append(" VALUES (@OrderId, @UserId, @CreatedDate, @Comment, @NoteTypeId)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@OrderNoteId", System.Data.DbType.Int32, this.OrderNoteId);
                        database.AddInParameter(insertCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);
                        database.AddInParameter(insertCommand, "@UserId", System.Data.DbType.Int32, NullableData.DbNullify(this.UserId));
                        database.AddInParameter(insertCommand, "@CreatedDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.CreatedDate));
                        database.AddInParameter(insertCommand, "@Comment", System.Data.DbType.String, this.Comment);
                        database.AddInParameter(insertCommand, "@NoteTypeId", System.Data.DbType.Byte, this.NoteTypeId);
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._OrderNoteId = result;
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
