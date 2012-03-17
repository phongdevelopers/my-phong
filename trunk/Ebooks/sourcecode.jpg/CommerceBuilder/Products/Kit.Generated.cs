//CUSTOMIZED
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
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// This class represents a Kit object in the database.
    /// </summary>
    public partial class Kit : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Kit() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="productId">Value of ProductId.</param>
        /// </summary>
        public Kit(Int32 productId)
        {
            this.ProductId = productId;
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
          columnNames.Add(prefix + "ProductId");
          columnNames.Add(prefix + "ItemizeDisplay");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given Kit object from the given database data reader.
        /// </summary>
        /// <param name="kit">The Kit object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(Kit kit, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            kit.ProductId = dr.GetInt32(0);
            kit.ItemizeDisplay = dr.GetBoolean(1);
            kit.IsDirty = false;
        }

#endregion

        private Int32 _ProductId;
        private Boolean _ItemizeDisplay;
        private bool _IsDirty;

        /// <summary>
        /// ProductId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 ProductId
        {
            get { return this._ProductId; }
            set
            {
                if (this._ProductId != value)
                {
                    this._ProductId = value;
                    this.IsDirty = true;
                    this._Product = null;
                }
            }
        }

        /// <summary>
        /// ItemizeDisplay
        /// </summary>
        public Boolean ItemizeDisplay
        {
            get { return this._ItemizeDisplay; }
            set
            {
                if (this._ItemizeDisplay != value)
                {
                    this._ItemizeDisplay = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this Kit object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private Product _Product;

        /// <summary>
        /// The Product object that this Kit object is associated with
        /// </summary>
        public Product Product
        {
            get
            {
                if (!this.ProductLoaded)
                {
                    this._Product = ProductDataSource.Load(this.ProductId);
                }
                return this._Product;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool ProductLoaded { get { return ((this._Product != null) && (this._Product.ProductId == this.ProductId)); } }

#endregion

        /// <summary>
        /// Deletes this Kit object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_Kits");
            deleteQuery.Append(" WHERE ProductId = @ProductId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this Kit object from the database for the given primary key.
        /// </summary>
        /// <param name="productId">Value of ProductId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 productId)
        {
            if (productId < 1) throw new ArgumentException("productId", "productId must be greater than 0");
            _Product = ProductDataSource.Load(productId);
            if (_Product == null) throw new InvalidProductException("The productId specified is invalid.");
            _ProductId = productId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Kits");
            selectQuery.Append(" WHERE ProductId = @productId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    LoadDataReader(this, dr);;
                }
                dr.Close();
            }
            return true;
        }

        /// <summary>
        /// Saves this Kit object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        protected SaveResult BaseSave()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;
                
                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_Kits");
                    selectQuery.Append(" WHERE ProductId = @ProductId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
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
                    updateQuery.Append("UPDATE ac_Kits SET ");
                    updateQuery.Append("ItemizeDisplay = @ItemizeDisplay");
                    updateQuery.Append(" WHERE ProductId = @ProductId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
                        database.AddInParameter(updateCommand, "@ItemizeDisplay", System.Data.DbType.Boolean, this.ItemizeDisplay);
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_Kits (ProductId, ItemizeDisplay)");
                    insertQuery.Append(" VALUES (@ProductId, @ItemizeDisplay)");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
                        database.AddInParameter(insertCommand, "@ItemizeDisplay", System.Data.DbType.Boolean, this.ItemizeDisplay);
                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);
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
