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
using CommerceBuilder.Web.UI;
using CommerceBuilder.Marketing;
using CommerceBuilder.Utility;
using CommerceBuilder.Users;
using CommerceBuilder.Common;

public partial class Admin_Marketing_Discounts_EditDiscount : AbleCommerceAdminPage
{
    private VolumeDiscount _VolumeDiscount;
    private int _VolumeDiscountId = 0;

    protected bool IsAdd
    {
        get
        {
            return (AlwaysConvert.ToInt(Request.QueryString["IsAdd"]) != 0);
        }
    }

    protected string GetLevelValue(LSDecimal levelValue)
    {
        if (levelValue == 0) return string.Empty;
        return string.Format("{0:0.##}", levelValue);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        if (this.IsAdd)
        {
            _VolumeDiscount.Delete();
        }
        Response.Redirect("Default.aspx");
    }

    protected void AddRowButton_Click(object sender, EventArgs e)
    {
        SaveDiscount();
        VolumeDiscount discount = _VolumeDiscount;
        VolumeDiscountLevel newDiscountLevel = new VolumeDiscountLevel();
        newDiscountLevel.IsDirty = true;
        discount.Levels.Add(newDiscountLevel);
        discount.Save();
        DiscountLevelGrid.DataSource = discount.Levels;
        DiscountLevelGrid.DataBind();
    }

    protected void DiscountLevelGrid_PreRender(object sender, EventArgs e)
    {
        VolumeDiscount discount = _VolumeDiscount;
        if (discount != null)
        {
            if (discount.Levels.Count == 0)
            {
                VolumeDiscountLevel newDiscountLevel = new VolumeDiscountLevel();
                newDiscountLevel.IsDirty = true;
                discount.Levels.Add(newDiscountLevel);
            }
            
            DiscountLevelGrid.DataSource = _VolumeDiscount.Levels;
            DiscountLevelGrid.DataBind();
        }
    }

    private void SaveDiscount()
    {
        VolumeDiscount discount = _VolumeDiscount;
        discount.Name = Name.Text;
        discount.IsValueBased = IsValueBased.SelectedIndex == 1 ? true : false;
        //LOOP THROUGH GRID ROWS AND SET MATRIX
        int rowIndex = 0;
        foreach (GridViewRow row in DiscountLevelGrid.Rows)
        {
            if (discount.Levels.Count < (rowIndex + 1))
            {
                // ADD A NEW DISCOUNT LEVEL FOR NEW ROWS
                VolumeDiscountLevel newDiscountLevel = new VolumeDiscountLevel();
                newDiscountLevel.IsDirty = true;
                newDiscountLevel.VolumeDiscountId = _VolumeDiscountId;
                discount.Levels.Add(newDiscountLevel);
            }
            LSDecimal minValue = AlwaysConvert.ToDecimal(((TextBox)row.FindControl("MinValue")).Text);
            LSDecimal maxValue = AlwaysConvert.ToDecimal(((TextBox)row.FindControl("MaxValue")).Text);
            LSDecimal discountAmount = AlwaysConvert.ToDecimal(((TextBox)row.FindControl("DiscountAmount")).Text);
            bool isPercent = (((DropDownList)row.FindControl("IsPercent")).SelectedIndex == 0);
            VolumeDiscountLevel thisLevel = discount.Levels[rowIndex];
            thisLevel.MinValue = minValue;
            thisLevel.MaxValue = maxValue;
            thisLevel.DiscountAmount = discountAmount;
            thisLevel.IsPercent = isPercent;
            rowIndex++;
            
        }
        //SCOPE
        discount.IsGlobal = (UseGlobalScope.SelectedIndex == 0);
        //GROUP RESTRICTION
        if (UseGroupRestriction.SelectedIndex > 0)
        {
            _VolumeDiscount.VolumeDiscountGroups.DeleteAll();
            foreach (ListItem item in GroupList.Items)
            {
                if (item.Selected) _VolumeDiscount.VolumeDiscountGroups.Add(new VolumeDiscountGroup(0, AlwaysConvert.ToInt(item.Value)));
            }
        }
        else
        {
            _VolumeDiscount.VolumeDiscountGroups.DeleteAll();
        }
        discount.Save();
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            SaveDiscount();
            Response.Redirect("Default.aspx");
        }
    }

    protected void EditDiscountScope_Click(object sender, EventArgs e)
    {
        SaveDiscount();
        Response.Redirect("EditDiscountScope.aspx?VolumeDiscountId=" + _VolumeDiscountId.ToString() + "&Edit=1");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _VolumeDiscountId = AlwaysConvert.ToInt(Request.QueryString["VolumeDiscountId"]);
        _VolumeDiscount = VolumeDiscountDataSource.Load(_VolumeDiscountId);
        if (!Page.IsPostBack)
        {
            AddCaption.Visible = this.IsAdd;
            EditCaption.Visible = !AddCaption.Visible;
            if (EditCaption.Visible) EditCaption.Text = string.Format(EditCaption.Text, _VolumeDiscount.Name);
            Name.Text = _VolumeDiscount.Name;
            IsValueBased.SelectedIndex = (_VolumeDiscount.IsValueBased ? 1 : 0);
            //SCOPE
            UseGlobalScope.SelectedIndex = (_VolumeDiscount.IsGlobal) ? 0 : 1;
            BindScope();
            //GROUP RESTRICTION
            UseGroupRestriction.SelectedIndex = (_VolumeDiscount.VolumeDiscountGroups.Count > 0) ? 1 : 0;
            BindGroups();
        }
    }

    protected void UseGroupRestriction_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindGroups();
    }

    protected void BindGroups()
    {
        GroupListPanel.Visible = (UseGroupRestriction.SelectedIndex > 0);
        if (GroupListPanel.Visible)
        {
            GroupList.DataSource = GroupDataSource.LoadForStore("Name");
            GroupList.DataBind();
            foreach (VolumeDiscountGroup group in _VolumeDiscount.VolumeDiscountGroups)
            {
                ListItem listItem = GroupList.Items.FindByValue(group.GroupId.ToString());
                if (listItem != null) listItem.Selected = true;
            }
        }
    }

    protected void UseGlobalScope_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindScope();
    }

    protected void BindScope()
    {
        ScopePanel.Visible = (UseGlobalScope.SelectedIndex > 0);
        if (ScopePanel.Visible)
        {
            Scope.Text = string.Format(Scope.Text, _VolumeDiscount.CategoryVolumeDiscounts.Count, _VolumeDiscount.ProductVolumeDiscounts.Count);
        }
    }

    protected void DiscountLevelGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DropDownList IsPercent = (DropDownList)e.Row.FindControl("IsPercent");
        if (IsPercent != null)
        {
            IsPercent.SelectedIndex = ((VolumeDiscountLevel)e.Row.DataItem).IsPercent ? 0 : 1;
        }
    }    

    protected void DiscountLevelGrid_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int index = e.RowIndex;
        SaveDiscount();
        VolumeDiscount discount = _VolumeDiscount;
        if (discount.Levels.Count >= (index + 1))
        {
            discount.Levels.DeleteAt(index);
            discount.Save();
        }
        DiscountLevelGrid.DataSource = discount.Levels;
        DiscountLevelGrid.DataBind();
    }
}
