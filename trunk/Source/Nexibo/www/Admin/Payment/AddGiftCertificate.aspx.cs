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
using CommerceBuilder.Payments;

public partial class Admin_Payment_AddGiftCertificate : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
   
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {			
			int daysToExpire = Token.Instance.Store.Settings.GiftCertificateDaysToExpire;
			if(daysToExpire > 0) {
				ExpireDate.SelectedDate = LocaleHelper.LocalNow.AddDays(daysToExpire);
			}else {
				ExpireDate.SelectedDate = System.DateTime.MinValue;
			}
        }
    }

	protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("GiftCertificates.aspx");
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        int gcId = SaveGiftCertificate();        
		if(gcId > 0)Response.Redirect("GiftCertificates.aspx");
    }

    private int SaveGiftCertificate()
    {
        if (Page.IsValid)
        {
            // VALIDATE IF A PROPER EXPIRE DATE IS SELECTED           
            if (ExpireDate.SelectedEndDate != DateTime.MinValue && DateTime.Compare(ExpireDate.SelectedEndDate, LocaleHelper.LocalNow) < 0)
            {
                CustomValidator dateValidator = new CustomValidator();
                dateValidator.ControlToValidate = "Name"; // THIS SHOULD BE "ExpireDate" CONTROL, BUT THAT CANNOT BE VALIDATED
                dateValidator.Text = "*";
                dateValidator.ErrorMessage = "Expiration date cannot be in the past.";
                dateValidator.IsValid = false;
                phExpirationValidator.Controls.Add(dateValidator);
                return 0;
            }
            GiftCertificate _GiftCertificate = new GiftCertificate();
            _GiftCertificate.Name = Name.Text;
            _GiftCertificate.Balance = AlwaysConvert.ToDecimal(Balance.Text);
            _GiftCertificate.ExpirationDate = ExpireDate.SelectedEndDate;
            _GiftCertificate.CreateDate = LocaleHelper.LocalNow;
            _GiftCertificate.AddCreatedManuallyTransaction();

            if (Activated.Checked)
            {
                IGiftCertKeyProvider gckProvider = new DefaultGiftCertKeyProvider();
                _GiftCertificate.SerialNumber = gckProvider.GenerateGiftCertificateKey();
                _GiftCertificate.AddActivatedTransaction();
            }
            else
            {
                _GiftCertificate.SerialNumber = "";
            }

            _GiftCertificate.Save();
            return _GiftCertificate.GiftCertificateId;
        }
        return 0;
    }


}
