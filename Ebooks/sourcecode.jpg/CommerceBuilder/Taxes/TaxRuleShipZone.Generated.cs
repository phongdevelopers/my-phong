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
using CommerceBuilder.Taxes;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Taxes
{
    /// <summary>
    /// This class represents a TaxRuleShipZone object in the database.
    /// </summary>
    public partial class TaxRuleShipZone : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TaxRuleShipZone() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="taxRuleId">Value of TaxRuleId.</param>
        /// <param name="shipZoneId">Value of ShipZoneId.</param>
        /// </summary>
        public TaxRuleShipZone(Int32 taxRuleId, Int32 shipZoneId)
        {
            this.TaxRuleId = taxRuleId;
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
          columnNames.Add(prefix + "TaxRuleId");
          columnNames.Add(prefix + "ShipZoneId");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given TaxRuleShipZone object from the given database data reader.
        /// </summary>
        /// <param name="taxRuleShipZone">The TaxRuleShipZone object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(TaxRuleShipZone taxRuleShipZone, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            taxRuleShipZone.TaxRuleId = dr.GetInt32(0);
            taxRuleShipZone.ShipZoneId = dr.GetInt32(1);
            taxRuleShipZone.IsDirty = false;
        }

#endregion

        private Int32 _TaxRuleId;
        private Int32 _ShipZoneId;
        private bool _IsDirty;

        /// <summary>
        /// TaxRuleId
        /// </summary>
        [DataObjectField(true, false, false)]
        public Int32 TaxRuleId
        {
            get { return this._TaxRuleId; }
            set
            {
                if (this._TaxRuleId != value)
                {
                    this._TaxRuleId = value;
                    this.IsDirty = true;
                    this._TaxRule = null;
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
        /// Indicates whether this TaxRuleShipZone object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private ShipZone _ShipZone;
        private TaxRule _TaxRule;

        /// <summary>
        /// The ShipZone object that this TaxRuleShipZone object is associated with
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

        /// <summary>
        /// The TaxRule object that this TaxRuleShipZone object is associated with
        /// </summary>
        public TaxRule TaxRule
        {
            get
            {
                if (!this.TaxRuleLoaded)
                {
                    this._TaxRule = TaxRuleDataSource.Load(this.TaxRuleId);
                }
                return this._TaxRule;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool TaxRuleLoaded { get { return ((this._TaxRule != null) && (this._TaxRule.TaxRuleId == this.TaxRuleId)); } }

#endregion

        /// <summary>
        /// Deletes this TaxRuleShipZone object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_TaxRuleShipZones");
            deleteQuery.Append(" WHERE TaxRuleId = @TaxRuleId AND ShipZoneId = @ShipZoneId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@TaxRuleId", System.Data.DbType.Int32, this.TaxRuleId);
                database.AddInParameter(deleteCommand, "@ShipZoneId", System.Data.DbType.Int32, this.ShipZoneId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }

        /// <summary>
        /// Load this TaxRuleShipZone object from the database for the given primary key.
        /// </summary>
        /// <param name="taxRuleId">Value of TaxRuleId of the object to load.</param>
        /// <param name="shipZoneId">Value of ShipZoneId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 taxRuleId, Int32 shipZoneId)
        {
            this.TaxRuleId = taxRuleId;
            this.ShipZoneId = shipZoneId;
            this.IsDirty = false;
            return true;
        }

        /// <summary>
        /// Saves this TaxRuleShipZone object to the database.
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
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_TaxRuleShipZones");
                    selectQuery.Append(" WHERE TaxRuleId = @TaxRuleId AND ShipZoneId = @ShipZoneId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@TaxRuleId", System.Data.DbType.Int32, this.TaxRuleId);
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
                    insertQuery.Append("INSERT INTO ac_TaxRuleShipZones (TaxRuleId, ShipZoneId)");
                    insertQuery.Append(" VALUES (@TaxRuleId, @ShipZoneId)");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@TaxRuleId", System.Data.DbType.Int32, this.TaxRuleId);
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
