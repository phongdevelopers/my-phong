using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Catalog
{
    [DataObject(true)]
    public partial class CategoryParentDataSource
    {
        /// <summary>
        /// Gets the category tree of the store in a flat collection, sorted by level.
        /// </summary>
        /// <param name="exclude">ID of the category to exclude from the collection; 0 if none.</param>
        /// <returns>A collection of categories in the store, organized by level.</returns>
        public static CategoryLevelNodeCollection GetCategoryLevels(int exclude)
        {
            return GetCategoryLevels(0, 0, exclude, false);
        }

        /// <summary>
        /// Gets the category tree of the store in a flat collection, sorted by level.
        /// </summary>
        /// <param name="exclude">ID of the category to exclude from the collection; 0 if none.</param>
        /// <param name="publicOnly">If true, only include public categories in result set.</param>
        /// <returns>A collection of categories in the store, organized by level.</returns>
        public static CategoryLevelNodeCollection GetCategoryLevels(int exclude, bool publicOnly)
        {
            return GetCategoryLevels(0, 0, exclude, publicOnly);
        }

        /// <summary>
        /// Gets the children for a given category.
        /// </summary>
        /// <param name="categoryId">Category to get children for</param>
        /// <param name="level">Level of the specified category</param>
        /// <param name="exclude">ID of the category to exclude from the collection; 0 if none.</param>
        /// <param name="publicOnly">If true, only include public categories in result set.</param>
        /// <returns></returns>
        private static CategoryLevelNodeCollection GetCategoryLevels(int categoryId, int level, int exclude, bool publicOnly)
        {
            CategoryLevelNodeCollection results = new CategoryLevelNodeCollection();
            CatalogNodeCollection catalogNodes = CatalogDataSource.Search(categoryId, string.Empty, true, publicOnly, false, CatalogNodeTypeFlags.Category);
            foreach (CatalogNode node in catalogNodes)
            {
                if (node.CatalogNodeId != exclude)
                {
                    CategoryLevelNode levelNode = new CategoryLevelNode();
                    levelNode.CategoryId = node.CatalogNodeId;
                    levelNode.CategoryLevel = level;
                    levelNode.Name = node.Name;
                    levelNode.ParentId = categoryId;
                    results.Add(levelNode);
                    results.AddRange(GetCategoryLevels(levelNode.CategoryId, level + 1, exclude, publicOnly));
                }
            }
            return results;
        }

    }
}
