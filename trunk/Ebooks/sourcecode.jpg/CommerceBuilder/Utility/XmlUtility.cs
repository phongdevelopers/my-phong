using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Utility class for XML processing
    /// </summary>
    public static class XmlUtility
    {
        /// <summary>
        /// Creates an XML element for the given xml document 
        /// </summary>
        /// <param name="xmlDoc">XML document to create the XML element for</param>
        /// <param name="sXPath">Path of the new element</param>
        /// <returns>Newly created element</returns>
        static public XmlElement CreateElement(XmlNode xmlDoc, string sXPath)
        {
            XmlElement returnValue;
            XmlNode objNode = null;
            XmlNode objNextChild;
            string[] arrPath;
            int i = 0;
            string sCurrentPath = "";
            if (sXPath.Length > 0)
            {
                arrPath = sXPath.Split('/');
                objNode = xmlDoc;
                do
                {
                    if (i > 0)
                    {
                        sCurrentPath = sCurrentPath + "/" + arrPath[i];
                    }
                    else
                    {
                        sCurrentPath = arrPath[i];
                    }
                    objNextChild = xmlDoc.SelectSingleNode(sCurrentPath);
                    if (objNextChild == null)
                    {
                        objNode = objNode.AppendChild(objNode.OwnerDocument.CreateElement("" + arrPath[i]));
                    }
                    else
                    {
                        objNode = objNextChild;
                    }
                    i++;
                } while (!(i > (arrPath.Length - 1) || (objNode == null)));
            }
            objNextChild = null;
            returnValue = (XmlElement)objNode;
            objNode = null;
            return returnValue;
        }

        /// <summary>
        /// Gets an XmlElement object for the given XPath expression from the XML document
        /// </summary>
        /// <param name="xmlDoc">XML document to get the XmlElement object from</param>
        /// <param name="sXPath">Path of the element to get from the xml document</param>
        /// <param name="bCreate">if <b>true</b> the element will be created if it does not already exist</param>
        /// <returns></returns>
        static public XmlElement GetElement(XmlNode xmlDoc, string sXPath, bool bCreate)
        {
            XmlNode objNode;
            if ((sXPath.Length > 0) && (xmlDoc != null))
            {
                objNode = xmlDoc.SelectSingleNode(sXPath);
                if (objNode != null)
                {
                    return ((XmlElement)objNode);
                }
                else
                {
                    if (bCreate)
                    {
                        return CreateElement(xmlDoc, sXPath);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets text value from the given XmlElement for node specified by the given XPath expression
        /// </summary>
        /// <param name="element">XML element to look into</param>
        /// <param name="xPath">XPath expression identifying the node to get value for</param>
        /// <returns>Text value for the required node</returns>
        static public string GetElementValue(XmlElement element, string xPath)
        {
            return XmlUtility.GetElementValue(element, xPath, null, string.Empty);
        }

        /// <summary>
        /// Gets text value from the given XmlNode for node specified by the given XPath expression
        /// </summary>
        /// <param name="element">XML node to look into</param>
        /// <param name="xPath">XPath expression identifying the node to get value for</param>
        /// <param name="defaultValue">The default value to return if the specified node could not be found</param>
        /// <returns>Text value for the required node</returns>
        static public string GetElementValue(XmlNode element, string xPath, string defaultValue)
        {
            return XmlUtility.GetElementValue((XmlElement)element, xPath, null, defaultValue);
        }

        /// <summary>
        /// Gets text value from the given XmlElement for node specified by the given XPath expression
        /// </summary>
        /// <param name="element">XML element to look into</param>
        /// <param name="xPath">XPath expression identifying the node to get value for</param>
        /// <param name="defaultValue">The default value to return if the specified node could not be found</param>
        /// <returns>Text value for the required node</returns>
        static public string GetElementValue(XmlElement element, string xPath, string defaultValue)
        {
            return XmlUtility.GetElementValue(element, xPath, null, defaultValue);
        }

        /// <summary>
        /// Gets text value from the given XmlElement for node specified by the given XPath expression
        /// </summary>
        /// <param name="element">XML element to look into</param>
        /// <param name="xPath">XPath expression identifying the node to get value for</param>
        /// <param name="nsmgr">XML namespace to use</param>
        /// <returns>Text value for the required node</returns>
        static public string GetElementValue(XmlElement element, string xPath, XmlNamespaceManager nsmgr)
        {
            return XmlUtility.GetElementValue(element, xPath, nsmgr, string.Empty);
        }

        /// <summary>
        /// Gets text value from the given XmlElement for node specified by the given XPath expression
        /// </summary>
        /// <param name="element">XML element to look into</param>
        /// <param name="xPath">XPath expression identifying the node to get value for</param>
        /// <param name="nsmgr">XML namespace to use</param>
        /// <param name="defaultValue">The default value to return if the specified node could not be found</param>
        /// <returns>Text value for the required node</returns>
        static public string GetElementValue(XmlElement element, string xPath, XmlNamespaceManager nsmgr, string defaultValue)
        {
            XmlElement childElement;
            childElement = element.SelectSingleNode(xPath, nsmgr) as XmlElement;
            if (childElement != null) return childElement.InnerText;
            return defaultValue;
        }
        
        /// <summary>
        /// Sets text value of the node specified by the given XPath expression in the given XML document
        /// </summary>
        /// <param name="xmlDoc">XML document to set value in</param>
        /// <param name="sXPath">XPath expression identifying the node to set value for</param>
        /// <param name="sValue">The value to set</param>
        static public void SetElementValue(XmlNode xmlDoc, string sXPath, string sValue)
        {
            SetElementValue(xmlDoc, sXPath, sValue, true, true);
        }

        /// <summary>
        /// Sets text value of the node specified by the given XPath expression in the given XML document
        /// </summary>
        /// <param name="xmlDoc">XML document to set value in</param>
        /// <param name="sXPath">XPath expression identifying the node to set value for</param>
        /// <param name="sValue">The value to set</param>
        /// <param name="bCreate">If <b>true</b>, a new node will be created if it does not exist already</param>
        static public void SetElementValue(XmlNode xmlDoc, string sXPath, string sValue, bool bCreate)
        {
            SetElementValue(xmlDoc, sXPath, sValue, bCreate, true);
        }

        /// <summary>
        /// Sets text value of the node specified by the given XPath expression in the given XML document
        /// </summary>
        /// <param name="xmlDoc">XML document to set value in</param>
        /// <param name="sXPath">XPath expression identifying the node to set value for</param>
        /// <param name="sValue">The value to set</param>
        /// <param name="bCreate">If <b>true</b>, a new node will be created if it does not exist already</param>
        /// <param name="bCreateIfEmpty">If <b>true</b>, a new node will be created if the existing node is empty</param>
        static public void SetElementValue(XmlNode xmlDoc, string sXPath, string sValue, bool bCreate, bool bCreateIfEmpty)
        {
            XmlElement objElement;
            objElement = GetElement(xmlDoc, sXPath, System.Convert.ToBoolean((sValue.Length == 0) ? (bCreate && bCreateIfEmpty) : bCreate));
            if (objElement != null)
            {
                objElement.InnerText = sValue;
                objElement = null;
            }
        }

        /// <summary>
        /// Gets value of the specified attribute from the given XML document
        /// </summary>
        /// <param name="xmlDoc">XML document to get the attribute value from</param>
        /// <param name="sAttribute">The name of the attribute to get value for</param>
        /// <param name="sDefault">The default value to return if the attribute is not found</param>
        /// <returns>Value of the specified attribute</returns>
        static public string GetAttributeValue(XmlNode xmlDoc, string sAttribute, string sDefault)
        {
            if (xmlDoc == null)
            {
                return sDefault;
            }
            else
            {
                return GetAttributeValue(((XmlElement)xmlDoc), sAttribute, sDefault);
            }
        }

        /// <summary>
        /// Gets value of the specified attribute from the given XML element
        /// </summary>
        /// <param name="xmlElem">XML element to get the attribute value from</param>
        /// <param name="sAttribute">The name of the attribute to get value for</param>
        /// <param name="sDefault">The default value to return if the attribute is not found</param>
        /// <returns>Value of the specified attribute</returns>
        static public string GetAttributeValue(XmlElement xmlElem, string sAttribute, string sDefault)
        {
            string returnValue;
            if (xmlElem.HasAttribute(sAttribute))
            {
                returnValue = xmlElem.GetAttribute(sAttribute);
            }
            else
            {
                returnValue = sDefault;
            }
            return returnValue;
        }

        /// <summary>
        /// Sets value of the specified attribute in the given XML document
        /// </summary>
        /// <param name="xmlDoc">XML document to set attribute value in</param>
        /// <param name="sAttribute">The name of the attribute to set value for</param>
        /// <param name="sValue">The value to set</param>
        static public void SetAttributeValue(XmlNode xmlDoc, string sAttribute, string sValue)
        {
            ((XmlElement)xmlDoc).SetAttribute(sAttribute, sValue);
        }

        /// <summary>
        /// Sets value of the specified attribute in the given XML element
        /// </summary>
        /// <param name="xmlElem">XML element to set attribute value in</param>
        /// <param name="sAttribute">The name of the attribute to set value for</param>
        /// <param name="sValue">The value to set</param>
        static public void SetAttributeValue(XmlElement xmlElem, string sAttribute, string sValue)
        {
            xmlElem.SetAttribute(sAttribute, sValue);
        }

        /// <summary>
        /// Dumps the given xml node in key-value pairs
        /// </summary>
        /// <param name="objNode">The XML node to get the key-value pairs</param>
        /// <returns>Key-value paris for the given XML node</returns>
        public static System.Collections.Specialized.StringDictionary XML2Dic(XmlNode objNode)
        {
            //recursive function to dump xml document into key_value pairs
            System.Collections.Specialized.StringDictionary dicContents = new System.Collections.Specialized.StringDictionary();
            R_XML2Dic(objNode, "", ref dicContents);
            return dicContents;
        }
        
        private static void R_XML2Dic(XmlNode objNode, string sPrefix, ref System.Collections.Specialized.StringDictionary dicContents)
        {
            //recursive function to dump xml document into key_value pairs
            XmlNode objChildNode;
            string sNodeName;
            foreach (XmlNode tempLoopVar_objChildNode in objNode.ChildNodes)
            {
                objChildNode = tempLoopVar_objChildNode;
                if (objChildNode.Name == "#text")
                {
                    sNodeName = sPrefix.Substring(0, sPrefix.Length - 1);
                    if (!dicContents.ContainsKey(sNodeName))
                    {
                        dicContents.Add(sNodeName, objChildNode.Value);
                    }
                }
                if (objChildNode.ChildNodes.Count > 0)
                {
                    R_XML2Dic(objChildNode, sPrefix + objChildNode.Name + "_", ref dicContents);
                }
            }
        }

        /// <summary>
        /// Dumps the given xml document in key-value pairs
        /// </summary>
        /// <param name="sXMLDoc">XML document as string</param>
        /// <returns>Key-value paris for the given XML</returns>
        public static System.Collections.Specialized.StringDictionary XML2Dic(string sXMLDoc)
        {
            //recursive function to dump xml document into key_value pairs
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sXMLDoc);
            return XML2Dic(xmlDoc);
        }


        /// <summary>
        /// Converts a string to bytes in UTF-8 encoding.
        /// </summary>
        /// <param name="InString">The string to convert.</param>
        /// <returns>The UTF-8 bytes.</returns>
        public static byte[] StringToUtf8Bytes(string InString)
        {
            UTF8Encoding utf8encoder = new UTF8Encoding(false, true);
            return utf8encoder.GetBytes(InString);
        }
        
        /// <summary>
        /// Converts a string to bytes in ISO-8859-1 encoding.
        /// </summary>
        /// <param name="InString">The string to convert</param>
        /// <returns>The ISO-8859-1 bytes</returns>
        public static byte[] StringToIso88591Bytes(string InString)
        {
            Encoding encoder = System.Text.Encoding.GetEncoding("ISO-8859-1");
            return encoder.GetBytes(InString);
        }
        
        /// <summary>
        /// Converts bytes in UTF-8 encoding to a regular string.
        /// </summary>
        /// <param name="InBytes">The UTF-8 bytes.</param>
        /// <returns>The input bytes as a string.</returns>
        public static string Utf8BytesToString(byte[] InBytes)
        {
            UTF8Encoding utf8encoder = new UTF8Encoding(false, true);
            return utf8encoder.GetString(InBytes);
        }

        /// <summary>
        /// Converts bytes in ISO-8859-1 encoding to a regular string.
        /// </summary>
        /// <param name="InBytes">The ISO-8859-1 bytes</param>
        /// <returns>The input bytes as a string.</returns>
        public static string Iso88591BytesToString(byte[] InBytes)
        {
            Encoding encoder = System.Text.Encoding.GetEncoding("ISO-8859-1");
            return encoder.GetString(InBytes);
        }

        /// <summary>
        /// Converts UTF8-encoded bytes from a stream to a string.
        /// </summary>
        /// <param name="Utf8Stream">The UTF8 stream.</param>
        /// <returns>
        /// The full stream contents as a string. Also closes the stream.
        /// </returns>
        public static string Utf8StreamToString(Stream Utf8Stream)
        {
            using (StreamReader SReader =
              new StreamReader(Utf8Stream, Encoding.UTF8))
            {
                return SReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Converts ISO-8859-1-encoded bytes from a stream to a string.
        /// </summary>
        /// <param name="stream">he ISO-8859-1 stream.</param>
        /// <returns>The full stream contents as a string. Also closes the stream.</returns>
        public static string Iso88591StreamToString(Stream stream)
        {
            using (StreamReader SReader =
              new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1")))
            {
                return SReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Gets the top element of an XML string.
        /// </summary>
        /// <param name="Xml">
        /// The XML string to extract the top element from.
        /// </param>
        /// <returns>
        /// The name of the first regular XML element.
        /// </returns>
        /// <example>
        /// </example>
        public static string GetTopElement(string Xml)
        {
            using (StringReader SReader = new StringReader(Xml))
            {
                XmlTextReader XReader = new XmlTextReader(SReader);
                XReader.WhitespaceHandling = WhitespaceHandling.None;
                XReader.Read();
                XReader.Read();
                string RetVal = XReader.Name;
                XReader.Close();
                return RetVal;
            }
        }

        /// <summary>
        /// Gets the value of the first element or attribute in a piece of XML.
        /// </summary>
        /// <param name="Xml">
        /// The XML to extract the element or attribute value from.
        /// </param>
        /// <param name="Element">
        /// Name of the element or attribute to search for.
        /// </param>
        /// <returns>
        /// The value of the first element or attribute. If there is no such
        /// element or attribute in the XML, an empty string is returned.
        /// </returns>
        /// <example>
        /// </example>
        public static string GetElementValue(string Xml, string Element)
        {
            string RetVal = string.Empty;
            using (StringReader SReader = new StringReader(Xml))
            {
                XmlTextReader XReader = new XmlTextReader(SReader);
                XReader.WhitespaceHandling = WhitespaceHandling.None;
                XReader.Read();
                while (!XReader.EOF)
                {
                    if (XReader.Name == Element)
                    {
                        XReader.Read();
                        RetVal = XReader.Value;
                        break;
                    }
                    if (XReader.MoveToAttribute(Element))
                    {
                        RetVal = XReader.Value;
                        break;
                    }
                    XReader.Read();
                }
            }
            return RetVal;
        }

        /// <summary>
        /// Escapes XML characters &lt; &gt; and &amp;.
        /// </summary>
        /// <param name="In">
        /// String that could contain &lt; &gt; and &amp; characters.
        /// </param>
        /// <returns>
        /// A new string where 
        /// <b>&amp;</b> has been replaced by <b>&amp;#x26;</b>,
        /// <b>&lt;</b> has been replaced by <b>&amp;#x3c;</b> and
        /// <b>&gt;</b> has been replaced by <b>&amp;#x3e;</b>.
        /// </returns>
        public static string EscapeXmlChars(string In)
        {
            string RetVal = In;
            RetVal = RetVal.Replace("&", "&#x26;");
            RetVal = RetVal.Replace("<", "&#x3c;");
            RetVal = RetVal.Replace(">", "&#x3e;");
            return RetVal;
        }

        /// <summary>
        /// Makes an object out of the specified XML.
        /// </summary>
        /// <param name="Xml">The XML that should be made into an object.</param>
        /// <param name="ThisType">
        /// Type of object that produced the XML. 
        /// </param>
        /// <returns>The reconstituted object.</returns>
        /// <example>
        /// <code>
        /// Car MyCar1 = new Car();
        /// byte[] CarBytes = XmlUtility.Serialize(MyCar1);
        /// string CarXml = XmlUtility.Utf8BytesToString(CarBytes);
        /// Car MyCar2 = (Car) Deserialize(CarXml, typeof(Car));
        /// // MyCar2 is now a copy of MyCar1.
        /// </code>
        /// </example>
        public static object Deserialize(string Xml, Type ThisType)
        {
            XmlSerializer myDeserializer = new XmlSerializer(ThisType);
            using (StringReader myReader = new StringReader(Xml))
            {
                return myDeserializer.Deserialize(myReader);
            }
        }

        /// <summary>
        /// Makes XML out of an object.
        /// </summary>
        /// <param name="ObjectToSerialize">The object to serialize.</param>
        /// <returns>An XML string representing the object.</returns>
        /// <example>
        /// <code>
        /// Car MyCar1 = new Car();
        /// byte[] CarBytes = EncodeHelper.Serialize(MyCar1);
        /// string CarXml = XmlUtility.Utf8BytesToString(CarBytes);
        /// Car MyCar2 = (Car) Deserialize(CarXml, typeof(Car));
        /// // MyCar2 is now a copy of MyCar1.
        /// </code>
        /// </example>
        public static byte[] Serialize(object ObjectToSerialize)
        {
            XmlSerializer Ser = new XmlSerializer(ObjectToSerialize.GetType());
            using (MemoryStream MS = new MemoryStream())
            {                
                XmlTextWriter W = new XmlTextWriter(MS, new UTF8Encoding(false));
                W.Formatting = Formatting.Indented;                
                Ser.Serialize(W, ObjectToSerialize);
                W.Flush();
                W.Close();
                return MS.ToArray();
            }
        }

        /// <summary>
        /// Makes XML out of an object.
        /// </summary>
        /// <param name="ObjectToSerialize">The object to serialize.</param>
        /// <param name="encoding">The encoding to use for XML document</param>
        /// <returns>An XML string representing the object.</returns>
        public static byte[] Serialize(object ObjectToSerialize, Encoding encoding)
        {
            XmlSerializer Ser = new XmlSerializer(ObjectToSerialize.GetType());
            using (MemoryStream MS = new MemoryStream())
            {
                XmlTextWriter W = new XmlTextWriter(MS, encoding);
                W.Formatting = Formatting.Indented;
                Ser.Serialize(W, ObjectToSerialize);
                W.Flush();
                W.Close();
                return MS.ToArray();
            }
        }

        /// <summary>
        /// Outputs an XML document to string
        /// </summary>
        /// <param name="xmlDocument">The xml document</param>
        /// <returns>The string representation</returns>
        public static string XmlToString(XmlDocument xmlDocument)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Encoding utf8 = new UTF8Encoding(false);
                using (XmlTextWriter writer = new XmlTextWriter(ms, utf8))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.IndentChar = '\t';
                    writer.Indentation = 1;
                    xmlDocument.Save(writer);
                }
                return utf8.GetString(ms.ToArray());
            }
        }
    }
}
