using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Common
{
    /// <summary>
    /// Enumeration that represents the state of a bit field.
    /// </summary>
    public enum BitFieldState
    {
        /// <summary>
        /// Any state; either true or false.
        /// </summary>
        Any, 
        
        /// <summary>
        /// The state is true
        /// </summary>
        True, 
        
        /// <summary>
        /// The state is false
        /// </summary>
        False
    }
}
