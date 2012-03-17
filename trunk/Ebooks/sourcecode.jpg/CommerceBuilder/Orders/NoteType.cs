using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Orders
{
    /// <summary>
    /// Enumeration that represents type of a note
    /// </summary>
    public enum NoteType
    {
        /// <summary>
        /// This is a public note.
        /// </summary>
        Public, 
        
        /// <summary>
        /// This is a private note.
        /// </summary>
        Private, 
        
        /// <summary>
        /// This is a public note by the system.
        /// </summary>
        SystemPublic, 
        
        /// <summary>
        /// This is a private note by the system.
        /// </summary>
        SystemPrivate
    }
}
