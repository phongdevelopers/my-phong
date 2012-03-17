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
using CommerceBuilder.Reporting;
using System.Collections.Generic;
using CommerceBuilder.DigitalDelivery;

public partial class Admin_Products_DigitalGoods_SerialKeyProviders_DefaultProvider_EditSerialKey : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _CategoryId;
    private int _ProductId;
    private DigitalGood _DigitalGood;
    private int _DigitalGoodId;
    private string _SerialKeyProviderId;
	private int _SerialKeyId;
	private SerialKey _SerialKey;

    protected void Page_Load(object sender, EventArgs e)
    {
        _CategoryId = PageHelper.GetCategoryId();
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _DigitalGoodId = AlwaysConvert.ToInt(Request.QueryString["DigitalGoodId"]);
        _DigitalGood = DigitalGoodDataSource.Load(_DigitalGoodId);
        _SerialKeyProviderId = Misc.GetClassId(typeof(DefaultSerialKeyProvider));

        if (_DigitalGood == null )
        {
			if (_ProductId > 0)
			{
				Response.Redirect("~/Admin/Products/EditProduct.aspx?CategoryId=" + _CategoryId.ToString()
					+ "&ProductId=" + _ProductId.ToString() + "&DigitalGoodId=" + _DigitalGoodId.ToString());
			}
			else 
			{
				Response.Redirect("~/Admin/DigitalGoods/EditDigitalGood.aspx?DigitalGoodId=" + _DigitalGoodId.ToString());
			}
        }

		_SerialKeyId = AlwaysConvert.ToInt(Request.QueryString["SerialKeyId"]);
		_SerialKey = SerialKeyDataSource.Load(_SerialKeyId);

        if (_SerialKey == null )
        {
			if (_ProductId > 0)
			{
				Response.Redirect("~/Admin/Products/EditProduct.aspx?CategoryId=" + _CategoryId.ToString()
					+ "&ProductId=" + _ProductId.ToString() + "&DigitalGoodId=" + _DigitalGoodId.ToString());
			}else{
				Response.Redirect("~/Admin/DigitalGoods/EditDigitalGood.aspx?DigitalGoodId=" + _DigitalGoodId.ToString());
			}
        }

        Caption.Text = string.Format(Caption.Text, _DigitalGood.Name);

        if (!Page.IsPostBack) InitializeForm();
    }

    private void InitializeForm()
    {
		SerialKeyData.Text = _SerialKey.SerialKeyData;
    }
        
    protected void CancelButton_Click(object sender, EventArgs e)
    {
		if (_ProductId > 0)
		{
			Response.Redirect("Configure.aspx?ProductId=" + _ProductId.ToString() + "&CategoryId=" + _CategoryId.ToString() + "&DigitalGoodId=" + _DigitalGoodId.ToString());
		}else {
			Response.Redirect("Configure.aspx?DigitalGoodId=" + _DigitalGoodId.ToString());
		}
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {            
            _SerialKey.SerialKeyData = SerialKeyData.Text;
            _SerialKey.Save();
			if (_ProductId > 0)
			{
				Response.Redirect("Configure.aspx?ProductId=" + _ProductId.ToString() + "&CategoryId=" + _CategoryId.ToString() + "&DigitalGoodId=" + _DigitalGoodId.ToString());
			}else {
				Response.Redirect("Configure.aspx?DigitalGoodId=" + _DigitalGoodId.ToString());
			}            
        }
    }

}
