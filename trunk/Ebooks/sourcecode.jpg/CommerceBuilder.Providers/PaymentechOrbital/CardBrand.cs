using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments.Providers.PaymentechOrbital
{
    internal enum CardBrand
    {
        /// <summary>
        ///  Credit Card
        /// </summary>
        CC, 
        /// <summary>
        /// Switch / Solo
        /// </summary>
        SW,
        /// <summary>
        /// Electronic Check
        /// </summary>
        EC
    }
}
