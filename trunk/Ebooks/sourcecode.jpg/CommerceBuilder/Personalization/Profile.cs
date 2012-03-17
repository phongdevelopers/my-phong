using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using System.Security.Permissions;

namespace CommerceBuilder.Personalization
{
    /// <summary>
    /// Class that represents a user profile
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public partial class Profile
    {
        /// <summary>
        /// Whether this profile has any properties
        /// </summary>
        public bool HasProperties
        {
            get { return (this.PropertyNames.Length > 0); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyValues"></param>
        public void GetPropertyValues(SettingsPropertyValueCollection propertyValues)
        {
            string[] names = (string.IsNullOrEmpty(this.PropertyNames) ? null : this.PropertyNames.Split(":".ToCharArray()));
            string values = this.PropertyValuesString;
            byte[] binaryValues = this.PropertyValuesBinary;
            if ((names != null) && ((values != null) || (binaryValues != null)) && (propertyValues != null))
            {
                try
                {
                    for (int i = 0; i < (names.Length / 4); i++)
                    {
                        string propertyName = names[i * 4];
                        SettingsPropertyValue propertyValue = propertyValues[propertyName];
                        if (propertyValue != null)
                        {
                            int startIndex = int.Parse(names[(i * 4) + 2], CultureInfo.InvariantCulture);
                            int dataLength = int.Parse(names[(i * 4) + 3], CultureInfo.InvariantCulture);
                            if ((dataLength == -1) && !propertyValue.Property.PropertyType.IsValueType)
                            {
                                propertyValue.PropertyValue = null;
                                propertyValue.IsDirty = false;
                                propertyValue.Deserialized = true;
                            }
                            if (((names[(i * 4) + 1] == "S") && (startIndex >= 0)) && ((dataLength > 0) && (values.Length >= (startIndex + dataLength))))
                            {
                                propertyValue.SerializedValue = values.Substring(startIndex, dataLength);
                            }
                            if (((names[(i * 4) + 1] == "B") && (startIndex >= 0)) && ((dataLength > 0) && (binaryValues.Length >= (startIndex + dataLength))))
                            {
                                byte[] tempBuffer = new byte[dataLength];
                                Buffer.BlockCopy(binaryValues, startIndex, tempBuffer, 0, dataLength);
                                propertyValue.SerializedValue = tempBuffer;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyValues"></param>
        /// <param name="userIsAuthenticated"></param>
        public void SetPropertyValues(SettingsPropertyValueCollection propertyValues, bool userIsAuthenticated)
        {
            StringBuilder propertyNamesBuilder = new StringBuilder();
            StringBuilder propertyValuesStringBuilder = new StringBuilder();
            MemoryStream stream1 = new MemoryStream();
            bool binarySupported = true;
            try
            {
                try
                {
                    bool isDirty = false;
                    foreach (SettingsPropertyValue thisValue in propertyValues)
                    {
                        if (!thisValue.IsDirty || (!userIsAuthenticated && !((bool)thisValue.Property.Attributes["AllowAnonymous"])) || thisValue.Property.IsReadOnly)
                        {
                            continue;
                        }
                        isDirty = true;
                        break;
                    }
                    if (!isDirty)
                    {
                        return;
                    }
                    foreach (SettingsPropertyValue thisValue in propertyValues)
                    {
                        if ((!userIsAuthenticated && !((bool)thisValue.Property.Attributes["AllowAnonymous"])) || (!thisValue.IsDirty && thisValue.UsingDefaultValue) || thisValue.Name.Equals("User") || thisValue.Property.IsReadOnly)
                        {
                            continue;
                        }
                        int num1 = 0;
                        int num2 = 0;
                        string text1 = null;
                        if (thisValue.Deserialized && (thisValue.PropertyValue == null))
                        {
                            num1 = -1;
                        }
                        else
                        {
                            object obj1 = thisValue.SerializedValue;
                            if (obj1 == null)
                            {
                                num1 = -1;
                            }
                            else
                            {
                                if (!(obj1 is string) && !binarySupported)
                                {
                                    obj1 = Convert.ToBase64String((byte[])obj1);
                                }
                                if (obj1 is string)
                                {
                                    text1 = (string)obj1;
                                    num1 = text1.Length;
                                    num2 = propertyValuesStringBuilder.Length;
                                }
                                else
                                {
                                    byte[] buffer1 = (byte[])obj1;
                                    num2 = (int)stream1.Position;
                                    stream1.Write(buffer1, 0, buffer1.Length);
                                    stream1.Position = num2 + buffer1.Length;
                                    num1 = buffer1.Length;
                                }
                            }
                        }
                        propertyNamesBuilder.Append(string.Concat(new string[] { thisValue.Name, ":", (text1 != null) ? "S" : "B", ":", num2.ToString(CultureInfo.InvariantCulture), ":", num1.ToString(CultureInfo.InvariantCulture), ":" }));
                        if (text1 != null)
                        {
                            propertyValuesStringBuilder.Append(text1);
                        }
                    }
                    if (binarySupported)
                    {
                        this.PropertyValuesBinary = stream1.ToArray();
                    }
                }
                finally
                {
                    if (stream1 != null)
                    {
                        stream1.Close();
                    }
                }
            }
            catch
            {
                throw;
            }
            this.PropertyNames = propertyNamesBuilder.ToString();
            if (!string.IsNullOrEmpty(this.PropertyNames)) this.PropertyNames.TrimEnd(":".ToCharArray());
            this.PropertyValuesString = propertyValuesStringBuilder.ToString();
        }

        /// <summary>
        /// Save this profile to database
        /// </summary>
        /// <returns>Result of the save operation</returns>
        public SaveResult Save()
        {
            if (string.IsNullOrEmpty(this.PropertyNames))
            {
                this.Delete();
                this.IsDirty = false;
                return SaveResult.RecordDeleted;
            }
            return this.BaseSave();
        }
    }
}
