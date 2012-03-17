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

public partial class Admin_Products_ProductTemplates_EditInputField : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _InputFieldId;
    private InputField _InputField;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        _InputFieldId = AlwaysConvert.ToInt(Request.QueryString["InputFieldId"]);
        _InputField = InputFieldDataSource.Load(_InputFieldId);
        if (_InputField == null) Response.Redirect("Default.aspx");
        if (!Page.IsPostBack)
        {
            //set caption
            if (_InputField.IsMerchantField)
                Caption.Text = string.Format(Caption.Text, _InputField.ProductTemplate.Name, "Merchant", _InputField.Name);
            else
                Caption.Text = string.Format(Caption.Text, _InputField.ProductTemplate.Name, "Customer", _InputField.Name);
            //load input types
            foreach (InputType inputType in Enum.GetValues(typeof(InputType)))
            {
                if ((inputType == InputType.Label) || (inputType == InputType.Hidden)) continue;
                ListItem item = new ListItem(StringHelper.SpaceName(inputType.ToString()), ((int)inputType).ToString());
                if (inputType == _InputField.InputType) item.Selected = true;
                InputTypeId.Items.Add(item);
            }
            //set initial values
            Name.Text = _InputField.Name;
            UserPrompt.Text = _InputField.UserPrompt;
            if (_InputField.Columns > 0) Columns.Text = _InputField.Columns.ToString();
            if (_InputField.Rows > 0) Rows.Text = _InputField.Rows.ToString();
            if (_InputField.MaxLength > 0) MaxLength.Text = _InputField.MaxLength.ToString();
            //BIND CHOICE GRID
            BindChoicesGrid();
            //update visible _InputFields
            UpdateVisibleElements();
        }
    }

    protected void BindChoicesGrid()
    {
        ChoicesGrid.DataSource = _InputField.InputChoices;
        ChoicesGrid.DataBind();
    }
    
    protected void RedirectMe()
    {
        Response.Redirect("EditProductTemplate.aspx?ProductTemplateId=" + _InputField.ProductTemplateId.ToString());
    }

    protected void BackButton_Click(object sender, System.EventArgs e)
    {
        RedirectMe();
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            SaveInputField();
            RedirectMe();
        }
    }

    private void SaveInputField()
    {
        _InputField.Name = Name.Text;
        _InputField.UserPrompt = UserPrompt.Text;
        _InputField.InputTypeId = AlwaysConvert.ToInt16(InputTypeId.SelectedValue);
        _InputField.Columns = AlwaysConvert.ToByte(Columns.Text);
        _InputField.Rows = AlwaysConvert.ToByte(Rows.Text);
        _InputField.MaxLength = AlwaysConvert.ToInt16(MaxLength.Text);
        //_InputField.IsMerchantField = (bool)ViewState["IsMerchantField"];
        //LOOP THROUGH GRID ROWS AND SET MATRIX
        int rowIndex = 0;
        int selectedChoice = AlwaysConvert.ToInt(Request.Form["SelectedChoice"]);
        foreach (GridViewRow row in ChoicesGrid.Rows)
        {
            string choiceText = ((TextBox)row.FindControl("ChoiceText")).Text;
            string choiceValue = ((TextBox)row.FindControl("ChoiceValue")).Text;
            bool isSelected = ((CheckBox)row.FindControl("IsSelected")).Checked;
            InputChoice thisChoice = _InputField.InputChoices[rowIndex];
            thisChoice.ChoiceText = choiceText;
            thisChoice.ChoiceValue = choiceValue;
            thisChoice.IsSelected = isSelected;
            rowIndex++;
        }
        _InputField.Save();
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
    }
    
    protected void InputTypeId_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateVisibleElements();
    }

    protected void AddChoiceButton_Click(object sender, EventArgs e)
    {
        // Save the existing dat
        if (Page.IsValid)
        {
            SaveInputField();
        }
        InputChoice newChoice = new InputChoice();
        newChoice.InputFieldId = _InputFieldId;
        newChoice.ChoiceText = "Choice " + ((int)(_InputField.InputChoices.Count + 1)).ToString();
        newChoice.Save();
        _InputField.InputChoices.Add(newChoice);
        BindChoicesGrid();
    }

    int _SelectedIndex = -1;
    private int GetSelectedIndex()
    {
        if (_SelectedIndex < 0)
        {
            for (int i = 0; ((i < _InputField.InputChoices.Count) && (_SelectedIndex < 0)); i++)
            {
                if (_InputField.InputChoices[i].IsSelected) _SelectedIndex = i;
            }
            _SelectedIndex = 0;
        }
        return _SelectedIndex;
    }

    protected string GetRadioChecked(int thisIndex)
    {
        if (thisIndex == GetSelectedIndex()) return "checked ";
        return string.Empty;
    }

    protected void ChoicesGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        _InputField.InputChoices.DeleteAt(e.RowIndex);
        BindChoicesGrid();
    }

}
