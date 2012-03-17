<%@ WebHandler Language="C#" Class="BasketSummary" %>

using System;
using System.Web;
using CommerceBuilder.Orders;
using CommerceBuilder.Catalog;
using CommerceBuilder.Utility;

public class BasketSummary : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        int basketId = AlwaysConvert.ToInt(context.Request.QueryString["BasketId"]);
        BasketItemCollection basketItems = BasketItemDataSource.LoadForBasket(basketId);
        context.Response.Clear();
        context.Response.Write("<div class=\"section\" style=\"width:300px\">\r\n");
        context.Response.Write("<div class=\"header\"><h2>Basket Contents</h2></div>\r\n");
        context.Response.Write("<table class=\"itemList\" cellpadding=\"4\" cellspacing=\"0\">\r\n");
        context.Response.Write("<tr>\r\n");
        context.Response.Write("<th align=\"left\">Item</th>\r\n");
        context.Response.Write("<th align=\"center\">Qty</th>\r\n");
        context.Response.Write("</tr>\r\n");
        foreach (BasketItem basketItem in basketItems)
        {
            context.Response.Write("<tr>\r\n");
            context.Response.Write("<td>");
            context.Response.Write(basketItem.Name);
            context.Response.Write("</td>\r\n");
            context.Response.Write("<td align=\"center\">");
            context.Response.Write(basketItem.Quantity.ToString());
            context.Response.Write("</td>\r\n");
            context.Response.Write("</tr>\r\n");
            
        }
        context.Response.Write("</table>\r\n");
        context.Response.Write("</div>");
        context.Response.End();
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}