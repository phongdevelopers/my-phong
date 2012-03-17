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
using System.Collections.Generic;
using CommerceBuilder.Utility;
using CommerceBuilder.Marketing;

public partial class Admin_Marketing_Discounts_Default : AbleCommerceAdminPage
{
    protected string GetNames(VolumeDiscount item)
    {
        List<string> groupNames = new List<string>();
        foreach (VolumeDiscountGroup vdg in item.VolumeDiscountGroups)
        {
            groupNames.Add(vdg.Group.Name);
        }
        return string.Join(", ", groupNames.ToArray());
    }

    protected void VolumeDiscountGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Copy"))
        {
            int volumeDiscountId = AlwaysConvert.ToInt(e.CommandArgument);
            VolumeDiscount copy = VolumeDiscount.Copy(volumeDiscountId, true);
            if (copy != null)
            {
                copy.Name = "Copy of " + copy.Name;
                copy.Save();
            }
            VolumeDiscountGrid.DataBind();
        }
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        VolumeDiscount newDiscount = new VolumeDiscount();
        newDiscount.Name = "New Discount";
        VolumeDiscountLevel newDiscountLevel = new VolumeDiscountLevel();
        newDiscount.Levels.Add(newDiscountLevel);
        newDiscount.Save();
        Response.Redirect("EditDiscount.aspx?VolumeDiscountId=" + newDiscount.VolumeDiscountId.ToString() + "&IsAdd=1");
    }

}
