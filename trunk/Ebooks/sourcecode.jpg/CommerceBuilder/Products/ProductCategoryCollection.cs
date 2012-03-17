using System;
using System.Collections.Generic;
using CommerceBuilder.Catalog;
using System.Text;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// A collection of category Ids associated with a product
    /// </summary>
    public class ProductCategoryCollection : List<int>
    {
        private int _ProductId;
        private Product _Product;
        List<int> _OrigValue = null;

        /// <summary>
        /// The Id of the product for this collection
        /// </summary>
        public int ProductId
        {
            get { return _ProductId; }
            set
            {
                if (_ProductId != value)
                {
                    _ProductId = value;
                    _Product = null;
                }
            }
        }

        /// <summary>
        /// The product object for this collection
        /// </summary>
        public Product Product
        {
            get
            {
                if (_Product == null) _Product = ProductDataSource.Load(_ProductId);
                return _Product;
            }
        }

        /// <summary>
        /// Saves this collection to database
        /// </summary>
        /// <returns><b>true</b> if save successful, <b>false</b> otherwise</returns>
        public bool Save()
        {
            Database database = Token.Instance.Database;
            //DELETE ANY CATEGORIES THAT APPEAR IN _ORIGVALUE BUT NOT IN CURRENT LIST
            foreach (int categoryId in _OrigValue)
            {
                if (this.IndexOf(categoryId) < 0)
                {
                    using (DbCommand selectCommand = database.GetSqlStringCommand("DELETE FROM ac_CatalogNodes WHERE CategoryId = @categoryId AND CatalogNodeId = @productId AND CatalogNodeTypeId = 1"))
                    {
                        database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);
                        database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, _ProductId);
                        database.ExecuteNonQuery(selectCommand);
                    }
                }
            }
            //ADD ANY CATEGORIES THAT DO NOT APPEAR IN _ORIGVALUE
            foreach (int categoryId in this)
            {
                if (_OrigValue.IndexOf(categoryId) < 0)
                {
                    CatalogNode node = new CatalogNode();
                    node.CategoryId = categoryId;
                    node.CatalogNodeId = _ProductId;
                    node.CatalogNodeType = CommerceBuilder.Catalog.CatalogNodeType.Product;
                    node.OrderBy = -1;
                    node.Save();
                }
            }
            //UPDATE THE ORIGVALUE WITH THE SAVED LIST
            _OrigValue = new List<int>();
            _OrigValue.AddRange(this);
            return true;
        }

        /// <summary>
        /// Loads this collection for the given product
        /// </summary>
        /// <param name="productId">Id of the product to load this collection for</param>
        public void Load(int productId)
        {
            _ProductId = productId;
            _Product = null;
            _OrigValue = new List<int>();
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT CategoryId FROM ac_CatalogNodes");
            selectQuery.Append(" WHERE CatalogNodeId = @productId AND CatalogNodeTypeId = 1");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    _OrigValue.Add(dr.GetInt32(0));
                }
                dr.Close();
            }
            this.Clear();
            this.AddRange(_OrigValue);
        }
    }
}
