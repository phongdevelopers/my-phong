using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// Enumeration that represents the result of a user creation operation
    /// </summary>
    public enum UserCreateStatus : int
    {
        /// <summary>
        /// User was created successfully
        /// </summary>
        Success,
        /// <summary>
        /// User was not created. Email address is invalid
        /// </summary>
        InvalidEmail,
        /// <summary>
        /// User was not created. Password does not satisfy password policy
        /// </summary>
        InvalidPassword,
        /// <summary>
        /// User was not created. Security question was invalid
        /// </summary>
        InvalidQuestion,
        /// <summary>
        /// User was not created. Answer to security question was invalid
        /// </summary>
        InvalidAnswer,
        /// <summary>
        /// User was not created. Email address already exists
        /// </summary>
        DuplicateEmail,
        /// <summary>
        /// User was not created. The Id used to save the user already exists in database
        /// </summary>
        DuplicateUserId,
        /// <summary>
        /// User was not created. An unknown error occured.
        /// </summary>
        UnknownError
    }
}
