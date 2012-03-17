using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.UI;

namespace CommerceBuilder.Web.UI.WebControls.WebParts
{
    public class WebPartZone : System.Web.UI.WebControls.WebParts.WebPartZone
    {
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            //GET THE OUTPUT OF THE BASE RENDER
            StringWriter customWriter = new StringWriter();
            HtmlTextWriter localwriter = new HtmlTextWriter(customWriter);
            base.Render(localwriter);
            string baseOutput = customWriter.ToString();
            baseOutput = baseOutput.Replace(" style=\"padding:5px;\"", string.Empty);
            baseOutput = baseOutput.Replace("cellspacing=\"0\" cellpadding=\"2\" border=\"0\"", "cellspacing=\"0\" cellpadding=\"0\" border=\"0\"");
            baseOutput = baseOutput.Replace("<tr>\r\n				<td style=\"padding:0;height:100%;\"></td>\r\n			</tr>", "");
            //WRITE OUT THE MODIFIED OUTPUT
            writer.Write(baseOutput);
        }
    }
}
