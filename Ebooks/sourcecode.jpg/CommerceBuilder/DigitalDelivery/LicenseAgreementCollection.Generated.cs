namespace CommerceBuilder.DigitalDelivery
{
    using System;
    using CommerceBuilder.Common;
    /// <summary>
    /// This class implements a PersistentCollection of LicenseAgreement objects.
    /// </summary>
    public partial class LicenseAgreementCollection : PersistentCollection<LicenseAgreement>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="licenseAgreementId">Value of LicenseAgreementId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 licenseAgreementId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (licenseAgreementId == this[i].LicenseAgreementId) return i;
            }
            return -1;
        }
    }
}
