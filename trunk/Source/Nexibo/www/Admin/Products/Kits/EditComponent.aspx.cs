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
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;
using System.Text.RegularExpressions;

public partial class Admin_Products_Kits_EditComponent : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private int _CategoryId = 0;
    private Category _Category;
    private Product _Product;
    private int _ProductId = 0;
    private KitComponent _KitComponent;
    private int _KitComponentId = 0;

    protected int CategoryId
    {
        get
        {
            if (_CategoryId.Equals(0))
            {
                _CategoryId = PageHelper.GetCategoryId();
            }
            return _CategoryId;
        }
    }

    protected Category Category
    {
        get
        {
            if (_Category == null)
            {
                _Category = CategoryDataSource.Load(this.CategoryId);
            }
            return _Category;
        }
    }

    protected Product Product
    {
        get
        {
            if (_Product == null)
            {
                _Product = ProductDataSource.Load(this.ProductId);
            }
            return _Product;
        }
    }

    protected int ProductId
    {
        get
        {
            if (_ProductId.Equals(0))
            {
                _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
            }
            return _ProductId;
        }
    }

    protected KitComponent KitComponent
    {
        get
        {
            if (_KitComponent == null)
            {
                _KitComponent = KitComponentDataSource.Load(this.KitComponentId);
            }
            return _KitComponent;
        }
    }

    protected int KitComponentId
    {
        get
        {
            if (_KitComponentId.Equals(0))
            {
                _KitComponentId = AlwaysConvert.ToInt(Request.QueryString["KitComponentId"]);
            }
            return _KitComponentId;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Product == null) Response.Redirect("../../Default.aspx");
        if (KitComponent == null) Response.Redirect(string.Format("EditKit.aspx?CategoryId={0}&ProductId={1}", this.CategoryId, this.ProductId));
        if (!Page.IsPostBack)
        {
            //LOAD THE INPUT TYPE LIST
            foreach (string inputName in Enum.GetNames(typeof(KitInputType)))
            {
                KitInputType inputType = (KitInputType)Enum.Parse(typeof(KitInputType), inputName);
                ListItem item = new ListItem(FixInputTypeName(inputName), inputType.ToString());
                if (KitComponent.InputType == inputType) item.Selected = true;
                InputTypeId.Items.Add(item);
            }
            //POPULATE FORM
            Caption.Text = string.Format(Caption.Text, KitComponent.Name);
            Name.Text = KitComponent.Name;
            InputTypeId.SelectedValue = KitComponent.InputTypeId.ToString();
            InputTypeChanged(sender, e);
            HeaderOption.Text = KitComponent.HeaderOption;
            //FOCUS INPUT
            Name.Focus();
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
        KitComponent.Name = Name.Text;
        KitComponent.InputTypeId = (short)(InputTypeId.SelectedIndex);
        KitComponent.HeaderOption = HeaderOption.Text;
        KitComponent.Save();
        Response.Redirect(string.Format("EditKit.aspx?CategoryId={0}&ProductId={1}", this.CategoryId, this.ProductId));
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Format("EditKit.aspx?CategoryId={0}&ProductId={1}", this.CategoryId, this.ProductId));
    }
}
