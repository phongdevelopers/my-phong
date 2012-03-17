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
    /// This class represents a ShipZoneCountry object in the database.
    /// </summary>
    public partial class ShipZoneCountry : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ShipZoneCountry() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="shipZoneId">Value of ShipZoneId.</param>
        /// <param name="countryCode">Value of CountryCode.</param>
        /// </summary>
        public ShipZoneCountry(Int32 shipZoneId, String countryCode)
        {
            this.ShipZoneId = shipZoneId;
            this.CountryCode = countryCode;
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
          columnNames.Add(prefix + "ShipZoneId");
          columnNames.Add(prefix + "CountryCode");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given ShipZoneCountry object from the given database data reader.
        /// </summary>
        /// <param name="shipZoneCountry">The ShipZoneCountry object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(ShipZoneCountry shipZoneCountry, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            shipZoneCountry.ShipZoneId = dr.GetInt32(0);
            shipZoneCountry.CountryCode = dr.GetString(1);
            shipZoneCountry.IsDirty = false;
        }

#endregion

        private Int32 _ShipZoneId;
        private String _CountryCode = string.Empty;
        private bool _IsDirty;

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
        /// CountryCode
        /// </summary>
        [DataObjectField(true, false, false)]
        public String CountryCode
        {
            get { return this._CountryCode; }
            set
            {
                if (this._CountryCode != value)
                {
                    this._CountryCode = value;
                    this.IsDirty = true;
                    this._Country = null;
                }
            }
        }

        /// <summary>
        /// Indicates whether this ShipZoneCountry object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private Country _Country;
        private ShipZone _ShipZone;

        /// <summary>
        /// The Country object that this ShipZoneCountry object is associated with
        /// </summary>
        public Country Country
        {
            get
            {
                if (!this.CountryLoaded)
                {
                    this._Country = CountryDataSource.Load(this.CountryCode);
                }
                return this._Country;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool CountryLoaded { get { return ((this._Country != null) && (this._Country.CountryCode == this.CountryCode)); } }

        /// <summary>
        /// The ShipZone object that this ShipZoneCountry object is associated with
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
        /// Deletes this ShipZoneCountry object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_ShipZoneCountries");
            deleteQuery.Append(" WHERE ShipZoneId = @ShipZoneId AND CountryCode = @CountryCode");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@ShipZoneId", System.Data.DbType.Int32, this.ShipZoneId);
                database.AddInParameter(deleteCommand, "@CountryCode", System.Data.DbType.String, this.CountryCode);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }

        /// <summary>
        /// Load this ShipZoneCountry object from the database for the given primary key.
        /// </summary>
        /// <param name="shipZoneId">Value of ShipZoneId of the object to load.</param>
        /// <param name="countryCode">Value of CountryCode of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 shipZoneId, String countryCode)
        {
            this.ShipZoneId = shipZoneId;
            this.CountryCode = countryCode;
            this.IsDirty = false;
            return true;
        }

        /// <summary>
        /// Saves this ShipZoneCountry object to the database.
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
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_ShipZoneCountries");
                    selectQuery.Append(" WHERE ShipZoneId = @ShipZoneId AND CountryCode = @CountryCode");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@ShipZoneId", System.Data.DbType.Int32, this.ShipZoneId);
                        database.AddInParameter(selectCommand, "@CountryCode", System.Data.DbType.String, this.CountryCode);
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
                    insertQuery.Append("INSERT INTO ac_ShipZoneCountries (ShipZoneId, CountryCode)");
                    insertQuery.Append(" VALUES (@ShipZoneId, @CountryCode)");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@ShipZoneId", System.Data.DbType.Int32, this.ShipZoneId);
                        database.AddInParameter(insertCommand, "@CountryCode", System.Data.DbType.String, this.CountryCode);
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
