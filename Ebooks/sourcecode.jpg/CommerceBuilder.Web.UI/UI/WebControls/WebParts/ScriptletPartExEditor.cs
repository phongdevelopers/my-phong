namespace CommerceBuilder.Web.UI.WebControls.WebParts
{
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.IO;
    using CommerceBuilder.Common;
    using CommerceBuilder.Stores;

    public class ScriptletPartExEditor : EditorPart
    {
        private DropDownList _ScriptletChoices;

        public ScriptletPartExEditor()
        {
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();
            _ScriptletChoices = new DropDownList();
            _ScriptletChoices.Items.Add(new ListItem("- choose scriptlet -", string.Empty));
            _ScriptletChoices.AppendDataBoundItems = true;
            _ScriptletChoices.DataTextField = "Identifier";
            _ScriptletChoices.DataValueField = "ScriptletType";
            _ScriptletChoices.DataSource = ScriptletDataSource.CacheLoad(this.Page.Theme, ScriptletType.Unspecified, "Identifier");
            _ScriptletChoices.DataBind();
            this.Controls.Add(_ScriptletChoices);
        }

        private int GetSelectedIndex(string identifier, string scriptletType)
        {
            for (int i = 0; i < _ScriptletChoices.Items.Count; i++)
            {
                ListItem thisItem = _ScriptletChoices.Items[i];
                if (thisItem.Text == identifier && thisItem.Value == scriptletType) return i;
            }
            return -1;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            ScriptletPartEx displayPart = this.WebPartToEdit as ScriptletPartEx;
            if (displayPart != null)
            {
                this.Title = "Edit " + displayPart.Title;
                _ScriptletChoices.SelectedIndex = GetSelectedIndex(displayPart.Identifier, displayPart.ScriptletType);
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            //OPEN TABLE
            writer.Write("<table border=0 cellspacing=6>");
            //INSTRUCTIONS
            writer.Write("<tr><td colspan=2>Select the layout scriptlet to be displayed.</td></tr>");
            //LAYOUT AND CONTENT
            writer.Write("<tr><th align=right>Scriptlet: </th><td>");
            this._ScriptletChoices.RenderControl(writer);
            writer.Write("</td></tr>");
            //CLOSE TABLE
            writer.Write("</table>");
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            ScriptletPartEx displayPart = WebPartToEdit as ScriptletPartEx;
            if (displayPart != null)
            {
                displayPart.Identifier = _ScriptletChoices.SelectedItem.Text;
                displayPart.ScriptletType = _ScriptletChoices.SelectedItem.Value;
                //recreate control collection for changes
                displayPart.InternalCreateChildControls();
            }
            return true;
        }
    }
}
