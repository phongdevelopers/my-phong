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
using CommerceBuilder.Payments;
using CommerceBuilder.Utility;

public partial class Admin_Payment_GiftCertificates : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        PageHelper.SetDefaultButton(SerialNumber, LookupButton.ClientID);
    }

    protected bool HasSerialNumber(object obj)
    {
        GiftCertificate oigc = (GiftCertificate)obj;
        return oigc.SerialNumber != null && oigc.SerialNumber.Length > 0;
    }

    protected void GiftCertificatesGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int index = AlwaysConvert.ToInt(e.CommandArgument);
        if (index >= GiftCertificatesGrid.DataKeys.Count) return;
        int GiftCertificateId = AlwaysConvert.ToInt(GiftCertificatesGrid.DataKeys[index].Value);
        GiftCertificate gc = GiftCertificateDataSource.Load(GiftCertificateId);
        if (gc == null) return;
        IGiftCertKeyProvider provider = new DefaultGiftCertKeyProvider();
        if (e.CommandName.Equals("Generate"))
        {
            gc.SerialNumber = provider.GenerateGiftCertificateKey();
            gc.AddActivatedTransaction();
            gc.Save();
            GiftCertificatesGrid.DataBind();
        }
        else if (e.CommandName.Equals("Deactivate"))
        {
            gc.SerialNumber = "";
            gc.AddDeactivatedTransaction();
            gc.Save();
            GiftCertificatesGrid.DataBind();
        }
    }

    protected void StatusList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (StatusList.SelectedValue == "0") InstructionsText.Visible = false;
        else InstructionsText.Visible = true;
    }

    protected bool IsDeleteable(int giftCertificateId)
    {
        GiftCertificate gc = GiftCertificateDataSource.Load(giftCertificateId);
        if (HasSerialNumber(gc) && !gc.IsExpired())
        {
            // IF ZERO BALANCE WITH ACTIVE GIFT CERTIFICATE
            if (gc.Balance == 0) return true;
            else return false;
        }
        else return true;
    }

    protected void MultipleRowDelete_Click(object sender, EventArgs e)
    {
        // Looping through all the rows in the GridView
        foreach (GridViewRow row in GiftCertificatesGrid.Rows)
        {
            CheckBox checkbox = (CheckBox)row.FindControl("DeleteCheckbox");
            if ((checkbox != null) && (checkbox.Checked))
            {
                // Retreive the GiftCertificateId
                int giftCertificateId = Convert.ToInt32(GiftCertificatesGrid.DataKeys[row.RowIndex].Value);
                GiftCertificate gc = GiftCertificateDataSource.Load(giftCertificateId);
                if (gc != null) gc.Delete();
            }
        }
        GiftCertificatesGrid.DataBind();
    }

    protected void PageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        GiftCertificatesGrid.PageIndex = 0;
        int pageSize = AlwaysConvert.ToInt(PageSize.SelectedValue);
        if (pageSize == 0)
        {
            GiftCertificatesGrid.AllowPaging = false;
        }
        else
        {
            GiftCertificatesGrid.AllowPaging = true;
            GiftCertificatesGrid.PageSize = pageSize;
        }
        GiftCertificatesGrid.DataBind();
    }

    protected void LookupButton_OnClick(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(SerialNumber.Text))
        {
            GiftCertificate gc = GiftCertificateDataSource.LoadForSerialNumber(SerialNumber.Text);
            if (gc != null)
            {
                InitializeViewGcPanel(gc);

                ViewGiftcertificatePanel.Visible = true;
                //Response.Redirect("~/Admin/Reports/GiftCertTransactions.aspx?GiftCertificateId=" + gc.GiftCertificateId);
            }
            else
            {
                NotFoundMessage.Visible = true;
                ViewGiftcertificatePanel.Visible = false;
            }
        }
    }

    private void InitializeViewGcPanel(GiftCertificate gc)
    {
        if (gc == null) return;

        Name.Text = gc.Name;
        if (gc.OrderItem != null)
        {
            GiftCertOrderIdLink.Text = gc.OrderItem.Order.OrderNumber.ToString();
            GiftCertOrderIdLink.NavigateUrl = "~/Admin/Orders/ViewOrder.aspx?OrderNumber=" + gc.OrderItem.Order.OrderNumber + "&OrderId=" + gc.OrderItem.OrderId;
        }
        else
        {
           GiftCertOrderIdLink.Text = "N/A";
        }
        if (gc.CreateDate != DateTime.MinValue) CreateDate.Text = String.Format("{0:d}", gc.CreateDate);
        if (gc.ExpirationDate != DateTime.MinValue) ExpirationDate.Text = String.Format("{0:d}", gc.ExpirationDate);
        Balance.Text = String.Format("{0:F2}", gc.Balance);
        GCSerialNumber.Text = gc.SerialNumber;

        Generate.CommandArgument = gc.GiftCertificateId.ToString();
        DeactivateButton.CommandArgument = gc.GiftCertificateId.ToString();
        DeleteButton.CommandArgument = gc.GiftCertificateId.ToString();

        Generate.Visible = !HasSerialNumber(gc);
        DeactivateButton.Visible = HasSerialNumber(gc) && !gc.IsExpired();
        DeleteButton.Visible = IsDeleteable(gc.GiftCertificateId);
        ViewHistory.NavigateUrl = "~/Admin/Reports/GiftCertTransactions.aspx?GiftCertificateId=" + gc.GiftCertificateId;

        HiddenGCID.Value = gc.GiftCertificateId.ToString();
    }

    protected void Deactivate_OnClick(object sender, EventArgs e)
    {
        int gcId = AlwaysConvert.ToInt(HiddenGCID.Value);
        GiftCertificate gc = GiftCertificateDataSource.Load(gcId);
        if (gc != null)
        {
            gc.SerialNumber = "";
            gc.AddDeactivatedTransaction();
            gc.Save();
            InitializeViewGcPanel(gc);
            GiftCertificatesGrid.DataBind();
            GiftCertificatesGridAjax.Update();
        }
    }

    protected void Delete_OnClick(object sender, EventArgs e)
    {
        int gcId = AlwaysConvert.ToInt(HiddenGCID.Value);
        GiftCertificate gc = GiftCertificateDataSource.Load(gcId);
        if (gc != null)
        {
            gc.Delete();
            ViewGiftcertificatePanel.Visible = false;
            GiftCertificatesGrid.DataBind();
            GiftCertificatesGridAjax.Update();
        }
    }

    protected void Generate_OnClick(object sender, EventArgs e)
    {
        int gcId = AlwaysConvert.ToInt(HiddenGCID.Value);
        GiftCertificate gc = GiftCertificateDataSource.Load(gcId);
        if (gc != null)
        {
            IGiftCertKeyProvider provider = new DefaultGiftCertKeyProvider();
            gc.SerialNumber = provider.GenerateGiftCertificateKey();
            gc.AddActivatedTransaction();
            gc.Save();
            InitializeViewGcPanel(gc);
            GiftCertificatesGrid.DataBind();
            GiftCertificatesGridAjax.Update();
        }
    }    
}
