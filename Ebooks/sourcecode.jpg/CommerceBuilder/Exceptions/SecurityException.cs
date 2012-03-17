using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Exceptions
{
    /// <summary>
    /// Class that represents a security exception
    /// </summary>
    public class SecurityException : CommerceBuilderException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SecurityException() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public SecurityException(string message) : base(message) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public SecurityException(string message, Exception innerException) : base(message, innerException) { }
    }
}
