using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace CommerceBuilder.Web.UI.WebControls.WebParts
{
    public class ScriptletZone : System.Web.UI.WebControls.WebParts.WebPartZone
    {
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            //ONLY RENDER THE CONTENTS OF THE ZONE (THE SCRIPTLETS)
            //DO NOT RENDER TITLE BARS, ETC.
            StringWriter customWriter = new StringWriter();
            HtmlTextWriter localwriter = new HtmlTextWriter(customWriter);
            foreach (WebPart o in base.WebParts)
            {
                o.RenderControl(localwriter);
            }
            string baseOutput = customWriter.ToString();
            writer.Write(baseOutput);
        }
    }
}
