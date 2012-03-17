using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using CommerceBuilder.Catalog;
using CommerceBuilder.Data;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Marketing;
using CommerceBuilder.Messaging;
using CommerceBuilder.Orders;
using CommerceBuilder.Payments;
using CommerceBuilder.Payments.Providers;
using CommerceBuilder.Products;
using CommerceBuilder.Reporting;
using CommerceBuilder.Shipping;
using CommerceBuilder.Stores;
using CommerceBuilder.Taxes;
using CommerceBuilder.Taxes.Providers;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using AjaxControlToolkit;

public partial class Admin_Orders_ViewDigitalGoods : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    private int _OrderId;
    private Order _Order;

    protected void Page_Init(object sender, EventArgs e)
    {
        _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
        _Order = OrderDataSource.Load(_OrderId);
        if (_Order == null) Response.Redirect("Default.aspx");
        AttachLink.NavigateUrl = String.Format(AttachLink.NavigateUrl, _Order.OrderNumber, _OrderId);
        BindDigitalGoodsGrid();
    }

    protected void BindDigitalGoodsGrid()
    {
        DigitalGoodsGrid.DataSource = GetDigitalGoods();
        DigitalGoodsGrid.DataBind();
    }

    protected List<OrderItemDigitalGood> GetDigitalGoods()
    {
        List<OrderItemDigitalGood> oidgList = new List<OrderItemDigitalGood>();
        foreach (OrderItem orderItem in _Order.Items)
        {
            foreach (OrderItemDigitalGood oidg in orderItem.DigitalGoods)
            {
                oidgList.Add(oidg);
            }
        }
        return oidgList;
    }

    protected OrderItem FindOrderItem(int orderItemId)
    {
        foreach (OrderItem orderItem in _Order.Items)
        {
            if (orderItem.OrderItemId == orderItemId)
                return orderItem;
        }
        return null;
    }

    protected OrderItemDigitalGood FindDigitalGood(int orderItemDigitalGoodId)
    {
        foreach (OrderItem orderItem in _Order.Items)
        {
            foreach (OrderItemDigitalGood oidg in orderItem.DigitalGoods)
            {
                if (oidg.OrderItemDigitalGoodId == orderItemDigitalGoodId) return oidg;
            }
        }
        return null;
    }

    protected void DigitalGoodsGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int orderItemDigitalGoodId = AlwaysConvert.ToInt(e.CommandArgument);
        OrderItemDigitalGood oidg = FindDigitalGood(orderItemDigitalGoodId);
        ISerialKeyProvider asd = oidg.DigitalGood.GetSerialKeyProviderInstance();
        if (oidg != null)
        {
            switch (e.CommandName)
            {
                case "Activate":
                    oidg.Activate();
                    oidg.Save();
                    break;
                case "Deactivate":
                    oidg.Deactivate();
                    oidg.Save();
                    break;
                case "GetKey":
                    if (oidg.DigitalGood.GetSerialKeyProviderInstance() != null)
                    {
                        oidg.AcquireSerialKey();
                        oidg.Save();
                    }
                    break;
                case "SetKey":
                    this.SerialKeyData.Text = String.Empty;
                    this.SaveKeyButton.Visible = true;
                    this.DeleteKeyButton.Visible = false;
                    this.OidgId.Value = oidg.OrderItemDigitalGoodId.ToString();
                    this.EditKeyPopup.Show();
                    break;
                //case "ReturnKey":
                //    if (oidg.DigitalGood.GetSerialKeyProviderInstance() != null)
                //    {
                //        oidg.ReturnSerialKey();
                //        oidg.Save();
                //    }
                //    else
                //    {
                //        oidg.SerialKeyData = null;
                //        oidg.Save();
                //    }
                //    break;
                case "ViewKey":
                    this.SerialKeyData.Text = oidg.SerialKeyData;
                    this.SaveKeyButton.Visible = false;
                    this.DeleteKeyButton.Visible = true;
                    this.OidgId.Value = oidg.OrderItemDigitalGoodId.ToString();
                    this.EditKeyPopup.Show();

                    break;
                case "Delete":
                    // GET THE ORDERITEM THAT CONTAINS THE DIGITAL GOOD
                    OrderItem oi = FindOrderItem(oidg.OrderItemId);
                    if (oi != null)
                    {
                        // DETERMINE IF THE ORDERITEM IS A PLACEHOLDER FOR DIGITAL GOOD
                        if (oi.CustomFields.TryGetValue("DigitalGoodIndicator") == "1")
                        {
                            // THE ORDERITEM IS A PLACEHOLDER, REMOVE THE ITEM (AND DIGITAL GOOD)
                            int oiIndex = _Order.Items.IndexOf(oi.OrderItemId);
                            if (oiIndex > -1)
                            {
                                _Order.Items.DeleteAt(oiIndex);
                            }
                        }
                        else
                        {
                            // NOT A PLACEHOLDER, DELETE THE DIGITAL GOOD ONLY
                            int dgIndex = oi.DigitalGoods.IndexOf(oidg.OrderItemDigitalGoodId);
                            if (dgIndex > -1)
                            {
                                oi.DigitalGoods.DeleteAt(dgIndex);
                            }
                        }
                    }
                    break;
            }
            BindDigitalGoodsGrid();
        }
    }

    protected string GetMaxDownloads(OrderItemDigitalGood oidg)
    {
        if (oidg.DigitalGood == null) return string.Empty;
        if (oidg.DigitalGood.MaxDownloads == 0) return "(no max)";
        return string.Format("(max {0})", oidg.DigitalGood.MaxDownloads);
    }

    protected bool HasSerialKey(OrderItemDigitalGood oidg)
    {
        return oidg.IsSerialKeyAcquired();
    }

    protected bool ProviderIsNull(OrderItemDigitalGood oidg)
    {
        return (oidg.DigitalGood == null || oidg.DigitalGood.GetSerialKeyProviderInstance() == null);
    }

    protected void DigitalGoodsGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        /* NO CODE */
    }

    protected bool SerialKeyEnabled(object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        if (oidg != null && oidg.DigitalGood != null)
        {
            return oidg.DigitalGood.EnableSerialKeys;
        }
        else return false;
    }

    protected bool ShowGetKey(object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        return SerialKeyEnabled(dataItem) && !HasSerialKey(oidg) && !ProviderIsNull(oidg) && oidg.DigitalGood.SerialKeys.Count > 0;
    }

    protected bool ShowSetKey(object dataItem)
    {
        OrderItemDigitalGood oidg = (OrderItemDigitalGood)dataItem;
        return SerialKeyEnabled(dataItem) && !HasSerialKey(oidg) && (ProviderIsNull(oidg) || oidg.DigitalGood.SerialKeys.Count == 0);
    }
    protected void DeleteKeyButton_Click(object sender, EventArgs e)
    {
        OrderItemDigitalGood oidg = OrderItemDigitalGoodDataSource.Load(AlwaysConvert.ToInt(this.OidgId.Value));
        if (oidg != null)
        {
            // WE NEED TO DELETE FROM THE CURRENT LOADED LIST TO AVOID DISPLAY ISSUES
            int i = _Order.Items.IndexOf(oidg.OrderItem.OrderItemId);
            int j = _Order.Items[i].DigitalGoods.IndexOf(oidg.OrderItemDigitalGoodId);
            OrderItemDigitalGood tempOidg = _Order.Items[i].DigitalGoods[j];
            tempOidg.SerialKeyData = null;
            tempOidg.Save();

            OidgId.Value = String.Empty;
            BindDigitalGoodsGrid();
            EditKeyPopup.Hide();
        }
    }

    protected void SaveKeyButton_Click(object sender, EventArgs e)
    {
        OrderItemDigitalGood oidg = OrderItemDigitalGoodDataSource.Load(AlwaysConvert.ToInt(this.OidgId.Value));
        if (oidg != null)
        {
            // WE NEED TO UPDATE THE CURRENT LOADED LIST TO AVOID DISPLAY ISSUES
            int i = _Order.Items.IndexOf(oidg.OrderItem.OrderItemId);
            int j = _Order.Items[i].DigitalGoods.IndexOf(oidg.OrderItemDigitalGoodId);
            OrderItemDigitalGood tempOidg = _Order.Items[i].DigitalGoods[j];
            tempOidg.SetSerialKey(SerialKeyData.Text, true);
            tempOidg.Save();
            OidgId.Value = String.Empty;
            BindDigitalGoodsGrid();
            EditKeyPopup.Hide();
        }
    }

}