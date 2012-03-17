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

namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// Collection of Category Ids for a WebPage object
    /// </summary>
    public class WebpageCategoryCollection : List<int>
    {
        private int _WebpageId;
        private Webpage _Webpage;
        List<int> _OrigValue = null;

        /// <summary>
        /// Webpage Id
        /// </summary>
        public int WebpageId
        {
            get { return _WebpageId; }
            set
            {
                if (_WebpageId != value)
                {
                    _WebpageId = value;
                    _Webpage = null;
                }
            }
        }

        /// <summary>
        /// The Webpage object
        /// </summary>
        public Webpage Webpage
        {
            get
            {
                if (_Webpage == null) _Webpage = WebpageDataSource.Load(_WebpageId);
                return _Webpage;
            }
        }

        /// <summary>
        /// Saves this collection to database
        /// </summary>
        /// <returns><b>true</b> if save is successful, <b>false</b> otherwise</returns>
        public bool Save()
        {
            Database database = Token.Instance.Database;
            //DELETE ANY CATEGORIES THAT APPEAR IN _ORIGVALUE BUT NOT IN CURRENT LIST
            foreach (int categoryId in _OrigValue)
            {
                if (this.IndexOf(categoryId) < 0)
                {
                    using (DbCommand selectCommand = database.GetSqlStringCommand("DELETE FROM ac_CatalogNodes WHERE CategoryId = @categoryId AND CatalogNodeId = @webpageId AND CatalogNodeTypeId = 2"))
                    {
                        database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);
                        database.AddInParameter(selectCommand, "@webpageId", System.Data.DbType.Int32, _WebpageId);
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
                    node.CatalogNodeId = _WebpageId;
                    node.CatalogNodeType = CommerceBuilder.Catalog.CatalogNodeType.Webpage;
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
        /// Loads this collection for given WegpageId
        /// </summary>
        /// <param name="webpageId">Id of the Wegpage for which to load this collection</param>
        public void Load(int webpageId)
        {
            _WebpageId = webpageId;
            _Webpage = null;
            _OrigValue = new List<int>();
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT CategoryId FROM ac_CatalogNodes");
            selectQuery.Append(" WHERE CatalogNodeId = @webpageId AND CatalogNodeTypeId = 2");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@webpageId", System.Data.DbType.Int32, webpageId);
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
