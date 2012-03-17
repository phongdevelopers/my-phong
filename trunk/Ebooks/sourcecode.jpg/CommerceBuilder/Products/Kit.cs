using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// Class that represents a kit
    /// </summary>
    public partial class Kit
    {
        private bool _Calculated = false;
        private LSDecimal _MinPrice;
        private LSDecimal _MaxPrice;
        private LSDecimal _DefaultPrice = 0;
        private LSDecimal _MinWeight;
        private LSDecimal _MaxWeight;
        private LSDecimal _DefaultWeight;
        private string _OptionList;

        /// <summary>
        /// Option list that the kit values were last calculated against.
        /// </summary>
        public string OptionList
        {
            get { return _OptionList; }
        }

        /// <summary>
        /// Recalculates the minimum, maximum and default price and weight for this kit
        /// </summary>
        public void Recalculate()
        {
            Recalculate(string.Empty);
        }

        /// <summary>
        /// Recalculates the kit
        /// </summary>
        /// <param name="optionList">Option list specifying the variant</param>
        public void Recalculate(string optionList)
        {
            //DETERMINE THE BASE PRICE OF THE PRODUCT
            LSDecimal basePrice = 0;
            LSDecimal baseWeight = 0;
            //CHECK FOR A SPECIFIED VARIANT
            ProductVariant variant = ProductVariantDataSource.LoadForOptionList(_Product.ProductId, optionList);
            if (variant == null)
            {
                //NO VARIANT, SET VALUES FROM BASE PRODCUT
                _OptionList = string.Empty;
                basePrice = _Product.Price;
                baseWeight = _Product.Weight;
            }
            else
            {
                //VARIANT PRESENT, CHECK VARIANT FOR CORRECT VALUES
                _OptionList = optionList;
                basePrice = (variant.Price == 0) ? _Product.Price : variant.Price;
                baseWeight = (variant.Weight == 0) ? _Product.Weight : variant.Weight;
            }
            _MinPrice = basePrice;
            _MaxPrice = basePrice;
            _DefaultPrice = basePrice;
            _MinWeight = baseWeight;
            _MaxWeight = baseWeight;
            _DefaultWeight = baseWeight;
            foreach (ProductKitComponent pkc in _Product.ProductKitComponents)
            {
                KitComponent component = pkc.KitComponent;
                if (component.KitProducts.Count > 0)
                {
                    switch (component.InputType)
                    {
                        case KitInputType.IncludedHidden:
                        case KitInputType.IncludedShown:
                            //ALL OF THESE GO INTO MINIMUM AND DEFAULT PRICES
                            foreach (KitProduct kp in component.KitProducts)
                            {
                                LSDecimal kpPrice = kp.CalculatedPrice;
                                LSDecimal kpWeight = kp.CalculatedWeight;
                                _MinPrice += kpPrice;
                                _MaxPrice += kpPrice;
                                _DefaultPrice += kpPrice;
                                _MinWeight += kpWeight;
                                _MaxWeight += kpWeight;
                                _DefaultWeight += kpWeight;
                            }
                            break;
                        case KitInputType.CheckBox:
                            //NONE GO INTO MINIMUM
                            //ALL GO INTO MAXIMUM
                            //DEFAULT SELECTED
                            foreach (KitProduct kp in component.KitProducts)
                            {
                                LSDecimal kpPrice = kp.CalculatedPrice;
                                LSDecimal kpWeight = kp.CalculatedWeight;
                                _MaxPrice += kpPrice;
                                _MaxWeight += kpWeight;
                                if (kp.IsSelected)
                                {
                                    _DefaultPrice += kpPrice;
                                    _DefaultWeight += kpWeight;
                                }
                            }
                            break;
                        case KitInputType.DropDown:
                        case KitInputType.RadioButton:
                            //LOWEST PRICE / WEIGHT GOES INTO MINIMUM
                            //HIGHEST PRICE / WEIGHT GOES INTO MAXIMUM
                            //DEFAULT SELECTED
                            LSDecimal tempDefaultPrice;
                            LSDecimal tempDefaultWeight;
                            LSDecimal tempHighPrice;
                            LSDecimal tempLowPrice;
                            LSDecimal tempHighWeight;
                            LSDecimal tempLowWeight;
                            //GET FIRST KIT PRODUCT
                            KitProduct firstKp = component.KitProducts[0];
                            LSDecimal firstKpPrice = firstKp.CalculatedPrice;
                            LSDecimal firstKpWeight = firstKp.CalculatedWeight;
                            //CHECK FOR HEADER OPTION
                            if (string.IsNullOrEmpty(component.HeaderOption))
                            {
                                //NO HEADER OPTION, VALUES MUST START WITH FIRST KIT PRODUCT
                                tempHighPrice = firstKpPrice;
                                tempHighWeight = firstKpWeight;
                                tempLowPrice = firstKpPrice;
                                tempLowWeight = firstKpWeight;
                                tempDefaultPrice = firstKpPrice;
                                tempDefaultWeight = firstKpWeight;
                            }
                            else
                            {
                                //HEADEROPTION IS PRESENT
                                tempHighPrice = firstKpPrice > 0 ? firstKpPrice : 0;
                                tempHighWeight = firstKpWeight > 0 ? firstKpWeight : 0;
                                tempLowPrice = firstKpPrice < 0 ? firstKpPrice : 0;
                                tempLowWeight = firstKpWeight < 0 ? firstKpWeight : 0;
                                if (firstKp.IsSelected)
                                {
                                    tempDefaultPrice = firstKpPrice;
                                    tempDefaultWeight = firstKpWeight;
                                }
                                else
                                {
                                    tempDefaultPrice = 0;
                                    tempDefaultWeight = 0;
                                }
                            }
                            bool foundDefault = firstKp.IsSelected;
                            //LOOP THE REMAINING PRODUCTS IN COLLECTION TO CALCULATE VALUES
                            for (int index = 1; index < component.KitProducts.Count; index++)
                            {
                                KitProduct kp = component.KitProducts[index];
                                LSDecimal kpPrice = kp.CalculatedPrice;
                                LSDecimal kpWeight = kp.CalculatedWeight;
                                if (kpPrice > tempHighPrice) tempHighPrice = kpPrice;
                                if (kpPrice < tempLowPrice) tempLowPrice = kpPrice;
                                if (kpWeight > tempHighWeight) tempHighWeight = kpWeight;
                                if (kpWeight < tempLowWeight) tempLowWeight = kpWeight;
                                if ((!foundDefault) && (kp.IsSelected))
                                {
                                    tempDefaultPrice = kpPrice;
                                    tempDefaultWeight = kpWeight;
                                    foundDefault = true;
                                }
                            }
                            _MinPrice += tempLowPrice;
                            _MinWeight += tempLowWeight;
                            _MaxPrice += tempHighPrice;
                            _MaxWeight += tempHighWeight;
                            _DefaultPrice += tempDefaultPrice;
                            _DefaultWeight += tempDefaultWeight;
                            break;
                    }
                }
            }
            _Calculated = true;
        }

        /// <summary>
        /// Minimum price (with least expensive options)
        /// </summary>
        public LSDecimal MinPrice
        {
            get
            {
                if (!_Calculated) this.Recalculate();
                return _MinPrice;
            }
        }

        /// <summary>
        /// Maximum price (with most expensive options)
        /// </summary>
        public LSDecimal MaxPrice
        {
            get
            {
                if (!_Calculated) this.Recalculate();
                return _MaxPrice;
            }
        }

        /// <summary>
        /// Default price (with default options)
        /// </summary>
        public LSDecimal DefaultPrice
        {
            get
            {
                if (!_Calculated) this.Recalculate();
                return _DefaultPrice;
            }
        }

        /// <summary>
        /// Minimum weight (with least weight options)
        /// </summary>
        public LSDecimal MinWeight
        {
            get
            {
                if (!_Calculated) this.Recalculate();
                return _MinWeight;
            }
        }

        /// <summary>
        /// Maximum weight (with most heavy options)
        /// </summary>
        public LSDecimal MaxWeight
        {
            get
            {
                if (!_Calculated) this.Recalculate();
                return _MaxWeight;
            }
        }

        /// <summary>
        /// Default weight (with default options)
        /// </summary>
        public LSDecimal DefaultWeight
        {
            get
            {
                if (!_Calculated) this.Recalculate();
                return _DefaultWeight;
            }
        }

        /// <summary>
        /// Gets a list of KitProducts for this Kit
        /// </summary>
        /// <returns></returns>
        public List<KitProduct> GetDefaultKitProducts()
        {
            List<KitProduct> defaultKitProducts = new List<KitProduct>();
            foreach (ProductKitComponent pkc in _Product.ProductKitComponents)
            {
                KitComponent component = pkc.KitComponent;
                switch (component.InputType)
                {
                    case KitInputType.IncludedHidden:
                    case KitInputType.IncludedShown:
                        //ALL OF THESE GO INTO MINIMUM AND DEFAULT PRICES
                        foreach (KitProduct kp in component.KitProducts)
                        {
                            defaultKitProducts.Add(kp);
                        }
                        break;
                    case KitInputType.CheckBox:
                        //NONE GO INTO MINIMUM
                        //ALL GO INTO MAXIMUM
                        //DEFAULT SELECTED
                        foreach (KitProduct kp in component.KitProducts)
                        {
                            if (kp.IsSelected) defaultKitProducts.Add(kp);
                        }
                        break;
                    case KitInputType.DropDown:
                    case KitInputType.RadioButton:
                        KitProduct tempDefaultKp = null;
                        bool foundDefault = false;
                        int index = 0;
                        foreach (KitProduct kp in component.KitProducts)
                        {
                            if ((!foundDefault) && (kp.IsSelected))
                            {
                                tempDefaultKp = kp;
                                foundDefault = true;
                            }
                            else if ((index == 0) && string.IsNullOrEmpty(component.HeaderOption))
                            {
                                tempDefaultKp = kp;
                            }
                            index++;
                        }
                        if (tempDefaultKp != null) defaultKitProducts.Add(tempDefaultKp);
                        break;
                }
            }
            return defaultKitProducts;
        }

        /// <summary>
        /// Checks the set of kit products selected for this kit to determine if they are valid and complete
        /// </summary>
        /// <param name="kitList">List of kit product ids</param>
        /// <returns>True if the kit choices represent a valid kit, false otherwise</returns>
        public bool ValidateChoices(string kitList)
        {
            // PARSE THE KIT LIST
            List<int> kitProductIds = new List<int>();
            if (!string.IsNullOrEmpty(kitList))
            {
                kitProductIds.AddRange(AlwaysConvert.ToIntArray(kitList));
            }

            // CHECK FOR INVALID KITPRODUCTS IN THE LIST
            int originalCount = kitProductIds.Count;
            RemoveInvalidKitProducts(kitProductIds);

            // IF ANY INVALID ITEMS WERE REMOVED, THIS IS NOT A VALID KITLIST
            if (kitProductIds.Count != originalCount) return false;

            // LOOP THE KIT COMPONENTS
            foreach (ProductKitComponent pkc in _Product.ProductKitComponents)
            {
                KitComponent kc = pkc.KitComponent;
                switch (kc.InputType)
                {
                    case KitInputType.IncludedHidden:
                    case KitInputType.IncludedShown:
                        foreach (KitProduct kp in kc.KitProducts)
                        {
                            // IF THIS INCLUDED ITEM IS NOT IN THE KITLIST IT IS INVALID
                            if (!kitProductIds.Contains(kp.KitProductId)) return false;
                        }
                        break;
                    case KitInputType.DropDown:
                    case KitInputType.RadioButton:
                        // WE EITHER NEED 0 OR 1 FROM THIS COMPONENT, CAN NEVER HAVE TWO
                        bool needOne = kc.KitProducts.Count > 0 && string.IsNullOrEmpty(kc.HeaderOption);
                        bool foundOne = false;
                        foreach (KitProduct kp in kc.KitProducts)
                        {
                            // IF THIS INCLUDED ITEM IS NOT IN THE KITLIST IT IS INVALID
                            if (kitProductIds.Contains(kp.KitProductId))
                            {
                                // IF WE HAVE ALREADY FOUND ONE CHOICE FROM THIS LIST THIS KIT IS INVALID
                                if (foundOne) return false;
                                foundOne = true;
                            }
                        }
                        // IF A CHOICE IS REQUIRED BUT NOT FOUND THIS KIT IS INVALID
                        if (needOne && !foundOne) return false;
                        break;
                }
            }

            // IF WE DID NOT FIND ANY PROBLEMS THE KIT IS VALID
            return true;
        }

        /// <summary>
        /// Removes from the list IDs of any kit product not currently associated with this kit
        /// </summary>
        /// <param name="kitProductIds">A list of kit product ids</param>
        private void RemoveInvalidKitProducts(List<int> kitProductIds)
        {
            // BUILD A MASTER LIST OF ALL KITPRODUCTS ASSOCIATED WITH THE KIT
            List<int> associatedKitProducts = new List<int>();
            foreach (ProductKitComponent pkc in _Product.ProductKitComponents)
            {
                KitComponent kc = pkc.KitComponent;
                foreach (KitProduct kp in kc.KitProducts)
                {
                    associatedKitProducts.Add(kp.KitProductId);
                }
            }

            // LOOP THE SPECIFIED KIT PRODUCTS TO REMOVE INVALID ONES
            for (int i = kitProductIds.Count - 1; i >= 0; i--)
            {
                // SEE WHETHER THIS KITPRODUCT IS ASSOCIATED WITH THE KIT
                int kitProductId = kitProductIds[i];
                if (!associatedKitProducts.Contains(kitProductId))
                {
                    kitProductIds.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Checks the list of kit products and updates as necessary.
        /// </summary>
        /// <param name="kitList">The list of kit products to refresh.</param>
        /// <returns>An updated list of kit products.</returns>
        /// <remarks>
        /// 1. Ensures any included kit products are in the list
        /// 2. Any kit products that are no longer associated to the kit are removed.
        /// </remarks>
        internal string RefreshKitProducts(string kitList)
        {
            // PARSE THE ORIGINAL KIT LIST
            List<int> kitProductIds = new List<int>();
            if (!string.IsNullOrEmpty(kitList))
            {
                kitProductIds.AddRange(AlwaysConvert.ToIntArray(kitList));
            }

            // REMOVE INVALID KITPRODUCTS
            RemoveInvalidKitProducts(kitProductIds);

            // ENSURE REQUIRED CHOICES ARE IN THE KITLIST
            foreach (ProductKitComponent pkc in _Product.ProductKitComponents)
            {
                KitComponent kc = pkc.KitComponent;
                if (kc.InputType == KitInputType.IncludedShown || kc.InputType == KitInputType.IncludedHidden)
                {
                    // THIS IS AN INCLUDED COMPONENT, SO ALL ITEMS ARE REQUIRED
                    foreach (KitProduct kp in kc.KitProducts)
                    {
                        if (!kitProductIds.Contains(kp.KitProductId))
                            kitProductIds.Add(kp.KitProductId);
                    }
                }
                else if ((kc.InputType == KitInputType.DropDown || kc.InputType == KitInputType.RadioButton)
                        && string.IsNullOrEmpty(kc.HeaderOption) && kc.KitProducts.Count == 1)
                {
                    // THIS TYPE OF CONTROL WITHOUT A HEADER OPTION AND A SINGLE KITPRODUCT FORCES CHOICE
                    KitProduct kp = kc.KitProducts[0];
                    if (!kitProductIds.Contains(kp.KitProductId))
                        kitProductIds.Add(kp.KitProductId);
                }
            }

            // RETURN THE REFRESHED KITLIST
            if (kitProductIds.Count == 0) return string.Empty;
            return AlwaysConvert.ToList(",", kitProductIds.ToArray());
        }

        /// <summary>
        /// Saves this BasketItem object to database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.ProductId < 1) throw new InvalidProductException("This kit instance does not point to a valid product.");
            _Product = ProductDataSource.Load(_ProductId);
            if (_Product == null) throw new InvalidProductException("This kit instance does not point to a valid product.");
            if (_Product.KitStatus != KitStatus.Master)
            {
                // IF THE PRODUCT IS NOT A KIT, DO NOT SAVE THIS DATA
                this.Delete();
                return CommerceBuilder.Common.SaveResult.RecordDeleted;
            }
            return this.BaseSave();
        }
    }
}