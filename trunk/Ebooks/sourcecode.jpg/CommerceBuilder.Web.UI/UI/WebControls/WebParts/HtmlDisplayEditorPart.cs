namespace CommerceBuilder.Web.UI.WebControls.WebParts
{
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    
    public class HtmlDisplayEditorPart : EditorPart
    {

        private TextBox _ContentEditor;
        private bool _DisplayErrorMessage = false;

        public HtmlDisplayEditorPart()
        {
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();
            _ContentEditor = new TextBox();
            _ContentEditor.TextMode = TextBoxMode.MultiLine;
            _ContentEditor.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            _ContentEditor.Height = System.Web.UI.WebControls.Unit.Parse("300px");
            this.Controls.Add(_ContentEditor);
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            HtmlDisplayWebPart displayPart;
            displayPart = WebPartToEdit as HtmlDisplayWebPart;
            if (displayPart !=  null)
            {
                this.Title = "Edit " + displayPart.Title;
                _ContentEditor.Text = displayPart.ContentText;
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "4");
            writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("Content:");
            writer.WriteBreak();
            this._ContentEditor.RenderControl(writer);
            if (_DisplayErrorMessage)
            {
                writer.WriteBreak();
                Label errorMessage = new Label();
                errorMessage.Text = "Error converting type";
                errorMessage.ApplyStyle(this.Zone.ErrorStyle);
                errorMessage.RenderControl(writer);
            }
            writer.RenderEndTag();
            //  Td
            writer.RenderEndTag();
            //  Tr
            writer.RenderEndTag();
            //  Table
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            HtmlDisplayWebPart displayPart;
            displayPart = WebPartToEdit as HtmlDisplayWebPart;
            if (displayPart != null)
            {
                try
                {
                    displayPart.ContentText = _ContentEditor.Text;
                }
                catch
                {
                    _DisplayErrorMessage = true;
                    return false;
                }
            }
            return true;
        }
    }
}
