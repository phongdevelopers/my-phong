namespace CommerceBuilder.Products
{
    using System;

    /// <summary>
    /// This class represents a ProductTemplate object in the database.
    /// </summary>
    public partial class ProductTemplate
    {
        /// <summary>
        /// Creates a copy of the given ProductTemplate
        /// </summary>
        /// <param name="productTemplateId">Id of the ProductTemplate object to create copy of</param>
        /// <param name="deepCopy">if <b>true</b> child collections are also copied</param>
        /// <returns>A copy of the given ProductTemplate</returns>
        public static ProductTemplate Copy(int productTemplateId, bool deepCopy)
        {
            ProductTemplate copy = ProductTemplateDataSource.Load(productTemplateId);
            if (copy != null)
            {
                if (deepCopy)
                {
                    //LOAD THE CHILD COLLECTIONS AND RESET
                    foreach (InputField field in copy.InputFields)
                    {
                        //LOAD THE CHILD COLLECTIONS AND RESET
                        foreach (InputChoice choice in field.InputChoices)
                        {
                            choice.InputChoiceId = 0;
                        }
                        field.InputFieldId = 0;
                    }
                }
                copy.ProductTemplateId = 0;
                return copy;
            }
            return null;
        }
    }
}
