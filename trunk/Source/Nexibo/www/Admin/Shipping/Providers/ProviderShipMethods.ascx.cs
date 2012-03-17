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
using CommerceBuilder.Shipping;
using System.Collections.Generic;
using CommerceBuilder.Utility;

public partial class Admin_Shipping_Providers_ProviderShipMethods : System.Web.UI.UserControl
{
    private int _ShipGatewayId = 0;
    private ShipGateway _ShipGateway;
    protected void ShipMethodGrid_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            int index;
            for (index = 1; (index <= (e.Row.Cells.Count - 2)); index++)
            {
                ShipMethod method = (ShipMethod)e.Row.DataItem;
                e.Row.Cells[index].Attributes.Add("onclick", "window.location=\'" + GetEditUrl(method) + "\'");
            }
        }
    }
    private void BindShipMethodsDs()
    {
        ShipMethodGrid.DataSource = ShipMethodDataSource.LoadForShipGateway(_ShipGatewayId);
        ShipMethodGrid.DataBind();
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

    protected string GetEditUrl(object dataItem)
    {
        ShipMethod method = (ShipMethod)dataItem;

        if ((method.ShipMethodType == ShipMethodType.FlatRate))
        {
            return Page.ResolveUrl("~/Admin/Shipping/Methods/EditShipMethodFixed.aspx?ShipMethodId=" + method.ShipMethodId.ToString());
        }
        else if ((method.ShipMethodType == ShipMethodType.IntegratedProvider))
        {
            return Page.ResolveUrl("~/Admin/Shipping/Methods/EditShipMethodProvider.aspx?ShipMethodId=" + method.ShipMethodId.ToString());
        }

        return Page.ResolveUrl("~/Admin/Shipping/Methods/EditShipMethodMatrix.aspx?ShipMethodId=" + method.ShipMethodId.ToString());

    }

    protected void InitServices()
    {
        //CheckBoxList ServicesCheckList = (CheckBoxList)ServicesConfigPanel.FindControl("ServicesCheckList");
        ServicesCheckList.Items.Clear();
        //update which are configured and which are not
        ShipMethodCollection Shipmethods = ShipMethodDataSource.LoadForShipGateway(_ShipGatewayId);
        ListItem[] servicesArray = _ShipGateway.GetProviderInstance().GetServiceListItems();
        foreach (ListItem item in servicesArray)
        {
            if (!IsShipMethodConfigured(item, Shipmethods))
            {
                ServicesCheckList.Items.Add(item);
            }
        }
        if (ServicesCheckList.Items.Count == 0)
        {
            ServicesCheckListPh.Visible = true;
        }
        else
        {
            ServicesCheckListPh.Visible = false;
        }
    }
    private Boolean IsShipMethodConfigured(ListItem item, ShipMethodCollection ShipMethods)
    {
        Boolean contains = false;
        foreach (ShipMethod shipMethod in ShipMethods)
        {
            if (shipMethod.ServiceCode.Equals(item.Value))
            {
                return true;
            }
        }
        return contains;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (_ShipGatewayId.Equals(0))
        {
            _ShipGatewayId = AlwaysConvert.ToInt(Request.QueryString["ShipGatewayId"]);
        }
        _ShipGateway = ShipGatewayDataSource.Load(_ShipGatewayId);

        if (!Page.IsPostBack)
        {
            InitServices();
            BindShipMethodsDs();
        }
    }

    protected void ServicesAddButton_Click(object sender, EventArgs e)
    {
        //CheckBoxList ServicesCheckList = (CheckBoxList)ServicesConfigPanel.FindControl("ServicesCheckList");
        List<ListItem> removeList = new List<ListItem>();
        foreach (ListItem item in ServicesCheckList.Items)
        {
            if (item.Selected)
            {
                ShipMethod method = new ShipMethod();
                method.ShipMethodType = ShipMethodType.IntegratedProvider;
                method.ShipGatewayId = _ShipGatewayId;
                method.ServiceCode = item.Value;
                method.Name = item.Text;
                method.Save();
            }
        }
        InitServices();
        BindShipMethodsDs();
    }

    protected void ShipMethodGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        ShipMethodDataSource.LoadForShipGateway(_ShipGatewayId).DeleteAt(e.RowIndex);
        InitServices();
        BindShipMethodsDs();
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
        InitServices();
        BindShipMethodsDs();
    }
}
