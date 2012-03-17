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
using CommerceBuilder.Payments;
using System.Collections.Generic;

public partial class Admin_Payment_Gateways : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    
    protected void Page_Init(object sender, EventArgs e)
    {
        string sqlCriteria = " Name<>'Gift Certificate Payment Provider' AND ClassId<>'" + StringHelper.SafeSqlString(GetGiftCertPayGatewayId()) + "'";
        PaymentGatewayDs.SelectParameters[0].DefaultValue = sqlCriteria;
    }

    private string GetGiftCertPayGatewayId()
    {
        return Misc.GetClassId(typeof(CommerceBuilder.Payments.Providers.GiftCertificatePaymentProvider));
    }
    
    protected string GetSupportedTransactions(object dataItem)
    {
        IPaymentProvider provider = ((PaymentGateway)dataItem).GetInstance();
        List<string> supportedFeatures = new List<string>();
        if (provider != null)
        {
            if ((provider.SupportedTransactions & SupportedTransactions.Authorize) != SupportedTransactions.None) supportedFeatures.Add(SupportedTransactions.Authorize.ToString());
            if ((provider.SupportedTransactions & SupportedTransactions.AuthorizeCapture) != SupportedTransactions.None) supportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.AuthorizeCapture.ToString()));
            if ((provider.SupportedTransactions & SupportedTransactions.Capture) != SupportedTransactions.None) supportedFeatures.Add(SupportedTransactions.Capture.ToString());
            if ((provider.SupportedTransactions & SupportedTransactions.PartialCapture) != SupportedTransactions.None) supportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.PartialCapture.ToString()));
            if ((provider.SupportedTransactions & SupportedTransactions.Void) != SupportedTransactions.None) supportedFeatures.Add(SupportedTransactions.Void.ToString());
            if ((provider.SupportedTransactions & SupportedTransactions.Refund) != SupportedTransactions.None) supportedFeatures.Add(SupportedTransactions.Refund.ToString());
            if ((provider.SupportedTransactions & SupportedTransactions.PartialRefund) != SupportedTransactions.None) supportedFeatures.Add(StringHelper.SpaceName(SupportedTransactions.PartialRefund.ToString()));
        }
        if (supportedFeatures.Count == 0) return string.Empty;
        return string.Join(", ", supportedFeatures.ToArray());
    }

    protected string GetPaymentMethods(object dataItem)
    {
        PaymentGateway gateway = (PaymentGateway)dataItem;
        List<string> paymentMethods = new List<string>();
        foreach(PaymentMethod method in gateway.PaymentMethods)
        {
            paymentMethods.Add(method.Name);
        }
        if (paymentMethods.Count == 0) return string.Empty;
        return string.Join(", ", paymentMethods.ToArray());
    }

    protected string GetConfigReference(object dataItem)
    {
        IPaymentProvider provider = ((PaymentGateway)dataItem).GetInstance();
        if (provider != null)
        {
            return provider.ConfigReference;
        }
        return string.Empty;        
    }

}
