//CUSTOMIZED
using System;
using System.Collections.Generic;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Products;

namespace CommerceBuilder.Catalog
{
    public partial class Category : CatalogableBase
    {
        string _NavigateUrl = string.Empty;

        /// <summary>
        /// The Navigation URL of this category
        /// </summary>
        public override string NavigateUrl
        {
            get
            {
                if (_NavigateUrl.Length == 0)
                {
                    _NavigateUrl = UrlGenerator.GetBrowseUrl(this.CategoryId, CatalogNodeType.Category, this.Name);
                }
                return _NavigateUrl;
            }
        }

        CustomUrl _CustomUrl;

        public string CustomUrl
        {
            get
            {
                string url = string.Empty;
                if (_CustomUrl == null)
                    _CustomUrl = CustomUrlDataSource.LoadCustomUrl(this.CategoryId, CatalogNodeType.Category);
                if (_CustomUrl != null)
                    url = _CustomUrl.Url;
                return url;
            }
            set
            {
                string url = value.Trim();
                if (_CustomUrl == null)
                    _CustomUrl = CustomUrlDataSource.LoadCustomUrl(this.CategoryId, CatalogNodeType.Category);

                if (!string.IsNullOrEmpty(url) && _CustomUrl == null)
                {
                    _CustomUrl = new CustomUrl();
                    _CustomUrl.CatalogNodeId = this.CategoryId;
                    _CustomUrl.CatalogNodeTypeId = (byte)CatalogNodeType.Category;
                    _CustomUrl.Url = url;
                }
                else
                    if (_CustomUrl != null)
                    {
                        _CustomUrl.Url = url;
                    }
            }
        }

        /// <summary>
        /// Saves this category to database
        /// </summary>
        /// <returns>Result of the save operation</returns>
        public SaveResult Save()
        {
            bool parentChanged = false;
            int savedParentId = this.ParentId;
            Database database = Token.Instance.Database;
            //GET THE CURRENT PARENT ID OF THIS CATEGORY
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT ParentId FROM ac_Categories WHERE CategoryId = @categoryId");
            database.AddInParameter(selectCommand, "@categoryId", DbType.Int32, this.CategoryId);
            object scalarResult = database.ExecuteScalar(selectCommand);
            if (scalarResult != null)
            {
                int currentParentId = (int)scalarResult;
                if (!currentParentId.Equals(this.ParentId))
                {
                    //CHANGING THE PARENT ID SHOULD ONLY BE HANDLED BY THE SETPARENT FUNCTION
                    //RETAIN THE DESIRED PARENT ID AND RUN THE SAVE PROCEDURE USING THE CURRENT PARENT
                    //IN CASE THE SET PARENT FAILS THE EXISTING PARENTID REFERENCE WILL HOLD INTACT.
                    parentChanged = true;
                    this.ParentId = currentParentId;
                }
            }
            else
            {
                //NEW RECORD, PARENT IS CHANGING
                parentChanged = true;
            }
            SaveResult result = this.BaseSave();
            if ((result != SaveResult.Failed) && parentChanged)
            {
                Category.UpdateParent(this.CategoryId, savedParentId);
                this.ParentId = savedParentId;
                this.IsDirty = false;
            }
            if (_CustomUrl != null)
            {
                if (!string.IsNullOrEmpty(_CustomUrl.Url))
                {
                    _CustomUrl.CatalogNodeId = this.CategoryId;
                    _CustomUrl.CatalogNodeType = CatalogNodeType.Category;
                    _CustomUrl.Save();
                }
                else
                    _CustomUrl.Delete();
            }
            return result;
        }


        /// <summary>
        /// Deletes this category object from database
        /// </summary>
        /// <returns>true if delete successful, false otherwise</returns>
        public bool Delete()
        {
            // DELETE ALL CHILDREN, DELETE NODES THAT DO NOT APPEAR IN OTHER CATEGORIES
            foreach (CatalogNode node in CatalogNodes)
            {
                if (node.CatalogNodeType == CatalogNodeType.Product)
                {
                    Product product = (Product)node.ChildObject;
                    if (product.Categories.Count == 1) product.Delete();
                }
                else if (node.CatalogNodeType == CatalogNodeType.Webpage)
                {
                    Webpage webpage = (Webpage)node.ChildObject;
                    if (webpage.Categories.Count == 1) webpage.Delete();
                }
                else if (node.CatalogNodeType == CatalogNodeType.Link)
                {
                    Link link = (Link)node.ChildObject;
                    if (link.Categories.Count == 1) link.Delete();
                }
                else if (node.CatalogNodeType == CatalogNodeType.Category)
                {
                    Category cat = (Category)node.ChildObject;
                    if (cat != null) cat.Delete();
                }
                //DELETE THE CHILD NODE
                node.Delete();
            }

            //GET DATABASE
            Database database = Token.Instance.Database;
            DbCommand deleteCommand;

            //DELETE ALL PATHS
            deleteCommand = database.GetSqlStringCommand("DELETE FROM ac_CategoryParents WHERE CategoryId=@categoryId");
            database.AddInParameter(deleteCommand, "@categoryId", DbType.Int32, this.CategoryId);
            database.ExecuteNonQuery(deleteCommand);

            //DELETE ALL CATALOG NODES LINKED TO OR FROM THIS CATEGORY
            deleteCommand = database.GetSqlStringCommand("DELETE FROM ac_CatalogNodes WHERE (CategoryId=@categoryId OR (CatalogNodeId=@categoryId AND CatalogNodeTypeId=@catalogNodeTypeId))");
            database.AddInParameter(deleteCommand, "@categoryId", DbType.Int32, this.CategoryId);
            database.AddInParameter(deleteCommand, "@catalogNodeTypeId", System.Data.DbType.Byte, (byte)CatalogNodeType.Category);
            database.ExecuteNonQuery(deleteCommand);

            //DELTE CUSTOM URL INFORMATION IF THERE IS ANY
            CustomUrlDataSource.Delete(this.CategoryId, CatalogNodeType.Category);

            //PERFORM BASE DELETE
            return this.BaseDelete();
        }

        /// <summary>
        /// Updates the parent of the given category
        /// </summary>
        /// <param name="categoryId">Id of the category for which to update the parent</param>
        /// <param name="parentId">Parent Id to set</param>
        public static void UpdateParent(int categoryId, int parentId)
        {
            //GET DATABSAE REFERENCES
            Database database = Token.Instance.Database;
            DbCommand selectCommand;

            //VERIFY THE SPECIFIED CATEGORY IS VALID
            if (parentId != 0)
            {
                selectCommand = database.GetSqlStringCommand("SELECT CategoryId FROM ac_Categories WHERE CategoryId = @categoryId");
                database.AddInParameter(selectCommand, "@categoryId", DbType.Int32, categoryId);
                object scalarResult = database.ExecuteScalar(selectCommand);
                if (scalarResult == null) throw new ArgumentException("This category instance has not been saved to the database.", "CategoryId");
            }

            //VERIFY THE SELECTED PARENT IS VALID
            if (parentId != 0)
            {
                selectCommand = database.GetSqlStringCommand("SELECT CategoryId FROM ac_Categories WHERE CategoryId = @categoryId");
                database.AddInParameter(selectCommand, "@categoryId", DbType.Int32, parentId);
                object scalarResult = database.ExecuteScalar(selectCommand);
                if (scalarResult == null) throw new ArgumentException("Invalid parent category specified.", "parentId");
            }

            //CHECK FOR CIRCULAR REFRENCES
            if (categoryId.Equals(parentId)) throw new ArgumentException("Specified parent category would result in a circular reference.", "parentId");
            selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) As ChildReferences FROM ac_CategoryParents WHERE ParentId = @categoryId AND CategoryId = @parentId");
            database.AddInParameter(selectCommand, "@categoryId", DbType.Int32, categoryId);
            database.AddInParameter(selectCommand, "@parentId", DbType.Int32, parentId);
            int childReferences = (int)database.ExecuteScalar(selectCommand);
            if (childReferences > 0) throw new ArgumentException("Specified parent category would result in a circular reference.", "parentId");

            //UPDATE CATEGORY PARENT
            DbCommand updateCommand = database.GetSqlStringCommand("UPDATE ac_Categories SET ParentId = @parentId WHERE CategoryId = @categoryId");
            database.AddInParameter(updateCommand, "@categoryId", DbType.Int32, categoryId);
            database.AddInParameter(updateCommand, "@parentId", DbType.Int32, parentId);
            database.ExecuteNonQuery(updateCommand);

            //DELETE ANY EXISTING PARENT ASSOCIATIONS
            DbCommand deleteCommand = database.GetSqlStringCommand("DELETE FROM ac_CategoryParents WHERE CategoryId = @categoryId");
            database.AddInParameter(deleteCommand, "@categoryId", DbType.Int32, categoryId);
            database.ExecuteNonQuery(deleteCommand);
            deleteCommand = database.GetSqlStringCommand("DELETE FROM ac_CatalogNodes WHERE CatalogNodeTypeId = 0 AND CatalogNodeId = @categoryId");
            database.AddInParameter(deleteCommand, "@categoryId", DbType.Int32, categoryId);
            database.ExecuteNonQuery(deleteCommand);

            //NOW INSERT MODIFIED PARENT PATH
            StringBuilder insertQuery;
            DbCommand insertCommand;
            int level;
            if (parentId != 0)
            {
                insertQuery = new StringBuilder();
                insertQuery.Append("INSERT INTO ac_CategoryParents (CategoryId, ParentId, ParentLevel, ParentNumber)");
                insertQuery.Append(" SELECT @categoryId, ParentId, ParentLevel, (ParentNumber + 1)");
                insertQuery.Append(" FROM ac_CategoryParents");
                insertQuery.Append(" WHERE CategoryId = @parentCategoryId;");
                insertCommand = database.GetSqlStringCommand(insertQuery.ToString());
                database.AddInParameter(insertCommand, "@categoryId", DbType.Int32, categoryId);
                database.AddInParameter(insertCommand, "@parentCategoryId", DbType.Int32, parentId);
                database.ExecuteNonQuery(insertCommand);
                //GET LEVEL OF NEW CATEGORY
                selectCommand = database.GetSqlStringCommand("SELECT (ParentLevel + 1) FROM ac_CategoryParents WHERE CategoryId = @parentCategoryId AND ParentNumber=0");
                database.AddInParameter(selectCommand, "@parentCategoryId", DbType.Int32, parentId);
                level = AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
            }
            else
            {
                insertQuery = new StringBuilder();
                insertQuery.Append("INSERT INTO ac_CategoryParents (CategoryId, ParentId, ParentLevel, ParentNumber)");
                insertQuery.Append(" VALUES (@categoryId, 0, 0, 1)");
                insertCommand = database.GetSqlStringCommand(insertQuery.ToString());
                database.AddInParameter(insertCommand, "@categoryId", DbType.Int32, categoryId);
                database.ExecuteNonQuery(insertCommand);
                level = 1;
            }

            // NOW INSERT SELF-REFERENCING PATH
            insertQuery = new StringBuilder();
            insertQuery.Append("INSERT INTO ac_CategoryParents (CategoryId, ParentId, ParentLevel, ParentNumber)");
            insertQuery.Append(" VALUES (@categoryId, @categoryId, @level, 0);");
            insertCommand = database.GetSqlStringCommand(insertQuery.ToString());
            database.AddInParameter(insertCommand, "@categoryId", DbType.Int32, categoryId);
            database.AddInParameter(insertCommand, "@level", DbType.Int32, level);
            database.ExecuteNonQuery(insertCommand);

            //GET ORDER OF NEW CATEGORY
            selectCommand = database.GetSqlStringCommand("SELECT (MAX(OrderBy) + 1) FROM ac_CatalogNodes WHERE CategoryId = @parentCategoryId");
            database.AddInParameter(selectCommand, "@parentCategoryId", DbType.Int32, parentId);
            Int16 order = AlwaysConvert.ToInt16(database.ExecuteScalar(selectCommand));

            //ADD IN CATALOG NODE
            insertQuery = new StringBuilder();
            insertQuery.Append("INSERT INTO ac_CatalogNodes (CategoryId, CatalogNodeId, CatalogNodeTypeId, OrderBy)");
            insertQuery.Append(" VALUES (@parentCategoryId, @categoryId, 0, @order)");
            insertCommand = database.GetSqlStringCommand(insertQuery.ToString());
            database.AddInParameter(insertCommand, "@parentCategoryId", DbType.Int32, parentId);
            database.AddInParameter(insertCommand, "@categoryId", DbType.Int32, categoryId);
            database.AddInParameter(insertCommand, "@order", DbType.Int16, order);
            database.ExecuteNonQuery(insertCommand);

            // GET ANY CHILD CATEGORIES OF CURRENT CATEGORY
            List<int> catalogNodeIds = new List<int>();
            selectCommand = database.GetSqlStringCommand("SELECT CategoryId FROM ac_Categories WHERE ParentId = @categoryId");
            database.AddInParameter(selectCommand, "@categoryId", DbType.Int32, categoryId);
            using (IDataReader reader = database.ExecuteReader(selectCommand))
            {
                while (reader.Read())
                {
                    catalogNodeIds.Add(reader.GetInt32(0));
                }
                reader.Close();
            }
            foreach (int catalogNodeId in catalogNodeIds)
            {
                Category.UpdateParent(catalogNodeId, categoryId);
            }
        }

    }
}
