using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;

namespace CommerceBuilder.Web.UI.WebControls.WebParts
{
    /// <summary>
    /// Class for programatically adding web user controls to a catalog.
    /// </summary>
    public class DynamicCatalogPart : CatalogPart
    {
        List<WebPartDescription> _DynamicParts = new List<WebPartDescription>();
        Dictionary<string, string> _UserControlMappings = new Dictionary<string,string>();
        Dictionary<string, Type> _ServerControlMappings = new Dictionary<string, Type>();

        /// <summary>
        /// Dynamically adds a web user control to the catalog.
        /// </summary>
        /// <param name="id">ID of the web part</param>
        /// <param name="title">Title of the webpart</param>
        /// <param name="description">Description of the webpart</param>
        /// <param name="imageUrl">Image url of the webpart</param>
        /// <param name="path">The virtual path to the user control for this webpart.</param>
        /// <remarks>This acts as a replacement for declarative catalog to avoid circular file reference warnings.  Controls should be added the during page init.</remarks>
        public void AddControl(string id, string title, string description, string imageUrl, string virtualPath)
        {
            WebPartDescription part = new WebPartDescription(id, title, description, imageUrl);
            _DynamicParts.Add(part);
            _UserControlMappings[id] = virtualPath;
        }

        public void AddControl(string classId)
        {
            Type partType = Type.GetType(classId);
            if (partType != null)
            {
                WebPart part = Activator.CreateInstance(partType) as WebPart;
                if (part != null)
                {
                    WebPartDescription partDescription = new WebPartDescription(part);
                    if (!_ServerControlMappings.ContainsKey(partDescription.ID))
                    {
                        _DynamicParts.Add(partDescription);
                        _ServerControlMappings[partDescription.ID] = partType;
                    }
                }
            }
        }

        public override WebPartDescriptionCollection GetAvailableWebPartDescriptions()
        {
            return new WebPartDescriptionCollection(_DynamicParts);
        }

        public override WebPart GetWebPart(WebPartDescription description)
        {
            if (_UserControlMappings.ContainsKey(description.ID))
            {
                string path = _UserControlMappings[description.ID];
                Control part = null;
                try
                {
                    part = this.Page.LoadControl(path);
                }
                catch { }
                if (part != null)
                {
                    part.ID = description.ID;
                    GenericWebPart webPart = this.WebPartManager.CreateWebPart(part);
                    webPart.Title = description.Title;
                    webPart.Description = description.Description;
                    return webPart;
                }
            }
            else if (_ServerControlMappings.ContainsKey(description.ID))
            {
                WebPart webPart = Activator.CreateInstance(_ServerControlMappings[description.ID]) as WebPart;
                return webPart;
            }
            return null;
        }

    }

}

