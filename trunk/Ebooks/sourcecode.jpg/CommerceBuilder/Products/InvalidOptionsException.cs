using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// Invalid options exception
    /// </summary>
    public class InvalidOptionsException : CommerceBuilder.Exceptions.CommerceBuilderException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidOptionsException() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public InvalidOptionsException(string message) : base(message) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public InvalidOptionsException(string message, Exception innerException) : base(message, innerException) { }
    }
}