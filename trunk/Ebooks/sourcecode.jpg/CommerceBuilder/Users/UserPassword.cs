using System;
using System.Text;
using System.Security.Cryptography;
using System.Web.Security;

namespace CommerceBuilder.Users
{
    /// <summary>
    /// This class represents a User-Password record in database
    /// </summary>
    public partial class UserPassword
    {
        /// <summary>
        /// Checks the given password to see if it is equal to the password represented by this instance.
        /// </summary>
        /// <param name="password">The unencrypted password to check</param>
        /// <returns>True if the input is equal to the password stored in this instance; false otherwise.</returns>
        public bool VerifyPassword(string password)
        {
            return UserPasswordHelper.VerifyPassword(password, this.PasswordFormat, this.Password);
        }
    }
}
