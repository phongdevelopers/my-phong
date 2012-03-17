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
using System.IO;
using System.Net;
using CommerceBuilder.Payments.Providers.GoogleCheckout.Util;

namespace CommerceBuilder.Payments.Providers.GoogleCheckout
{
  /// <summary>
  /// This class contains methods for sending API requests to Google Checkout.
  /// </summary>
  public abstract class GCheckoutRequest {
    protected string _MerchantID;
    protected string _MerchantKey;
    protected EnvironmentType _Environment = EnvironmentType.Sandbox;

    public abstract byte[] GetXml();

    private static string GetAuthorization(string User, string Password) {
      return Convert.ToBase64String(EncodeHelper.StringToUtf8Bytes(
        string.Format("{0}:{1}", User, Password)));
    }

    protected static EnvironmentType StringToEnvironment(string Env) {
      return (EnvironmentType)Enum.Parse(typeof(EnvironmentType), Env, true);
    }

    public GCheckoutResponse Send() {
      byte[] Data = GetXml();
      // Prepare web request.
      HttpWebRequest myRequest =
        (HttpWebRequest)WebRequest.Create(GetPostUrl());
      myRequest.Method = "POST";
      myRequest.ContentLength = Data.Length;
      myRequest.Headers.Add("Authorization",
        string.Format("Basic {0}",
        GetAuthorization(_MerchantID, _MerchantKey)));
      myRequest.ContentType = "application/xml; charset=UTF-8";
      myRequest.Accept = "application/xml; charset=UTF-8";
      myRequest.KeepAlive = false;
      // Send the data.
      using (Stream requestStream = myRequest.GetRequestStream()) {
        requestStream.Write(Data, 0, Data.Length);
      }

      //TODO : Log if debug mode enabled
      //string sw = System.Text.Encoding.UTF8.GetString(Data);
      //CommerceBuilder.Payments.Providers.PaymentProviderBase.RecordCommunication("GoogleCheckout", PaymentProviderBase.CommunicationDirection.Send, sw.ToString(), null);

      // Read the response.
      string responseXml = string.Empty;
      try {
        using (HttpWebResponse myResponse =
          (HttpWebResponse)myRequest.GetResponse()) {
          using (Stream ResponseStream = myResponse.GetResponseStream()) {
            using (StreamReader ResponseStreamReader =
              new StreamReader(ResponseStream)) {
              responseXml = ResponseStreamReader.ReadToEnd();
            }
          }
        }
      }
      catch (WebException WebExcp) {
        if (WebExcp.Response != null) {
          using (HttpWebResponse HttpWResponse =
            (HttpWebResponse)WebExcp.Response) {
            using (StreamReader sr =
              new StreamReader(HttpWResponse.GetResponseStream())) {
              responseXml = sr.ReadToEnd();
            }
          }
        }
      }

      //TODO : Log if debug mode enabled
      //CommerceBuilder.Payments.Providers.PaymentProviderBase.RecordCommunication("GoogleCheckout", PaymentProviderBase.CommunicationDirection.Receive, responseXml , null);
     
      return new GCheckoutResponse(responseXml);
    }

    public string GetPostUrl() {
      if (_Environment == EnvironmentType.Sandbox) {
        return string.Format(
          "https://sandbox.google.com/checkout/cws/v2/Merchant/{0}/request",
          _MerchantID);
      }
      else {
        return string.Format(
          "https://checkout.google.com/cws/v2/Merchant/{0}/request",
          _MerchantID);
      }
    }

    public string MerchantID {
      get {
        return _MerchantID;
      }
      set {
        _MerchantID = value;
      }
    }

    public string MerchantKey {
      get {
        return _MerchantKey;
      }
      set {
        _MerchantKey = value;
      }
    }

    public EnvironmentType Environment {
      get {
        return _Environment;
      }
      set {
        _Environment = value;
      }
    }

  }

  public enum EnvironmentType {
    Sandbox = 0,
    Production = 1,
    Unknown = 2
  }
}
