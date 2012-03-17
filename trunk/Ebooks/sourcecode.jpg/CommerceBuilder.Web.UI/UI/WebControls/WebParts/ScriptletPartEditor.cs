namespace CommerceBuilder.Web.UI.WebControls.WebParts
{
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.IO;
    using CommerceBuilder.Common;
    using CommerceBuilder.Stores;

    public class ScriptletPartEditor : EditorPart
    {
        private DropDownList _LayoutChoices;
        private ImageButton _EditLayout;
        private DropDownList _ContentChoices;
        private ImageButton _EditContent;
        private DropDownList _LeftSidebarChoices;
        private ImageButton _EditLeftSidebar;
        private DropDownList _RightSidebarChoices;
        private ImageButton _EditRightSidebar;
        private DropDownList _HeaderChoices;
        private ImageButton _EditHeader;
        private DropDownList _FooterChoices;
        private ImageButton _EditFooter;

        public ScriptletPartEditor()
        {
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();

            string pageTheme = this.Page.Theme;

            _LayoutChoices = new DropDownList();
            _LayoutChoices.Items.Add(new ListItem("- choose layout -", string.Empty));
            _LayoutChoices.AppendDataBoundItems = true;
            _LayoutChoices.DataTextField = "Identifier";
            _LayoutChoices.DataValueField = "Identifier";
            _LayoutChoices.DataSource = ScriptletDataSource.CacheLoad(pageTheme, ScriptletType.Layout, "Identifier");
            _LayoutChoices.DataBind();
            _LayoutChoices.EnableViewState = false;
            this.Controls.Add(_LayoutChoices);

            _EditLayout = new ImageButton();
            _EditLayout.SkinID = "EditIcon";
            _EditLayout.AlternateText = "Edit";
            _EditLayout.ImageAlign = ImageAlign.Top;
            //_EditLayout.OnClientClick = "return editScriptlet(" + _LayoutChoices.ClientID + ",\"layout\");";
            _EditLayout.EnableViewState = false;
            _EditLayout.Click += new ImageClickEventHandler(_EditLayout_Click);
            this.Controls.Add(_EditLayout);

            _ContentChoices = new DropDownList();
            _ContentChoices.Items.Add(new ListItem("- choose content -", string.Empty));
            _ContentChoices.AppendDataBoundItems = true;
            _ContentChoices.DataTextField = "Identifier";
            _ContentChoices.DataValueField = "Identifier";
            _ContentChoices.DataSource = ScriptletDataSource.CacheLoad(pageTheme, ScriptletType.Content, "Identifier");
            _ContentChoices.DataBind();
            _ContentChoices.EnableViewState = false;
            this.Controls.Add(_ContentChoices);

            _EditContent = new ImageButton();
            _EditContent.SkinID = "EditIcon";
            _EditContent.AlternateText = "Edit";
            _EditContent.ImageAlign = ImageAlign.Top;
            //_EditContent.OnClientClick = "return editScriptlet(" + _ContentChoices.ClientID + ",\"content\");";
            _EditContent.Click += new ImageClickEventHandler(_EditContent_Click);
            _EditContent.EnableViewState = false;
            this.Controls.Add(_EditContent);

            _HeaderChoices = new DropDownList();
            _HeaderChoices.Items.Add(new ListItem("- choose header -", string.Empty));
            _HeaderChoices.AppendDataBoundItems = true;
            _HeaderChoices.DataTextField = "Identifier";
            _HeaderChoices.DataValueField = "Identifier";
            _HeaderChoices.DataSource = ScriptletDataSource.CacheLoad(pageTheme, ScriptletType.Header, "Identifier");
            _HeaderChoices.DataBind();
            _HeaderChoices.EnableViewState = false;
            this.Controls.Add(_HeaderChoices);

            _EditHeader = new ImageButton();
            _EditHeader.SkinID = "EditIcon";
            _EditHeader.AlternateText = "Edit";
            _EditHeader.ImageAlign = ImageAlign.Top;
            _EditHeader.Click += new ImageClickEventHandler(_EditHeader_Click);
            _EditHeader.EnableViewState = false;
            this.Controls.Add(_EditHeader);

            _LeftSidebarChoices = new DropDownList();
            _LeftSidebarChoices.Items.Add(new ListItem("- choose left sidebar -", string.Empty));
            _LeftSidebarChoices.AppendDataBoundItems = true;
            _LeftSidebarChoices.DataTextField = "Identifier";
            _LeftSidebarChoices.DataValueField = "Identifier";
            _LeftSidebarChoices.DataSource = ScriptletDataSource.CacheLoad(pageTheme, ScriptletType.Sidebar, "Identifier");
            _LeftSidebarChoices.DataBind();
            _LeftSidebarChoices.EnableViewState = false;
            this.Controls.Add(_LeftSidebarChoices);

            _EditLeftSidebar = new ImageButton();
            _EditLeftSidebar.SkinID = "EditIcon";
            _EditLeftSidebar.AlternateText = "Edit";
            _EditLeftSidebar.ImageAlign = ImageAlign.Top;
            //_EditLeftSidebar.OnClientClick = "return editScriptlet(" + _LeftSidebarChoices.ClientID + ",\"sidebar\");";
            _EditLeftSidebar.Click += new ImageClickEventHandler(_EditLeftSidebar_Click);
            _EditLeftSidebar.EnableViewState = false;
            this.Controls.Add(_EditLeftSidebar);

            _RightSidebarChoices = new DropDownList();
            _RightSidebarChoices.Items.Add(new ListItem("- choose right sidebar -", string.Empty));
            _RightSidebarChoices.AppendDataBoundItems = true;
            _RightSidebarChoices.DataTextField = "Identifier";
            _RightSidebarChoices.DataValueField = "Identifier";
            _RightSidebarChoices.DataSource = ScriptletDataSource.CacheLoad(pageTheme, ScriptletType.Sidebar, "Identifier");
            _RightSidebarChoices.DataBind();
            _RightSidebarChoices.EnableViewState = false;
            this.Controls.Add(_RightSidebarChoices);

            _EditRightSidebar = new ImageButton();
            _EditRightSidebar.SkinID = "EditIcon";
            _EditRightSidebar.AlternateText = "Edit";
            _EditRightSidebar.ImageAlign = ImageAlign.Top;
            //_EditRightSidebar.OnClientClick = "return editScriptlet(" + _RightSidebarChoices.ClientID + ",\"sidebar\");";
            _EditRightSidebar.Click += new ImageClickEventHandler(_EditRightSidebar_Click);
            _EditRightSidebar.EnableViewState = false;
            this.Controls.Add(_EditRightSidebar);

            _FooterChoices = new DropDownList();
            _FooterChoices.Items.Add(new ListItem("- choose footer -", string.Empty));
            _FooterChoices.AppendDataBoundItems = true;
            _FooterChoices.DataTextField = "Identifier";
            _FooterChoices.DataValueField = "Identifier";
            _FooterChoices.DataSource = ScriptletDataSource.CacheLoad(pageTheme, ScriptletType.Footer, "Identifier"); 
            _FooterChoices.DataBind();
            _FooterChoices.EnableViewState = false;
            this.Controls.Add(_FooterChoices);

            _EditFooter = new ImageButton();
            _EditFooter.SkinID = "EditIcon";
            _EditFooter.AlternateText = "Edit";
            _EditFooter.ImageAlign = ImageAlign.Top;
            _EditFooter.Click += new ImageClickEventHandler(_EditFooter_Click);
            _EditFooter.EnableViewState = false;
            this.Controls.Add(_EditFooter);
        }

        void _EditFooter_Click(object sender, ImageClickEventArgs e)
        {
            HandleEdit(_FooterChoices, ScriptletType.Footer);
        }

        void _EditHeader_Click(object sender, ImageClickEventArgs e)
        {
            HandleEdit(_HeaderChoices, ScriptletType.Header);
        }

        void _EditRightSidebar_Click(object sender, ImageClickEventArgs e)
        {
            HandleEdit(_RightSidebarChoices, ScriptletType.Sidebar);
        }

        void _EditLeftSidebar_Click(object sender, ImageClickEventArgs e)
        {
            HandleEdit(_LeftSidebarChoices, ScriptletType.Sidebar);
        }

        void _EditContent_Click(object sender, ImageClickEventArgs e)
        {
            HandleEdit(_ContentChoices, ScriptletType.Content);
        }

        void _EditLayout_Click(object sender, ImageClickEventArgs e)
        {
            HandleEdit(_LayoutChoices, ScriptletType.Layout);
        }

        private void HandleEdit(DropDownList choices, ScriptletType scriptletType)
        {
            Control c = RecursiveFindControl(this.Page, "EditScriptlet");
            if (c != null)
            {
                Scriptlet s = ScriptletDataSource.Load(this.Page.Theme, choices.SelectedValue, scriptletType);
                if (s != null)
                {
                    System.Reflection.PropertyInfo i = c.GetType().GetProperty("Identifier");
                    i.SetValue(c, s.Identifier, null);
                    System.Reflection.PropertyInfo t = c.GetType().GetProperty("ScriptletType");
                    t.SetValue(c, s.ScriptletType, null);
                    //we do not want the edit-mode section to disappear when user clicked on
                    //the edit icon without selecting any valid scriptlet in the dropdown
                    c = RecursiveFindControl(this.Page, "phEditor");
                    if (c != null)
                    {
                        c.Visible = false;
                    }
                }
            }
        }

        private static Control RecursiveFindControl(Control parent, string controlId)
        {
            if (parent.ID == controlId) return parent;
            if (parent.HasControls())
            {
                Control find = null;
                int i = 0;
                while ((find == null) && (i < parent.Controls.Count))
                {
                    find = RecursiveFindControl(parent.Controls[i], controlId);
                    i++;
                }
                return find;
            }
            return null;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            ScriptletPart displayPart = this.WebPartToEdit as ScriptletPart;
            if (displayPart != null)
            {
                this.Title = "Edit " + displayPart.Title;
                ListItem selected = _LayoutChoices.Items.FindByValue(displayPart.Layout);
                if (selected != null) _LayoutChoices.SelectedIndex = _LayoutChoices.Items.IndexOf(selected);
                selected = _HeaderChoices.Items.FindByValue(displayPart.Header);
                if (selected != null) _HeaderChoices.SelectedIndex = _HeaderChoices.Items.IndexOf(selected);
                selected = _LeftSidebarChoices.Items.FindByValue(displayPart.LeftSidebar);
                if (selected != null) _LeftSidebarChoices.SelectedIndex = _LeftSidebarChoices.Items.IndexOf(selected);
                selected = _ContentChoices.Items.FindByValue(displayPart.Content);
                if (selected != null) _ContentChoices.SelectedIndex = _ContentChoices.Items.IndexOf(selected);
                selected = _RightSidebarChoices.Items.FindByValue(displayPart.RightSidebar);
                if (selected != null) _RightSidebarChoices.SelectedIndex = _RightSidebarChoices.Items.IndexOf(selected);
                selected = _FooterChoices.Items.FindByValue(displayPart.Footer);
                if (selected != null) _FooterChoices.SelectedIndex = _FooterChoices.Items.IndexOf(selected);
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            //OPEN TABLE
            writer.Write("<table border=0 cellspacing=6>");
            //INSTRUCTIONS
            writer.Write("<tr><td colspan=4>Select the layout and content scriptlets to be displayed.  Not all layouts will make use of every element.</td></tr>");
            //LAYOUT AND CONTENT
            writer.Write("<tr><th align=right>Active Layout: </th><td>");
            this._LayoutChoices.RenderControl(writer);
            writer.Write("&nbsp;");
            this._EditLayout.RenderControl(writer);
            writer.Write("</td><th align=right>Content: </th><td>");
            this._ContentChoices.RenderControl(writer);
            writer.Write("&nbsp;");
            this._EditContent.RenderControl(writer);
            writer.Write("</td></tr>");
            //HEADER AND FOOTER
            writer.Write("<tr><th align=right>Header: </th><td>");
            this._HeaderChoices.RenderControl(writer);
            writer.Write("&nbsp;");
            this._EditHeader.RenderControl(writer);
            writer.Write("</td>");
            writer.Write("<th align=right>Footer: </th><td>");
            this._FooterChoices.RenderControl(writer);
            writer.Write("&nbsp;");
            this._EditFooter.RenderControl(writer);
            writer.Write("</td></tr>");
            //Left SIDEBAR AND Right SIDEBAR
            writer.Write("<tr><th align=right>Left Sidebar: </th><td>");
            this._LeftSidebarChoices.RenderControl(writer);
            writer.Write("&nbsp;");
            this._EditLeftSidebar.RenderControl(writer);
            writer.Write("</td>");
            writer.Write("<th align=right>Right Sidebar: </th><td>");
            this._RightSidebarChoices.RenderControl(writer);
            writer.Write("&nbsp;");
            this._EditRightSidebar.RenderControl(writer);
            
            writer.Write("</td></tr>");
            //CLOSE TABLE
            writer.Write("</table>");
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            ScriptletPart displayPart = WebPartToEdit as ScriptletPart;
            if (displayPart != null)
            {
                displayPart.Layout = _LayoutChoices.SelectedValue;
                displayPart.Header = _HeaderChoices.SelectedValue;
                displayPart.LeftSidebar = _LeftSidebarChoices.SelectedValue;
                displayPart.Content = _ContentChoices.SelectedValue;
                displayPart.RightSidebar = _RightSidebarChoices.SelectedValue;
                displayPart.Footer = _FooterChoices.SelectedValue;
                //recreate control collection for changes
                displayPart.InternalCreateChildControls();
            }
            // ATTEMPT TO RESET CACHE FOR ONE PAGE CHECKOUT INDICATOR
            WebflowManager.ClearCache();
            return true;
        }
    }
}
