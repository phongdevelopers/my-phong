//-----------------------------------------------------------------------
// <copyright file="AddressValidatorServiceBase.cs" company="Able Solutions Corporation">
//     Copyright (c) 2010 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Shipping.Providers
{
    using CommerceBuilder.Users;

    /// <summary>
    /// Base class for building address validation services
    /// </summary>
    public abstract class AddressValidatorServiceBase : IAddressValidatorService
    {
        /// <summary>
        /// Validates a warehouse address
        /// </summary>
        /// <param name="warehouse">The warehouse address to validate</param>
        public abstract void ValidateAddress(Warehouse warehouse);

                /// <summary>
        /// Validates a customer address
        /// </summary>
        /// <param name="address">The customer address to validate</param>
        public abstract void ValidateAddress(Address address);
    }
}
