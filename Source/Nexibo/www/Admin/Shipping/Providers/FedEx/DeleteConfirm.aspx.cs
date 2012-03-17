using CommerceBuilder.Shipping.Providers.FedEx;
using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.Web;

partial class Admin_Shipping_Providers_FedEx_DeleteConfirm : CommerceBuilder.Web.UI.AbleCommerceAdminPage {
    
    protected void FinishButton_Click(object sender, System.EventArgs e) {
        RedirectToManageProvider();
    }
    
    protected void RedirectToManageProvider() {
        FedEx provider;
        provider = new FedEx();
        Response.Redirect(("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(Misc.GetClassId(provider.GetType()))));
    }
}
