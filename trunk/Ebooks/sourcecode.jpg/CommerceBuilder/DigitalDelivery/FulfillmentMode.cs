//-----------------------------------------------------------------------
// <copyright file="FulfillmentMode.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.DigitalDelivery
{
    using System;

    /// <summary>
    /// Enumeration that represents the fulfillment mode of a digital good.
    /// </summary>
    public enum FulfillmentMode
    {
        /// <summary>
        /// Manual fulfillment
        /// </summary>
        Manual = 0,

        /// <summary>
        /// Automatic fulfillment on placement of order
        /// </summary>
        OnOrder,

        /// <summary>
        /// Automatic fulfillment on payment of order
        /// </summary>
        OnPaidOrder
    }
}