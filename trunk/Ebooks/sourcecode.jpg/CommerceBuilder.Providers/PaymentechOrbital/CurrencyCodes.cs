using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Payments.Providers.PaymentechOrbital
{
    public class CurrencyCodes
    {
        public static CurrencyData[] Items;

        static CurrencyCodes()
        {
            Items = new CurrencyData[12];

            Items[0] = new CurrencyData();
            Items[0].ISOCode = "USD";
            Items[0].numericCode = 840;
            Items[0].exponent = 2;

            Items[1] = new CurrencyData(); 
            Items[1].ISOCode = "CAD";
            Items[1].numericCode = 124;
            Items[1].exponent = 2;

            Items[2] = new CurrencyData(); 
            Items[2].ISOCode = "EUR";
            Items[2].numericCode = 978;
            Items[2].exponent = 2;

            Items[3] = new CurrencyData(); 
            Items[3].ISOCode = "GBP";
            Items[3].numericCode = 826;
            Items[3].exponent = 2;

            Items[4] = new CurrencyData(); 
            Items[4].ISOCode = "AUD";
            Items[4].numericCode = 036;
            Items[4].exponent = 2;

            Items[5] = new CurrencyData(); 
            Items[5].ISOCode = "JPY";
            Items[5].numericCode = 392;
            Items[5].exponent = 0;

            Items[6] = new CurrencyData(); 
            Items[6].ISOCode = "CNY";
            Items[6].numericCode = 156;
            Items[6].exponent = 2;

            Items[7] = new CurrencyData(); 
            Items[7].ISOCode = "HKD";
            Items[7].numericCode = 344;
            Items[7].exponent = 2;

            Items[8] = new CurrencyData(); 
            Items[8].ISOCode = "CHF";
            Items[8].numericCode = 756;
            Items[8].exponent = 2;

            Items[9] = new CurrencyData(); 
            Items[9].ISOCode = "INR";
            Items[9].numericCode = 356;
            Items[9].exponent = 2;

            Items[10] = new CurrencyData(); 
            Items[10].ISOCode = "RUB ";
            Items[10].numericCode = 643;
            Items[10].exponent = 2;

            Items[11] = new CurrencyData(); 
            Items[11].ISOCode = "NZD ";
            Items[11].numericCode = 554;
            Items[11].exponent = 2;
          
        }

        public static int GetExponent(int currencyCode)
        {
            if (currencyCode == 392) return 0;
            return 2;
        }
    }

    public class CurrencyData
    {
        public string ISOCode;
        public int numericCode;
        public int exponent;
    }
}
