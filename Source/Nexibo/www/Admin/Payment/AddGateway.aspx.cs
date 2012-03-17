using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Catalog;
using CommerceBuilder.Orders;
using CommerceBuilder.Products;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using CommerceBuilder.Payments.Providers;
using System.Collections.Generic;

public partial class Admin_Payment_AddGateway : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ProviderList.DataSource = PaymentProviderDataSource.GetProviders();
            ProviderList.DataBind();
            if (ProviderList.Items.Count == 0)
            {
                EmptyDataSection.Visible = true;
            }
            else
            {
                EmptyDataSection.Visible = false;
            }
        }
    }

    protected string GetClassId(object dataItem)
    {
        return Misc.GetClassId(dataItem.GetType());
    }
    
    protected string GetLogoUrl(object dataItem)
    {
        IPaymentProvider provider = (IPaymentProvider)dataItem;
        return provider.GetLogoUrl(Page.ClientScript);
    }

    protected bool HasUnsupportedFeatures(object dataItem)
    {
        IPaymentProvider provider = (IPaymentProvider)dataItem;
        return ((provider.SupportedTransactions & SupportedTransactions.All) != SupportedTransactions.All);
    }

    protected string GetUnsupportedFeatures(object dataItem)
    {
        IPaymentProvider provider = (IPaymentProvider)dataItem;
        List<string> unsupportedFeatures = new List<string>();
        if ((provider.SupportedTransactions & SupportedTransactions.Authorize) == SupportedTransactions.None) unsupportedFeatures.Add(SupportedTransactions.Authorize.ToString());
        if ((provider.SupportedTransactions & SupportedTransactions.AuthorizeCapture) == SupportedTransactions.None) unsupportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.AuthorizeCapture.ToString()));
        if ((provider.SupportedTransactions & SupportedTransactions.PartialCapture) == SupportedTransactions.None) unsupportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.PartialCapture.ToString()));
        if ((provider.SupportedTransactions & SupportedTransactions.Capture) == SupportedTransactions.None) unsupportedFeatures.Add(SupportedTransactions.Capture.ToString());
        if ((provider.SupportedTransactions & SupportedTransactions.PartialRefund) == SupportedTransactions.None) unsupportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.PartialRefund.ToString()));
        if ((provider.SupportedTransactions & SupportedTransactions.Refund) == SupportedTransactions.None) unsupportedFeatures.Add(SupportedTransactions.Refund.ToString());
        if ((provider.SupportedTransactions & SupportedTransactions.Void) == SupportedTransactions.None) unsupportedFeatures.Add(SupportedTransactions.Void.ToString());
        if ((provider.SupportedTransactions & SupportedTransactions.RecurringBilling) == SupportedTransactions.None) unsupportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.RecurringBilling.ToString()));
        if (unsupportedFeatures.Count == 0) return string.Empty;
        return string.Join(", ", unsupportedFeatures.ToArray());
    }
    
    protected string GetSupportedTransactions(object dataItem)
    {
        IPaymentProvider provider = (IPaymentProvider)dataItem;
        List<string> supportedFeatures = new List<string>();
        if ((provider.SupportedTransactions & SupportedTransactions.Authorize) != SupportedTransactions.None) supportedFeatures.Add(SupportedTransactions.Authorize.ToString());
        if ((provider.SupportedTransactions & SupportedTransactions.AuthorizeCapture) != SupportedTransactions.None) supportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.AuthorizeCapture.ToString()));
        if ((provider.SupportedTransactions & SupportedTransactions.Capture) != SupportedTransactions.None) supportedFeatures.Add(SupportedTransactions.Capture.ToString());
        if ((provider.SupportedTransactions & SupportedTransactions.PartialCapture) != SupportedTransactions.None) supportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.PartialCapture.ToString()));
        if ((provider.SupportedTransactions & SupportedTransactions.Void) != SupportedTransactions.None) supportedFeatures.Add(SupportedTransactions.Void.ToString());
        if ((provider.SupportedTransactions & SupportedTransactions.Refund) != SupportedTransactions.None) supportedFeatures.Add(SupportedTransactions.Refund.ToString());
        if ((provider.SupportedTransactions & SupportedTransactions.PartialRefund) != SupportedTransactions.None) supportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.PartialRefund.ToString()));
        if ((provider.SupportedTransactions & SupportedTransactions.RecurringBilling) != SupportedTransactions.None) supportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.RecurringBilling.ToString()));
        if (supportedFeatures.Count == 0) return string.Empty;
        return string.Join(", ", supportedFeatures.ToArray());
    }  

    protected bool IsSupported(object dataItem, SupportedTransactions supported)
    {
        IPaymentProvider provider = (IPaymentProvider)dataItem;
        return ((provider.SupportedTransactions & supported) == supported);
    }

    protected void ProviderGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Add"))
        {
            Guid sessionKey = Guid.NewGuid();
            Session[sessionKey.ToString()] = (string)e.CommandArgument;
            Response.Redirect("AddGateway2.aspx?ProviderClassId=" + sessionKey.ToString());
        }
    }

    protected void ProviderList_ItemCommand(object source, DataListCommandEventArgs e)
    {
        if (e.CommandName.Equals("Add"))
        {
            Guid sessionKey = Guid.NewGuid();
            Session[sessionKey.ToString()] = (string)e.CommandArgument;
            Response.Redirect("AddGateway2.aspx?ProviderClassId=" + sessionKey.ToString());
        }
    }

}
