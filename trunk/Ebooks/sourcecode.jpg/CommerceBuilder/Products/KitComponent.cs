using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using CommerceBuilder.Taxes;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// Class that represents a KitComponent in the database
    /// </summary>
    public partial class KitComponent
    {
        /// <summary>
        /// Indicates the type of input for example hidden, shown, drop-down, radio button, or checkbox
        /// </summary>
        public KitInputType InputType
        {
            get { return (KitInputType)this.InputTypeId; }
            set { this.InputTypeId = (byte)value; }
        }

        /// <summary>
        /// Is this KitComponent optional?
        /// </summary>
        /// <returns><b>true</b> if optional, <b>false</b> otherwise</returns>
        public bool IsOptional()
        {
            return this.InputType == KitInputType.CheckBox;
        }

        /// <summary>
        /// Is this KitComponent attached to the given product
        /// </summary>
        /// <param name="productId">Id of the product to check for attachment</param>
        /// <returns><b>true</b> if attached to the given product, <b>false</b> otherwise</returns>
        public bool IsAttached(int productId)
        {
            //REMOVE SPECIFIED PRODUCT FROM ORIGINAL COMPONENT
            foreach (ProductKitComponent pkc in this.ProductKitComponents)
            {
                if (pkc.ProductId.Equals(productId)) return true;
            }
            return false;
        }

        private KitComponent InternalCopy()
        {
            //CREATE AN EXACT DUPLICATE OF THIS KIT COMPONENT
            KitComponent copiedComponent = KitComponentDataSource.Load(this.KitComponentId, false);
            //MAKE COPIES OF THE ASSOCIATED PRODUCTS
            foreach (KitProduct item in copiedComponent.KitProducts)
            {
                item.KitProductId = 0;
            }
            //RESET THE COMPONENTID
            copiedComponent.KitComponentId = 0;
            //SAVE THE NEW COMPONENT
            copiedComponent.Save();
            return copiedComponent;
        }

        /// <summary>
        /// Creates a new KitComponent object based on this KitComponent object and attaches it as a component to the given product
        /// </summary>
        /// <param name="productId">Id of the product to which to attach the branched component</param>
        /// <returns>The newly created KitComponent</returns>
        public KitComponent Branch(int productId)
        {
            //REMOVE SPECIFIED PRODUCT FROM ORIGINAL COMPONENT
            short orderBy = -1;
            int index = this.ProductKitComponents.IndexOf(productId, this.KitComponentId);
            if (index > -1)
            {
                orderBy = this.ProductKitComponents[index].OrderBy;
                this.ProductKitComponents.DeleteAt(index);
            }
            //CREATE A COPY
            KitComponent branchedComponent = this.InternalCopy();
            //ATTACH THE CURRENT PRODUCT
            ProductKitComponent pkc = new ProductKitComponent(productId, branchedComponent.KitComponentId);
            pkc.OrderBy = orderBy;
            branchedComponent.ProductKitComponents.Add(pkc);
            branchedComponent.Save();
            //RETURN THE BRANCHED COMPONENT
            return branchedComponent;
        }

        /// <summary>
        /// Creates a copy of this KitComponent object
        /// </summary>
        /// <param name="productId">Id of the product to attach the new object to</param>
        /// <returns>A copy of this KitComponent object</returns>
        public KitComponent Copy(int productId)
        {
            KitComponent copiedComponent = this.InternalCopy();
            copiedComponent.ProductKitComponents.Add(new ProductKitComponent(productId, copiedComponent.KitComponentId));
            copiedComponent.Save();
            return copiedComponent;
        }

        /// <summary>
        /// Returns a unique Id of this KitComponent object
        /// </summary>
        public string UniqueId
        {
            get { return "KitComponent_" + this.KitComponentId.ToString(); }
        }

        /// <summary>
        /// Removes invalid KitProducts from this KitComponent
        /// </summary>
        public void RemoveInvalidKitProducts()
        {            
            KitProduct kc;
            for (int i = this.KitProducts.Count-1; i >= 0; i--)
            {
                kc = KitProducts[i];
                //REMOVE KIT PRODUCTS THAT HAVE INVALID OPTION LISTS
                if (kc != null && !string.IsNullOrEmpty(kc.OptionList) && kc.ProductVariant == null)
                {
                    KitProducts.RemoveAt(i);
                    kc.Delete();                    
                }
            }
        }

        /// <summary>
        /// Verifies that the inventory settings for a kit product are sufficient
        /// for the product to be included in the kit purchase.
        /// </summary>
        /// <param name="kp">The kit product to check</param>
        /// <returns>False is returned when there is not sufficient quantity of the 
        /// product to be included as part of the kit.</returns>
        /// <remarks>This method does not check whether inventory is enabled for the store.</remarks>
        private bool VerifyInventory(KitProduct kp)
        {
            InventoryManagerData invData = kp.CheckStock();
            return (invData.InventoryMode == InventoryMode.None || invData.InStock >= kp.Quantity || invData.AllowBackorder);
        }

        /// <summary>
        /// Create the control represented by this kit component.
        /// </summary>
        /// <returns>A web control populated for this kit component</returns>
        public System.Web.UI.WebControls.WebControl GetControl()
        {
            return GetControl(false);
        }
        
        /// <summary>
        /// Create the control represented by this kit component.
        /// </summary>
        /// <param name="ignoreInventory">If true, inventory is ignored for the purpose of generating choices for the component.</param>
        /// <returns>A web control populated for this kit component</returns>
        public System.Web.UI.WebControls.WebControl GetControl(bool ignoreInventory)
        {
            bool setSelected = false;
            bool inventoryIsDisabled = !Token.Instance.Store.EnableInventory;
            switch (this.InputType)
            {
                case KitInputType.CheckBox:
                    CheckBoxList checkBoxList = new CheckBoxList();
                    checkBoxList.ID = this.UniqueId;
                    foreach (KitProduct choice in this.KitProducts)
                    {
                        if (ignoreInventory || inventoryIsDisabled || VerifyInventory(choice))
                        {
                            string choiceValue = choice.KitProductId.ToString();
                            string choiceName = GetChoiceName(choice);
                            ListItem item = new ListItem(choiceName, choiceValue);
                            item.Selected = choice.IsSelected;
                            checkBoxList.Items.Add(item);
                        }
                    }
                    if (checkBoxList.Items.Count <= 0)
                    {
                        return GetComponentOutOfStockLabel();
                    }
                    else
                    {
                        return checkBoxList;
                    }
                case KitInputType.DropDown:
                    DropDownList dropDown = new DropDownList();
                    dropDown.ID = this.UniqueId;
                    //CHECK FOR HEADER OPTION
                    if (!string.IsNullOrEmpty(this.HeaderOption))
                        dropDown.Items.Add(new ListItem(this.HeaderOption, "0"));
                    //ADD IN PRODUCT CHOICES
                    foreach (KitProduct choice in this.KitProducts)
                    {
                        if (ignoreInventory || inventoryIsDisabled || VerifyInventory(choice))
                        {
                            string choiceValue = choice.KitProductId.ToString();
                            string choiceName = GetChoiceName(choice);
                            ListItem item = new ListItem(choiceName, choiceValue);
                            if (!setSelected && choice.IsSelected)
                            {
                                item.Selected = (choice.IsSelected);
                                setSelected = true;
                            }
                            dropDown.Items.Add(item);
                        }
                    }
                    if (dropDown.Items.Count <= 0)
                    {
                        return GetComponentOutOfStockLabel();
                    }
                    else
                    {
                        return dropDown;
                    }
                case KitInputType.IncludedShown:
                    StringBuilder sb = new StringBuilder();
                    bool listOpen = false;
                    foreach (KitProduct choice in this.KitProducts)
                    {
                        if (!listOpen)
                        {
                            sb.Append("<ul>\n");
                            listOpen = true;
                        }
                        string choiceName = GetChoiceName(choice);
                        sb.Append("<li>" + choiceName + "</li>\n");
                    }
                    if (listOpen)
                    {
                        sb.Append("</ul>\n");
                        Label bulletedList = new Label();
                        bulletedList.ID = this.UniqueId;
                        bulletedList.Text = sb.ToString();
                        return bulletedList;
                    }
                    else
                    {
                        return GetComponentOutOfStockLabel();
                    }
                case KitInputType.RadioButton:
                    RadioButtonList radioButtonList = new RadioButtonList();
                    radioButtonList.ID = this.UniqueId;
                    if (this.Columns > 1) radioButtonList.RepeatColumns = this.Columns;
                    //CHECK FOR HEADER OPTION
                    if (!string.IsNullOrEmpty(this.HeaderOption))
                        radioButtonList.Items.Add(new ListItem(this.HeaderOption, "0"));
                    //ADD IN PRODUCT CHOICES
                    foreach (KitProduct choice in this.KitProducts)
                    {
                        if (ignoreInventory || inventoryIsDisabled || VerifyInventory(choice))
                        {
                            string choiceValue = choice.KitProductId.ToString();
                            string choiceName = GetChoiceName(choice);
                            ListItem item = new ListItem(choiceName, choiceValue);
                            if (!setSelected && choice.IsSelected)
                            {
                                item.Selected = (choice.IsSelected);
                                setSelected = true;
                            }
                            radioButtonList.Items.Add(item);
                        }
                    }
                    if (!setSelected && this.KitProducts.Count > 0) radioButtonList.SelectedIndex = 0;
                    if (radioButtonList.Items.Count <= 0)
                    {
                        return GetComponentOutOfStockLabel();
                    }
                    else
                    {
                        return radioButtonList;
                    }
            }
            return null;
        }

        private string GetChoiceName(KitProduct choice)
        {
            if (!choice.Name.Contains("$price"))
            {
                LSDecimal price = choice.CalculatedPrice;
                if (price != 0)
                {
                    // TO MAINTAIN BACKWARD COMPATIBILITY, WE NEED TO INCLUDE THE PRICE WITH THE NAME
                    string tempName = choice.Name;
                    choice.Name += " - ($price)";
                    string displayName = choice.DisplayName;
                    choice.Name = tempName;
                    return displayName;
                }
            }
            return choice.DisplayName;
        }

        private Label GetComponentOutOfStockLabel()
        {
            string message;
            if (this.KitProducts.Count > 0)
            {
                message = "All options for '{0}' are currently out of stock.";
            }
            else
            {
                message = "No options available for '{0}' at this time.";
            }
            Label lbl = new Label();
            lbl.ID = this.UniqueId;
            lbl.Text = string.Format(message, this.Name);
            return lbl;
        }

        /// <summary>
        /// Gets the products selected for a kit component.
        /// </summary>
        /// <param name="control">The control created for the kit component.</param>
        /// <returns>A list of product IDs that are selected for the component</returns>
        public List<int> GetControlValue(WebControl control)
        {
            List<int> values = new List<int>();
            int val;
            switch (this.InputType)
            {
                case KitInputType.CheckBox:
                    CheckBoxList checkBoxList = control as CheckBoxList;
                    if (checkBoxList != null)
                    {
                        foreach (ListItem item in checkBoxList.Items)
                        {
                            if (item.Selected)
                            {
                                val = AlwaysConvert.ToInt(item.Value);
                                if (val != 0) values.Add(val);
                            }
                        }
                    }
                    break;
                case KitInputType.DropDown:
                    DropDownList dropDownList = control as DropDownList;
                    if (dropDownList != null)
                    {
                        ListItem selectedItem = dropDownList.Items[dropDownList.SelectedIndex];
                        if (selectedItem != null)
                        {
                            val = AlwaysConvert.ToInt(selectedItem.Value);
                            if (val != 0) values.Add(val);
                        }
                    }
                    break;
                case KitInputType.IncludedHidden:
                case KitInputType.IncludedShown:
                    foreach (KitProduct choice in this.KitProducts)
                    {
                        values.Add(choice.KitProductId);
                    }
                    break;
                case KitInputType.RadioButton:
                    RadioButtonList radioButtonList = control as RadioButtonList;
                    if (radioButtonList != null)
                    {
                        ListItem selectedItem = radioButtonList.Items[radioButtonList.SelectedIndex];
                        if (selectedItem != null)
                        {
                            val = AlwaysConvert.ToInt(selectedItem.Value);
                            if (val != 0) values.Add(val);
                        }
                    }
                    break;
            }
            return values;
        }


    }
}
