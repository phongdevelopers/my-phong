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
    /// This class represents a RelatedProduct object in the database.
    /// </summary>
    public partial class RelatedProduct : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public RelatedProduct() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="productId">Value of ProductId.</param>
        /// <param name="childProductId">Value of ChildProductId.</param>
        /// </summary>
        public RelatedProduct(Int32 productId, Int32 childProductId)
        {
            this.ProductId = productId;
            this.ChildProductId = childProductId;
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
          columnNames.Add(prefix + "OrderBy");
          columnNames.Add(prefix + "ChildProductId");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given RelatedProduct object from the given database data reader.
        /// </summary>
        /// <param name="relatedProduct">The RelatedProduct object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(RelatedProduct relatedProduct, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            relatedProduct.ProductId = dr.GetInt32(0);
            relatedProduct.OrderBy = dr.GetInt16(1);
            relatedProduct.ChildProductId = dr.GetInt32(2);
            relatedProduct.IsDirty = false;
        }

#endregion

        private Int32 _ProductId;
        private Int16 _OrderBy = -1;
        private Int32 _ChildProductId;
        private bool _IsDirty;

        /// <summary>
        /// ProductId
        /// </summary>
        [DataObjectField(true, false, false)]
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
        /// OrderBy
        /// </summary>
        public Int16 OrderBy
        {
            get { return this._OrderBy; }
            set
            {
                if (this._OrderBy != value)
                {
                    this._OrderBy = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// ChildProductId
        /// </summary>
        [DataObjectField(true, false, false)]
        public Int32 ChildProductId
        {
            get { return this._ChildProductId; }
            set
            {
                if (this._ChildProductId != value)
                {
                    this._ChildProductId = value;
                    this.IsDirty = true;
                    this._Product = null;
                }
            }
        }

        /// <summary>
        /// Indicates whether this RelatedProduct object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private Product _Product;

        /// <summary>
        /// The Product object that this RelatedProduct object is associated with
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
        /// Deletes this RelatedProduct object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_RelatedProducts");
            deleteQuery.Append(" WHERE ProductId = @ProductId AND ChildProductId = @ChildProductId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
                database.AddInParameter(deleteCommand, "@ChildProductId", System.Data.DbType.Int32, this.ChildProductId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this RelatedProduct object from the database for the given primary key.
        /// </summary>
        /// <param name="productId">Value of ProductId of the object to load.</param>
        /// <param name="childProductId">Value of ChildProductId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 productId, Int32 childProductId)
        {
            bool result = false;
            this.ProductId = productId;
            this.ChildProductId = childProductId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_RelatedProducts");
            selectQuery.Append(" WHERE ProductId = @productId AND ChildProductId = @childProductId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            database.AddInParameter(selectCommand, "@childProductId", System.Data.DbType.Int32, childProductId);
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
        /// Saves this RelatedProduct object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;
                
                if (this.OrderBy < 0) this.OrderBy = RelatedProductDataSource.GetNextOrderBy(this.ProductId);

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_RelatedProducts");
                    selectQuery.Append(" WHERE ProductId = @ProductId AND ChildProductId = @ChildProductId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
                        database.AddInParameter(selectCommand, "@ChildProductId", System.Data.DbType.Int32, this.ChildProductId);
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
                    updateQuery.Append("UPDATE ac_RelatedProducts SET ");
                    updateQuery.Append("OrderBy = @OrderBy");
                    updateQuery.Append(" WHERE ProductId = @ProductId AND ChildProductId = @ChildProductId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
                        database.AddInParameter(updateCommand, "@OrderBy", System.Data.DbType.Int16, this.OrderBy);
                        database.AddInParameter(updateCommand, "@ChildProductId", System.Data.DbType.Int32, this.ChildProductId);
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_RelatedProducts (ProductId, OrderBy, ChildProductId)");
                    insertQuery.Append(" VALUES (@ProductId, @OrderBy, @ChildProductId)");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@ProductId", System.Data.DbType.Int32, this.ProductId);
                        database.AddInParameter(insertCommand, "@OrderBy", System.Data.DbType.Int16, this.OrderBy);
                        database.AddInParameter(insertCommand, "@ChildProductId", System.Data.DbType.Int32, this.ChildProductId);
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