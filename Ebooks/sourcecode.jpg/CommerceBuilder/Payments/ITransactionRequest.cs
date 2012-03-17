using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Payments
{
    /// <summary>
    /// Interface that represents a generic transaction request
    /// </summary>
    public interface ITransactionRequest
    {
        /// <summary>
        /// The amount of transaction
        /// </summary>
        LSDecimal Amount { get; set; }

        /// <summary>
        /// The currency used
        /// </summary>
        string CurrencyCode { get; set; }

        /// <summary>
        /// Name-Value pairs containg additional configuration properties
        /// </summary>
        Dictionary<string, string> ExtendedProperties { get; }
        
        /// <summary>
        /// Payment associated with this transaction
        /// </summary>
        Payment Payment { get; }

        /// <summary>
        /// Remote IP of the User initiating the transaction
        /// </summary>
        string RemoteIP { get; set;}

        /// <summary>
        /// The type of transaction
        /// </summary>
        TransactionType TransactionType { get; }
    }
}
