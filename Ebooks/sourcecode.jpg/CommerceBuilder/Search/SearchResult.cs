using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Search
{
    /// <summary>
    /// Represents a single search result
    /// </summary>
    public partial class SearchResult
    {
        private int _Id;
        /// <summary>
        /// Id of the Object.
        /// </summary>
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        private string _Name;
        /// <summary>
        /// Name of the object
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
    }
}
