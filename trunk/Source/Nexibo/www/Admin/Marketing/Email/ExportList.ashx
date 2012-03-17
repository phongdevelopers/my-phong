<%@ WebHandler Language="C#" Class="ExportList" %>

using System;
using System.Web;
using System.Text;
using CommerceBuilder.Marketing;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

public class ExportList : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        int _EmailListId = AlwaysConvert.ToInt(context.Request.QueryString["EmailListId"]);
        EmailList _EmailList = EmailListDataSource.Load(_EmailListId);
        if (_EmailList.Users.Count > 0)
        {
            StringBuilder userData = new StringBuilder();
            userData.Append("Email,FirstName,LastName,Company,Address1,Address2,City,StateProvince,PostalCode,Country,Phone,Fax\r\n");
            foreach (EmailListUser elu in _EmailList.Users)
            {
                Address address = null;
                User user = UserDataSource.LoadMostRecentForEmail(elu.Email);
                if (user != null) address = user.PrimaryAddress;
                if (address == null) address = new Address();
                userData.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}\r\n", elu.Email,
                    address.FirstName, address.LastName, address.Company, address.Address1, address.Address2,
                    address.City, address.Province, address.PostalCode, address.Country == null ? "" : address.Country.Name, address.Phone, address.Fax));
            }
            string outFileName = "email_list.csv";
            PageHelper.SendFileDataToClient(userData.ToString(), outFileName);
        }
    }
 
    public bool IsReusable {
        get {
            return true;
        }
    }

}