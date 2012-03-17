namespace CommerceBuilder.Personalization
{
    using System.Collections.Generic;
    using CommerceBuilder.Users;

    /// <summary>
    /// Personalization record associated with a script path and a specific user
    /// </summary>
    public partial class UserPersonalization
    {
        /// <summary>
        /// Initializes the personalization object
        /// </summary>
        /// <param name="path">The path that the personalization applies to</param>
        /// <param name="user">The user that the personalization applies to</param>
        public UserPersonalization(PersonalizationPath path, User user)
            : this(path.PersonalizationPathId, user.UserId)
        {
            this._PersonalizationPath = path;
            this._User = user;
        }

        /// <summary>
        /// Decodes the byte array personalization blob
        /// </summary>
        /// <returns>A dictionary of PersonalizationInfo objects</returns>
        public Dictionary<string, PersonalizationInfo> DecodePageSettings()
        {
            return PersonalizationBlobDecoder.Decode(this.PageSettings);
        }
    }
}