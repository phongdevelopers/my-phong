namespace CommerceBuilder.Users
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of UserPassword objects.
    /// </summary>
    public partial class UserPasswordCollection : PersistentCollection<UserPassword>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="userId">Value of UserId of the required object.</param>
        /// <param name="passwordNumber">Value of PasswordNumber of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 userId, Byte passwordNumber)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((userId == this[i].UserId) && (passwordNumber == this[i].PasswordNumber)) return i;
            }
            return -1;
        }
    }
}
