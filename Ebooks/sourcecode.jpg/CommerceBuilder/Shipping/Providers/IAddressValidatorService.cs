//-----------------------------------------------------------------------
// <copyright file="IAddressValidatorService.cs" company="Able Solutions Corporation">
//     Copyright (c) 2010 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Shipping.Providers
{
    using CommerceBuilder.Users;

    /// <summary>
    /// Defines a service for validating AbleCommerce addresses
    /// </summary>
    public interface IAddressValidatorService
    {
        /// <summary>
        /// Validates a warehouse address
        /// </summary>
        /// <param name="warehouse">The warehouse address to validate</param>
        void ValidateAddress(Warehouse warehouse);

        /// <summary>
        /// Validates a customer address
        /// </summary>
        /// <param name="address">The customer address to validate</param>
        void ValidateAddress(Address address);
    }
}
