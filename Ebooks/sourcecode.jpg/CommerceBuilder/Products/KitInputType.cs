namespace CommerceBuilder.Products
{
    
    /// <summary>
    /// Enumeration that represents the type of input field for kit items
    /// </summary>
    public enum KitInputType
    {
        /// <summary>
        /// The kit item is included in the kit as hidden
        /// </summary>
        IncludedHidden, 
        
        /// <summary>
        /// The kit item is included in the kit and shown separately
        /// </summary>
        IncludedShown, 
        
        /// <summary>
        /// The kit item can be selected from a drop down list
        /// </summary>
        DropDown, 
        
        /// <summary>
        /// The kit item can be selected from a radio button list
        /// </summary>
        RadioButton, 
        
        /// <summary>
        /// The kit item can be selected from a check-box list
        /// </summary>
        CheckBox
    }
}
