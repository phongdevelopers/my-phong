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

public partial class Admin_Products_DigitalGoods_SerialKeyProviders_DefaultProvider_Configure : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{


    protected int _CategoryId = 0;
    protected int _ProductId = 0;
    protected DigitalGood _DigitalGood;
    protected int _DigitalGoodId;
    protected string _SerialKeyProviderId;

    protected void Page_Load(object sender, EventArgs e)
    {
        _CategoryId = PageHelper.GetCategoryId();
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _DigitalGoodId = AlwaysConvert.ToInt(Request.QueryString["DigitalGoodId"]);
        _DigitalGood = DigitalGoodDataSource.Load(_DigitalGoodId);
        _SerialKeyProviderId = Misc.GetClassId(typeof(DefaultSerialKeyProvider));
        

		if (_DigitalGood == null) 
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
        
        Caption.Text = string.Format(Caption.Text, _DigitalGood.Name);

        if (!Page.IsPostBack) InitializeForm();
    }

    protected void InitializeForm()
    {
		BindSerialKeysGrid();
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
		if (_ProductId > 0)
		{
			Response.Redirect("~/Admin/DigitalGoods/EditDigitalGood.aspx?CategoryId=" + _CategoryId.ToString()
				+ "&ProductId=" + _ProductId.ToString() + "&DigitalGoodId=" + _DigitalGoodId.ToString());
		}
		else 
		{
			Response.Redirect("~/Admin/DigitalGoods/EditDigitalGood.aspx?DigitalGoodId=" + _DigitalGoodId.ToString());
		}
    }

    protected void BindSerialKeysGrid()
    {
        SerialKeysGrid.Columns[0].Visible = true;
        SerialKeysGrid.DataSource = _DigitalGood.SerialKeys;
        SerialKeysGrid.DataBind();
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AddKeys.aspx?ProductId=" + _ProductId.ToString() + "&CategoryId=" + _CategoryId.ToString() + "&DigitalGoodId=" + _DigitalGoodId.ToString());
    }

    protected void BackButton_Click(object sender, EventArgs e)
    {
		if (_ProductId > 0)
		{
			Response.Redirect("~/Admin/DigitalGoods/EditDigitalGood.aspx?CategoryId=" + _CategoryId.ToString()
				+ "&ProductId=" + _ProductId.ToString() + "&DigitalGoodId=" + _DigitalGoodId.ToString());
		}
		else 
		{
			Response.Redirect("~/Admin/DigitalGoods/EditDigitalGood.aspx?DigitalGoodId=" + _DigitalGoodId.ToString());
		}
    }
    
    protected void SerialKeysGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        _DigitalGood.SerialKeys.DeleteAt(e.RowIndex);
        BindSerialKeysGrid();
        e.Cancel = true;        
    }

}
