/*************************************************
 * Copyright (C) 2006-2007 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*************************************************/

using System;
using CommerceBuilder.Common;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.MerchantCalculation {
  /// <summary>
  /// This class contains a sample class that inherits from CallbackRules.cs. 
  /// This class demonstrates how you could subclass CallbackRules.cs to 
  /// define your own merchant-calculated shipping, tax and coupon options.
  /// </summary>
  public class ExampleRules : CallbackRules {
    private const string SAVE10 = "SAVE10";
    private const string SAVE20 = "SAVE20";
    private const string GIFTCERT = "GIFTCERT";

    public override MerchantCodeResult GetMerchantCodeResult(Order ThisOrder, 
      AnonymousAddress Address, string MerchantCode) {
      MerchantCodeResult RetVal = new MerchantCodeResult();
      if (MerchantCode.ToUpper() == SAVE10) {
        RetVal.Amount = 10;
        RetVal.Type = MerchantCodeType.Coupon;
        RetVal.Valid = true;
        RetVal.Message = "You saved $10!";
      }
      else if (MerchantCode.ToUpper() == SAVE20) {
        RetVal.Amount = 20;
        RetVal.Type = MerchantCodeType.Coupon;
        RetVal.Valid = true;
        RetVal.Message = "You saved $20!";
      }
      else if (MerchantCode.ToUpper() == GIFTCERT) {
        RetVal.Amount = 23.46m;
        RetVal.Type = MerchantCodeType.GiftCertificate;
        RetVal.Valid = true;
        RetVal.Message = "Your gift certificate has a balance of $23.46.";
      }
      else {
        RetVal.Message = "Sorry, we didn't recognize code '" + MerchantCode + 
          "'.";
      }
      return RetVal;
    }

    public override LSDecimal GetTaxResult(Order ThisOrder, 
      AnonymousAddress Address, LSDecimal ShippingRate) {
      LSDecimal RetVal = 0;
      if (Address.Region == "HI") {
        LSDecimal Total = 0;
        foreach (OrderLine Line in ThisOrder) {
          Total += Line.UnitPrice * Line.Quantity;
        }
        RetVal = Decimal.Round((Decimal)Total * 0.09m, 2);
      }
      return RetVal;
    }

    public override ShippingResult GetShippingResult(string ShipMethodName, 
      Order ThisOrder, AnonymousAddress Address) {
      ShippingResult RetVal = new ShippingResult();
      if (ShipMethodName == "UPS Ground" && Address.Region != "HI" && 
        Address.Region != "AL") {
        RetVal.Shippable = true;
        RetVal.ShippingRate = 20;
      }
      else if (ShipMethodName == "SuperShip") {
        RetVal.Shippable = true;
        RetVal.ShippingRate = 0;
      }
      else {
        //next we will look at the merchant-private-item-data
        //if the supplier-id is "ABC Candy Company" then you will get free shipping.
        //do not assume the nodes will be in the same order, we will walk the node
        //list looking for a node with the name of "supplier-id"

        //you can just as easily import all the nodes into an XmlDocument and perform
        //XPath statements.

        //You can also create a string dictionary by performing a foreach statement
        //on the nodes and using the node name as the key and the innerText as the
        //value.

        //We are just showing one of many ways to work with an array of XmlNodes.
        //As you can see from the sample code, you may also have children on any
        //of the MerchantPrivateDataNodes.

        string supplierID = "ABC Candy Company".ToUpper();

        foreach (OrderLine Line in ThisOrder) {
          foreach (System.Xml.XmlNode node in Line.MerchantPrivateDataNodes) {
            if (node.Name == "supplier-id") {
              if (supplierID == node.InnerText.ToUpper()) {
                RetVal.Shippable = true;
                RetVal.ShippingRate = 0;
                break;
              }
            }
          }
        }
      }
      return RetVal;
    }

  }
}
