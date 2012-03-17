using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Web;
using System.Web.Caching;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Shipping;
using CommerceBuilder.Marketing;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// This class represents the store object in database
    /// </summary>
    public partial class Store
    {
        private Warehouse _DefaultWarehouse;

        /// <summary>
        /// The default warehouse of this store
        /// </summary>
        public Warehouse DefaultWarehouse
        {
            get
            {
                if (!DefaultWarehouseLoaded)
                {
                    this._DefaultWarehouse = WarehouseDataSource.Load(this.DefaultWarehouseId);
                    if (_DefaultWarehouse == null)
                    {
                        if (Warehouses.Count > 0)
                        {
                            _DefaultWarehouse = Warehouses[0];
                            _DefaultWarehouseId = Warehouses[0].WarehouseId;
                        }
                        else
                        {
                            //get countries for store
                            string countryCode;
                            CountryCollection countries = CountryDataSource.LoadForStore();
                            if (countries.Count > 0)
                            {
                                countryCode = countries[0].CountryCode;
                            }
                            else
                            {
                                //add default country
                                Country us = new Country();
                                us.Name = "United States";
                                us.CountryCode = "US";
                                us.Save();
                                countryCode = us.CountryCode;
                            }
                            //add a default warehouse item
                            _DefaultWarehouse = new Warehouse();
                            _DefaultWarehouse.Name = "Default Warehouse";
                            _DefaultWarehouse.Address1 = "Address";
                            _DefaultWarehouse.City = "City";
                            _DefaultWarehouse.CountryCode = countryCode;
                            _DefaultWarehouse.Save();
                            _DefaultWarehouseId = _DefaultWarehouse.WarehouseId;
                        }
                    }
                }
                return _DefaultWarehouse;
            }
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal bool DefaultWarehouseLoaded { get { return ((this._DefaultWarehouse != null) && (this._DefaultWarehouse.WarehouseId == this.DefaultWarehouseId)); } }

        /// <summary>
        /// The weight unit for this store
        /// </summary>
        public WeightUnit WeightUnit
        {
            get
            {
                return (WeightUnit)this.WeightUnitId;
            }
            set
            {
                this.WeightUnitId = (short)value;
            }
        }
 
        /// <summary>
        /// The measurement unit for this store
        /// </summary>
        public MeasurementUnit MeasurementUnit
        {
            get
            {
                return (MeasurementUnit)this.MeasurementUnitId;
            }
            set
            {
                this.MeasurementUnitId = (short)value;
            }
        }

        /// <summary>
        /// The volume discount mode set for this store
        /// </summary>
        public VolumeDiscountMode VolumeDiscountMode
        {
            get
            {
                return (VolumeDiscountMode)AlwaysConvert.ToInt(this.Settings.GetValueByKey("VolumeDiscountMode"));
            }
            set
            {
                this.Settings.SetValueByKey("VolumeDiscountMode", ((int)value).ToString());
            }
        }

        /// <summary>
        /// Whether inventory is enabled for this store or not
        /// </summary>
        public bool EnableInventory
        {
            get
            {
                return AlwaysConvert.ToBool(this.Settings.GetValueByKey("EnableInventory"), false);
            }
            set
            {
                this.Settings.SetValueByKey("EnableInventory", value.ToString());
            }
        }

        /// <summary>
        /// The URL of the store
        /// </summary>
        public string StoreUrl
        {
            get
            {
                return this.Settings.GetValueByKey("Store_StoreUrl");
            }
            set
            {
                this.Settings.SetValueByKey("Store_StoreUrl", value);
            }
        }

        private double _TimeZoneOffset;
        private bool _TimeZoneOffsetInitialized;
        
        /// <summary>
        /// The time-zone offset for this store
        /// </summary>
        public double TimeZoneOffset
        {
            get
            {
                if (!_TimeZoneOffsetInitialized)
                {
                    _TimeZoneOffset = this.Settings.TimeZoneOffset;
                    _TimeZoneOffsetInitialized = true;
                }
                return _TimeZoneOffset;
            }
            set
            {
                this.Settings.TimeZoneOffset = value;
                _TimeZoneOffset = value;
                _TimeZoneOffsetInitialized = true;
            }
        }

        /// <summary>
        /// Retrieves settings for the current store using application cache.
        /// </summary>
        /// <returns>Settings for the current store.</returns>
        public static StoreSettingCollection GetCachedSettings()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                StoreSettingCollection settings = context.Cache["AbleCommerceStoreSettings"] as StoreSettingCollection;
                if (settings == null)
                {
                    settings = Token.Instance.Store.Settings;
                    context.Cache.Insert("AbleCommerceStoreSettings", settings, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                }
                return settings;
            }
            //CALLED FROM NON-WEB CONTEXT, LOAD STORE SETTINGS
            return Token.Instance.Store.Settings;
        }
        
        /// <summary>
        /// Clears any cached store settings from the HTTP application cache.
        /// </summary>
        /// <remarks>If called from a non-web context, this method does nothing.</remarks>
        public static void ClearCachedSettings()
        {
            HttpContext context = HttpContext.Current;
            if (context != null) context.Cache.Remove("AbleCommerceStoreSettings");
        }

        private Currency _BaseCurrency;
        /// <summary>
        /// The base currency of this store
        /// </summary>
        public Currency BaseCurrency
        {
            get
            {
                if (_BaseCurrency == null)
                {
                    int baseCurrencyId = this.Settings.BaseCurrencyId;
                    int index = this.Currencies.IndexOf(baseCurrencyId);
                    if (index < 0)
                    {
                        //WE NEED TO OBTAIN A BASE CURRENCY
                        if (this.Currencies.Count > 0)
                        {
                            //CURRENCIES EXIST, USE THE FIRST ONE AS THE DEFAULT
                            _BaseCurrency = this.Currencies[0];
                        }
                        else
                        {
                            //NO CURRENCIES, CREATE DEFAULT
                            _BaseCurrency = new Currency();
                            _BaseCurrency.Name = "US Dollar";
                            _BaseCurrency.ExchangeRate = 1;
                            _BaseCurrency.ISOCode = "USD";
                            _BaseCurrency.ISOCodePattern = 0;
                            _BaseCurrency.CurrencySymbol = "$";
                            _BaseCurrency.PositivePattern = 0;
                            _BaseCurrency.NegativePattern = 1;
                            _BaseCurrency.NegativeSign = "-";
                            _BaseCurrency.DecimalSeparator = ".";
                            _BaseCurrency.DecimalDigits = 2;
                            _BaseCurrency.GroupSeparator = ",";
                            _BaseCurrency.GroupSizes = "3";
                            _BaseCurrency.Save();
                        }
                        //UPDATE THE BASE CURRENCY ID
                        this.Settings.BaseCurrencyId = _BaseCurrency.CurrencyId;
                        this.Settings.Save();
                    }
                    else _BaseCurrency = this.Currencies[index];
                }
                return _BaseCurrency;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _BaseCurrency = value;
                this.Settings.BaseCurrencyId = _BaseCurrency.CurrencyId;
            }
        }

        /// <summary>
        /// Save this store object to database
        /// </summary>
        /// <returns>Result of the save operation</returns>
        public SaveResult Save()
        {
            //USE A CUSTOM SAVE SO THAT ORDER NUMBER CAN BE REMOVED
            //WE SHOULD ONLY UPDATE ORDER NUMBER THROUGH THE TRANSACTIONALIZED
            //DATASOURCE ACCESS METHODS
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;

                if (this.StoreId == 0) recordExists = false;

                if (recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_Stores");
                    selectQuery.Append(" WHERE StoreId = @storeId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, this.StoreId);
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
                    updateQuery.Append("UPDATE ac_Stores SET ");
                    updateQuery.Append("Name = @Name");
                    updateQuery.Append(", ApplicationName = @ApplicationName");
                    updateQuery.Append(", LoweredApplicationName = @LoweredApplicationName");
                    updateQuery.Append(", LicenseKey = @LicenseKey");
                    updateQuery.Append(", DefaultWarehouseId = @DefaultWarehouseId");
                    updateQuery.Append(", OrderIdIncrement = @OrderIdIncrement");
                    updateQuery.Append(", WeightUnitId = @WeightUnitId");
                    updateQuery.Append(", MeasurementUnitId = @MeasurementUnitId");
                    updateQuery.Append(" WHERE StoreId = @StoreId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(updateCommand, "@Name", System.Data.DbType.String, this.Name);
                        database.AddInParameter(updateCommand, "@ApplicationName", System.Data.DbType.String, this.ApplicationName);
                        database.AddInParameter(updateCommand, "@LoweredApplicationName", System.Data.DbType.String, this.LoweredApplicationName);
                        database.AddInParameter(updateCommand, "@LicenseKey", System.Data.DbType.String, NullableData.DbNullify(this.LicenseKey));
                        database.AddInParameter(updateCommand, "@DefaultWarehouseId", System.Data.DbType.Int32, NullableData.DbNullify(this.DefaultWarehouseId));
                        database.AddInParameter(updateCommand, "@OrderIdIncrement", System.Data.DbType.Int16, this.OrderIdIncrement);
                        database.AddInParameter(updateCommand, "@WeightUnitId", System.Data.DbType.Int16, this.WeightUnitId);
                        database.AddInParameter(updateCommand, "@MeasurementUnitId", System.Data.DbType.Int16, this.MeasurementUnitId);
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_Stores (Name, ApplicationName, LoweredApplicationName, LicenseKey, DefaultWarehouseId, NextOrderId, OrderIdIncrement, WeightUnitId, MeasurementUnitId)");
                    insertQuery.Append(" VALUES (@Name, @ApplicationName, @LoweredApplicationName, @LicenseKey, @DefaultWarehouseId, @NextOrderId, @OrderIdIncrement, @WeightUnitId, @MeasurementUnitId)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(insertCommand, "@Name", System.Data.DbType.String, this.Name);
                        database.AddInParameter(insertCommand, "@ApplicationName", System.Data.DbType.String, this.ApplicationName);
                        database.AddInParameter(insertCommand, "@LoweredApplicationName", System.Data.DbType.String, this.LoweredApplicationName);
                        database.AddInParameter(insertCommand, "@LicenseKey", System.Data.DbType.String, NullableData.DbNullify(this.LicenseKey));
                        database.AddInParameter(insertCommand, "@DefaultWarehouseId", System.Data.DbType.Int32, NullableData.DbNullify(this.DefaultWarehouseId));
                        database.AddInParameter(insertCommand, "@NextOrderId", System.Data.DbType.Int32, this.NextOrderId);
                        database.AddInParameter(insertCommand, "@OrderIdIncrement", System.Data.DbType.Int16, this.OrderIdIncrement);
                        database.AddInParameter(insertCommand, "@WeightUnitId", System.Data.DbType.Int16, this.WeightUnitId);
                        database.AddInParameter(insertCommand, "@MeasurementUnitId", System.Data.DbType.Int16, this.MeasurementUnitId);
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._StoreId = result;
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

    }
}
