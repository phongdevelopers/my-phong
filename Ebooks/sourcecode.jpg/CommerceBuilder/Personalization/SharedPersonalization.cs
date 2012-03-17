namespace CommerceBuilder.Personalization
{
    using System.Collections.Generic;

    /// <summary>
    /// Personalization record associated with a script path shared for all users
    /// </summary>
    public partial class SharedPersonalization
    {
        /// <summary>
        /// Initializes the personalization object
        /// </summary>
        /// <param name="path">The path that the personalization applies to</param>
        public SharedPersonalization(PersonalizationPath path)
            : this(path.PersonalizationPathId)
        {
            this._PersonalizationPath = path;
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