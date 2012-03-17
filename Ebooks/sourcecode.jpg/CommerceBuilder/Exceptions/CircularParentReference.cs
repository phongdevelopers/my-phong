using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Exceptions
{
    /// <summary>
    /// Exception representing a circular parent reference error
    /// </summary>
    public class CircularParentReference : CommerceBuilderException
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public CircularParentReference() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public CircularParentReference(string message) : base(message) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">The inner exception</param>
        public CircularParentReference(string message, Exception innerException) : base(message, innerException) { }
    }
}
