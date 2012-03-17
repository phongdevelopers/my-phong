using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Web.UI.WebControls.WebParts;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;
using CommerceBuilder.Personalization;
using CommerceBuilder.Users;

namespace CommerceBuilder.Personalization
{
    /// <summary>
    /// Implements AbleCommerce specific functionality for personalization.
    /// </summary>
    public class PersonalizationProvider : System.Web.UI.WebControls.WebParts.PersonalizationProvider
    {
        /// <summary>
        /// Gets or sets the name of the application configured for the provider.
        /// </summary>
        /// <remarks>AbleCommerce only supports a single application configured at "/".</remarks>
        public override string ApplicationName
        {
            get { return "/"; }
            set { }
        }

        /// <summary>
        /// Gets a collection of PersonalizationStateInfo objects for the given PersonalizationStateQuery
        /// </summary>
        /// <param name="scope">A PersonalizationScope with the personalization information to be queried. This value cannot be a null reference (Nothing in Visual Basic).</param>
        /// <param name="query">A PersonalizationStateQuery containing a query. This value can be a null reference (Nothing in Visual Basic).</param>
        /// <param name="pageIndex">The location where the query starts.</param>
        /// <param name="pageSize">The number of records to return.</param>
        /// <param name="totalRecords">The total number of records available.</param>
        /// <returns>A PersonalizationStateInfoCollection containing zero or more PersonalizationStateInfo-derived objects.</returns>
        public override System.Web.UI.WebControls.WebParts.PersonalizationStateInfoCollection FindState(System.Web.UI.WebControls.WebParts.PersonalizationScope scope, System.Web.UI.WebControls.WebParts.PersonalizationStateQuery query, int pageIndex, int pageSize, out int totalRecords)
        {
            if (pageIndex < 0) throw new ArgumentException("pageIndex cannot be less than zero", "pageIndex");
            if (pageSize <= 0) throw new ArgumentException("pageSize must be greater than zero", "pageSize");
            if (scope == System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared)
                return SharedPersonalizationDataSource.FindState(query, pageIndex, pageSize, out totalRecords);
            else return UserPersonalizationDataSource.FindState(query, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Returns the number of rows in the underlying data store that exist within the specified scope.
        /// </summary>
        /// <param name="scope">A PersonalizationScope with the personalization information to be queried. This value cannot be a null reference (Nothing in Visual Basic).</param>
        /// <param name="query">A PersonalizationStateQuery containing a query. This value can be a null reference (Nothing in Visual Basic).</param>
        /// <returns>The number of rows in the underlying data store that exist for the specified scope parameter.</returns>
        public override int GetCountOfState(System.Web.UI.WebControls.WebParts.PersonalizationScope scope, System.Web.UI.WebControls.WebParts.PersonalizationStateQuery query)
        {
            int totalRecords;
            PersonalizationStateInfoCollection col = FindState(scope, query, 0, int.MaxValue, out totalRecords);
            return totalRecords;
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        /// <summary>
        /// Loads raw personalization data from the underlying data store.
        /// </summary>
        /// <param name="webPartManager">The WebPartManager managing the personalization data.</param>
        /// <param name="path">The path for personalization information to be used as the retrieval key.</param>
        /// <param name="userName">The user name for personalization information to be used as the retrieval key.</param>
        /// <param name="sharedDataBlob">The returned data for the Shared scope.</param>
        /// <param name="userDataBlob">The returned data for the User scope.</param>
        protected override void LoadPersonalizationBlobs(System.Web.UI.WebControls.WebParts.WebPartManager webPartManager, string path, string userName, ref byte[] sharedDataBlob, ref byte[] userDataBlob)
        {
            sharedDataBlob = null;
            userDataBlob = null;
            SharedPersonalization sharedPersonalization = SharedPersonalizationDataSource.LoadForPath(path, false);
            if (sharedPersonalization != null)
            {
                sharedDataBlob = sharedPersonalization.PageSettings;
            }
            UserPersonalization UserPersonalization = UserPersonalizationDataSource.LoadForPath(path, userName, false);
            if (UserPersonalization != null)
            {
                userDataBlob = UserPersonalization.PageSettings;
            }
        }

        /// <summary>
        /// Deletes raw personalization data from the underlying data store.
        /// </summary>
        /// <param name="webPartManager">The WebPartManager managing the personalization data.</param>
        /// <param name="path">The path for personalization information to be used as the data store key.</param>
        /// <param name="userName">The user name for personalization information to be used as the data store key.</param>
        protected override void ResetPersonalizationBlob(System.Web.UI.WebControls.WebParts.WebPartManager webPartManager, string path, string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                SharedPersonalization sharedPersonalization = SharedPersonalizationDataSource.LoadForPath(path, false);
                if (sharedPersonalization != null)
                {
                    sharedPersonalization.Delete();
                }
            }
            else
            {
                UserPersonalization userPersonalization = UserPersonalizationDataSource.LoadForPath(path, userName, false);
                if (userPersonalization != null)
                {
                    userPersonalization.Delete();
                }
            }
        }

        /// <summary>
        /// Deletes personalization state from the underlying data store based on the specified parameters.
        /// </summary>
        /// <param name="scope">A PersonalizationScope of the personalization information to be reset. This value cannot be a null reference (Nothing in Visual Basic).</param>
        /// <param name="paths">The paths for personalization information to be deleted.</param>
        /// <param name="usernames">The user names for personalization information to be deleted.</param>
        /// <returns>The number of rows deleted.</returns>
        /// <remarks>This method is not implemented in AbleCommerce 7.</remarks>
        public override int ResetState(System.Web.UI.WebControls.WebParts.PersonalizationScope scope, string[] paths, string[] usernames)
        {
            throw new PersonalizationProviderException("The method or operation ResetState is not implemented.");
        }

        /// <summary>
        /// Deletes Web Parts personalization data from the underlying data store based on the specified parameters.
        /// </summary>
        /// <param name="path">The path of the personalization data to be deleted. This value can be a null reference (Nothing in Visual Basic) but cannot be an empty string ("").</param>
        /// <param name="userInactiveSinceDate">The date indicating the last time a Web site user changed personalization data.</param>
        /// <returns>The number of rows deleted from the underlying data store.</returns>
        /// <remarks>This method is not implemented in AbleCommerce 7.</remarks>
        public override int ResetUserState(string path, DateTime userInactiveSinceDate)
        {
            throw new PersonalizationProviderException("The method or operation ResetUserState is not implemented.");
        }

        /// <summary>
        /// Saves raw personalization data to the underlying data store. 
        /// </summary>
        /// <param name="webPartManager">The WebPartManager managing the personalization data.</param>
        /// <param name="path">The path for personalization information to be used as the data store key.</param>
        /// <param name="userName">The user name for personalization information to be used as the key.</param>
        /// <param name="dataBlob">The byte array of data to be saved.</param>
        protected override void SavePersonalizationBlob(System.Web.UI.WebControls.WebParts.WebPartManager webPartManager, string path, string userName, byte[] dataBlob)
        {
            if (userName == null)
            {
                SharedPersonalization sharedPersonalization = SharedPersonalizationDataSource.LoadForPath(path, true);
                sharedPersonalization.PageSettings = dataBlob;
                sharedPersonalization.Save();
            }
            else
            {
                UserPersonalization userPersonalization = UserPersonalizationDataSource.LoadForPath(path, userName, true);
                userPersonalization.PageSettings = dataBlob;
                userPersonalization.Save();
            }
        }
    }
}
