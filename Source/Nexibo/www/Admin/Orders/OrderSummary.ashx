<%@ WebHandler Language="C#" Class="OrderSummary" %>

using System;
using System.Web;
using CommerceBuilder.Catalog;
using CommerceBuilder.Orders;
using CommerceBuilder.Utility;

public class OrderSummary : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        int orderId = AlwaysConvert.ToInt(context.Request.QueryString["OrderId"]);
        Order order = OrderDataSource.Load(orderId);
        if (order == null) order = new Order();
        context.Response.Clear();
        context.Response.Write("<div class=\"section\" style=\"width:300px\">\r\n");
        context.Response.Write("<div class=\"header\"><h2>Order #" + order.OrderNumber.ToString() + " Contents</h2></div>\r\n");
        context.Response.Write("<table class=\"itemList\" cellpadding=\"4\" cellspacing=\"0\">\r\n");
        context.Response.Write("<tr>\r\n");
        context.Response.Write("<th align=\"left\">Item</th>\r\n");
        context.Response.Write("<th align=\"center\">Qty</th>\r\n");
        context.Response.Write("</tr>\r\n");
        foreach (OrderItem item in order.Items)
        {
            if (item.OrderItemType == OrderItemType.Product)
            {
                context.Response.Write("<tr>\r\n");
                context.Response.Write("<td>");
                context.Response.Write(item.Name);
                if (item.VariantName.Length > 0) context.Response.Write(" (" + item.VariantName + ")");
                context.Response.Write("</td>\r\n");
                context.Response.Write("<td align=\"center\">");
                context.Response.Write(item.Quantity);
                context.Response.Write("</td>\r\n");
                context.Response.Write("</tr>\r\n");
            }
        }
        context.Response.Write("</table>\r\n");
        context.Response.Write("</div>");
        context.Response.End();
    }
    
    public bool IsReusable {
        get {
            return false;
        }
    }

}