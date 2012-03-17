namespace CommerceBuilder.Products
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Text;
    using System.Web;
    using CommerceBuilder.Common;
    using CommerceBuilder.Products;
    using CommerceBuilder.Utility;

    /// <summary>
    /// DataSource class for OptionChoice objects
    /// </summary>
    [DataObject(true)]
    public partial class OptionChoiceDataSource
    {
        /// <summary>
        /// Gets available choices for a product for given option
        /// </summary>
        /// <param name="productId">Id of the product</param>
        /// <param name="targetOptionId">Id of the option to check for choices</param>
        /// <param name="selectedChoices">A Dictionary<int, int> of option choices already selected.  The dictionary key is the OptionId and the value is the OptionChoiceId.</param>
        /// <returns>Collection of OptionChoice objects</returns>
        public static OptionChoiceCollection GetAvailableChoices(int productId, int targetOptionId, Dictionary<int, int> selectedChoices)
        {
            OptionChoiceCollection availableOptionChoices = new OptionChoiceCollection();
            Product product = ProductDataSource.Load(productId);
            if (product != null)
            {
                // SEE IF OPTIONS EXIST, NOTE WE ARE LOADING THE OPTIONS ACCORDING TO THE SORT SPECIFIED FOR VARIANT MANAGEMENT
                // THIS WILL NOT BE THE SAME AS THE DEFAULT SORT ORDER THAT IS DETERMINED BY THE MERCHANT
                OptionCollection options = OptionDataSource.LoadForProduct(productId, ProductVariant.VARIANT_SORT_EXPRESSION);
                if (options.Count > 0)
                {
                    //CAST OFF ANY ATTRIBUTES PAST THE MAXIMUM ALLOWED COUNT
                    while (options.Count > ProductVariant.MAXIMUM_ATTRIBUTES) options.RemoveAt(ProductVariant.MAXIMUM_ATTRIBUTES);

                    // DETERMINE THE INDEX USED OR VARIANT MANAGEMENT, THIS ROUGHLY CORRESPONDS TO THE COLUMN NAMES IN ac_ProductVariants TABLE
                    // BUT NOTE THIS VARIANT INDEX IS ZERO BASED, WHERE THE TABLE COLUMN NAMES ARE 1 BASED: OPTION1, OPTION2, ETC.
                    int targetOptionVariantIndex = options.IndexOf(targetOptionId);
                    if (targetOptionVariantIndex > -1)
                    {
                        // SET THE TARGET OPTION
                        Option targetOption = options[targetOptionVariantIndex];

                        // BUILD THE LIST OF CHOICES SELECTED SO FAR
                        // SELECTED CHOICES KEY IS OPTION INDEX FOR VARIANT MANAGER
                        // THE VALUE IS THE SELECTED CHOICE FOR THAT OPTION INDEX
                        Dictionary<int, int> validSelectedChoices = new Dictionary<int, int>();
                        for (int i = 0; i < options.Count; i++)
                        {
                            Option option = options[i];
                            // IGNORE ANY SELECTED CHOICE FOR THE TARGET OPTION
                            if (option.OptionId != targetOptionId && selectedChoices.ContainsKey(option.OptionId))
                            {
                                // SEE IF THERE IS A CHOICE INDICATED FOR THIS OPTION IN THE QUERY STRING
                                int choiceId = selectedChoices[option.OptionId];
                                int choiceIndex = option.Choices.IndexOf(choiceId);
                                if (choiceIndex > -1)
                                {
                                    // THIS IS A VALID SELECTED CHOICE
                                    validSelectedChoices.Add(i, choiceId);
                                }
                            }
                        }

                        // IF INVENTORY IS ENABLED, WE SIMPLY QUERY THE VARIANT GRID
                        if (Token.Instance.Store.EnableInventory
                            && !product.AllowBackorder
                            && product.InventoryMode == InventoryMode.Variant)
                        {
                            List<int> probableChoices = OptionChoiceDataSource.GetAvailableChoicesFromInventory(productId, validSelectedChoices, targetOptionVariantIndex);
                            // THIS STEP IS PROBABLY UNNEEDED, BUT LETS CONFIRM THE CHOICES FROM THE DATABASE ARE VALID
                            // (THE VARIANT TABLE CANNOT BENEFIT FROM CONSTRAINTS TO ENFORCE DATA INTEGRITY)
                            foreach (int choiceId in probableChoices)
                            {
                                int choiceIndex = targetOption.Choices.IndexOf(choiceId);
                                if (choiceIndex > -1)
                                {
                                    // THIS IS A VALID OPTION CHOICE ID
                                    availableOptionChoices.Add(targetOption.Choices[choiceIndex]);
                                }
                            }
                        }
                        else
                        {
                            // CALCULATE THE REMAINING POSSIBLE OPTION COMBINATIONS FOR EACH CHOICE
                            int possibleCount = 1;
                            foreach (Option option in options)
                            {
                                // WE SHOULD NOT INCLUDE THE TARGET OPTION IN THIS COUNT
                                if (option.OptionId != targetOptionId)
                                {
                                    // WE SHOULD NOT INCLUDE ANY OPTION THAT HAS ALREADY BEEN SELECTED
                                    int variantIndex = options.IndexOf(option.OptionId);
                                    if (!validSelectedChoices.ContainsKey(variantIndex))
                                    {
                                        // WE HAVE FOUND A CHOICE THAT IS NOT SELECTED AND IS NOT
                                        // THE TARGET CHOICE.  SO WE MUST CONSIDER ALL CHOICES AS POTENTIAL
                                        // AND INCLUDE THEM IN THE COUNT
                                        possibleCount = possibleCount * option.Choices.Count;
                                    }
                                }
                            }

                            // BUILD THE DICTIONARY OF UNAVAILABLE COUNTS
                            // THIS WILL BE GROUPED BY THE CHOICEID FOR THE TARGET OPTION
                            // AND WILL TELL US HOW MANY COMBINATIONS ARE INVALID FOR THAT CHOICE
                            // IF THE NUMBER MATCHES THE POSSIBLE COUNT, THEN ALL COMBINATIONS ARE UNAVAILABLE
                            // AND THIS CHOICE SHOULD NOT BE INCLUDED AS AVAILABLE
                            Dictionary<int, int> unavailableVariantCounts = OptionChoiceDataSource.GetUnavailableVariantCounts(productId, validSelectedChoices, targetOptionVariantIndex);
                            foreach (OptionChoice possibleChoice in targetOption.Choices)
                            {
                                // CHECK IF THERE ARE NO UNAVAILABLE VARIANTS FOR THIS CHOICE
                                // OR THE UNAVAILABLE COUNT IS LESS THAN THE POSSIBLE COMBINATION COUNT
                                if (!unavailableVariantCounts.ContainsKey(possibleChoice.OptionChoiceId)
                                    || unavailableVariantCounts[possibleChoice.OptionChoiceId] < possibleCount)
                                {
                                    // THIS IS A VALID CHOICE
                                    availableOptionChoices.Add(possibleChoice);
                                }

                            }
                        }
                    }
                }
            }
            return availableOptionChoices;
        }

        /// <summary>
        /// Gets a list of option choices that are available for the specified variant index
        /// </summary>
        /// <param name="productId">The product</param>
        /// <param name="selectedChoices">The choices that selections are known for</param>
        /// <param name="targetOptionVariantIndex">The variant index of the option</param>
        /// <returns>A integer list of choices available for the specified option</returns>
        /// <remarks>Only call this method if variant inventory is enabled for the product.</remarks>
        private static List<int> GetAvailableChoicesFromInventory(int productId, Dictionary<int, int> selectedChoices, int targetOptionVariantIndex)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT DISTINCT Option" + (targetOptionVariantIndex + 1) + ",OrderBy");
            sql.Append(" FROM ac_ProductVariants JOIN ac_OptionChoices ON ac_ProductVariants.Option" + (targetOptionVariantIndex + 1) + " = ac_OptionChoices.OptionChoiceId WHERE ProductId = " + productId);
            foreach (int variantOptionIndex in selectedChoices.Keys)
            {
                sql.Append(" AND Option" + (variantOptionIndex + 1) + " = " + selectedChoices[variantOptionIndex]);
            }
            sql.Append(" AND InStock > 0 AND Available = 1");
            sql.Append(" ORDER BY OrderBy ASC ");
            List<int> optionIds = new List<int>();
            using (IDataReader reader = Token.Instance.Database.ExecuteReader(CommandType.Text, sql.ToString()))
            {
                while (reader.Read())
                {
                    optionIds.Add(reader.GetInt32(0));
                }
                reader.Close();
            }
            return optionIds;
        }

        /// <summary>
        /// Gets a count of unavailable variants grouped by option choice
        /// </summary>
        /// <param name="productId">The product</param>
        /// <param name="selectedChoices">The choices that selections are known for</param>
        /// <param name="targetOptionVariantIndex">The variant index of the option</param>
        /// <remarks>Only call this method if variant inventory is not enabled for the product.</remarks>
        private static Dictionary<int, int> GetUnavailableVariantCounts(int productId, Dictionary<int, int> selectedChoices, int targetOptionVariantIndex)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT Option" + (targetOptionVariantIndex + 1) + ", COUNT(*) AS UnavailableCount");
            sql.Append(" FROM ac_ProductVariants WHERE ProductId = " + productId);
            foreach (int variantOptionIndex in selectedChoices.Keys)
            {
                sql.Append(" AND Option" + (variantOptionIndex + 1) + " = " + selectedChoices[variantOptionIndex]);
            }
            sql.Append(" AND Available = 0");
            sql.Append(" GROUP BY Option" + (targetOptionVariantIndex + 1));
            Dictionary<int, int> unavailableVariantCounts = new Dictionary<int, int>();
            using (IDataReader reader = Token.Instance.Database.ExecuteReader(CommandType.Text, sql.ToString()))
            {
                while (reader.Read())
                {
                    int choiceId = reader.GetInt32(0);
                    int unavailCount = reader.GetInt32(1);
                    unavailableVariantCounts.Add(choiceId, unavailCount);
                }
                reader.Close();
            }
            return unavailableVariantCounts;
        }
    }
}