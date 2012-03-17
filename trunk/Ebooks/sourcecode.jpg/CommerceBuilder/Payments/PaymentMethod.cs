using CommerceBuilder.Users;

namespace CommerceBuilder.Payments
{
    public partial class PaymentMethod
    {
        /// <summary>
        /// Payment instruments associated with this payment method
        /// </summary>
        public PaymentInstrument PaymentInstrument
        {
            get { return (PaymentInstrument)this.PaymentInstrumentId; }
            set { this.PaymentInstrumentId = (short)value; }
        }

        /// <summary>
        /// Indicates whether the specified user can access the payment method, taking
        /// group restrictions and user membership into account.
        /// </summary>
        /// <param name="user">The user to check payment method access for.</param>
        /// <returns>True if the user can access the method, false otherwise.</returns>
        public bool UserHasAccess(User user)
        {
            //IF THERE ARE NO ROLE RESTRICTIONS THE METHOD IS ACCESSIBLE
            if (this.PaymentMethodGroups.Count == 0) return true;
            //THERE ARE ROLE RESTRICTIONS, IF WE HAVE NO USER CONTEXT THE METHOD IS NOT ACCESSIBLE
            if (user == null) return false;
            //LOOP THE PAYMENT METHOD ROLES AND SEE IF THE USER HAS ANY
            foreach (PaymentMethodGroup pmr in this.PaymentMethodGroups)
            {
                //IF THE USER HAS THE SPECIFIED ROLE, THE METHOD IS ACCESSIBLE
                if (user.UserGroups.IndexOf(user.UserId, pmr.GroupId) > -1) return true;
            }
            //NO ROLE MATCHES WERE FOUND, METHOD IS NOT ACCESIBLE
            return false;
        }

        /// <summary>
        /// Indicates whether this method is recognized as a major debit or credit card.
        /// </summary>
        /// <returns>True if the payment instrument is a major debit or credit card.  False otherwise.</returns>
        /// <remarks>Instruments recognized as credit cards: American Express, Diner's Club, Discover, JCB, MasterCard, Visa</remarks>
        public bool IsCreditCard()
        {
            return ((this.PaymentInstrument == PaymentInstrument.AmericanExpress) || (this.PaymentInstrument == PaymentInstrument.DinersClub) || (this.PaymentInstrument == PaymentInstrument.Discover) || (this.PaymentInstrument == PaymentInstrument.JCB) || (this.PaymentInstrument == PaymentInstrument.MasterCard) || (this.PaymentInstrument == PaymentInstrument.Visa) || (this.PaymentInstrument == PaymentInstrument.CreditCard));
        }

        /// <summary>
        /// Indicates whether this method is recognizes as a credit or international debit card.
        /// </summary>
        /// <returns>True if the payment instrument is a major debit or credit card.  False otherwise.</returns>
        /// <remarks>Instruments recognized as credit cards: American Express, Diner's Club, Discover, JCB, Maestro, MasterCard, Switch/Solo, Visa, Visa (e.g. Delta and Electron)</remarks>
        public bool IsCreditOrDebitCard()
        {
            return (this.IsCreditCard() || this.IsIntlDebitCard());
        }

        /// <summary>
        /// Indicates whether this method is recognized as an international debit card.
        /// </summary>
        /// <returns>True if the payment instrument is an international debit card.  False otherwise.</returns>
        /// <remarks>Instruments recognized as international debit cards: Maestro, Switch/Solo, and Visa Debit (e.g. Delta and Electron)</remarks>
        public bool IsIntlDebitCard()
        {
            return ((this.PaymentInstrument == PaymentInstrument.Maestro) || (this.PaymentInstrument == PaymentInstrument.SwitchSolo) || (this.PaymentInstrument == PaymentInstrument.VisaDebit));
        }
    }
}
