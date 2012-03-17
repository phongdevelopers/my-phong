using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommerceBuilder.Utility;

public partial class Admin_Dashboard_DigitalClock : System.Web.UI.UserControl
{
    private void BuildScript(DateTime localNow)
    {
        StringBuilder script = new StringBuilder();

        script.Append("var serverHourOffset;\r\n");
        script.Append("function startClock(){\r\n");
        script.Append(" var cHours = (new Date()).getHours()\r\n");
        script.Append(" var sHours = " + localNow.Hour.ToString() + ";\r\n");
        script.Append(" serverHourOffset = (sHours - cHours);\r\n");
        script.Append(" showTime();\r\n");
        script.Append("}\r\n");
        script.Append("function showTime() {\r\n");
        script.Append(" var now = new Date()\r\n");
        script.Append(" var cHours = now.getHours()\r\n");
        script.Append(" var sHours = cHours + serverHourOffset\r\n");
        script.Append(" if (sHours > 24) sHours -= 12;\r\n");
        script.Append(" var minutes = now.getMinutes()\r\n");
        script.Append(" var seconds = now.getSeconds()\r\n");
        script.Append(" var cTimeValue = '' + ((cHours > 12) ? cHours - 12 : cHours)\r\n");
        script.Append(" cTimeValue  += ((minutes < 10) ? ':0' : ':') + minutes\r\n");
        script.Append(" cTimeValue  += ((seconds < 10) ? ':0' : ':') + seconds\r\n");
        script.Append(" cTimeValue  += (cHours >= 12) ? ' PM' : ' AM'\r\n");
        script.Append(" var cTimer = document.getElementById('" + ClientTime.ClientID + "');\r\n");
        script.Append(" cTimer.innerText = cTimeValue;\r\n");
        script.Append(" var sTimeValue = '' + ((sHours > 12) ? sHours - 12 : sHours)\r\n");
        script.Append(" sTimeValue  += ((minutes < 10) ? ':0' : ':') + minutes\r\n");
        script.Append(" sTimeValue  += ((seconds < 10) ? ':0' : ':') + seconds\r\n");
        script.Append(" sTimeValue  += (sHours >= 12) ? ' PM' : ' AM'\r\n");
        script.Append(" var sTimer = document.getElementById('" + ServerTime.ClientID + "');\r\n");
        script.Append(" sTimer.innerText = sTimeValue;\r\n");
        script.Append(" setTimeout('showTime()',1000);\r\n");
        script.Append("}\r\n");
        script.Append("startClock();\r\n");
        Page.ClientScript.RegisterStartupScript(this.GetType(), "showTime", script.ToString(), true);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        DateTime localNow = LocaleHelper.LocalNow;
        ServerTime.Text = localNow.ToString("h:mm:ss tt");
        BuildScript(localNow);
    }
}
