using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Marketing
{
    /// <summary>
    /// Enumeration that represents the email list sign up rules.
    /// </summary>
    public enum EmailListSignupRule
    {
        /// <summary>
        /// The verification is done before the signup
        /// </summary>
        VerifiedOptIn, 
        
        /// <summary>
        /// Signup is done but a confirmation message is sent
        /// </summary>
        ConfirmedOptIn, 
        
        /// <summary>
        /// Signup is done without verification of confirmation
        /// </summary>
        OptIn
    }
}
