//-----------------------------------------------------------------------
// <copyright file="RoundingRule.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Taxes
{
    /// <summary>
    /// Enum used to establish rules for rounding tax calculations
    /// </summary>
    public enum RoundingRule
    {
        /// <summary>
        /// Round up on 5 if the number is odd, round down on five if the number is even
        /// </summary>
        RoundToEven,

        /// <summary>
        /// Round up on 5
        /// </summary>
        Common,        

        /// <summary>
        /// Always round up to the next whole number for fractional amounts
        /// </summary>
        AlwaysRoundUp
    }
}
