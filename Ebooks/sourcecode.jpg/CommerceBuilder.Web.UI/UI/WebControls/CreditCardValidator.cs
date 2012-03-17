using System;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Web.UI.WebControls
{
    [Serializable]
    public enum CardType
    {
        Unknown = 0,
        Visa = 1,
        MasterCard = 2,
        Discover = 3,
        AmericanExpress = 4,
        JCB = 5
    }

	/// <summary>
	/// Credit Card Validation web control for ASP.NET.
	/// </summary>
	public class CreditCardValidator : Sample.Web.UI.Compatibility.BaseValidator
	{
     	private System.Web.UI.WebControls.TextBox _cardNumber;

		protected override bool ControlPropertiesValid()
		{
			// Should have a text box control to check
			Control ctrl = FindControl(ControlToValidate);
            WebTrace.Write("Found control to validate: " + (null!=ctrl));
			if ( null != ctrl )
			{
				if (ctrl is System.Web.UI.WebControls.TextBox)	// ensure its a text box
				{
					_cardNumber = (System.Web.UI.WebControls.TextBox) ctrl;	// set the member variable
                    WebTrace.Write("Control to validate is textbox: " + (null!=_cardNumber));
					return ( null != _cardNumber );		// check that it's been set ok
				} else
					return false;
			}
			else
				return false;
		}

		protected override bool EvaluateIsValid()
		{
            if (IsValidCardType(_cardNumber.Text))
            {
                WebTrace.Write("Valid Card Type Detected.  Accepted Card Type: " + AcceptedCardType.ToString());
                return ValidateCardNumber(_cardNumber.Text);
            }
            return false;
		}

		/// <summary>
		/// Will ensure that the card is a valid length for the card type. If the
		/// card type isn't recognised it will return true by default.
		/// 
		/// The CreditCardValidator control also includes a CardTypes property that determines
		/// what card types should be accepted. If the card isn't recognised, and the Unknown card
		/// type is in the AcceptedCardTypes property then the DefaultLengthTest value will be
		/// returned.
		/// </summary>
        public bool IsValidCardType(string cardNumber)
        {
            CardType cardType = (CardType)Enum.Parse(typeof(CardType), this.AcceptedCardType, true);
            switch (cardType)
            {
                case CardType.AmericanExpress:
                    // AMEX -- 34 or 37 -- 15 length
                    return ((cardNumber.Length == 15) && (Regex.IsMatch(cardNumber, "^(34|37)")));
                case CardType.MasterCard:
                    // MasterCard -- 51 through 55 -- 16 length
                    return ((cardNumber.Length == 16) && (Regex.IsMatch(cardNumber, "^(51|52|53|54|55)")));
                case CardType.Visa:
                    // VISA -- 4 -- 13 and 16 length
                    return ((cardNumber.Length == 13 || cardNumber.Length == 16) && (Regex.IsMatch(cardNumber, "^(4)")));
                case CardType.Discover:
                    // Discover -- 6011 -- 16 length
                    return ((cardNumber.Length == 16) && (Regex.IsMatch(cardNumber, "^(6011)")));
                case CardType.JCB:
                    // JCB -- 3 -- 16 length
                    // JCB -- 2131, 1800 -- 15 length
                    if (cardNumber.Length == 16) return (Regex.IsMatch(cardNumber, "^(3)"));
                    return ((cardNumber.Length == 15) && (Regex.IsMatch(cardNumber, "^(2131|1800)")));

            }
            return true;
        }

        /// <summary>
        /// Validates a credit card number using the standard Luhn/mod10
        /// validation algorithm.
        /// </summary>
        /// <param name="cardNumber">Card number, without punctuation or spaces.</param>
        /// <returns>True if card number appears valid, false if not
        /// </returns>
        public static bool ValidateCardNumber(string cardNumber)
        {
            try
            {
                if (String.IsNullOrEmpty(cardNumber)) return false;
                int cardLength = cardNumber.Length;
                if ((cardLength < 9) || (cardLength > 19)) return false;
                int multiplier = 1;
                int digit, sum, total = 0;
                for (int i = cardLength; i > 0; i--)
                {
                    digit = AlwaysConvert.ToInt(cardNumber.Substring(i - 1, 1));
                    sum = digit * multiplier;
                    if (sum > 9) sum -= 9;
                    total += sum;
                    multiplier = (multiplier == 1 ? 2 : 1);
                }
                return (total % 10 == 0);
            }
            catch
            {
                return false;
            }
        }

        private static int CalculateLastDigit(string cardNumber)
        {
            int cardLength = cardNumber.Length;
            int multiplier, digit, sum, total = 0;
            for (int i = 1; i <= cardLength; i++)
            {
                multiplier = 1 + (i % 2);
                digit = AlwaysConvert.ToInt(cardNumber.Substring(i - 1, 1));
                sum = digit * multiplier;
                if (sum > 9) sum -= 9;
                total += sum;
            }
            return (10 - (total % 10));
        }


        public static string GenerateRandomNumber(CardType cardType)
        {
            StringBuilder sb = new StringBuilder();
            switch (cardType)
            {
                case CardType.AmericanExpress:
                    // AMEX -- 34 or 37 -- 15 length
                    sb.Append("34");
                    sb.Append(StringHelper.RandomNumber(12));
                    sb.Append(CalculateLastDigit(sb.ToString()));
                    break;
                case CardType.MasterCard:
                    // MasterCard -- 51 through 55 -- 16 length
                    sb.Append("51");
                    sb.Append(StringHelper.RandomNumber(13));
                    sb.Append(CalculateLastDigit(sb.ToString()));
                    break;
                case CardType.Visa:
                    // VISA -- 4 -- 13 and 16 length
                    sb.Append("4");
                    sb.Append(StringHelper.RandomNumber(14));
                    sb.Append(CalculateLastDigit(sb.ToString()));
                    break;
                case CardType.Discover:
                    // Discover -- 6011 -- 16 length
                    sb.Append("6011");
                    sb.Append(StringHelper.RandomNumber(11));
                    sb.Append(CalculateLastDigit(sb.ToString()));
                    break;
                case CardType.JCB:
                    // JCB -- 3 -- 16 length
                    // JCB -- 2131, 1800 -- 15 length
                    sb.Append("3");
                    sb.Append(StringHelper.RandomNumber(14));
                    sb.Append(CalculateLastDigit(sb.ToString()));
                    break;
            }
            return sb.ToString();
        }

        public string AcceptedCardType
		{
			get
			{
                object vsCardType = this.ViewState["AcceptedCardType"];
                if (vsCardType != null)
                {
                    return (string)vsCardType;
                }
                return CardType.Unknown.ToString();
			}
			set
			{
                CardType cardType = (CardType)Enum.Parse(typeof(CardType), value, true);
                this.ViewState["AcceptedCardType"] = cardType.ToString();
            }
		}

    }
}
