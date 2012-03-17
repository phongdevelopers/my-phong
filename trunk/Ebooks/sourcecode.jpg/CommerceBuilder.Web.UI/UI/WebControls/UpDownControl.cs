using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Web.UI.WebControls
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [ToolboxData("<{0}:UpDownControl Runat=server>/>")]
    [Designer(typeof(UpDownControl))]
    public class UpDownControl : TextBox
    {
        public enum ArrowStyleType
        {
            Right, Left
        }

        private string _UpImageUrl = string.Empty;
        private string _DownImageUrl = string.Empty;
        private ArrowStyleType _ArrowStyle = ArrowStyleType.Right;
        private int _MaxValue = 1000;
        private int _MinValue = 1;
        private bool _TextValueSet = false;
        private bool _HideZero = false;

        
        [Bindable(true), Category("Appearance"), DefaultValue(""), Description("The value of the input field.")]
        public override string Text
        {
            get
            {
                if (!_TextValueSet)
                {
                    this.InitText(string.Empty);
                    _TextValueSet = true;
                }
                return base.Text;
            }
            set
            {
                base.Text = value;
                _TextValueSet = true;
            }
        }

        [Bindable(true), Category("Appearance"), DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor)), Description("The URL of the image to be shown for the up arrow.")]
        public string UpImageUrl
        {
            get { return this._UpImageUrl; }
            set { this._UpImageUrl = value; }
        }

        [Bindable(true), Category("Appearance"), DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor)), Description("The URL of the image to be shown for the down arrow.")]
        public string DownImageUrl
        {
            get
            {
                return this._DownImageUrl;
            }
            set
            {
                this._DownImageUrl = value;
            }
        }

        [Category("Layout"), Description("Style for the display of arrow controls.")]
        public ArrowStyleType ArrowStyle
        {
            get
            {
                return this._ArrowStyle;
            }
            set
            {
                this._ArrowStyle = value;
            }
        }

        [Category("Behavior"), Description("The maximum value for the input field.")]
        public int MaxValue
        {
            get
            {
                return this._MaxValue;
            }
            set
            {
                this._MaxValue = value;
            }
        }

        [Category("Behavior"), Description("The minimum value for the input field.")]
        public int MinValue
        {
            get
            {
                return this._MinValue;
            }
            set
            {
                this._MinValue = value;
            }
        }

        [Category("Behavior"), Description("Indicates whether a zero value should be displayed as an empty string.")]
        public bool HideZero
        {
            get
            {
                return this._HideZero;
            }
            set
            {
                this._HideZero = value;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //BUILD THE CONTROL
            StringBuilder strRender = new StringBuilder();
            strRender.Append("<table name=\"" + this.UniqueID + "_Table\" id=\"" + this.ClientID + "_Table\"");
            strRender.Append(" CellPadding=\"0\" CellSpacing=\"0\"");
            if (!string.IsNullOrEmpty(this.CssClass))
            {
                strRender.Append(" class=\"" + this.CssClass + "\"");
                this.CssClass = string.Empty;
            }
            else
            {
                strRender.Append(" style=\"display:inline;padding:0px;margin:0px\"");
            }
            if (this.Height.Value > 0)
            {
                strRender.Append(" height=\"" + this.Height.ToString() + "\"");
                //this.Height = new System.Web.UI.WebControls.Unit(0);
            }
            if (this.Width.Value > 0)
            {
                strRender.Append(" width=\"" + this.Width.Value.ToString() + "\"");
                //this.Width = new System.Web.UI.WebControls.Unit(0);
            }
            strRender.Append("><tr>\r\n");
            if (this.ArrowStyle == ArrowStyleType.Left)
            {
                strRender.Append("<td valign=\"middle\">" + RenderUpArrow() + "<br /> " + RenderDownArrow() + "</td>");
                strRender.Append("<td>" + RenderTextBox() + "</td>\r\n");
            }
            else
            {
                strRender.Append("<td>" + RenderTextBox() + "</td>\r\n");
                strRender.Append("<td valign=\"middle\">" + RenderUpArrow() + "<br /> " + RenderDownArrow() + "</td>");
                //strRender.Append("<td rowSpan=\"2\">" + RenderTextBox() + "</td>");
                //strRender.Append("<td><table cellpadding=\"0\" cellspacing=\"0\"><tr><td valign=\"bottom\">" + RenderUpArrow() + "<td></tr>\r\n<tr><td valign=\"top\">" + RenderDownArrow() + "</td></tr></table></td>\r\n");
            }
            strRender.Append("</tr></table>\r\n");
            //SEND CONTROL TO OUTPUT
            writer.Write(strRender.ToString());
        }

        private string RenderUpArrow()
        {
            string arrowHtml;
            if (!string.IsNullOrEmpty(this.UpImageUrl))
            {
                arrowHtml = "<img title=\"More\" alt=\"More\" hspace=\"2\" vspace=\"1\" style=\"border:0px;cursor:pointer\" src=\"" + this.Page.ResolveUrl(this.UpImageUrl) + "\" onclick=\"javascript:NPB_UpDown_Click('" + this.ClientID + "'," + this.MinValue + "," + this.MaxValue + ",1," + this.HideZero.ToString().ToLowerInvariant() + ")\">";
            }
            else
            {
                arrowHtml = "<input type=\"button\" title=\"More\" value=\"+\" onclick=\"javascript:NPB_UpDown_Click('" + this.ClientID + "'," + this.MinValue + "," + this.MaxValue + ",1," + this.HideZero.ToString().ToLowerInvariant() + ")\">";
            }
            return arrowHtml;
        }
        private string RenderDownArrow()
        {
            string arrowHtml;
            if (!string.IsNullOrEmpty(this.DownImageUrl))
            {
                arrowHtml = "<img title=\"Less\" alt=\"Less\" hspace=\"2\" vspace=\"1\" style=\"border:0px;cursor:pointer\" src=\"" + this.Page.ResolveUrl(this.DownImageUrl) + "\" onclick=\"javascript:NPB_UpDown_Click('" + this.ClientID + "'," + this.MinValue + "," + this.MaxValue + ",-1," + this.HideZero.ToString().ToLowerInvariant() + ")\">";
            }
            else
            {
                arrowHtml = "<input type=\"button\" title=\"Less\" value=\"-\" onclick=\"javascript:NPB_UpDown_Click('" + this.ClientID + "'," + this.MinValue + "," + this.MaxValue + ",-1," + this.HideZero.ToString().ToLowerInvariant() + ")\">";
            }
            return arrowHtml;
        }

        private string RenderTextBox()
        {
            //GET THE OUTPUT OF THE BASE RENDER
            StringWriter customWriter = new StringWriter();
            HtmlTextWriter localWriter = new HtmlTextWriter(customWriter);
            base.Render(localWriter);
            return customWriter.ToString();
        }

        private string GetErrorMessageHtml(string message)
        {
            return "<div style=\"text-align:center;color:red;font-weight:bold\">" + message + "</div>";
        }

        private void InitText(string uniqueId)
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                if (string.IsNullOrEmpty(uniqueId)) uniqueId = this.UniqueID;
                WebTrace.Write("Init text, using uniqueid: " + this.UniqueID);
                if (string.IsNullOrEmpty(context.Request.Form[uniqueId]))
                {
                    this.Text = "0";
                }
                else
                {
                    this.Text = context.Request.Form[uniqueId];
                }
                WebTrace.Write("text: " + this.Text);
            }
        }

        protected override void OnInit(System.EventArgs e)
        {
            this.Page.ClientScript.RegisterClientScriptBlock(string.Empty.GetType(), "UpDownArrowClick", GetJavaScript());
            base.OnInit(e);
        }


        private string GetJavaScript()
        {
            StringBuilder script = new StringBuilder();
            script.Append("<script language=\"javascript\" type=\"text/javascript\">\r\n");
            script.Append("function NPB_UpDown_Click(sControlID, Min, Max, Increment, hideZero) {\r\n");
            script.Append("	var thisControl = document.getElementById(sControlID);\r\n");
            script.Append("	var thisValue = parseInt(\"0\" + thisControl.value, 10)\r\n");
            script.Append("	if (((thisValue + Increment) >= Min) && ((thisValue + Increment) <= Max))  {\r\n");
            script.Append("     var newValue = (thisValue + Increment);\r\n");
            script.Append("     if (hideZero && newValue == 0) newValue = \"\";\r\n");
            script.Append("		thisControl.value = newValue;\r\n");
            script.Append("	}\r\n");
            script.Append("	else\r\n");
            script.Append("	{\r\n");
            script.Append("	if (((thisValue + Increment) > Max)){\r\n");
            script.Append("	thisControl.value = Max;}\r\n");
            script.Append("	}\r\n");
            script.Append("}\r\n");
            script.Append("</script>");
            return script.ToString();
        }

    }
}
