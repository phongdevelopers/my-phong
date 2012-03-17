//-----------------------------------------------------------------------
// <copyright file="KeywordCriterion.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Search
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using CommerceBuilder.Data;

    /// <summary>
    /// Provides access to the parsed where clause and database parameters
    /// needed to search for a keyword without FTS
    /// </summary>
    public class KeywordCriterion
    {
        /// <summary>
        /// Where clause field
        /// </summary>
        private string _WhereClause;

        /// <summary>
        /// Internal parameters collection
        /// </summary>
        private Collection<DatabaseParameter> _Parameters;

        /// <summary>
        /// Initializes a new instance of the KeywordCriterion class.
        /// </summary>
        /// <param name="whereClause">The where clause</param>
        /// <param name="parameters">The parameters</param>
        internal KeywordCriterion(string whereClause, IEnumerable<DatabaseParameter> parameters)
        {
            this._WhereClause = whereClause;
            this._Parameters = new Collection<DatabaseParameter>();
            foreach (DatabaseParameter param in parameters)
            {
                this._Parameters.Add(param);
            }
        }

        /// <summary>
        /// Gets the where clause
        /// </summary>
        public string WhereClause
        {
            get { return this._WhereClause; }
        }

        /// <summary>
        /// Gets the required database parameters
        /// </summary>
        public Collection<DatabaseParameter> Parameters
        {
            get { return this._Parameters; }
        }
    }
}