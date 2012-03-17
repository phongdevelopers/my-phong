namespace CommerceBuilder.Web.UI.WebControls.WebParts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using CommerceBuilder.Catalog;
    using CommerceBuilder.Common;
    using CommerceBuilder.Stores;
    using CommerceBuilder.Products;
    using CommerceBuilder.Messaging;
    using CommerceBuilder.Utility;

    public class ScriptletPartEx : WebPart
    {
        public ScriptletPartEx()
        {
            this.ID = "ScriptletPart1";
            this.Title = "Scriptlet Part";
            this.Description = "Displays scriptlet Identifier specified via the merchant administration.";
        }

        private string _Identifier = string.Empty;
        private string _ScriptletType = "Content";

        [Personalizable()]
        public string Identifier
        {
            get { return _Identifier; }
            set { _Identifier = value; }
        }

        [Personalizable()]
        public string ScriptletType
        {
            get { return _ScriptletType; }
            set { _ScriptletType = value; }
        }

        public override object WebBrowsableObject
        {
            get { return this; }
        }

        internal void InternalCreateChildControls()
        {
            this.Controls.Clear();
            CreateChildControls();
        }

        protected override void CreateChildControls()
        {
            bool created = false;
            Scriptlet Identifier = ScriptletDataSource.Load(this.Page.Theme, this.Identifier, (ScriptletType)Enum.Parse(typeof(ScriptletType), this.ScriptletType), true);
            string IdentifierScriptletData = string.Empty;
            if (Identifier != null)
            {
                if (!string.IsNullOrEmpty(Identifier.HeaderData)) this.Page.Header.Controls.Add(new LiteralControl(Identifier.HeaderData));
                IdentifierScriptletData = Identifier.ScriptletData;
            }
            string template = IdentifierScriptletData;
            //PROCESS THE MERGED NVELOCITY TEMPLATE
            System.Collections.Hashtable parameters = new System.Collections.Hashtable();
            object store = Token.Instance.Store;
            object customer = Token.Instance.User;
            parameters.Add("store", store);
            parameters.Add("customer", customer);
            //TODO: OBSOLETE PARAMETERS, REMOVE FOR AC7.1
            parameters.Add("Store", store);
            parameters.Add("User", customer);

            //CHECK FOR CATEGORY
            int categoryId = GetCategoryId();
            Category category = CategoryDataSource.Load(categoryId);
            if (category != null) parameters.Add("Category", category);

            //CHECK FOR PRODUCT
            int productId = GetProductId();
            Product product = ProductDataSource.Load(productId);
            if (product != null) parameters.Add("Product", product);

            //CHECK FOR WEBPAGE
            int webpageId = GetWebpageId();
            Webpage webpage = WebpageDataSource.Load(webpageId);
            if (webpage != null) parameters.Add("Webpage", webpage);

            //CHECK FOR LINK
            int linkId = GetLinkId();
            Link link = LinkDataSource.Load(linkId);
            if (link != null) parameters.Add("Link", link);

            string result = NVelocityEngine.Instance.Process(parameters, template);
            if (this.Page != null)
            {
                string baseUrl = this.Page.ResolveUrl("~");
                result = result.Replace("href=\"~/", "href=\"" + baseUrl);
                result = result.Replace("src=\"~/", "src=\"" + baseUrl);
                //DISABLE THE ABILITY TO REFER TO ANY USER CONTROL
                //MatchCollection userControls = Regex.Matches(result, "\\[\\[(ConLib|UserControl):([^\\]]+)\\]\\]", RegexOptions.IgnoreCase);
                MatchCollection userControls = Regex.Matches(result, "\\[\\[(ConLib):([^\\]]+)\\]\\]", RegexOptions.IgnoreCase);
                if (userControls.Count > 0)
                {
                    int currentIndex = 0;
                    foreach (Match match in userControls)
                    {
                        //output the literal content up to the match index
                        if (currentIndex < match.Index)
                        {
                            this.Controls.Add(new LiteralControl(result.Substring(currentIndex, (match.Index - currentIndex))));
                            currentIndex = match.Index;
                        }
                        string controlPath;
                        Dictionary<string, string> attributes;
                        ParseControlPath(match.Groups[2].Value.Trim(), out controlPath, out attributes);
                        if (match.Groups[1].Value.ToLowerInvariant() == "conlib") controlPath = "~/ConLib/" + controlPath + ".ascx";
                        //LOAD AND OUTPUT THE CONTROL
                        try
                        {
                            Control control = this.Page.LoadControl(controlPath);
                            if (control != null)
                            {
                                //TRY TO SET PARAMS
                                foreach (string propName in attributes.Keys)
                                {
                                    //CHECK WHETHER PROPERTY EXISTS
                                    System.Reflection.PropertyInfo pi = control.GetType().GetProperty(propName);
                                    if (pi != null)
                                    {
                                        //SET THE PROPERTY WITH THE GIVEN VALUE
                                        object value = Convert.ChangeType(attributes[propName], pi.PropertyType);
                                        pi.SetValue(control, value, null);
                                    }
                                }
                                //ADD CONTROL TO THE COLLECTION
                                this.Controls.Add(control);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Controls.Add(new LiteralControl(match.Value + " " + ex.Message));
                        }
                        //advance the current index
                        currentIndex += match.Length;
                    }
                    //output any remaining literal content
                    if (currentIndex < result.Length)
                    {
                        this.Controls.Add(new LiteralControl(result.Substring(currentIndex)));
                    }
                    created = true;
                }
            }
            if (!created) this.Controls.Add(new LiteralControl(result));
        }

        private void ParseControlPath(string input, out string controlPath, out Dictionary<string, string> attributes)
        {
            attributes = new Dictionary<string, string>();
            int firstSpaceIndex = input.IndexOf(" ");
            if (firstSpaceIndex > 0)
            {
                controlPath = input.Substring(0, firstSpaceIndex);
                string attributeData = input.Substring(firstSpaceIndex);
                string pattern = @" ([A-Za-z0-9]+)=""([^""]*)""";
                MatchCollection attributeTokens = Regex.Matches(attributeData, pattern);
                foreach (Match m in attributeTokens)
                {
                    attributes.Add(m.Groups[1].Value, m.Groups[2].Value);
                }
            }
            else controlPath = input;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            EnsureChildControls();
            base.Render(writer);
        }

        public override EditorPartCollection CreateEditorParts()
        {
            List<EditorPart> customEditorPartCollection = new List<EditorPart>();
            ScriptletPartExEditor o = new ScriptletPartExEditor();
            o.ID = "ScriptletPartExEditor1";
            customEditorPartCollection.Add(o);
            EditorPartCollection EditorPartCollection = new EditorPartCollection(customEditorPartCollection);
            return EditorPartCollection;
        }

        /// <summary>
        /// Determines the category context for the current request.
        /// </summary>
        /// <returns>The category context for the current request.</returns>
        private static int GetCategoryId()
        {
            HttpRequest request = HttpContext.Current.Request;
            int categoryId = AlwaysConvert.ToInt(request.QueryString["CategoryId"]);
            if (categoryId != 0) return categoryId;
            int productId = GetProductId();
            if (productId != 0)
            {
                categoryId = CatalogDataSource.GetCategoryId(productId, CatalogNodeType.Product);
                if (categoryId != 0) return categoryId;
            }
            int webpageId = GetWebpageId();
            if (webpageId != 0)
            {
                categoryId = CatalogDataSource.GetCategoryId(webpageId, CatalogNodeType.Webpage);
                if (categoryId != 0) return categoryId;
            }
            int linkId = GetLinkId();
            if (linkId != 0)
            {
                categoryId = CatalogDataSource.GetCategoryId(linkId, CatalogNodeType.Link);
                if (categoryId != 0) return categoryId;
            }
            return 0;
        }

        /// <summary>
        /// Determines the product context for the current request.
        /// </summary>
        /// <returns>The product context for the current request.</returns>
        private static int GetProductId()
        {
            return AlwaysConvert.ToInt(HttpContext.Current.Request.QueryString["ProductId"]);
        }

        /// <summary>
        /// Determines the webpage context for the current request.
        /// </summary>
        /// <returns>The webpage context for the current reuqest.</returns>
        private static int GetWebpageId()
        {
            return AlwaysConvert.ToInt(HttpContext.Current.Request.QueryString["WebpageId"]);
        }

        /// <summary>
        /// Determines the link context for the current request.
        /// </summary>
        /// <returns>The link context for the current reuqest.</returns>
        private static int GetLinkId()
        {
            return AlwaysConvert.ToInt(HttpContext.Current.Request.QueryString["LinkId"]);
        }
    }
}