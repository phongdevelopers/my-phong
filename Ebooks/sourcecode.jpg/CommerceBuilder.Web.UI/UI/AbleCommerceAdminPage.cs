using CommerceBuilder.Common;
using CommerceBuilder.Stores;

namespace CommerceBuilder.Web.UI
{
    /* This module is responsible for:
     * - applying dynamic theme to page
     */

    public class AbleCommerceAdminPage : AbleCommercePage
    {
        /// <summary>
        /// Initialize theme.  Admin pages do not support layout theming.
        /// </summary>
        protected override void InitializeTheme()
        {
            //GET DEFAULT STORE THEME
            Store store = Token.Instance.Store;
            if (store != null)
            {
                string theme = store.Settings.AdminTheme;
                if (!string.IsNullOrEmpty(theme) && CommerceBuilder.UI.Styles.Theme.Exists(theme))
                {
                    this.Theme = theme;
                }
            }
        }
    }
}