namespace CommerceBuilder.Stores
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of AuditEvent objects.
    /// </summary>
    public partial class AuditEventCollection : PersistentCollection<AuditEvent>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="auditEventId">Value of AuditEventId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 auditEventId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (auditEventId == this[i].AuditEventId) return i;
            }
            return -1;
        }
    }
}
