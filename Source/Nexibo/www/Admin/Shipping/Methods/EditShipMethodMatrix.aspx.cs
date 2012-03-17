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
using CommerceBuilder.Shipping;
using CommerceBuilder.Taxes;

public partial class Admin_Shipping_Methods_EditShipMethodMatrix : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{

    int _ShipMethodId;
    ShipMethod _ShipMethod;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _ShipMethodId = AlwaysConvert.ToInt(Request.QueryString["ShipMethodId"]);
        _ShipMethod = ShipMethodDataSource.Load(_ShipMethodId);
        if (_ShipMethod == null) RedirectMe();
        //BIND TAX CODES
        TaxCodeCollection taxCodes = Token.Instance.Store.TaxCodes;
        TaxCode.DataSource = taxCodes;
        TaxCode.DataBind();
        ListItem item = TaxCode.Items.FindByValue(_ShipMethod.TaxCodeId.ToString());
        if (item != null) TaxCode.SelectedIndex = TaxCode.Items.IndexOf(item);

        SurchargeTaxCode.DataSource = taxCodes;
        SurchargeTaxCode.DataBind();
        item = SurchargeTaxCode.Items.FindByValue(_ShipMethod.SurchargeTaxCodeId.ToString());
        if (item != null) SurchargeTaxCode.SelectedIndex = SurchargeTaxCode.Items.IndexOf(item);
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Caption.Text = string.Format(Caption.Text, _ShipMethod.Name);
        RateMatrixHelpText.Text = string.Format(RateMatrixHelpText.Text, GetMatrixType());
        if (!Page.IsPostBack)
        {
            Name.Text = _ShipMethod.Name;
            ShipMethodType.Text = StoreDataHelper.GetFriendlyShipMethodType(_ShipMethod);
            BindShipRateMatrix();
            if (_ShipMethod.Surcharge > 0) Surcharge.Text = string.Format("{0:F2}", _ShipMethod.Surcharge);
            SurchargeIsPercent.SelectedIndex = _ShipMethod.SurchargeIsPercent ? 1 : 0;
            SurchargeIsVisible.SelectedIndex = _ShipMethod.SurchargeIsVisible ? 1 : 0;
            UseWarehouseRestriction.SelectedIndex = (_ShipMethod.ShipMethodWarehouses.Count == 0) ? 0 : 1;
            BindWarehouses();
            UseZoneRestriction.SelectedIndex = (_ShipMethod.ShipMethodShipZones.Count == 0) ? 0 : 1;
            BindZones();
            UseGroupRestriction.SelectedIndex = (_ShipMethod.ShipMethodGroups.Count == 0) ? 0 : 1;
            BindGroups();
            if (_ShipMethod.MinPurchase > 0) MinPurchase.Text = string.Format("{0:F2}", _ShipMethod.MinPurchase);
        }
        trSurchargeTaxCode.Visible = _ShipMethod.SurchargeIsVisible;
        PageHelper.ConvertEnterToTab(Name);
        PageHelper.ConvertEnterToTab(Surcharge);
    }
    
    protected void RedirectMe()
    {
        Response.Redirect("Default.aspx");
    }

    protected void UseWarehouseRestriction_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindWarehouses();
    }

    protected void BindWarehouses()
    {
        WarehouseListPanel.Visible = (UseWarehouseRestriction.SelectedIndex > 0);
        if (WarehouseListPanel.Visible)
        {
            WarehouseList.DataSource = WarehouseDataSource.LoadForStore("Name");
            WarehouseList.DataBind();
            if (WarehouseList.Items.Count > 4) WarehouseList.Rows = 8;
            foreach (ShipMethodWarehouse item in _ShipMethod.ShipMethodWarehouses)
            {
                ListItem listItem = WarehouseList.Items.FindByValue(item.WarehouseId.ToString());
                if (listItem != null) listItem.Selected = true;
            }
        }
    }

    protected void UseZoneRestriction_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindZones();
    }

    protected void BindZones()
    {
        ZoneListPanel.Visible = (UseZoneRestriction.SelectedIndex > 0);
        if (ZoneListPanel.Visible)
        {
            ZoneList.DataSource = ShipZoneDataSource.LoadForStore("Name");
            ZoneList.DataBind();
            if (ZoneList.Items.Count > 4) ZoneList.Rows = 8;
            foreach (ShipMethodShipZone item in _ShipMethod.ShipMethodShipZones)
            {
                ListItem listItem = ZoneList.Items.FindByValue(item.ShipZoneId.ToString());
                if (listItem != null) listItem.Selected = true;
            }
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
            foreach (ShipMethodGroup item in _ShipMethod.ShipMethodGroups)
            {
                ListItem listItem = GroupList.Items.FindByValue(item.GroupId.ToString());
                if (listItem != null) listItem.Selected = true;
            }
        }
    }

    protected void BindShipRateMatrix()
    {
        List<ShipRateMatrix> rows = new List<ShipRateMatrix>();
        if (_ShipMethod.ShipRateMatrices.Count == 0)
        {
            //ADD A DEFAULT ROW
            ShipRateMatrix item = new ShipRateMatrix();
            item.ShipMethodId = _ShipMethodId;
            _ShipMethod.ShipRateMatrices.Add(item);
            _ShipMethod.Save();
        }
        foreach (ShipRateMatrix item in _ShipMethod.ShipRateMatrices)
        {
            rows.Add(item);
        }
        rows.Sort(CompareShipRateMatrix);
        RateMatrixGrid.DataSource = rows;
        RateMatrixGrid.DataBind();
    }

    private string GetTextBoxValue(GridViewRow row, string controlId)
    {
        TextBox tb = row.FindControl(controlId) as TextBox;
        if (tb != null) return tb.Text;
        return String.Empty;
    }

    private bool GetCheckBoxValue(GridViewRow row, string controlId)
    {
        CheckBox cb = row.FindControl(controlId) as CheckBox;
        if (cb != null) return cb.Checked;
        return false;
    }

    protected void UpdateRanges()
    {
        //PROCESS MATRIX UPDATES
        int index = 0;
        foreach (GridViewRow row in RateMatrixGrid.Rows)
        {
            ShipRateMatrix matrixItem = _ShipMethod.ShipRateMatrices[index];
            matrixItem.RangeStart = AlwaysConvert.ToDecimal(GetTextBoxValue(row, "RangeStart"));
            matrixItem.RangeEnd = AlwaysConvert.ToDecimal(GetTextBoxValue(row, "RangeEnd"));
            matrixItem.Rate = AlwaysConvert.ToDecimal(GetTextBoxValue(row, "Rate"));
            matrixItem.IsPercent = GetCheckBoxValue(row, "IsPercent");
            index++;
        }
    }

    protected string GetMatrixType()
    {
        string matrixType = _ShipMethod.ShipMethodType.ToString();
        return matrixType.Replace("Based", "").Replace("([A-Z])", " $1").Trim();
    }

    protected int CompareShipRateMatrix(ShipRateMatrix x, ShipRateMatrix y)
    {
        if (x.RangeStart.Equals(0) && x.RangeEnd.Equals(0) && x.Rate.Equals(0)) return 1;
        if (y.RangeStart.Equals(0) && y.RangeEnd.Equals(0) && y.Rate.Equals(0)) return -1;
        if (!x.RangeStart.Equals(y.RangeStart)) return x.RangeStart.CompareTo(y.RangeStart);
        if (!x.RangeEnd.Equals(y.RangeEnd))
        {
            //NEED SPECIAL HANDLING, 0 AS RANGEEND INDICATES NO MAXIMUM
            if (x.RangeEnd == 0) return 1;
            if (y.RangeEnd == 0) return -1;
            return x.RangeEnd.CompareTo(y.RangeEnd);
        }
        if (!x.Rate.Equals(y.Rate)) return x.Rate.CompareTo(y.Rate);
        return 0;
    }

    protected void AddRowButton_Click(object sender, EventArgs e)
    {
        UpdateRanges();
        ShipRateMatrix item = new ShipRateMatrix();
        item.ShipMethodId = _ShipMethodId;
        _ShipMethod.ShipRateMatrices.Add(item);
        _ShipMethod.ShipRateMatrices.Save();
        BindShipRateMatrix();
        RateMatrixAjax.Update();
    }

    protected int GetIndex(int shipRateMatrixId)
    {
        int index = 0;
        foreach (ShipRateMatrix item in _ShipMethod.ShipRateMatrices)
        {
            if (item.ShipRateMatrixId.Equals(shipRateMatrixId)) return index;
            index++;
        }
        return -1;
    }

    protected void RateMatrixGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int shipRateMatrixId = (int)RateMatrixGrid.DataKeys[e.RowIndex].Value;
        int index = GetIndex(shipRateMatrixId);
        if (index > -1) _ShipMethod.ShipRateMatrices.DeleteAt(index);
        BindShipRateMatrix();
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        RedirectMe();
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            //UPDATE NAME
            _ShipMethod.Name = Name.Text;
            //UPDATE SHIP RATE
            UpdateRanges();
            //UPDATE SURCHARGE
            _ShipMethod.Surcharge = AlwaysConvert.ToDecimal(Surcharge.Text);
            if (_ShipMethod.Surcharge < 0) _ShipMethod.Surcharge = 0;
            if (_ShipMethod.Surcharge > 0)
            {
                _ShipMethod.SurchargeIsPercent = (SurchargeIsPercent.SelectedIndex > 0);
                _ShipMethod.SurchargeIsVisible = (SurchargeIsVisible.SelectedIndex > 0);
            } else {
                _ShipMethod.SurchargeIsPercent = false;
                _ShipMethod.SurchargeIsVisible = false;
            }

            if (_ShipMethod.SurchargeIsVisible)
            {
                _ShipMethod.SurchargeTaxCodeId = AlwaysConvert.ToInt(SurchargeTaxCode.SelectedValue);
            }
            else _ShipMethod.SurchargeTaxCodeId = 0;
            //UPDATE WAREHOUSES
            _ShipMethod.ShipMethodWarehouses.DeleteAll();
            if (UseWarehouseRestriction.SelectedIndex > 0)
            {
                foreach (ListItem item in WarehouseList.Items)
                {
                    if (item.Selected) _ShipMethod.ShipMethodWarehouses.Add(new ShipMethodWarehouse(_ShipMethodId, AlwaysConvert.ToInt(item.Value)));
                }
            }
            //UPDATE ZONES
            _ShipMethod.ShipMethodShipZones.DeleteAll();
            if (UseZoneRestriction.SelectedIndex > 0)
            {
                foreach (ListItem item in ZoneList.Items)
                {
                    if (item.Selected) _ShipMethod.ShipMethodShipZones.Add(new ShipMethodShipZone(_ShipMethodId, AlwaysConvert.ToInt(item.Value)));
                }
            }
            //UPDATE ROLES
            _ShipMethod.ShipMethodGroups.DeleteAll();
            if (UseGroupRestriction.SelectedIndex > 0)
            {
                foreach (ListItem item in GroupList.Items)
                {
                    if (item.Selected) _ShipMethod.ShipMethodGroups.Add(new ShipMethodGroup(_ShipMethodId, AlwaysConvert.ToInt(item.Value)));
                }
            }
            //UPDATE MIN PURCHASE
            _ShipMethod.MinPurchase = AlwaysConvert.ToDecimal(MinPurchase.Text);
            //UPDATE TAX CODES
            _ShipMethod.TaxCodeId = AlwaysConvert.ToInt(TaxCode.SelectedValue);
            //SAVE METHOD AND REDIRECT TO LIST
            _ShipMethod.Save();
            RedirectMe();
        }
    }

    protected void SurchargeIsVisible_SelectedIndexChanged(object sender, EventArgs e)
    {
        trSurchargeTaxCode.Visible = SurchargeIsVisible.SelectedIndex == 1;
    }


}
