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
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Shipping
{
    /// <summary>
    /// This class represents a Country object in the database.
    /// </summary>
    public partial class Country : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Country() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="countryCode">Value of CountryCode.</param>
        /// </summary>
        public Country(String countryCode)
        {
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
          columnNames.Add(prefix + "CountryCode");
          columnNames.Add(prefix + "StoreId");
          columnNames.Add(prefix + "Name");
          columnNames.Add(prefix + "AddressFormat");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given Country object from the given database data reader.
        /// </summary>
        /// <param name="country">The Country object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(Country country, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            country.CountryCode = dr.GetString(0);
            country.StoreId = dr.GetInt32(1);
            country.Name = dr.GetString(2);
            country.AddressFormat = NullableData.GetString(dr, 3);
            country.IsDirty = false;
        }

#endregion

        private String _CountryCode = string.Empty;
        private Int32 _StoreId;
        private String _Name = string.Empty;
        private String _AddressFormat = string.Empty;
        private bool _IsDirty;

        /// <summary>
        /// CountryCode
        /// </summary>
        [DataObjectField(true, true, false)]
        public String CountryCode
        {
            get { return this._CountryCode; }
            set
            {
                if (this._CountryCode != value)
                {
                    this._CountryCode = value;
                    this.IsDirty = true;
                    this.EnsureChildProperties();
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
        /// Name
        /// </summary>
        public String Name
        {
            get { return this._Name; }
            set
            {
                if (this._Name != value)
                {
                    this._Name = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// AddressFormat
        /// </summary>
        public String AddressFormat
        {
            get { return this._AddressFormat; }
            set
            {
                if (this._AddressFormat != value)
                {
                    this._AddressFormat = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this Country object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                if (this._IsDirty) return true;
                if (this.AddressesLoaded && this.Addresses.IsDirty) return true;
                if (this.ProvincesLoaded && this.Provinces.IsDirty) return true;
                if (this.ShipZoneCountriesLoaded && this.ShipZoneCountries.IsDirty) return true;
                if (this.WarehousesLoaded && this.Warehouses.IsDirty) return true;
                return false;
            }
            set { this._IsDirty = value; }
        }

        /// <summary>
        /// Ensures that child objects of this Country are properly associated with this Country object.
        /// </summary>
        public virtual void EnsureChildProperties()
        {
            if (this.AddressesLoaded) { foreach (Address address in this.Addresses) { address.CountryCode = this.CountryCode; } }
            if (this.ProvincesLoaded) { foreach (Province province in this.Provinces) { province.CountryCode = this.CountryCode; } }
            if (this.ShipZoneCountriesLoaded) { foreach (ShipZoneCountry shipZoneCountry in this.ShipZoneCountries) { shipZoneCountry.CountryCode = this.CountryCode; } }
            if (this.WarehousesLoaded) { foreach (Warehouse warehouse in this.Warehouses) { warehouse.CountryCode = this.CountryCode; } }
        }

#region Parents
        private Store _Store;

        /// <summary>
        /// The Store object that this Country object is associated with
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

#region Children
        private AddressCollection _Addresses;
        private ProvinceCollection _Provinces;
        private WarehouseCollection _Warehouses;

        /// <summary>
        /// A collection of Address objects associated with this Country object.
        /// </summary>
        public AddressCollection Addresses
        {
            get
            {
                if (!this.AddressesLoaded)
                {
                    this._Addresses = AddressDataSource.LoadForCountry(this.CountryCode);
                }
                return this._Addresses;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool AddressesLoaded { get { return (this._Addresses != null); } }

        /// <summary>
        /// A collection of Province objects associated with this Country object.
        /// </summary>
        public ProvinceCollection Provinces
        {
            get
            {
                if (!this.ProvincesLoaded)
                {
                    this._Provinces = ProvinceDataSource.LoadForCountry(this.CountryCode);
                }
                return this._Provinces;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool ProvincesLoaded { get { return (this._Provinces != null); } }

        /// <summary>
        /// A collection of Warehouse objects associated with this Country object.
        /// </summary>
        public WarehouseCollection Warehouses
        {
            get
            {
                if (!this.WarehousesLoaded)
                {
                    this._Warehouses = WarehouseDataSource.LoadForCountry(this.CountryCode);
                }
                return this._Warehouses;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool WarehousesLoaded { get { return (this._Warehouses != null); } }

#endregion

#region Associations
        private ShipZoneCountryCollection _ShipZoneCountries;

        /// <summary>
        /// A collection of ShipZoneCountry objects associated with this Country object.
        /// </summary>
        public ShipZoneCountryCollection ShipZoneCountries
        {
            get
            {
                if (!this.ShipZoneCountriesLoaded)
                {
                    this._ShipZoneCountries = ShipZoneCountryDataSource.LoadForCountry(this.CountryCode);
                }
                return this._ShipZoneCountries;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool ShipZoneCountriesLoaded { get { return (this._ShipZoneCountries != null); } }
#endregion

        /// <summary>
        /// Deletes this Country object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_Countries");
            deleteQuery.Append(" WHERE CountryCode = @CountryCode");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@CountryCode", System.Data.DbType.String, this.CountryCode);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this Country object from the database for the given primary key.
        /// </summary>
        /// <param name="countryCode">Value of CountryCode of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(String countryCode)
        {
            bool result = false;
            this.CountryCode = countryCode;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Countries");
            selectQuery.Append(" WHERE CountryCode = @countryCode");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@countryCode", System.Data.DbType.String, countryCode);
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
        /// Saves this Country object to the database.
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
                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_Countries");
                    selectQuery.Append(" WHERE CountryCode = @CountryCode");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@CountryCode", System.Data.DbType.String, this.CountryCode);
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
                    updateQuery.Append("UPDATE ac_Countries SET ");
                    updateQuery.Append("StoreId = @StoreId");
                    updateQuery.Append(", Name = @Name");
                    updateQuery.Append(", AddressFormat = @AddressFormat");
                    updateQuery.Append(" WHERE CountryCode = @CountryCode");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@CountryCode", System.Data.DbType.String, this.CountryCode);
                        database.AddInParameter(updateCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(updateCommand, "@Name", System.Data.DbType.String, this.Name);
                        database.AddInParameter(updateCommand, "@AddressFormat", System.Data.DbType.String, NullableData.DbNullify(this.AddressFormat));
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_Countries (CountryCode, StoreId, Name, AddressFormat)");
                    insertQuery.Append(" VALUES (@CountryCode, @StoreId, @Name, @AddressFormat)");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@CountryCode", System.Data.DbType.String, this.CountryCode);
                        database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(insertCommand, "@Name", System.Data.DbType.String, this.Name);
                        database.AddInParameter(insertCommand, "@AddressFormat", System.Data.DbType.String, NullableData.DbNullify(this.AddressFormat));
                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);
                    }
                }
                this.SaveChildren();

                //OBJECT IS DIRTY IF NO RECORDS WERE UPDATED OR INSERTED
                this.IsDirty = (result == 0);
                if (this.IsDirty) { return SaveResult.Failed; }
                else { return (recordExists ? SaveResult.RecordUpdated : SaveResult.RecordInserted); }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

        /// <summary>
        /// Saves that child objects associated with this Country object.
        /// </summary>
        public virtual void SaveChildren()
        {
            this.EnsureChildProperties();
            if (this.AddressesLoaded) this.Addresses.Save();
            if (this.ProvincesLoaded) this.Provinces.Save();
            if (this.ShipZoneCountriesLoaded) this.ShipZoneCountries.Save();
            if (this.WarehousesLoaded) this.Warehouses.Save();
        }

     }
}
