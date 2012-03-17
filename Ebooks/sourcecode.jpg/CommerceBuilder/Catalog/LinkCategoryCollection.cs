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
    /// A collection of category Ids for a link object
    /// </summary>
    public class LinkCategoryCollection : List<int>
    {
        private int _LinkId;
        private Link _Link;
        List<int> _OrigValue = null;

        /// <summary>
        /// Id of the Link object
        /// </summary>
        public int LinkId
        {
            get { return _LinkId; }
            set
            {
                if (_LinkId != value)
                {
                    _LinkId = value;
                    _Link = null;
                }
            }
        }

        /// <summary>
        /// The Link object
        /// </summary>
        public Link Link
        {
            get
            {
                if (_Link == null) _Link = LinkDataSource.Load(_LinkId);
                return _Link;
            }
        }

        /// <summary>
        /// Saves this collection to the database
        /// </summary>
        /// <returns><b>true</b> if all items of this collection are saved to database successfuly, <b>false</b> otherwise</returns>
        public bool Save()
        {
            Database database = Token.Instance.Database;
            //DELETE ANY CATEGORIES THAT APPEAR IN _ORIGVALUE BUT NOT IN CURRENT LIST
            foreach (int categoryId in _OrigValue)
            {
                if (this.IndexOf(categoryId) < 0)
                {
                    using (DbCommand selectCommand = database.GetSqlStringCommand("DELETE FROM ac_CatalogNodes WHERE CategoryId = @categoryId AND CatalogNodeId = @linkId AND CatalogNodeTypeId = @catalogNodeTypeId"))
                    {
                        database.AddInParameter(selectCommand, "@categoryId", System.Data.DbType.Int32, categoryId);
                        database.AddInParameter(selectCommand, "@linkId", System.Data.DbType.Int32, _LinkId);
                        database.AddInParameter(selectCommand, "@catalogNodeTypeId", System.Data.DbType.Byte, CommerceBuilder.Catalog.CatalogNodeType.Link);
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
                    node.CatalogNodeId = _LinkId;
                    node.CatalogNodeType = CommerceBuilder.Catalog.CatalogNodeType.Link;
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
        /// Loads this collection for given link id
        /// </summary>
        /// <param name="linkId">Link Id to load this collection for</param>
        public void Load(int linkId)
        {
            _LinkId = linkId;
            _Link = null;
            _OrigValue = new List<int>();
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT CategoryId FROM ac_CatalogNodes");
            selectQuery.Append(" WHERE CatalogNodeId = @linkId AND CatalogNodeTypeId = 3");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@linkId", System.Data.DbType.Int32, linkId);
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
