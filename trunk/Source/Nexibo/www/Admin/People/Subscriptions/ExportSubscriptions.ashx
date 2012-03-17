<%@ WebHandler Language="C#" Class="ExportList" %>

using System;
using System.Web;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Orders;
using System.Web.SessionState;
using CommerceBuilder.Products;
public class ExportList : IHttpHandler, IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        if (context.Session["Subscriptions_To_Export"] == null)
            context.Response.Redirect("Default.aspx");
        List<int> selectedSubscriptions = context.Session["Subscriptions_To_Export"] as List<int>;
        if (selectedSubscriptions != null && selectedSubscriptions.Count > 0)
        {
            bool perUser = AlwaysConvert.ToBool(context.Request.QueryString["PerUser"], false);
            Dictionary<int, int> userCounts = new Dictionary<int, int>();
            StringBuilder userData = new StringBuilder();
            if (perUser)
            {
                userData.Append("Email,FirstName,LastName,Company,Address1,Address2,City,StateProvince,PostalCode,Country,Phone,Fax\r\n");
            }
            else
            {
                userData.Append("Email,FirstName,LastName,Company,Address1,Address2,City,StateProvince,PostalCode,Country,Phone,Fax,Plan,Expiration,OrderNumber\r\n");
            }
            // LOOP THE SPECIFIED SUBSCRIPTIONS TO BUILD EXPORT FILE
            foreach (int subscriptionId in selectedSubscriptions)
            {
                Subscription subscription = SubscriptionDataSource.Load(subscriptionId);
                SubscriptionPlan subscriptionPlan = subscription.SubscriptionPlan;
                Order order = subscription.OrderItem.Order;
                Address address = null;
                User user = subscription.User;
                if (user != null) address = user.PrimaryAddress;
                if (address == null) address = new Address();
                if (perUser)
                {
                    // WE ONLY WANT ONE EXPORT LINE PER USER ID
                    if (!userCounts.ContainsKey(user.UserId))
                    {
                        userData.Append(GetCsvValue(user.Email) + ",");
                        userData.Append(GetCsvValue(address.FirstName) + ",");
                        userData.Append(GetCsvValue(address.LastName) + ",");
                        userData.Append(GetCsvValue(address.Company) + ",");
                        userData.Append(GetCsvValue(address.Address1) + ",");
                        userData.Append(GetCsvValue(address.Address2) + ",");
                        userData.Append(GetCsvValue(address.City) + ",");
                        userData.Append(GetCsvValue(address.Province) + ",");
                        userData.Append(GetCsvValue(address.PostalCode) + ",");
                        userData.Append(GetCsvValue(address.CountryCode) + ",");
                        userData.Append(GetCsvValue(address.Phone) + ",");
                        userData.Append(GetCsvValue(address.Fax) + "\r\n");
                        userCounts[user.UserId] = 1;
                    }
                }
                else
                {
                    userData.Append(GetCsvValue(user.Email) + ",");
                    userData.Append(GetCsvValue(address.FirstName) + ",");
                    userData.Append(GetCsvValue(address.LastName) + ",");
                    userData.Append(GetCsvValue(address.Company) + ",");
                    userData.Append(GetCsvValue(address.Address1) + ",");
                    userData.Append(GetCsvValue(address.Address2) + ",");
                    userData.Append(GetCsvValue(address.City) + ",");
                    userData.Append(GetCsvValue(address.Province) + ",");
                    userData.Append(GetCsvValue(address.PostalCode) + ",");
                    userData.Append(GetCsvValue(address.CountryCode) + ",");
                    userData.Append(GetCsvValue(address.Phone) + ",");
                    userData.Append(GetCsvValue(address.Fax) + ",");
                    userData.Append(GetCsvValue(subscriptionPlan.Name) + ",");
                    userData.Append(GetCsvValue(subscription.ExpirationDate.ToString()) + ",");
                    userData.Append(GetCsvValue(order.OrderNumber.ToString()) + "\r\n");
                }
            }
            string outFileName = "subscriptions.csv";
            PageHelper.SendFileDataToClient(userData.ToString(), outFileName);
        }
        else
        {
            context.Response.Redirect("Default.aspx");
        }
        context.Session["Subscriptions_To_Export"] = null;
    }

    /// <summary>
    /// Escapes delimiters and field qualifies for CSV output
    /// </summary>
    /// <param name="input">The input value</param>
    /// <returns>An output value formatted for CSV</returns>
    private static string GetCsvValue(string input)
    {
        string output = input.Replace("\"", "\"\"");
        if (output.Contains(",")) return "\"" + output + "\"";
        return output;
    }

    public bool IsReusable
    {
        get
        {
            return true;
        }
    }
}