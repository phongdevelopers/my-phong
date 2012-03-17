using System.Collections.Generic;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;

namespace CommerceBuilder.Products
{
    public partial class Option
    {
        /// <summary>
        /// Active Thumbnail Width
        /// </summary>
        public int ActiveThumbnailWidth
        {
            get
            {
                if (this.ThumbnailWidth > 0) return this.ThumbnailWidth;
                return Store.GetCachedSettings().OptionThumbnailWidth;
            }
        }

        /// <summary>
        /// Active Thumbnail Height
        /// </summary>
        public int ActiveThumbnailHeight
        {
            get
            {
                if (this.ThumbnailHeight > 0) return this.ThumbnailHeight;
                return Store.GetCachedSettings().OptionThumbnailHeight;
            }
        }

        /// <summary>
        /// Active Thumbnail Columns
        /// </summary>
        public int ActiveThumbnailColumns
        {
            get
            {
                if (this.ThumbnailColumns > 0) return this.ThumbnailColumns;
                return Store.GetCachedSettings().OptionThumbnailColumns;
            }
        }

        /// <summary>
        /// Delete this Option object from database
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise</returns>
        public virtual bool Delete()
        {
            if (this.ProductOptions.Count > 0)
            {
                //DELETE PRODUCT ASSOCIATIONS
                foreach (ProductOption po in this.ProductOptions)
                {
                    //THIS ALSO RESETS THE VARIANT GRID
                    po.Delete();
                }
                //DELETING THE LAST PRODUCTOPTION RECORD WILL ALSO DELETE THIS OPTION
                return true;
            }
            else return this.BaseDelete();
        }

    }
}
