using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// This Enumeration is used to indicate the result when testing a new user password against the password policy.
    /// This enumeration can be used in bitwise comparisons
    /// </summary>
    [Flags]
    public enum PasswordTestResult: int
    {        
        /// <summary>
        /// New password meets the criteria against the current password policy
        /// </summary>
        Success = 1,
        /// <summary>
        /// New password length is not according as specified in the current password policy
        /// </summary>
        PasswordTooShort = 2,
        /// <summary>
        /// New password requires at least one uppercase character
        /// </summary>
        RequireUpper = 4,
        /// <summary>
        /// New password requires at least one lowercase character
        /// </summary>
        RequireLower = 8,
        /// <summary>
        /// New password requires at least one number
        /// </summary>
        RequireNumber = 16,
        /// <summary>
        /// New password requires at least one symbol (punctuation, underscore)
        /// </summary>
        RequireSymbol = 32,
        /// <summary>
        /// New password requires at least one non-letter (number or symbol)
        /// </summary>
        RequireNonAlpha = 64,
        /// <summary>
        /// Password is not new and already present in user recent password history
        /// </summary>
        PasswordHistoryLimitation = 128   
    }
}
