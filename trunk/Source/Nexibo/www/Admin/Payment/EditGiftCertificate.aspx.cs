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

public partial class Admin_Payment_EditGiftCertificate : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
   
    
    private int _GiftCertificateId;
    private GiftCertificate _GiftCertificate;

	private int _OrderId = 0;
    
    protected void Page_Load(object sender, EventArgs e)
    {
		_OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _GiftCertificateId = AlwaysConvert.ToInt(Request.QueryString["GiftCertificateId"]);
        _GiftCertificate = GiftCertificateDataSource.Load(_GiftCertificateId);

        if (_GiftCertificate == null)
        {
            RedirectBack();
        }
        
        if (!Page.IsPostBack)
        {
            SerialNumber.Text = _GiftCertificate.SerialNumber;
			Name.Text = _GiftCertificate.Name;
            Balance.Text = string.Format("{0:F2}", _GiftCertificate.Balance);
            ExpireDate.SelectedDate = _GiftCertificate.ExpirationDate;
            if (!string.IsNullOrEmpty(_GiftCertificate.SerialNumber))
            {
                ReGenerateSerialNumberLbl.Visible = true;
                GenerateSerialNumberLbl.Visible = false;
            }
            else
            {
                ReGenerateSerialNumberLbl.Visible = false;
                GenerateSerialNumberLbl.Visible = true;
            }
        }
    }	

	protected void CancelButton_Click(object sender, EventArgs e)
    {
        RedirectBack();
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        int gcId = SaveGiftCertificate();
        //if (gcId != 0) Response.Redirect("EditGiftCertificate.aspx?GiftCertificateId=" + gcId.ToString());
        if (gcId != 0) RedirectBack();
    }

	private int SaveGiftCertificate() {
		if (Page.IsValid)
        {   
			IGiftCertKeyProvider gckProvider = new DefaultGiftCertKeyProvider();            
            _GiftCertificate.Name = Name.Text;
			LSDecimal newBalance = AlwaysConvert.ToDecimal(Balance.Text);
			
			if(newBalance != _GiftCertificate.Balance) {
				_GiftCertificate.AddBalanceUpdatedTransaction(_GiftCertificate.Balance,newBalance);
				_GiftCertificate.Balance = newBalance;
			}
            
			DateTime newDate = ExpireDate.SelectedEndDate;
			if(!_GiftCertificate.ExpirationDate.Equals(newDate)) {
				_GiftCertificate.AddExpiryUpdatedTransaction(_GiftCertificate.ExpirationDate,newDate);
				_GiftCertificate.ExpirationDate = ExpireDate.SelectedEndDate;
			}
			
			if(GenerateSerialNumber.Checked) {
				if(string.IsNullOrEmpty(_GiftCertificate.SerialNumber)) {
					_GiftCertificate.AddActivatedTransaction();
				}
				_GiftCertificate.SerialNumber = gckProvider.GenerateGiftCertificateKey();
			}
            _GiftCertificate.Save();
            return _GiftCertificate.GiftCertificateId;
        }
        return 0;
	}

	private bool HasSerialNumber() {
        return _GiftCertificate.SerialNumber != null && _GiftCertificate.SerialNumber.Length > 0;        
	}

    private void RedirectBack()
    {
        if (_OrderId == 0)
        {
			Response.Redirect("GiftCertificates.aspx");
		}
        else
        {
            int orderNumber = OrderDataSource.LookupOrderNumber(_OrderId);
            Response.Redirect( "~/Admin/Orders/ViewGiftCertificates.aspx?OrderNumber=" + orderNumber.ToString() + "&OrderId=" + _OrderId.ToString());
	}
    }


}
