using System;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Web.UI.WebControls;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Web.UI.WebControls
{
	public class ExpirationDropDownValidator : Sample.Web.UI.Compatibility.BaseValidator
	{
        private string _monthControlToValidate;
        private string _yearControlToValidate;
        private System.Web.UI.WebControls.DropDownList _monthDropDown;
        private System.Web.UI.WebControls.DropDownList _yearDropDown;

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        protected new string ControlToValidate
        {
            get
            {
                return string.Empty;
            }
            set
            {
                throw new ArgumentException("ControlToValidate is unsupported.  Set MonthControlToValidate and YearControlToValidate to valid DropDownList controls.");
            }
        }

        [TypeConverter(typeof(ValidatedControlConverter)), IDReferenceProperty, Category("Behavior"), Themeable(false), DefaultValue(""), Description("ID of the month dropdown control to validate.")]
        public string MonthControlToValidate
        {
            get
            {
                return _monthControlToValidate;
            }
            set
            {
                _monthControlToValidate = value;
            }
        }

        [TypeConverter(typeof(ValidatedControlConverter)), IDReferenceProperty, Category("Behavior"), Themeable(false), DefaultValue(""), Description("ID of the year dropdown control to validate.")]
        public string YearControlToValidate
        {
            get
            {
                return _yearControlToValidate;
            }
            set
            {
                _yearControlToValidate = value;
            }
        }

        protected override bool ControlPropertiesValid()
        {
            //FIND THE MONTH CONTROL
            Control monthControl = FindControl(MonthControlToValidate);
            if ((monthControl != null) && (monthControl is System.Web.UI.WebControls.DropDownList))
            {
                _monthDropDown = (System.Web.UI.WebControls.DropDownList)monthControl;
                //FIND THE year CONTROL
                Control yearControl = FindControl(YearControlToValidate);
                if ((yearControl != null) && (yearControl is System.Web.UI.WebControls.DropDownList))
                {
                    _yearDropDown = (System.Web.UI.WebControls.DropDownList)yearControl;
                    return true;
                }
            }
            return false;
        }

		protected override bool EvaluateIsValid()
		{
            int thisYear = LocaleHelper.LocalNow.Year;
            int thisMonth = LocaleHelper.LocalNow.Month;
            int selectedYear = AlwaysConvert.ToInt(_yearDropDown.SelectedValue);
            int selectedMonth = AlwaysConvert.ToInt(_monthDropDown.SelectedValue);
            if (selectedYear > thisYear) return ((selectedMonth > 0) && (selectedMonth < 13));
            if (selectedYear == thisYear) return ((selectedMonth >= thisMonth) && (selectedMonth < 13));
            return false;
		}

    }
}
