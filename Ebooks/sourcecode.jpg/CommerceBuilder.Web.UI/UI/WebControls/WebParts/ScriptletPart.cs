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

    public class ScriptletPart : WebPart
    {
        public ScriptletPart()
        {
            this.ID = "ScriptletPart1";
            this.Title = "Scriptlet Part";
            this.Description = "Displays scriptlet content specified via the merchant administration.";

            // WHEN THIS CONTROL IS INITIALIZED, WE WANT TO LINK INTO THE Page.InitComplete EVENT
            this.Init += new EventHandler(ScriptletPart_Init);
        }

        void ScriptletPart_Init(object sender, EventArgs e)
        {
            // MAKE SURE WE CAN ACCESS THE PARENT PAGE
            if (this.Page != null)
            {
                // TACK INTO THE InitComplete EVENT TO ENSURE ALL CHILD CONTROLS 
                // ARE CREATED AFTER PERSONALIZATION IS APPLIED BUT PRIOR TO THE PAGE LOAD EVENT
                this.Page.InitComplete+= new EventHandler(Page_InitComplete);
            }
        }

        void Page_InitComplete(object sender, EventArgs e)
        {
            WebTrace.Write("InitComplete, Ensuring Child Controls for ScriptletPart");
            // MAKE SURE ALL CONTROLS ARE CREATED AFTER PERSONALIZATION IS APPLIED
            // BUT BEFORE THE PAGE LOAD EVENT OCCURS
            this.EnsureChildControls();
        }

        private string _Layout = string.Empty;
        private string _Header = string.Empty;
        private string _LeftSidebar = string.Empty;
        private string _Content = string.Empty;
        private string _RightSidebar = string.Empty;
        private string _Footer = string.Empty;

        [Personalizable()]
        public string Layout
        {
            get { return _Layout; }
            set { _Layout = value; }
        }

        [Personalizable()]
        public string Header
        {
            get { return _Header; }
            set { _Header = value; }
        }

        [Obsolete("Sidebar is deprecated. Use LeftSidebar instead.")]
        [Personalizable()]
        public string Sidebar
        {
            get { return _LeftSidebar; }
            set { _LeftSidebar = value; }
        }

        [Personalizable()]
        public string LeftSidebar
        {
            get { return _LeftSidebar; }
            set { _LeftSidebar = value; }
        }

        [Personalizable()]
        public string Content
        {
            get { return _Content; }
            set { _Content = value; }
        }

        [Obsolete("Sidebar2 is deprecated. Use RightSidebar instead.")]
        [Personalizable()]
        public string Sidebar2
        {
            get { return _RightSidebar; }
            set { _RightSidebar = value; }
        }

        [Personalizable()]
        public string RightSidebar
        {
            get { return _RightSidebar; }
            set { _RightSidebar = value; }
        }

        [Personalizable()]
        public string Footer
        {
            get { return _Footer; }
            set { _Footer = value; }
        }

        public override object WebBrowsableObject
        {
            get { return this; }
        }

        private string GetMergedTemplate()
        {
            Scriptlet layout = ScriptletDataSource.Load(this.Page.Theme, this.Layout, ScriptletType.Layout, true);
            //IF THE DESIRED LAYOUT IS NOT FOUND, RETURN DEFAULT THREE COLUMN LAYOUT
            if (layout == null) return "<table width=100%><tr><td colspan=3>[[layout:header]]</td></tr><tr><td></td><td>[[layout:leftsidebar]]</td><td>[[layout:content]]</td><td>[[layout:rightsidebar]]</td></tr><tr><td colspan=3>[[layout:footer]]</td></tr></table>";
            //CHECK FOR HEADER OPTIONS
            if (!string.IsNullOrEmpty(layout.HeaderData)) this.Page.Header.Controls.Add(new LiteralControl(layout.HeaderData));
            string mergedTemplate = layout.ScriptletData;
            MatchCollection nestedScriptlets = Regex.Matches(mergedTemplate, "\\[\\[(header|footer|sidebar|sidebar2|leftsidebar|rightsidebar|content):([^\\]]+)\\]\\]", RegexOptions.IgnoreCase);
            if (nestedScriptlets.Count > 0)
            {
                StringBuilder mergedTemplateBuilder = new StringBuilder();
                int currentIndex = 0;
                foreach (Match match in nestedScriptlets)
                {
                    //output the literal content up to the match index
                    if (currentIndex < match.Index)
                    {
                        mergedTemplateBuilder.Append(mergedTemplate.Substring(currentIndex, (match.Index - currentIndex)));
                        currentIndex = match.Index;
                    }
                    //include the nested scriptlet
                    ScriptletType nestedType = (ScriptletType)Enum.Parse(typeof(ScriptletType), match.Groups[1].Value);
                    string nestedIdentifier = match.Groups[2].Value.Trim();
                    Scriptlet nestedScriptlet = ScriptletDataSource.Load(this.Page.Theme, nestedIdentifier, nestedType, true);
                    if (nestedScriptlet != null)
                    {
                        if (!string.IsNullOrEmpty(nestedScriptlet.HeaderData)) this.Page.Header.Controls.Add(new LiteralControl(nestedScriptlet.HeaderData));
                        mergedTemplateBuilder.Append(nestedScriptlet.ScriptletData);
                    }
                    //advance the current index
                    currentIndex += match.Length;
                }
                //output any remaining literal content
                if (currentIndex < mergedTemplate.Length)
                {
                    mergedTemplateBuilder.Append(mergedTemplate.Substring(currentIndex));
                }
                mergedTemplate = mergedTemplateBuilder.ToString();
            }
            return mergedTemplate;
        }

        internal void InternalCreateChildControls()
        {
            this.Controls.Clear();
            CreateChildControls();
        }

        protected override void CreateChildControls()
        {
            bool created = false;
            string template = GetMergedTemplate();
            //TODO: Sidebar and Sidebar2 are obsolete. Phase them out in future
            string[] layoutPartNames = { "Header", "LeftSidebar", "Content", "RightSidebar", "Footer", "Sidebar", "Sidebar2" };
            string[] scriptletIdentifiers = { this.Header, this.LeftSidebar, this.Content, this.RightSidebar, this.Footer, this.LeftSidebar, this.RightSidebar };
            ScriptletType[] scriptletTypes = { ScriptletType.Header, ScriptletType.Sidebar, ScriptletType.Content, ScriptletType.Sidebar, ScriptletType.Footer, ScriptletType.Sidebar, ScriptletType.Sidebar };
            for (int i = 0; i < layoutPartNames.Length; i++)
            {
                string layoutPartName = layoutPartNames[i];
                string replaceKey = "\\[\\[layout:" + layoutPartName + "\\]\\]";
                string scriptletIdentifier = scriptletIdentifiers[i];
                Scriptlet layoutPart = ScriptletDataSource.Load(this.Page.Theme, scriptletIdentifier, scriptletTypes[i], true);
                if (layoutPart != null)
                {
                    if (!string.IsNullOrEmpty(layoutPart.HeaderData)) this.Page.Header.Controls.Add(new LiteralControl(layoutPart.HeaderData));
                    template = Regex.Replace(template, replaceKey, layoutPart.ScriptletData, RegexOptions.IgnoreCase);
                }
            }
            //PROCESS THE MERGED NVELOCITY TEMPLATE
            System.Collections.Hashtable parameters = new System.Collections.Hashtable();
            object store = Token.Instance.Store;
            object customer = Token.Instance.User;
            parameters.Add("store", store);
            parameters.Add("customer", customer);
            parameters.Add("page", this.Page);
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
                            this.Controls.Add(new LiteralControl(match.Value + " " + HttpUtility.HtmlEncode(ex.Message)));
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
            ScriptletPartEditor o = new ScriptletPartEditor();
            o.ID = "ScriptletPartEditor1";
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