namespace CommerceBuilder.Web.UI.WebControls.WebParts
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;

    public class HtmlDisplayWebPart : WebPart
    {

        //  Fields - Private
        private string _ContentText;

        //  Constructor
        public HtmlDisplayWebPart()
        {
            this.Title = "Custom HTML";
        }

        [Personalizable(true)]
        public string ContentText
        {
            get
            {
                return _ContentText;
            }
            set
            {
                _ContentText = value;
            }
        }

        public bool AllowPageDesign
        {
            get
            {
                if (base.WebPartManager == null)
                {
                    return this.DesignMode;
                }
                return base.WebPartManager.DisplayMode.AllowPageDesign;
            }
        }

        public override object WebBrowsableObject
        {
	        get 
	        {
                return this;
	        }
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write(this.ContentText);
        }

        public override void RenderControl(System.Web.UI.HtmlTextWriter writer)
        {
            if (this.AllowPageDesign)
            {
                base.RenderControl(writer);
            }
            else
            {
                this.RenderContents(writer);
            }
        }

        public override EditorPartCollection CreateEditorParts()
        {
            List<EditorPart> customEditorPartCollection = new List<EditorPart>();
            HtmlDisplayEditorPart o = new HtmlDisplayEditorPart();
            o.ID = "ContentEditor1";
            customEditorPartCollection.Add(o);
            EditorPartCollection EditorPartCollection = new EditorPartCollection(customEditorPartCollection);
            return EditorPartCollection;
        }

    }
}