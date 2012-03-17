namespace CommerceBuilder.Stores
{
    //TRY TO KEEP AUDITEVENTTYPE IN ALPHABETIC ORDER
    //THIS WILL ALLOW SORTING BY EVENT NAME (TYPE)
    //LEAVE GAPS BETWEEN EVENTS TO ALLOW ADDITIONAL FUTURE ENTRIES
    /// <summary>
    /// Enumeration that represents the type of an audit event
    /// </summary>
    public enum AuditEventType
    {
        /// <summary>
        /// Unknown event
        /// </summary>
        Unknown,

        /// <summary>
        /// Application has started
        /// </summary>
        ApplicationStarted = 1000,

        /// <summary>
        /// Encryption key was backed up
        /// </summary>
        BackupEncryptionKey = 2000,

        /// <summary>
        /// Encryption key was changed
        /// </summary>
        ChangeEncryptionKey = 3000,

        /// <summary>
        /// Audit log has been cleared
        /// </summary>
        ClearAuditLog = 4000,

        /// <summary>
        /// Login event occured
        /// </summary>
        Login = 5000,

        /// <summary>
        /// Logout event occured
        /// </summary>
        Logout = 6000,

        /// <summary>
        /// Password was changed
        /// </summary>
        PasswordChanged = 7000,

        /// <summary>
        /// Password request was initiated
        /// </summary>
        PasswordRequest = 8000,

        /// <summary>
        /// Encryption key was restored
        /// </summary>
        RestoreEncryptionKey = 9000,

        /// <summary>
        /// Audit log was viewed
        /// </summary>
        ViewAuditLog = 10000,

        /// <summary>
        /// Credit card data was viewed
        /// </summary>
        ViewCardData = 11000
    }

    public partial class AuditEvent
    {
        /// <summary>
        /// The type of Audit Event
        /// </summary>
        public AuditEventType EventType
        {
            get { return (AuditEventType)this.EventTypeId; }
            set { this.EventTypeId = (int)value; }
        }
    }
}
