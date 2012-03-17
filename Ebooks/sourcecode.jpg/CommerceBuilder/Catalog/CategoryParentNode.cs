using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Catalog
{
    /// <summary>
    /// Class representing level of a category in category hierarchy
    /// </summary>
    public class CategoryLevelNode
    {
        private int _CategoryId;
        private int _ParentId;
        private string _Name;
        private int _CategoryLevel;

        /// <summary>
        /// Id of the category
        /// </summary>
        public int CategoryId
        {
            get { return _CategoryId; }
            set { _CategoryId = value; }
        }

        /// <summary>
        /// Id of the parent of the category
        /// </summary>
        public int ParentId
        {
            get { return _ParentId; }
            set { _ParentId = value; }
        }

        /// <summary>
        /// Name of the category
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        
        /// <summary>
        /// Level of the category
        /// </summary>
        public int CategoryLevel
        {
            get { return _CategoryLevel; }
            set { _CategoryLevel = value; }
        }
    }
}
