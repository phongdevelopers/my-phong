namespace CommerceBuilder.Products
{
    using System;
    public partial class WrapGroup
    {
        /// <summary>
        /// Creates a copy of the given wrap group
        /// </summary>
        /// <param name="wrapGroupId">Id of the wrap group to create copy of</param>
        /// <param name="deepCopy">If true all wrap styles are also copied</param>
        /// <returns></returns>
        public static WrapGroup Copy(int wrapGroupId, bool deepCopy)
        {
            WrapGroup copy = WrapGroupDataSource.Load(wrapGroupId);
            if (copy != null)
            {
                if (deepCopy)
                {
                    //LOAD THE CHILD COLLECTIONS AND RESET
                    foreach (WrapStyle style in copy.WrapStyles)
                    {
                        style.WrapStyleId = 0;
                        style.WrapGroupId = 0;
                    }
                }
                copy.WrapGroupId = 0;
                return copy;
            }
            return null;
        }
    }
}
