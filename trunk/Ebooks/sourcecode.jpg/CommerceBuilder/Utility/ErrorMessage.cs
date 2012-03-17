namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Class that represents an error message
    /// </summary>
    public partial class ErrorMessage
    {
        /// <summary>
        /// Severity of the error message
        /// </summary>
        public MessageSeverity MessageSeverity
        {
            get { return (MessageSeverity)this.MessageSeverityId; }
            set { this.MessageSeverityId = (byte)value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="severity">Severity of the error message</param>
        /// <param name="text">The error message text</param>
        /// <param name="debugData">Debugging data for the error</param>
        public ErrorMessage(MessageSeverity severity, string text, string debugData)
        {
            this.MessageSeverity = severity;
            this.Text = text;
            this.DebugData = debugData;
        }

    }
}
