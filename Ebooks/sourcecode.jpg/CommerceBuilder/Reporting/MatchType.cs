using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// Enumeration that represents the type of a match
    /// </summary>
    public enum MatchType : byte
    {
        /// <summary>
        /// An equality match
        /// </summary>
        EqualTo, 
        
        /// <summary>
        /// A containment match
        /// </summary>
        Contains
    }
}
