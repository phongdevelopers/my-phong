<%@ WebHandler Language="C#" Class="ExportErrorLog" %>

using System;
using System.Web;
using System.Text;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

public class ExportErrorLog : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        ErrorMessageCollection errorMessages = ErrorMessageDataSource.LoadForStore();

        if (errorMessages.Count > 0)
        {
            StringBuilder errorData = new StringBuilder();
            //errorData.Append("Date\tSeverity\tMessage\tDebug Data\r\n");
            foreach (ErrorMessage message in errorMessages)
            {
                errorData.Append(string.Format("{0}\t{1}\t{2}\t{3}\r\n", message.EntryDate.ToString(),message.MessageSeverity.ToString(),message.Text,message.DebugData));
            }
            string outFileName = "ErrorLog.log";
            PageHelper.SendFileDataToClient(errorData.ToString(), outFileName);            
        }        
    }
 
    public bool IsReusable {
        get {
            return true;
        }
    }
}
