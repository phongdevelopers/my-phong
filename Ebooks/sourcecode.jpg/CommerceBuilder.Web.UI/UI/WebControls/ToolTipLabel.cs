using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CommerceBuilder.Web.UI.WebControls
{
    public class ToolTipLabel : System.Web.UI.WebControls.WebControl
    {
        private string _LabelCssClass;
        private string _Text;
        private string _AssociatedControlID;
        private string _ToolTipClass = "toolTip";

        public string ToolTipClass
        {
            get { return _ToolTipClass; }
            set { _ToolTipClass = value; }
        }
        
        public string LabelCssClass
        {
            get { return _LabelCssClass; }
            set { _LabelCssClass = value; }
        }

        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        public string AssociatedControlID
        {
            get { return _AssociatedControlID; }
            set { _AssociatedControlID = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.ToolTip.Length > 0)
            {
                HyperLink tooltip = new HyperLink();
                tooltip.ID = Guid.NewGuid().ToString("N");
                tooltip.CssClass = this.ToolTipClass;
                tooltip.NavigateUrl = "#";
                tooltip.ToolTip = this.ToolTip;
                tooltip.Attributes.Add("onclick", "return false;");
                tooltip.TabIndex = -1;
                Label tooltipLabel = new Label();
                tooltipLabel.Text = this.Text;
                tooltipLabel.CssClass = this.LabelCssClass;
                tooltipLabel.AssociatedControlID = this.AssociatedControlID;
                tooltip.Controls.Add(tooltipLabel);
                this.Controls.Add(tooltip);            
            }
            else
            {
                Label tooltipLabel = new Label();
                tooltipLabel.Text = this.Text;
                tooltipLabel.AssociatedControlID = this.AssociatedControlID;
                this.Controls.Add(tooltipLabel);
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            this.RenderChildren(writer);
        }
    }
}