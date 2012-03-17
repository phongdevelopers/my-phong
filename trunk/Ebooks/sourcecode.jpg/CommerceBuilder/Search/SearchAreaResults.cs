//-----------------------------------------------------------------------
// <copyright file="SearchAreaResults.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Search
{
    /// <summary>
    /// Represents a collection of SearchResults for a specific SearchArea
    /// </summary>
    public partial class SearchAreaResults
    {
        private SearchArea _SearchArea;

        /// <summary>
        /// Search area which was searched
        /// </summary>
        public SearchArea SearchArea
        {
            get { return _SearchArea; }
            set { _SearchArea = value; }
        }

        private int _TotalMatches;
        /// <summary>
        /// Total number of results available for search keywords in this search area
        /// </summary>
        public int TotalMatches
        {
            get { return _TotalMatches; }
            set { _TotalMatches = value; }
        }

        private List<SearchResult> _SearchResults;
        /// <summary>
        /// List for search results
        /// </summary>
        public List<SearchResult> SearchResults
        {
            get {
                if (_SearchResults == null) _SearchResults = new List<SearchResult>();
                return _SearchResults; 
            }
            set { _SearchResults = value; }
        }
    }
}
