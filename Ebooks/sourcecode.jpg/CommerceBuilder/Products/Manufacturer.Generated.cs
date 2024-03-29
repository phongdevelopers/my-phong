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
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// This class represents a Manufacturer object in the database.
    /// </summary>
    public partial class Manufacturer : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Manufacturer() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="manufacturerId">Value of ManufacturerId.</param>
        /// </summary>
        public Manufacturer(Int32 manufacturerId)
        {
            this.ManufacturerId = manufacturerId;
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
          columnNames.Add(prefix + "ManufacturerId");
          columnNames.Add(prefix + "StoreId");
          columnNames.Add(prefix + "Name");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given Manufacturer object from the given database data reader.
        /// </summary>
        /// <param name="manufacturer">The Manufacturer object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(Manufacturer manufacturer, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            manufacturer.ManufacturerId = dr.GetInt32(0);
            manufacturer.StoreId = dr.GetInt32(1);
            manufacturer.Name = dr.GetString(2);
            manufacturer.IsDirty = false;
        }

#endregion

        private Int32 _ManufacturerId;
        private Int32 _StoreId;
        private String _Name = string.Empty;
        private bool _IsDirty;

        /// <summary>
        /// ManufacturerId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 ManufacturerId
        {
            get { return this._ManufacturerId; }
            set
            {
                if (this._ManufacturerId != value)
                {
                    this._ManufacturerId = value;
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
        /// Indicates whether this Manufacturer object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                if (this._IsDirty) return true;
                if (this.ProductsLoaded && this.Products.IsDirty) return true;
                return false;
            }
            set { this._IsDirty = value; }
        }

        /// <summary>
        /// Ensures that child objects of this Manufacturer are properly associated with this Manufacturer object.
        /// </summary>
        public virtual void EnsureChildProperties()
        {
            if (this.ProductsLoaded) { foreach (Product product in this.Products) { product.ManufacturerId = this.ManufacturerId; } }
        }

#region Parents
        private Store _Store;

        /// <summary>
        /// The Store object that this Manufacturer object is associated with
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
        private ProductCollection _Products;

        /// <summary>
        /// A collection of Product objects associated with this Manufacturer object.
        /// </summary>
        public ProductCollection Products
        {
            get
            {
                if (!this.ProductsLoaded)
                {
                    this._Products = ProductDataSource.LoadForManufacturer(this.ManufacturerId);
                }
                return this._Products;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool ProductsLoaded { get { return (this._Products != null); } }

#endregion

        /// <summary>
        /// Deletes this Manufacturer object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_Manufacturers");
            deleteQuery.Append(" WHERE ManufacturerId = @ManufacturerId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@ManufacturerId", System.Data.DbType.Int32, this.ManufacturerId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this Manufacturer object from the database for the given primary key.
        /// </summary>
        /// <param name="manufacturerId">Value of ManufacturerId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 manufacturerId)
        {
            bool result = false;
            this.ManufacturerId = manufacturerId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Manufacturers");
            selectQuery.Append(" WHERE ManufacturerId = @manufacturerId");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@manufacturerId", System.Data.DbType.Int32, manufacturerId);
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
        /// Saves this Manufacturer object to the database.
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
                if (this.ManufacturerId == 0) recordExists = false;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_Manufacturers");
                    selectQuery.Append(" WHERE ManufacturerId = @ManufacturerId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@ManufacturerId", System.Data.DbType.Int32, this.ManufacturerId);
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
                    updateQuery.Append("UPDATE ac_Manufacturers SET ");
                    updateQuery.Append("StoreId = @StoreId");
                    updateQuery.Append(", Name = @Name");
                    updateQuery.Append(" WHERE ManufacturerId = @ManufacturerId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@ManufacturerId", System.Data.DbType.Int32, this.ManufacturerId);
                        database.AddInParameter(updateCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(updateCommand, "@Name", System.Data.DbType.String, this.Name);
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_Manufacturers (StoreId, Name)");
                    insertQuery.Append(" VALUES (@StoreId, @Name)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@ManufacturerId", System.Data.DbType.Int32, this.ManufacturerId);
                        database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(insertCommand, "@Name", System.Data.DbType.String, this.Name);
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._ManufacturerId = result;
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
        /// Saves that child objects associated with this Manufacturer object.
        /// </summary>
        public virtual void SaveChildren()
        {
            this.EnsureChildProperties();
            if (this.ProductsLoaded) this.Products.Save();
        }

     }
}
