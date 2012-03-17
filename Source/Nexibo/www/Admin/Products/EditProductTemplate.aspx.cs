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
using System.Text;
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

public partial class Admin_Products_EditProductTemplate : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _ProductId = 0;
    private Product _Product;

    protected void Page_Init(object sender, EventArgs e)
    {
        _ProductId = PageHelper.GetProductId();
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect(NavigationHelper.GetAdminUrl("Catalog/Default.aspx"));
        Caption.Text = string.Format(Caption.Text, _Product.Name);
        CancelButton.OnClientClick = "window.location='EditProduct.aspx?ProductId=" + _ProductId.ToString() + "';return false";

        // DETERMINE THE TEMPLATE LIST - ON FIRST VISIT THIS IS THE SAVED TEMPLATES ASSOCIATED WITH PRODUCT
        // ON POSTBACK IT COULD BE DIFFERENT IF CHANGED BY THE MERCHANT
        InitializeTemplateList();

        // INITIALIZE THE DIALOG TO UPDATE SELECTED TEMPLATES
        InitializeChangeTemplatesJS();
        InitializeChangeTemplatesDialog();

        // BUILD CHOICES BASED ON CURRENT TEMPLATES
        BuildProductChoices();
    }

    private void InitializeTemplateList()
    {
        if (Page.IsPostBack)
        {
            _Product.ProductProductTemplates.Clear();
            int[] selectedTemplates = AlwaysConvert.ToIntArray(Request.Form[HiddenSelectedTemplates.UniqueID]);
            if (selectedTemplates != null && selectedTemplates.Length > 0)
            {
                foreach (int ptid in selectedTemplates)
                {
                    _Product.ProductProductTemplates.Add(new ProductProductTemplate(_ProductId, ptid));
                }
                trUnsavedChanges.Visible = true;
            }
        }
        else
        {
            HiddenSelectedTemplates.Value = GetTemplateIdList();
        }
        TemplateList.Text = GetTemplateList();
    }

    /// <summary>
    /// Initializes javascript required by the change Templates dialog
    /// </summary>
    private void InitializeChangeTemplatesJS()
    {
        this.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "selectbox", this.ResolveUrl("~/js/selectbox.js"));
        string leftBoxName = AvailableTemplates.ClientID;
        string rightBoxName = SelectedTemplates.ClientID;
        AvailableTemplates.Attributes.Add("onDblClick", "moveSelectedOptions(this.form['" + leftBoxName + "'], this.form['" + rightBoxName + "'], true, '')");
        SelectedTemplates.Attributes.Add("onDblClick", "moveSelectedOptions(this.form['" + rightBoxName + "'], this.form['" + leftBoxName + "'], true, '');");
        SelectAllTemplates.Attributes.Add("onclick", "moveAllOptions(this.form['" + leftBoxName + "'], this.form['" + rightBoxName + "'], true, ''); return false;");
        SelectTemplate.Attributes.Add("onclick", "moveSelectedOptions(this.form['" + leftBoxName + "'], this.form['" + rightBoxName + "'], true, ''); return false;");
        UnselectTemplate.Attributes.Add("onclick", "moveSelectedOptions(this.form['" + rightBoxName + "'], this.form['" + leftBoxName + "'], true, ''); return false;");
        UnselectAllTemplates.Attributes.Add("onclick", "moveAllOptions(this.form['" + rightBoxName + "'], this.form['" + leftBoxName + "'], true, ''); return false;");
        StringBuilder changeTemplateListScript = new StringBuilder();
        changeTemplateListScript.AppendLine("function changeTemplateList(){");
        changeTemplateListScript.AppendLine("\t$get('" + HiddenSelectedTemplates.ClientID + "').value=getOptions($get('" + rightBoxName + "'));");
        changeTemplateListScript.AppendLine("\t$get('" + TemplateList.ClientID + "').innerHTML=getOptionNames($get('" + rightBoxName + "'));");
        changeTemplateListScript.AppendLine("\t$get('" + TemplateListChanged.ClientID + "').value='1';");
        changeTemplateListScript.AppendLine("}");
        this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "changeTemplateList", changeTemplateListScript.ToString(), true);
    }


    /// <summary>
    /// Initializes the change Templates dialog with current user Template settings
    /// </summary>
    private void InitializeChangeTemplatesDialog()
    {
        AvailableTemplates.Items.Clear();
        SelectedTemplates.Items.Clear();
        ProductTemplateCollection allTemplates = ProductTemplateDataSource.LoadForStore("Name");
        foreach (ProductTemplate t in allTemplates)
        {
            ListItem newItem = new ListItem(t.Name, t.ProductTemplateId.ToString());
            bool templateSelected = (_Product.ProductProductTemplates.IndexOf(_ProductId, t.ProductTemplateId) > -1);
            if (templateSelected) SelectedTemplates.Items.Add(newItem);
            else AvailableTemplates.Items.Add(newItem);
        }
    }

    /// <summary>
    /// Gets a comma delimited list of assigned template names for the current product
    /// </summary>
    /// <returns>Comma delimited list of template names, or the empty text if no 
    /// templates are assigned to the product</returns>
    protected string GetTemplateList()
    {
        List<string> templateNames = new List<string>();
        foreach (ProductProductTemplate ppt in _Product.ProductProductTemplates)
        {
            if (ppt.ProductTemplate != null)
            {
                templateNames.Add(ppt.ProductTemplate.Name);
            }
        }
        if (templateNames.Count == 0) return string.Empty;
        return string.Join(", ", templateNames.ToArray());
    }

    protected string GetTemplateIdList()
    {
        List<string> templateIds = new List<string>();
        foreach (ProductProductTemplate ppt in _Product.ProductProductTemplates)
        {
            if (ppt.ProductTemplate != null)
            {
                templateIds.Add(ppt.ProductTemplateId.ToString());
            }
        }
        if (templateIds.Count == 0) return string.Empty;
        return string.Join(",", templateIds.ToArray());
    }

    protected void BuildProductChoices()
    {
        if (_Product.ProductProductTemplates.Count > 0)
        {
            foreach (ProductProductTemplate ppt in _Product.ProductProductTemplates)
            {
                // ADD IN THE PRODUCT TEMPLATE CHOICES
                ProductTemplate template = ppt.ProductTemplate;
                if (template != null)
                {
                    StringBuilder customerFields = new StringBuilder();
                    // SHOW THE SECTION HEADER
                    phCustomFields.Controls.Add(new LiteralControl("<div class=\"sectionHeader\">" + template.Name + "</div>"));
                    phCustomFields.Controls.Add(new LiteralControl("<table class=\"inputForm\" cellpadding=\"4\" cellspacing=\"0\">"));
                    foreach (InputField input in template.InputFields)
                    {
                        if (input.IsMerchantField)
                        {
                            // OPEN THE ROW
                            phCustomFields.Controls.Add(new LiteralControl("<tr>"));
                            // CREATE A LABEL FOR THE CHOICE
                            phCustomFields.Controls.Add(new LiteralControl("<th class=\"rowHeader\" valign=\"top\">" + input.UserPrompt + "</th>"));
                            // ADD THE CONTROL TO THE PLACEHOLDER
                            phCustomFields.Controls.Add(new LiteralControl("<td valign=\"top\">"));
                            string tempValue = string.Empty;
                            ProductTemplateField cf = FindTemplateField(input.InputFieldId);
                            if (cf != null) tempValue = cf.InputValue;
                            WebControl o = input.GetControl(tempValue);
                            if (o != null) phCustomFields.Controls.Add(o);
                            phCustomFields.Controls.Add(new LiteralControl("</td></tr>"));
                        }
                        else
                        {
                            // DISPLAY INFO ABOUT THE CUSTOMER FIELD
                            customerFields.Append("<tr>");
                            customerFields.Append("<th class=\"rowHeader\">" + input.UserPrompt + "</th>");
                            customerFields.Append("<td>");
                            customerFields.Append("<i>Customer Field, " + StringHelper.SpaceName(input.InputType.ToString()) + "</i>");
                            customerFields.Append("</td></tr>");
                        }
                    }
                    if (customerFields.Length > 0)
                    {
                        phCustomFields.Controls.Add(new LiteralControl(customerFields.ToString()));
                    }
                    phCustomFields.Controls.Add(new LiteralControl("</table><br />"));
                }
            }
        }
        else
        {
            phCustomFields.Controls.Clear();
        }
    }

    protected ProductTemplateField FindTemplateField(int inputFieldId)
    {
        if (_Product != null)
        {
            foreach (ProductTemplateField cf in _Product.TemplateFields)
            {
                if (cf.InputFieldId == inputFieldId) return cf;
            }
        }
        return null;
    }

    public void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("EditProduct.aspx?ProductId=" + _ProductId.ToString());
    }

    public void SaveButton_Click(object sender, EventArgs e)
    {
        // DELETE ANY PRODUCT CUSTOM FIELDS SAVED TO THE DATABASE
        _Product.TemplateFields.DeleteAll();

        // UPDATE TEMPLATE AND COLLECT ANY VALUES
        if (_Product.ProductProductTemplates.Count > 0)
        {
            // DELETE THE OLD PRODUCT TEMPLATE ASSOCIATION FROM DATABASE
            ProductProductTemplateCollection pptCollection = ProductProductTemplateDataSource.LoadForProduct(_ProductId);
            int[] selectedTemplates = AlwaysConvert.ToIntArray(Request.Form[HiddenSelectedTemplates.UniqueID]);
            if (selectedTemplates != null && selectedTemplates.Length > 0)
            {
                foreach (ProductProductTemplate ppt in pptCollection)
                {
                    if (Array.IndexOf(selectedTemplates, ppt.ProductTemplateId) < 0) ppt.Delete();
                }
            }

            // GATEHR NEW VALUES FOR PRODUCT CUSTOM FIELDS
            _Product.ProductProductTemplates.Save();
            ProductHelper.CollectProductTemplateInput(_Product, phCustomFields);
            _Product.TemplateFields.Save();
        }
        else
        {
            // COMMIT ANY ALTERATIONS TO THE PRODCT
            ProductProductTemplateDataSource.DeleteForProduct(_ProductId);
        }

        // DISPLAY CONFIRMATION
        SavedMessage.Visible = true;
        SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
    }

}
