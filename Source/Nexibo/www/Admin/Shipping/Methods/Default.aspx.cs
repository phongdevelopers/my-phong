using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Common;
using CommerceBuilder.Shipping;
using CommerceBuilder.Utility;

public partial class Admin_Shipping_Methods_Default : CommerceBuilder.Web.UI.AbleCommerceAdminPage
{
    private ShipGatewayCollection _ShipProviders;
    private string _IconPath = string.Empty;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _ShipProviders = ShipGatewayDataSource.LoadForStore();        
        if (_ShipProviders.Count == 0)
        {
            NoShipProviderMessage.Visible = true;
            IntegratedProviderPanel.Visible = false;
        }
        else
        {
            NoShipProviderMessage.Visible = false;
            IntegratedProviderPanel.Visible = true;
            Provider.DataSource = _ShipProviders;
            Provider.DataBind();
        }
        _IconPath = Page.ResolveUrl("~/App_Themes/" + Page.Theme + "/Images/Icons/");
        PageHelper.SetDefaultButton(AddShipMethodName, AddShipMethodButton.ClientID);
    }

    protected string GetWarehouseNames(object dataItem)
    {
        ShipMethod method = (ShipMethod)dataItem;
        if ((method.ShipMethodWarehouses.Count == 0))
        {
            return "All";
        }
        List<string> warehouses = new List<string>();
        foreach (ShipMethodWarehouse warehouseAssn in method.ShipMethodWarehouses)
        {
            warehouses.Add(warehouseAssn.Warehouse.Name);
        }
        string warehouseList = string.Join(", ", warehouses.ToArray());
        if ((warehouseList.Length > 100))
        {
            warehouseList = (warehouseList.Substring(0, 100) + "...");
        }
        return warehouseList;
    }

    protected string GetZoneNames(object dataItem)
    {
        ShipMethod method = (ShipMethod)dataItem;
        if ((method.ShipMethodShipZones.Count == 0))
        {
            return "All";
        }
        List<string> zones = new List<string>();
        foreach (ShipMethodShipZone zoneAssn in method.ShipMethodShipZones)
        {
            zones.Add(zoneAssn.ShipZone.Name);
        }
        string zoneList = string.Join(", ", zones.ToArray());
        if ((zoneList.Length > 100))
        {
            zoneList = (zoneList.Substring(0, 100) + "...");
        }
        return zoneList;
    }

    protected string GetNames(object dataItem)
    {
        ShipMethod method = (ShipMethod)dataItem;
        if ((method.ShipMethodGroups.Count == 0))
        {
            return "All";
        }
        List<string> groups = new List<string>();
        foreach (ShipMethodGroup groupAssn in method.ShipMethodGroups)
        {
            groups.Add(groupAssn.Group.Name);
        }
        string groupList = string.Join(", ", groups.ToArray());
        if ((groupList.Length > 100))
        {
            groupList = (groupList.Substring(0, 100) + "...");
        }
        return groupList;
    }

    protected string GetMinPurchase(object dataItem)
    {
        ShipMethod method = (ShipMethod)dataItem;
        if (method.MinPurchase > 0) return string.Format("{0:lc}", method.MinPurchase);
        return string.Empty;
    }

    protected string GetEditUrl(object dataItem)
    {
        ShipMethod method = (ShipMethod)dataItem;
        if ((method.ShipMethodType == ShipMethodType.FlatRate))
        {
            return ("EditShipMethodFixed.aspx?ShipMethodId=" + method.ShipMethodId.ToString());
        }
        else if ((method.ShipMethodType == ShipMethodType.IntegratedProvider))
        {
            return ("EditShipMethodProvider.aspx?ShipMethodId=" + method.ShipMethodId.ToString());
        }
        return ("EditShipMethodMatrix.aspx?ShipMethodId=" + method.ShipMethodId.ToString());
    }

    protected void AddShipMethodButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            ShipMethod method = new ShipMethod();
            string selectedMethodType = AddShipMethodType.SelectedValue;
            method.ShipMethodType = ((ShipMethodType)(AlwaysConvert.ToInt16(AddShipMethodType.SelectedValue)));
            method.Name = AddShipMethodName.Text;
            method.Save();
            Response.Redirect(GetEditUrl(method));
        }
    }

    protected ShipGateway GetProvider(int shipGatewayId)
    {
        foreach (ShipGateway provider in _ShipProviders)
        {
            if (provider.ShipGatewayId.Equals(shipGatewayId)) return provider;
        }
        return null;
    }

    protected void Provider_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindServiceCodes();
    }

    private void BindServiceCodes()
    {
        ServiceCode.Items.Clear();
        int shipGatewayId = AlwaysConvert.ToInt(Provider.SelectedValue);
        ShipGateway provider = GetProvider(shipGatewayId);
        if (provider != null)
        {
            ShipMethodCollection Shipmethods = ShipMethodDataSource.LoadForShipGateway(shipGatewayId);
            ListItem[] servicesArray = provider.GetProviderInstance().GetServiceListItems();
            foreach (ListItem item in servicesArray)
            {
                ServiceCode.Items.Add(item);
            }
        }
    }

    protected void AddProviderMethod_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            ListItem li = ServiceCode.SelectedItem;
            ShipMethod method = new ShipMethod();
            method.ShipMethodType = ShipMethodType.IntegratedProvider;
            method.ShipGatewayId = AlwaysConvert.ToInt(Provider.SelectedValue);
            method.ServiceCode = li.Value;
            method.Name = li.Text;
            method.Save();
            Response.Redirect("EditShipMethodProvider.aspx?ShipMethodId=" + method.ShipMethodId.ToString());
        }
    }

    protected string GetConfirmDelete(object obj)
    {
        string name = (string)obj;
        return string.Format("return confirm('Are you sure you want to delete {0}?\')", name.Replace("'", "\\'"));
    }

    protected void MultipleRowDelete_Click(object sender, EventArgs e)
    {
        // Looping through all the rows in the GridView
        foreach (GridViewRow row in ShipMethodGrid.Rows)
        {
            CheckBox checkbox = (CheckBox)row.FindControl("DeleteCheckbox");
            if ((checkbox != null) && (checkbox.Checked))
            {
                // Retreive the GiftCertificateId
                int shipMethodId = Convert.ToInt32(ShipMethodGrid.DataKeys[row.RowIndex].Value);
                ShipMethod sm = ShipMethodDataSource.Load(shipMethodId);
                if (sm != null) sm.Delete();
            }
        }
        ShipMethodGrid.DataBind();
    }
    protected void ShipMethodGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        ShipMethodCollection shipMethods = ShipMethodDataSource.LoadForStore(ShipMethodGrid.SortExpression);
        if (e.CommandName.StartsWith("Do_"))
        {
            int shipMethodId  = AlwaysConvert.ToInt(e.CommandArgument.ToString());            
            int index;
            index = shipMethods.IndexOf(shipMethodId);
            switch (e.CommandName)
            {
                case "Do_Up":                    
                    ReorderMethod(shipMethods, index, index - 1);
                    ShipMethodGrid.DataBind();
                    break;
                case "Do_Down":
                    ReorderMethod(shipMethods, index, index + 1);
                    ShipMethodGrid.DataBind();
                    break;
            }
        }
    }

    protected void ReorderMethod(ShipMethodCollection _ShipMethods,int oldIndex, int newIndex)
    {
        if ((oldIndex < 0) || (oldIndex >= _ShipMethods.Count)) return;
        if ((newIndex < 0) || (newIndex >= _ShipMethods.Count)) return;
        if (oldIndex == newIndex) return;
        //MAKE SURE ITEMS ARE IN CORRECT ORDER
        for (short i = 0; i < _ShipMethods.Count; i++)
            _ShipMethods[i].OrderBy = (short)(i + 1);
        //LOCATE THE DESIRED ITEM
        ShipMethod temp = _ShipMethods[oldIndex];
        _ShipMethods.RemoveAt(oldIndex);
        if (newIndex < _ShipMethods.Count)
            _ShipMethods.Insert(newIndex, temp);
        else
            _ShipMethods.Add(temp);
        //MAKE SURE ITEMS ARE IN CORRECT ORDER
        for (short i = 0; i < _ShipMethods.Count; i++)
            _ShipMethods[i].OrderBy = (short)(i + 1);
        _ShipMethods.Save();
    }
    protected string GetIconUrl(string icon)
    {
        return _IconPath + icon;
    }
}