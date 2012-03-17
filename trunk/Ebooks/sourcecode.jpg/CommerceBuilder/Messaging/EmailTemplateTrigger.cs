using CommerceBuilder.Stores;

namespace CommerceBuilder.Messaging
{
    /// <summary>
    /// Class representing a row in EmailTemplateTriggers table database
    /// </summary>
    public partial class EmailTemplateTrigger
    {
        /// <summary>
        /// The store event associated with this EmailTemplateTrigger
        /// </summary>
        public StoreEvent StoreEvent
        {
            get
            {
                return (StoreEvent)this.StoreEventId;
            }
            set
            {
                this.StoreEventId = (int)value;
            }
        }
    }
}
