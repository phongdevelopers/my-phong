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
using CommerceBuilder.Products;
using CommerceBuilder.Utility;
using System.Text.RegularExpressions;

public partial class Admin_Products_Kits_AddComponent : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private Product _Product;
    private int _ProductId = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect("../../Default.aspx");
        if (!Page.IsPostBack)
        {
            foreach (string inputName in Enum.GetNames(typeof(KitInputType)))
            {
                InputTypeId.Items.Add(new ListItem(FixInputTypeName(inputName), Enum.Parse(typeof(KitInputType), inputName).ToString()));
            }
            InputTypeChanged(sender, e);
            Name.Focus();
            AttachComponent.NavigateUrl += "?ProductId=" + _ProductId.ToString();
        }
    }

    private string FixInputTypeName(string name)
    {
        switch (name.ToUpperInvariant())
        {
            case "INCLUDEDHIDDEN":
                return "Included - Hidden";
            case "INCLUDEDSHOWN":
                return "Included - Shown";
            default:
                return Regex.Replace(name, "([A-Z])", " $1").Trim();
        }
    }

    protected void InputTypeChanged(object sender, EventArgs e)
    {
        //HEADER OPTION SHOULD SHOW FOR DROPDOWN AND RADIO
        trHeaderOption.Visible = (InputTypeId.SelectedIndex == 2 || InputTypeId.SelectedIndex == 3);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        //CREATE THE KIT COMPONENT
        KitComponent component = new KitComponent();
        component.Name = Name.Text;
        component.InputTypeId = (short)(InputTypeId.SelectedIndex);
        component.HeaderOption = HeaderOption.Text;
        component.Save();
        //ATTACH COMPONENT TO PRODUCT
        _Product.ProductKitComponents.Add(new ProductKitComponent(_ProductId, component.KitComponentId));
        _Product.Save();
        Response.Redirect("EditKit.aspx?ProductId=" + _ProductId.ToString());
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("EditKit.aspx?ProductId=" + _ProductId.ToString());
    }
}
