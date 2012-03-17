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

public partial class Admin_Products_ProductTemplates_EditProductTemplate : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _ProductTemplateId;
    private ProductTemplate _ProductTemplate;

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            _ProductTemplate.Name = Name.Text;
            _ProductTemplate.Save();
            Caption.Text = string.Format((string)ViewState["Caption"], _ProductTemplate.Name);
            Response.Redirect("Default.aspx");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductTemplateId = AlwaysConvert.ToInt(Request.QueryString["ProductTemplateId"]);
        _ProductTemplate = ProductTemplateDataSource.Load(_ProductTemplateId);
        if (_ProductTemplate == null) Response.Redirect("Default.aspx");
        if (!Page.IsPostBack)
        {
            ViewState["Caption"] = Caption.Text;
            Caption.Text = string.Format(Caption.Text, _ProductTemplate.Name);
            Name.Text = _ProductTemplate.Name;
            AssignedProducts.Text = ProductDataSource.CountForProductTemplate(_ProductTemplateId).ToString();
            AssignedProducts.NavigateUrl += "?ProductTemplateId=" + _ProductTemplateId.ToString();
        }
    }

    protected void FieldGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "MoveUp")
        {
            InputFieldCollection inputFields;
            if (((GridView)sender).ID == "MerchantFieldGrid")
                inputFields = InputFieldDataSource.LoadForProductTemplate(_ProductTemplateId, InputFieldDataSource.InputFieldScope.Merchant);
            else
                inputFields = InputFieldDataSource.LoadForProductTemplate(_ProductTemplateId, InputFieldDataSource.InputFieldScope.Customer);
            int itemIndex = AlwaysConvert.ToInt(e.CommandArgument);
            if ((itemIndex < 1) || (itemIndex > inputFields.Count - 1)) return;
            InputField selectedItem = inputFields[itemIndex];
            InputField swapItem = inputFields[itemIndex - 1];
            inputFields.RemoveAt(itemIndex - 1);
            inputFields.Insert(itemIndex, swapItem);
            for (int i = 0; i < inputFields.Count; i++)
            {
                inputFields[i].OrderBy = (short)i;
            }
            inputFields.Save();            
        }
        else if (e.CommandName == "MoveDown")
        {
            InputFieldCollection inputFields;
            if (((GridView)sender).ID == "MerchantFieldGrid")
                inputFields = InputFieldDataSource.LoadForProductTemplate(_ProductTemplateId, InputFieldDataSource.InputFieldScope.Merchant);
            else
                inputFields = InputFieldDataSource.LoadForProductTemplate(_ProductTemplateId, InputFieldDataSource.InputFieldScope.Customer);
            int itemIndex = AlwaysConvert.ToInt(e.CommandArgument);
            if ((itemIndex > inputFields.Count - 2) || (itemIndex < 0)) return;
            InputField selectedItem = inputFields[itemIndex];
            InputField swapItem = inputFields[itemIndex + 1];
            inputFields.RemoveAt(itemIndex + 1);
            inputFields.Insert(itemIndex, swapItem);
            for (int i = 0; i < inputFields.Count; i++)
            {
                inputFields[i].OrderBy = (short)i;
            }
            inputFields.Save();
        }
        else if (e.CommandName == "Copy")
        {
            int inputFieldId = AlwaysConvert.ToInt(e.CommandArgument);
            InputField copy = InputField.Copy(inputFieldId, true);
            if (copy != null)
            {
                // THE NAME SHOULD NOT EXCEED THE MAX 100 CHARS
                String newName = "Copy of " + copy.Name;
                if (newName.Length > 100)
                {
                    newName = newName.Substring(0, 97) + "...";
                }
                copy.Name = newName;
                copy.ProductTemplateId = _ProductTemplateId;
                _ProductTemplate.InputFields.Add(copy);
                _ProductTemplate.InputFields.Save();
            }
        }
        ((GridView)sender).DataBind();
    }

    protected void AddMerchantFieldButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AddInputField.aspx?ProductTemplateId=" + _ProductTemplateId.ToString() + "&m=1");
    }

    protected void AddCustomerFieldButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AddInputField.aspx?ProductTemplateId=" + _ProductTemplateId.ToString());
    }

    protected string GetInputType(object dataItem)
    {
        InputField field = (InputField)dataItem;
        return StringHelper.SpaceName(field.InputType.ToString());
    }

    protected string GetChoices(object dataItem)
    {
        InputField field = (InputField)dataItem;
        if ((field.InputType == InputType.CheckBoxList) || (field.InputType == InputType.DropDownListBox)
            || (field.InputType == InputType.ListBox) || (field.InputType == InputType.MultipleListBox) || (field.InputType == InputType.RadioButtonList))
        {
            return field.InputChoices.Count.ToString();
        }
        return "n/a";
    }


    protected void BackButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
    

}
