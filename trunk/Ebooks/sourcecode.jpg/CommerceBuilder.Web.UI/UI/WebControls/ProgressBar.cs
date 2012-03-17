using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace CommerceBuilder.Web.UI.WebControls
{
    [DefaultProperty("Value")]
    [ToolboxData("<{0}:ProgressBar runat=server></{0}:ProgressBar>")]
    public class ProgressBar : WebControl
    {
        public ProgressBar()
            : base(HtmlTextWriterTag.Input)
        {

        }

        private Int32 minimum = 0;
        private Int32 maximum = 100;
        private Int32 value = 0;
        private Color progressBarColor = Color.Red;
        private Color backgroundColor = Color.Gray;

        private string progressText = "";

        [DefaultValue(typeof(Color), "Color.Gray")]
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }

        [DefaultValue(typeof(Color), "Color.Red")]
        public Color ProgressBarColor
        {
            get { return progressBarColor; }
            set { progressBarColor = value; }
        }

        [DefaultValue("0")]
        public Int32 Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        [DefaultValue("0")]
        public Int32 Minimum
        {
            get { return minimum; }
            set { minimum = value; }
        }

        [DefaultValue("100")]
        public Int32 Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }

        public string ProgressText
        {
            get
            { return progressText; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    progressText = "";
                }
                else
                {
                    progressText = value;
                }

            }

        }


        protected override void Render(HtmlTextWriter output)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table");
            sb.Append(" id = \"");
            sb.Append(this.ID.Trim());
            sb.Append("\" height=");
            sb.Append(Height);
            sb.Append(" border=0 width=");
            sb.Append(Width);
            sb.Append(" bgcolor=");
            sb.Append(System.Drawing.ColorTranslator.ToHtml(backgroundColor));
            sb.Append("><tr><td bgcolor=");
            sb.Append(System.Drawing.ColorTranslator.ToHtml(progressBarColor));
            sb.Append(" width = ");
            sb.Append(Width.Value * Value / (Maximum - Minimum));
            sb.Append(" id = \"");
            sb.Append(this.ID.Trim());
            sb.Append("Done\"");
            sb.Append("></td><td width=");
            sb.Append(Width.Value * (1 - Value / (Maximum - Minimum)));
            sb.Append(" id = \"");
            sb.Append(this.ID.Trim());
            sb.Append("Left\"");
            sb.Append("></td></tr>");
            sb.Append("<tr><td colspan=\"2\" align=\"center\">");
            sb.Append(progressText);
            sb.Append("<td></tr>");
            sb.Append("</table>");

            output.Write(sb.ToString());
        }
    }
}