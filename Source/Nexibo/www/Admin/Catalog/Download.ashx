<%@ WebHandler Language="C#" Class="Download" %>

using System;
using System.Web;
using CommerceBuilder.DigitalDelivery;
using CommerceBuilder.Utility;

public class Download : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        int digitalGoodId = AlwaysConvert.ToInt(context.Request.QueryString["DigitalGoodId"]);
        DigitalGood dg = DigitalGoodDataSource.Load(digitalGoodId);
        if (dg != null) PageHelper.SendFileDataToClient(dg);
    }
 
    public bool IsReusable {
        get {
            return true;
        }
    }

}