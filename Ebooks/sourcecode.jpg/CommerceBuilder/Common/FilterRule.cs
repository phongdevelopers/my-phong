using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Common
{
    /// <summary>
    /// Enum used to establish rules for filtering (such as shipzones and a list of associated countries)
    /// </summary>
    public enum FilterRule
    {
        /// <summary>
        /// Include all results (do not filter this critiera)
        /// </summary>
        All,

        /// <summary>
        /// Only include items in the linked list
        /// </summary>
        IncludeSelected,

        /// <summary>
        /// Include all items except those in the linked list
        /// </summary>
        ExcludeSelected
    }
}