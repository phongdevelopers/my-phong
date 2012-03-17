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

public partial class Admin_Products_ProductTemplates_AddInputField : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _ProductTemplateId;
    private ProductTemplate _ProductTemplate;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductTemplateId = AlwaysConvert.ToInt(Request.QueryString["ProductTemplateId"]);
        _ProductTemplate = ProductTemplateDataSource.Load(_ProductTemplateId);
        if (_ProductTemplate == null) Response.Redirect("Default.aspx");
        if (!Page.IsPostBack)
        {
            //set caption
            bool isMerchantField = (AlwaysConvert.ToInt(Request.QueryString["m"]) == 1);
            if (isMerchantField)
                Caption.Text = string.Format(Caption.Text, "Merchant", _ProductTemplate.Name);
            else
                Caption.Text = string.Format(Caption.Text, "Customer", _ProductTemplate.Name);
            ViewState["IsMerchantField"] = isMerchantField;
            //load input types
            foreach (InputType inputType in Enum.GetValues(typeof(InputType)))
            {
                if ((inputType == InputType.Label) || (inputType == InputType.Hidden)) continue;
                ListItem item = new ListItem(StringHelper.SpaceName(inputType.ToString()), ((int)inputType).ToString());
                InputTypeId.Items.Add(item);
            }
            //update visible inputs
            UpdateVisibleElements();
        }
    }

    protected void RedirectMe()
    {
        Response.Redirect("EditProductTemplate.aspx?ProductTemplateId=" + _ProductTemplateId.ToString());
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        RedirectMe();
    }

    private int CreateInput()
    {
        if (Page.IsValid)
        {
            InputField input = new InputField();
            input.ProductTemplateId = _ProductTemplateId;
            input.Name = Name.Text;
            input.UserPrompt = UserPrompt.Text;
            input.InputTypeId = AlwaysConvert.ToInt16(InputTypeId.SelectedValue);
            input.Columns = AlwaysConvert.ToByte(Columns.Text);
            input.Rows = AlwaysConvert.ToByte(Rows.Text);
            input.MaxLength = AlwaysConvert.ToInt16(MaxLength.Text);
            input.IsMerchantField = (bool)ViewState["IsMerchantField"];
            input.Save();
            return input.InputFieldId;
        }
        return 0;
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (!CreateInput().Equals(0)) RedirectMe();
    }

    private void UpdateVisibleElements()
    {
        InputType inputType = (InputType)AlwaysConvert.ToInt(InputTypeId.SelectedValue);
        switch (inputType)
        {
            case InputType.TextBox:
                trRows.Visible = false;
                trColumns.Visible = true;
                trMaxLength.Visible = true;
                trChoices.Visible = false;
                break;
            case InputType.TextArea:
                trRows.Visible = true;
                trColumns.Visible = true;
                trMaxLength.Visible = false;
                trChoices.Visible = false;
                break;
            case InputType.RadioButtonList:
            case InputType.CheckBoxList:
                trRows.Visible = false;
                trColumns.Visible = true;
                trMaxLength.Visible = false;
                trChoices.Visible = true;
                break;
            case InputType.DropDownListBox:
                trRows.Visible = false;
                trColumns.Visible = false;
                trMaxLength.Visible = false;
                trChoices.Visible = true;
                break;
            case InputType.MultipleListBox:
            case InputType.ListBox:
                trRows.Visible = true;
                trColumns.Visible = false;
                trMaxLength.Visible = false;
                trChoices.Visible = true;
                break;
        }
        NextButton.Visible = trChoices.Visible;
        SaveButton.Visible = !trChoices.Visible;
    }
    
    protected void InputTypeId_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateVisibleElements();
    }

    protected void NextButton_Click(object sender, EventArgs e)
    {
        int id = CreateInput();
        if (!id.Equals(0))
            Response.Redirect("EditInputField.aspx?InputFieldId=" + id.ToString());
    }

}
