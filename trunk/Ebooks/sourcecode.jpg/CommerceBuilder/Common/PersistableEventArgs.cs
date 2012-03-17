namespace CommerceBuilder.Common
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using CommerceBuilder.Data;
    using CommerceBuilder.Common;

    /// <summary>
    /// Event arguments data for CommerceBuilder.IPersistable.Saving 
    /// </summary>
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class PersistableEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item">Persistable Item</param>
        public PersistableEventArgs(IPersistable item)
        {
            this.item = item;
        }

        /// <summary>
        /// Persistable Item
        /// </summary>
        public IPersistable Item
        {
            get
            {
                return this.item;
            }
        }

        private IPersistable item;
    }
}
