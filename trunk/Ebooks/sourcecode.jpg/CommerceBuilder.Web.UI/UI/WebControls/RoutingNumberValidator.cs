using System;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Text.RegularExpressions;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Web.UI.WebControls
{
	/// <summary>
	/// Routing number validation control for ASP.NET.
	/// </summary>
	public class RoutingNumberValidator : System.Web.UI.WebControls.BaseValidator
	{
     	private System.Web.UI.WebControls.TextBox _routingNumber;

		protected override bool ControlPropertiesValid()
		{
			// Should have a text box control to check
			Control ctrl = FindControl(ControlToValidate);
            WebTrace.Write("Found control to validate: " + (null!=ctrl));
			if ( null != ctrl )
			{
				if (ctrl is System.Web.UI.WebControls.TextBox)	// ensure its a text box
				{
					_routingNumber = (System.Web.UI.WebControls.TextBox) ctrl;	// set the member variable
                    WebTrace.Write("Control to validate is textbox: " + (null!=_routingNumber));
					return ( null != _routingNumber );		// check that it's been set ok
				} else
					return false;
			}
			else
				return false;
		}

		protected override bool EvaluateIsValid()
		{
            return ValidateRoutingNumber(_routingNumber.Text);
		}

        /// <summary>
        /// Validates a credit card number using the standard Luhn/mod10
        /// validation algorithm.
        /// </summary>
        /// <param name="routingNumber">Card number, without punctuation or spaces.</param>
        /// <returns>True if card number appears valid, false if not
        /// </returns>
        public bool ValidateRoutingNumber(string routingNumber)
        {
            try
            {
                if (String.IsNullOrEmpty(routingNumber)) return false;
                if (routingNumber.Length != 9) return false;
                int digit, sum = 0;
                for (int i = 0; i < routingNumber.Length; i+=3)
                {
                    digit = AlwaysConvert.ToInt(routingNumber.Substring(i, 1));
                    sum += digit * 3;
                    digit = AlwaysConvert.ToInt(routingNumber.Substring(i+1, 1));
                    sum += digit * 7;
                    digit = AlwaysConvert.ToInt(routingNumber.Substring(i+2, 1));
                    sum += digit;
                }
                WebTrace.Write("sum: " + sum);
                return ((sum > 0) && (sum % 10 == 0));
            }
            catch
            {
                return false;
            }
        }

    }
}
