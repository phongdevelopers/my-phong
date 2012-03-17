<%@ WebHandler Language="C#" Class="CreateOrder" %>
using System;
using System.Web;

public class CreateOrder : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.Redirect("CreateOrder1.aspx");
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}