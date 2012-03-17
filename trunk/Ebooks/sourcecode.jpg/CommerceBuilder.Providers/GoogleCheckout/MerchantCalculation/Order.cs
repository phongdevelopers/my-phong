/*************************************************
 * Copyright (C) 2006 Google Inc.
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
using System.Xml;
using System.Collections;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout.MerchantCalculation {
  /// <summary>
  /// This class contains methods that parse a 
  /// &lt;merchant-calculation-callback&gt; request and creates an object 
  /// identifying the items in the customer's shopping cart. Your class that 
  /// inherits from CallbackRules.cs will receive an object of this type to 
  /// identify the items in the customer's order.
  /// </summary>
  public class Order {
    private ArrayList _OrderLines;
    private XmlNode[] _MerchantPrivateDataNodes = new XmlNode[] {};
    private AutoGen.ShoppingCart _ShoppingCart;
    private CommerceBuilder.Orders.Basket _AcBasket;
    private CommerceBuilder.Shipping.ShipMethodCollection _AcShipMethods;

    public Order(AutoGen.MerchantCalculationCallback Callback) {
       _ShoppingCart = Callback.shoppingcart;

      _OrderLines = new ArrayList();
      for (int i = 0; i < Callback.shoppingcart.items.Length; i++) {
        AutoGen.Item ThisItem = Callback.shoppingcart.items[i];

        XmlNode[] merchantItemPrivateDataNodes = new XmlNode[] {};

        if (ThisItem.merchantprivateitemdata != null 
          && ThisItem.merchantprivateitemdata.Any != null
          && ThisItem.merchantprivateitemdata.Any.Length > 0) {

          merchantItemPrivateDataNodes = ThisItem.merchantprivateitemdata.Any;
        }
        _OrderLines.Add(
          new OrderLine(ThisItem.itemname, ThisItem.itemdescription,
          ThisItem.quantity, ThisItem.unitprice.Value,
          ThisItem.taxtableselector,
          merchantItemPrivateDataNodes));
      }

      if (Callback.shoppingcart.merchantprivatedata != null
        && Callback.shoppingcart.merchantprivatedata.Any != null
        && Callback.shoppingcart.merchantprivatedata.Any.Length > 0) {
        
        _MerchantPrivateDataNodes = Callback.shoppingcart.merchantprivatedata.Any;
      }
    }

    public IEnumerator GetEnumerator() {
      return _OrderLines.GetEnumerator();
    }

    [Obsolete("This property must be converted to a XmlNode Array. Only the First XmlNode will be returned.")]
    public string MerchantPrivateData {
      get {
        if (_MerchantPrivateDataNodes != null
          && _MerchantPrivateDataNodes.Length > 0)
          return _MerchantPrivateDataNodes[0].OuterXml;
        
        return string.Empty;
      }
    }

    public System.Xml.XmlNode[] MerchantPrivateDataNodes {
      get {
        return _MerchantPrivateDataNodes;
      }
    }

      public AutoGen.ShoppingCart ShoppingCart
      {
          get { return _ShoppingCart; }
      }

      public CommerceBuilder.Orders.Basket AcBasket
      {
          get { return _AcBasket; }
          set { _AcBasket = value; }
      }

      public CommerceBuilder.Shipping.ShipMethodCollection AcShipMethods
      {
          get { return _AcShipMethods; }
          set { _AcShipMethods = value; }
      }

  }

}
