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

public partial class Admin_Products_DigitalGoods_SerialKeyProviders_DefaultProvider_AddKeys : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    
    private int _CategoryId;
    private int _ProductId;
    private DigitalGood _DigitalGood;
    private int _DigitalGoodId;
    private string _SerialKeyProviderId;

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
        InitializeForm();
    }

    private void InitializeForm()
    {
        Caption.Text = string.Format(Caption.Text, _DigitalGood.Name);
        InstructionText.Text = string.Format(InstructionText.Text, _DigitalGood.SerialKeys.Count, GetConfigUrl());        
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(GetConfigUrl());
    }

    private string GetConfigUrl()
    {
        return "Configure.aspx?ProductId=" + _ProductId.ToString() + "&CategoryId=" + _CategoryId.ToString() + "&DigitalGoodId=" + _DigitalGoodId.ToString();
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
		ErrorMessagePanel.Visible=false;
        if (Page.IsValid)
        {            
            string inputData = SerialKeyData.Text;
            string delimiter;
            switch (KeyDelimiter.SelectedIndex)
            {
                case 2: delimiter = ","; break;
                case 1: delimiter = "\r\n\r\n"; break;
                default: delimiter = "\r\n"; break;
            }
            string[] delimiterArray = {delimiter};
            string[] keys = inputData.Split(delimiterArray, StringSplitOptions.RemoveEmptyEntries);
            
			List<string> duplicateKeys = new List<string>();
			string key;
			bool added = false;
            foreach (string tkey in keys)
            {
				key = tkey.Trim();
				if(_DigitalGood.HasSerialKey(key)) {
					duplicateKeys.Add(key);
				}else{
					SerialKey skey = new SerialKey();
	                skey.SerialKeyData = key;
		            skey.DigitalGoodId = _DigitalGood.DigitalGoodId;
			        _DigitalGood.SerialKeys.Add(skey);
					added = true;
				}
            }
			if(added) 
			{
				_DigitalGood.SerialKeys.Save();
			}
			if(duplicateKeys.Count == 0) 
			{
				Response.Redirect("Configure.aspx?ProductId=" + _ProductId.ToString()
					+ "&CategoryId=" + _CategoryId.ToString()
					+ "&DigitalGoodId=" + _DigitalGoodId.ToString());
			}else {
				//show error messages regarding duplicate keys
                ErrorMessageList.Items.Clear();
				ErrorMessagePanel.Visible=true;
                foreach (string skey in duplicateKeys)
                {
                    ListItem item = new ListItem(skey);
                    ErrorMessageList.Items.Add(item);
                }
			}
        }
    }
    

}
