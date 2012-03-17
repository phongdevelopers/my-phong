using System;
namespace CommerceBuilder.Data
{
    /// <summary>
    /// Update behavior for updating dataset
    /// </summary>
    public enum UpdateBehavior
    {
        /// <summary>
        /// Standard behavior
        /// </summary>
        Standard,
        /// <summary>
        /// Continue behavior
        /// </summary>
        Continue,
        /// <summary>
        /// Transactional behavior
        /// </summary>
        Transactional
    }
}