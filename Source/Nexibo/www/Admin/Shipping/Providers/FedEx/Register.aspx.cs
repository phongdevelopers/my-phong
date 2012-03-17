using CommerceBuilder.Shipping.Providers.FedEx;
using System;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;
using System.Web;

partial class Admin_Shipping_Providers_FedEx_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage {

	protected void RedirectToMain()
	{
     Response.Redirect("../Default.aspx");
	}

	protected void CancelButton_Click(object sender, System.EventArgs e)
	{
        RedirectToMain();
	}

	protected void NextButton_Click(object sender, System.EventArgs e)
	{        
        Response.Redirect("Register1.aspx?Accept=true");
	}

}
