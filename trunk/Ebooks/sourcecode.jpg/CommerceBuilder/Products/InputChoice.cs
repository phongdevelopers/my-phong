namespace CommerceBuilder.Products
{
    /// <summary>
    /// This class represents a InputChoice object in the database.
    /// </summary>
    public partial class InputChoice
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="choiceText">Choice text</param>
        /// <param name="choiceValue">Choice value</param>
        /// <param name="isSelected">Whether the choice is selected or not</param>
        public InputChoice(string choiceText, string choiceValue, bool isSelected) : this()
        {
            this.ChoiceText = choiceText;
            this.ChoiceValue = choiceValue;
            this.IsSelected = isSelected;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="choiceText">Choice text</param>
        /// <param name="isSelected">Choice value</param>
        public InputChoice(string choiceText, bool isSelected) : this(choiceText, string.Empty, isSelected) {}
    }
}
