
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Web.UI.WebControls
{
    [ToolboxData("<{0}:SimpleIdTextBox Runat=server>/>")]
    [Designer(typeof(SimpleIdTextBox))]
    public class SimpleIdTextBox : TextBox
    {
        /// <summary>
        /// Override to force simple IDs all around
        /// </summary>
        public override string UniqueID
        {
            get
            {
                return this.ID;
            }
        }

        /// <summary>
        /// Override to force simple IDs all around
        /// </summary>
        public override string ClientID
        {
            get
            {
                return this.ID;
            }
        }
    }
}
