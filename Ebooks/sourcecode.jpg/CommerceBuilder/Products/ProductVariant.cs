using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// This class represents a ProductVariant object in the database.
    /// </summary>
    public partial class ProductVariant
    {
        /// <summary>
        /// This constant is the number of columns for option Ids in the ProductVariants table
        /// </summary>
        public const int MAXIMUM_ATTRIBUTES = 8;
        private static Regex optionRegex = new Regex("^\\d+(,\\d+){7,}$");

        /// <summary>
        /// This constant is the default sort expression for sorting product variants
        /// </summary>
        public const string VARIANT_SORT_EXPRESSION = "OrderBy ASC";

        private bool _Calculated = false;
        private string _CalculatedName;
        private LSDecimal _CalculatedPrice;
        private LSDecimal _CalculatedWeight;
        private string _CalculatedSku;
        private List<int> _AdditionalOptions;

        /// <summary>
        /// Price modifier mode
        /// </summary>
        public ModifierMode PriceMode
        {
            get { return (ModifierMode)this.PriceModeId; }
            set { this.PriceModeId = (byte)value; }
        }

        /// <summary>
        /// Weight modifier mode
        /// </summary>
        public ModifierMode WeightMode
        {
            get { return (ModifierMode)this.WeightModeId; }
            set { this.WeightModeId = (byte)value; }
        }

        /// <summary>
        /// Determines how the option count is determined.  ActualCount is the true number
        /// of valid option choices, minimum will return at least the number supported
        /// by the database variant grid and more if provided, fixed will return exactly the
        /// number supported by the database.
        /// </summary>
        public enum OptionCountBehavior
        {
            /// <summary>
            /// The actual number of valid options.
            /// </summary>
            ActualCount,
            /// <summary>
            /// At least the number supported by the database, more if available.
            /// </summary>
            Minimum,
            /// <summary>
            /// Exactly the number of options supported by the database.
            /// </summary>
            Fixed
        }

        /// <summary>
        /// Gets an array of option choices that apply to this variant
        /// </summary>
        /// <param name="behavior">Helps determines the number of options included in the array</param>
        /// <returns>An array of option choices that apply to this variant</returns>
        public int[] GetOptionChoices(OptionCountBehavior behavior)
        {
            //BUILD A LIST OF ALL OPTION VALUES
            List<int> allOptions = new List<int>();
            allOptions.Add(this.Option1);
            allOptions.Add(this.Option2);
            allOptions.Add(this.Option3);
            allOptions.Add(this.Option4);
            allOptions.Add(this.Option5);
            allOptions.Add(this.Option6);
            allOptions.Add(this.Option7);
            allOptions.Add(this.Option8);
            if ((_AdditionalOptions != null) && (_AdditionalOptions.Count > 0))
                allOptions.AddRange(_AdditionalOptions);
            //BUILD A LIST THAT INCLUDES NON-ZERO OPTIONS
            List<int> returnedOptions = new List<int>();
            int counter = 0;
            foreach (int opt in allOptions)
            {
                if (behavior == OptionCountBehavior.ActualCount)
                {
                    //ONLY INCLUDE NON ZERO OPTIONS
                    if (opt != 0) returnedOptions.Add(opt);
                    else break;
                }
                else if (behavior == OptionCountBehavior.Minimum)
                {
                    //INCLUDE EVERYTHING TO A MINIMUM OF THE FIXED LENGTH
                    if ((opt != 0) || (counter < MAXIMUM_ATTRIBUTES)) returnedOptions.Add(opt);
                    else break;
                }
                else
                {
                    //BEHAVIOR IS FIXED LENGTH
                    if (counter < MAXIMUM_ATTRIBUTES) returnedOptions.Add(opt);
                    else break;
                }
                counter++;
            }
            return returnedOptions.ToArray();
        }

        /// <summary>
        /// Get the option list for the variant, in the form of #,#,#,#,#,#,#,#
        /// </summary>
        /// <remarks>The </remarks>
        public string OptionList
        {
            get
            {
                return AlwaysConvert.ToList(",", GetOptionChoices(OptionCountBehavior.Minimum));
            }
        }

        /// <summary>
        /// Load the specified product variant
        /// </summary>
        /// <param name="productId">The Id of the product</param>
        /// <param name="optionChoices">A dictionary with OptionId as the key and the selected OptionChoiceId as the value.</param>
        /// <returns><b>true</b> if load successful, <b>false</b> otherwise</returns>
        public bool Load(int productId, Dictionary<int, int> optionChoices)
        {
            return Load(productId, ProductVariantManager.GetSortedOptionChoices(productId, optionChoices));
        }

        /// <summary>
        /// Loads the specified product variant.
        /// </summary>
        /// <param name="productId">The Id of the product</param>
        /// <param name="optionList">The list of option choices for the variant.</param>
        /// <returns><b>true</b> if load successful, <b>false</b> otherwise</returns>
        public bool Load(int productId, string optionList)
        {
            return Load(productId, ParseOptionList(optionList));
        }

        /// <summary>
        /// Load the specified product variant
        /// </summary>
        /// <param name="productId">The Id of the product</param>
        /// <param name="optionChoices">An array of int with the option choices</param>
        /// <returns><b>true</b> if load successful, <b>false</b> otherwise</returns>
        public bool Load(int productId, int[] optionChoices)
        {
            //VALIDATE THE NUMBER OF OPTIONS
            if ((optionChoices == null) || (optionChoices.Length == 0)) return false;
            int actualProductOptionCount = OptionDataSource.CountForProduct(productId);
            //COUNT THE NUMBER OF NON-ZERO OPTION CHOICES
            int relevantOptionCount = 0;
            foreach (int choice in optionChoices)
            {
                if (choice != 0) relevantOptionCount++;
                else break;
            }
            //IF PASSED IN OPTION COUNT DOES NOT MATCH ACTUAL COUNT, THEY ARE INVALID
            if (relevantOptionCount != actualProductOptionCount) return false;

            //CREATE COMPLETE OPTION ARRAY
            //THE ARRAY MUST BE AT LEAST AS LONG AS THE MAXIMUM NUMBER OF OPTIONS
            int activeCount = Math.Max(MAXIMUM_ATTRIBUTES, optionChoices.Length);
            int[] allOptions = new int[activeCount];
            int optionCount = optionChoices.Length;
            for (int i = 0; i < activeCount; i++)
            {
                if (i < optionCount) allOptions[i] = optionChoices[i];
                else allOptions[i] = 0;
            }

            //INITIALIZE THE VALUES OF THIS OBJECT
            this.ProductVariantId = 0;
            this.ProductId = productId;
            this.Option1 = allOptions[0];
            this.Option2 = allOptions[1];
            this.Option3 = allOptions[2];
            this.Option4 = allOptions[3];
            this.Option5 = allOptions[4];
            this.Option6 = allOptions[5];
            this.Option7 = allOptions[6];
            this.Option8 = allOptions[7];
            _AdditionalOptions = new List<int>();
            for (int i = MAXIMUM_ATTRIBUTES; i < allOptions.Length; i++)
            {
                _AdditionalOptions.Add(allOptions[i]);
            }
            this.VariantName = string.Empty;
            this.Sku = string.Empty;
            this.Price = 0;
            this.PriceMode = ModifierMode.Modify;
            this.Weight = 0;
            this.WeightMode = ModifierMode.Modify;
            this.CostOfGoods = 0;
            this.InStock = 0;
            this.InStockWarningLevel = 0;
            this.Available = true;
            this.IsDirty = false;

            //CREATE THE DYNAMIC SQL TO LOAD
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductVariants");
            selectQuery.Append(" WHERE ProductId = @productId");
            //ADD OPTION CRITERIA
            for (int i = 1; i <= MAXIMUM_ATTRIBUTES; i++)
                selectQuery.Append(" AND Option" + i.ToString() + " = @opt" + i.ToString());

            //CREATE DATABASE COMMAND
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            for (int i = 1; i <= MAXIMUM_ATTRIBUTES; i++)
                database.AddInParameter(selectCommand, "@opt" + i.ToString(), System.Data.DbType.Int32, allOptions[i - 1]);

            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    ProductVariant.LoadDataReader(this, dr); ;
                }
                dr.Close();
            }

            //CALCULATE THE VARIANT VALUES REGARDLESS OF THE LOAD RESULT
            //THIS ALLOWS THE VALUES TO BE READ IF THIS RECORD DOES NOT EXIST YET IN THE DB
            return CalculateVariant();
        }

        /// <summary>
        /// Loads this ProductVariant object for the given Id
        /// </summary>
        /// <param name="productVariantId">Id of the ProductVariant to load</param>
        /// <returns><b>true</b> if load successful, <b>false</b> otherwise</returns>
        public bool Load(Int32 productVariantId)
        {
            bool result = BaseLoad(productVariantId);
            if (result) result = this.CalculateVariant();
            return result;
        }

        /// <summary>
        /// Indicates whether any properties of this variant
        /// are different from the calculated values.
        /// </summary>
        public virtual bool IsCustomized
        {
            get {
                if (!_Calculated) CalculateVariant();
                if (!this.Available) return true;
                if (this.CostOfGoods != 0) return true;
                if (this.InStock != 0) return true;
                if (this.InStockWarningLevel != 0) return true;
                if (this.Price != _CalculatedPrice) return true;
                if (this.PriceMode != ModifierMode.Modify) return true;
                if (this.Sku != _CalculatedSku) return true;
                if (this.VariantName != _CalculatedName) return true;
                if (this.Weight != _CalculatedWeight) return true;
                if (this.WeightMode != ModifierMode.Modify) return true;
                return false;
            }
        }

        /// <summary>
        /// Save this ProductVariant object to the database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            int productVariantId = ProductVariantDataSource.GetProductVariantId(this.ProductId, this.GetOptionChoices(OptionCountBehavior.Fixed));
            //ONLY RESET THE ID IF AN EXISTING ONE IS FOUND
            //WE DO NOT WANT TO RESET THE VALUE TO GUID.EMPTY, IN THE EVENT THAT THE VALUE HAS
            //BEEN EXPLICITLY SET FOR A NEW RECORD PRIOR TO THIS CALL
            if (productVariantId != 0) this.ProductVariantId = productVariantId;
            if (!_Calculated) CalculateVariant();
            SaveResult result;
            if (this.IsCustomized)
            {
                //DO NOT WRITE VALUES TO DATABASE IF THEY MATCH THE CALCULATED VALUES
                if (this.VariantName == _CalculatedName) this.VariantName = string.Empty;
                if (this.Price == _CalculatedPrice) this.Price = 0;
                if (this.Weight == _CalculatedWeight) this.Weight = 0;
                if (this.Sku == _CalculatedSku) this.Sku = string.Empty;
                //THE RECORD IS REQUIRED
                result = this.BaseSave();
            }
            else if (this.ProductVariantId != 0)
            {
                //WE SHOULD NOT STORE THIS RECORD
                this.Delete();
                result = SaveResult.RecordDeleted;
            }
            else
            {
                //RECORD IS NOT REQUIRED AND WE DID NOT SAVE OR DELETE
                result = SaveResult.NotDirty;
            }
            if (this.VariantName == string.Empty) this.VariantName = _CalculatedName;
            if (this.Price == 0) this.Price = _CalculatedPrice;
            if (this.Weight == 0) this.Weight = _CalculatedWeight;
            if (this.Sku == string.Empty) this.Sku = _CalculatedSku;
            if (result != SaveResult.Failed) this.IsDirty = false;
            return result;
        }

        /// <summary>
        /// Calculates variant
        /// </summary>
        /// <returns><b>true</b> if calculation successful, <b>false</b> otherwise</returns>
        public bool CalculateVariant()
        {
            _Calculated = false;
            int[] choiceIds = this.GetOptionChoices(OptionCountBehavior.ActualCount);
            if (choiceIds != null && choiceIds.Length > 0)
            {
                //INITIALIZE THE CALCULATED VALUES
                _CalculatedPrice = 0;
                _CalculatedWeight = 0;
                //BUILD CRITERIA TO LOAD CORRECT OPTIONS
                string criteria;
                if (choiceIds.Length == 1)
                    criteria = "OptionChoiceId = " + choiceIds[0].ToString();
                else
                {
                    string idList = AlwaysConvert.ToList(",", choiceIds);
                    criteria = "OptionChoiceId IN (" + idList + ")";
                }
                //RECALCULATE ALL ITEMS
                OptionChoiceCollection choices = OptionChoiceDataSource.LoadForCriteria(criteria);
                OptionChoice choice;
                List<string> names = new List<string>();
                StringBuilder sku = new StringBuilder();
                sku.Append(this.Product.Sku);
                //LOOP ALL CHOICES INDICATED FOR THIS VARIANT AND CALCULATE THE MODIFIERS
                foreach (int optionChoiceId in choiceIds)
                {
                    int index = choices.IndexOf(optionChoiceId);
                    if (index > -1)
                    {
                        choice = choices[index];
                        names.Add(choice.Name);
                        _CalculatedPrice += choice.PriceModifier;
                        _CalculatedWeight += choice.WeightModifier;
                        sku.Append(choice.SkuModifier);
                    }
                }
                //SET THE CALCULATED VALUES
                _CalculatedName = String.Join(", ", names.ToArray());
                _CalculatedSku = sku.ToString();
                if (this.VariantName == string.Empty) this.VariantName = _CalculatedName;
                if (this.Price == 0) this.Price = _CalculatedPrice;
                if (this.Weight == 0) this.Weight = _CalculatedWeight;
                if (this.Sku == string.Empty) this.Sku = _CalculatedSku;
                _Calculated = true;
            }
            return _Calculated;
        }


        /// <summary>
        /// Gets the name of the selected option choice
        /// </summary>
        /// <param name="choices">Collection of option choices</param>
        /// <returns>The name of the selected option choice</returns>
        public string GetSelectedOptionChoiceName(OptionChoiceCollection choices)
        {            
            int index = choices.IndexOf(this.Option1);
            if (index > -1)
            {
                return choices[index].Name;
            }

            index = choices.IndexOf(this.Option2);
            if (index > -1)
            {
                return choices[index].Name;
            }

            index = choices.IndexOf(this.Option3);
            if (index > -1)
            {
                return choices[index].Name;
            }

            index = choices.IndexOf(this.Option4);
            if (index > -1)
            {
                return choices[index].Name;
            }

            index = choices.IndexOf(this.Option5);
            if (index > -1)
            {
                return choices[index].Name;
            }

            index = choices.IndexOf(this.Option6);
            if (index > -1)
            {
                return choices[index].Name;
            }

            index = choices.IndexOf(this.Option7);
            if (index > -1)
            {
                return choices[index].Name;
            }

            index = choices.IndexOf(this.Option8);
            if (index > -1)
            {
                return choices[index].Name;
            }
            
            return "";
        }

        /// <summary>
        /// Checks that a given option list is valid.
        /// </summary>
        /// <param name="optionList">Option list to validate.</param>
        /// <returns>The option list if the format valid, empty string if the format is not valid.</returns>
        public static string ValidateOptionList(string optionList)
        {
            if (string.IsNullOrEmpty(optionList)) return string.Empty;
            Match m = optionRegex.Match(optionList);
            if (m.Success) return optionList;
            return string.Empty;
        }

        /// <summary>
        /// Parses an option list to an int array
        /// </summary>
        /// <param name="optionList"></param>
        /// <returns></returns>
        public static int[] ParseOptionList(string optionList)
        {
            optionList = ValidateOptionList(optionList);
            if (string.IsNullOrEmpty(optionList)) return null;
            string[] choices = optionList.Split(',');
            List<int> choiceList = new List<int>();
            foreach (string choice in choices)
            {
                int choiceId = AlwaysConvert.ToInt(choice.Trim());
                if (choiceId != 0) choiceList.Add(choiceId);
            }
            return choiceList.ToArray();
        }

    }
}