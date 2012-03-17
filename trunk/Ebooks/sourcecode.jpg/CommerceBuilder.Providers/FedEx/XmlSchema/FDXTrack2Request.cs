﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.42.
// 
namespace FDXTrack2Request {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class FDXTrack2Request {
        
        private RequestHeader requestHeaderField;
        
        private PackageIdentifier packageIdentifierField;
        
        private string trackingNumberUniqueIdentifierField;
        
        private System.DateTime shipDateRangeBeginField;
        
        private bool shipDateRangeBeginFieldSpecified;
        
        private System.DateTime shipDateRangeEndField;
        
        private bool shipDateRangeEndFieldSpecified;
        
        private string shipmentAccountNumberField;
        
        private FDXTrack2RequestDestination destinationField;
        
        private FDXTrack2RequestLanguage languageField;
        
        private bool detailScansField;
        
        private bool detailScansFieldSpecified;
        
        private string pagingTokenField;
        
        /// <remarks/>
        public RequestHeader RequestHeader {
            get {
                return this.requestHeaderField;
            }
            set {
                this.requestHeaderField = value;
            }
        }
        
        /// <remarks/>
        public PackageIdentifier PackageIdentifier {
            get {
                return this.packageIdentifierField;
            }
            set {
                this.packageIdentifierField = value;
            }
        }
        
        /// <remarks/>
        public string TrackingNumberUniqueIdentifier {
            get {
                return this.trackingNumberUniqueIdentifierField;
            }
            set {
                this.trackingNumberUniqueIdentifierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="date")]
        public System.DateTime ShipDateRangeBegin {
            get {
                return this.shipDateRangeBeginField;
            }
            set {
                this.shipDateRangeBeginField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ShipDateRangeBeginSpecified {
            get {
                return this.shipDateRangeBeginFieldSpecified;
            }
            set {
                this.shipDateRangeBeginFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="date")]
        public System.DateTime ShipDateRangeEnd {
            get {
                return this.shipDateRangeEndField;
            }
            set {
                this.shipDateRangeEndField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ShipDateRangeEndSpecified {
            get {
                return this.shipDateRangeEndFieldSpecified;
            }
            set {
                this.shipDateRangeEndFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public string ShipmentAccountNumber {
            get {
                return this.shipmentAccountNumberField;
            }
            set {
                this.shipmentAccountNumberField = value;
            }
        }
        
        /// <remarks/>
        public FDXTrack2RequestDestination Destination {
            get {
                return this.destinationField;
            }
            set {
                this.destinationField = value;
            }
        }
        
        /// <remarks/>
        public FDXTrack2RequestLanguage Language {
            get {
                return this.languageField;
            }
            set {
                this.languageField = value;
            }
        }
        
        /// <remarks/>
        public bool DetailScans {
            get {
                return this.detailScansField;
            }
            set {
                this.detailScansField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DetailScansSpecified {
            get {
                return this.detailScansFieldSpecified;
            }
            set {
                this.detailScansFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public string PagingToken {
            get {
                return this.pagingTokenField;
            }
            set {
                this.pagingTokenField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RequestHeader {
        
        private string customerTransactionIdentifierField;
        
        private string accountNumberField;
        
        private string meterNumberField;
        
        private string clientProductIDField;
        
        private string clientProductVersionField;
        
        private string middlewareProductIDField;
        
        private string middlewareProductVersionField;
        
        private CarrierCode carrierCodeField;
        
        private bool carrierCodeFieldSpecified;
        
        private System.Xml.XmlElement[] anyField;
        
        /// <remarks/>
        public string CustomerTransactionIdentifier {
            get {
                return this.customerTransactionIdentifierField;
            }
            set {
                this.customerTransactionIdentifierField = value;
            }
        }
        
        /// <remarks/>
        public string AccountNumber {
            get {
                return this.accountNumberField;
            }
            set {
                this.accountNumberField = value;
            }
        }
        
        /// <remarks/>
        public string MeterNumber {
            get {
                return this.meterNumberField;
            }
            set {
                this.meterNumberField = value;
            }
        }
        
        /// <remarks/>
        public string ClientProductID {
            get {
                return this.clientProductIDField;
            }
            set {
                this.clientProductIDField = value;
            }
        }
        
        /// <remarks/>
        public string ClientProductVersion {
            get {
                return this.clientProductVersionField;
            }
            set {
                this.clientProductVersionField = value;
            }
        }
        
        /// <remarks/>
        public string MiddlewareProductID {
            get {
                return this.middlewareProductIDField;
            }
            set {
                this.middlewareProductIDField = value;
            }
        }
        
        /// <remarks/>
        public string MiddlewareProductVersion {
            get {
                return this.middlewareProductVersionField;
            }
            set {
                this.middlewareProductVersionField = value;
            }
        }
        
        /// <remarks/>
        public CarrierCode CarrierCode {
            get {
                return this.carrierCodeField;
            }
            set {
                this.carrierCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CarrierCodeSpecified {
            get {
                return this.carrierCodeFieldSpecified;
            }
            set {
                this.carrierCodeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    public enum CarrierCode {
        
        /// <remarks/>
        FDXE,
        
        /// <remarks/>
        FDXG,
        
        /// <remarks/>
        FDXC,
        
        /// <remarks/>
        FXCC,
        
        /// <remarks/>
        FXFR,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PackageIdentifier {
        
        private string valueField;
        
        private PackageIdentifierType typeField;
        
        private System.Xml.XmlElement[] anyField;
        
        /// <remarks/>
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
        
        /// <remarks/>
        public PackageIdentifierType Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public enum PackageIdentifierType {
        
        /// <remarks/>
        TRACKING_NUMBER_OR_DOORTAG,
        
        /// <remarks/>
        EXPRESS_MPS_MASTER,
        
        /// <remarks/>
        GROUND_SHIPMENT_ID,
        
        /// <remarks/>
        RMA,
        
        /// <remarks/>
        TCN,
        
        /// <remarks/>
        BOL,
        
        /// <remarks/>
        PARTNERCARNBR,
        
        /// <remarks/>
        REF,
        
        /// <remarks/>
        INV,
        
        /// <remarks/>
        PO,
        
        /// <remarks/>
        CUSTAUTHNUM,
        
        /// <remarks/>
        CUR,
        
        /// <remarks/>
        DEPARTMENT,
        
        /// <remarks/>
        PARTNUM,
        
        /// <remarks/>
        SHR,
        
        /// <remarks/>
        GOVBOL,
        
        /// <remarks/>
        COD,
        
        /// <remarks/>
        RETURNTOSHIPPER,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class FDXTrack2RequestDestination {
        
        private string destinationCountryCodeField;
        
        private string destinationPostalCodeField;
        
        /// <remarks/>
        public string DestinationCountryCode {
            get {
                return this.destinationCountryCodeField;
            }
            set {
                this.destinationCountryCodeField = value;
            }
        }
        
        /// <remarks/>
        public string DestinationPostalCode {
            get {
                return this.destinationPostalCodeField;
            }
            set {
                this.destinationPostalCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class FDXTrack2RequestLanguage {
        
        private string languageCodeField;
        
        private string localeCodeField;
        
        /// <remarks/>
        public string LanguageCode {
            get {
                return this.languageCodeField;
            }
            set {
                this.languageCodeField = value;
            }
        }
        
        /// <remarks/>
        public string LocaleCode {
            get {
                return this.localeCodeField;
            }
            set {
                this.localeCodeField = value;
            }
        }
    }
}