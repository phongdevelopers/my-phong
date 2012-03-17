namespace CommerceBuilder.Users
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of UserSetting objects.
    /// </summary>
    public partial class UserSettingCollection : PersistentCollection<UserSetting>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="userSettingId">Value of UserSettingId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 userSettingId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (userSettingId == this[i].UserSettingId) return i;
            }
            return -1;
        }
    }
}
