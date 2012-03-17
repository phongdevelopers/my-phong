//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Shipping
{
    /// <summary>
    /// This class represents a ShipMethodShipZone object in the database.
    /// </summary>
    public partial class ShipMethodShipZone : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ShipMethodShipZone() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="shipMethodId">Value of ShipMethodId.</param>
        /// <param name="shipZoneId">Value of ShipZoneId.</param>
        /// </summary>
        public ShipMethodShipZone(Int32 shipMethodId, Int32 shipZoneId)
        {
            this.ShipMethodId = shipMethodId;
            this.ShipZoneId = shipZoneId;
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
          columnNames.Add(prefix + "ShipMethodId");
          columnNames.Add(prefix + "ShipZoneId");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given ShipMethodShipZone object from the given database data reader.
        /// </summary>
        /// <param name="shipMethodShipZone">The ShipMethodShipZone object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(ShipMethodShipZone shipMethodShipZone, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            shipMethodShipZone.ShipMethodId = dr.GetInt32(0);
            shipMethodShipZone.ShipZoneId = dr.GetInt32(1);
            shipMethodShipZone.IsDirty = false;
        }

#endregion

        private Int32 _ShipMethodId;
        private Int32 _ShipZoneId;
        private bool _IsDirty;

        /// <summary>
        /// ShipMethodId
        /// </summary>
        [DataObjectField(true, false, false)]
        public Int32 ShipMethodId
        {
            get { return this._ShipMethodId; }
            set
            {
                if (this._ShipMethodId != value)
                {
                    this._ShipMethodId = value;
                    this.IsDirty = true;
                    this._ShipMethod = null;
                }
            }
        }

        /// <summary>
        /// ShipZoneId
        /// </summary>
        [DataObjectField(true, false, false)]
        public Int32 ShipZoneId
        {
            get { return this._ShipZoneId; }
            set
            {
                if (this._ShipZoneId != value)
                {
                    this._ShipZoneId = value;
                    this.IsDirty = true;
                    this._ShipZone = null;
                }
            }
        }

        /// <summary>
        /// Indicates whether this ShipMethodShipZone object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private ShipMethod _ShipMethod;
        private ShipZone _ShipZone;

        /// <summary>
        /// The ShipMethod object that this ShipMethodShipZone object is associated with
        /// </summary>
        public ShipMethod ShipMethod
        {
            get
            {
                if (!this.ShipMethodLoaded)
                {
                    this._ShipMethod = ShipMethodDataSource.Load(this.ShipMethodId);
                }
                return this._ShipMethod;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool ShipMethodLoaded { get { return ((this._ShipMethod != null) && (this._ShipMethod.ShipMethodId == this.ShipMethodId)); } }

        /// <summary>
        /// The ShipZone object that this ShipMethodShipZone object is associated with
        /// </summary>
        public ShipZone ShipZone
        {
            get
            {
                if (!this.ShipZoneLoaded)
                {
                    this._ShipZone = ShipZoneDataSource.Load(this.ShipZoneId);
                }
                return this._ShipZone;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool ShipZoneLoaded { get { return ((this._ShipZone != null) && (this._ShipZone.ShipZoneId == this.ShipZoneId)); } }

#endregion

        /// <summary>
        /// Deletes this ShipMethodShipZone object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_ShipMethodShipZones");
            deleteQuery.Append(" WHERE ShipMethodId = @ShipMethodId AND ShipZoneId = @ShipZoneId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@ShipMethodId", System.Data.DbType.Int32, this.ShipMethodId);
                database.AddInParameter(deleteCommand, "@ShipZoneId", System.Data.DbType.Int32, this.ShipZoneId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }

        /// <summary>
        /// Load this ShipMethodShipZone object from the database for the given primary key.
        /// </summary>
        /// <param name="shipMethodId">Value of ShipMethodId of the object to load.</param>
        /// <param name="shipZoneId">Value of ShipZoneId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 shipMethodId, Int32 shipZoneId)
        {
            this.ShipMethodId = shipMethodId;
            this.ShipZoneId = shipZoneId;
            this.IsDirty = false;
            return true;
        }

        /// <summary>
        /// Saves this ShipMethodShipZone object to the database.
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
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_ShipMethodShipZones");
                    selectQuery.Append(" WHERE ShipMethodId = @ShipMethodId AND ShipZoneId = @ShipZoneId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@ShipMethodId", System.Data.DbType.Int32, this.ShipMethodId);
                        database.AddInParameter(selectCommand, "@ShipZoneId", System.Data.DbType.Int32, this.ShipZoneId);
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
                    insertQuery.Append("INSERT INTO ac_ShipMethodShipZones (ShipMethodId, ShipZoneId)");
                    insertQuery.Append(" VALUES (@ShipMethodId, @ShipZoneId)");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@ShipMethodId", System.Data.DbType.Int32, this.ShipMethodId);
                        database.AddInParameter(insertCommand, "@ShipZoneId", System.Data.DbType.Int32, this.ShipZoneId);
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