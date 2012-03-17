//-----------------------------------------------------------------------
// <copyright file="DatabaseParameter.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Data
{
    using System.Data;

    /// <summary>
    /// Storage class for a database parameter to be added to a command.
    /// </summary>
    public class DatabaseParameter
    {
        /// <summary>
        /// Parameter type field
        /// </summary>
        private DbType _DbType;

        /// <summary>
        /// Parameter name field
        /// </summary>
        private string _Name;

        /// <summary>
        /// Parameter value field
        /// </summary>
        private object _Value;

        /// <summary>
        /// Initializes a new instance of the DatabaseParameter class.
        /// </summary>
        /// <param name="dbType">Parameter type</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        public DatabaseParameter(DbType dbType, string name, object value)
        {
            this._DbType = dbType;
            this._Name = name;
            this._Value = value;
        }

        /// <summary>
        /// Gets the parameter type
        /// </summary>
        public DbType DbType
        {
            get { return this._DbType; }
        }

        /// <summary>
        /// Gets the parameter name
        /// </summary>
        public string Name
        {
            get { return this._Name; }
        }

        /// <summary>
        /// Gets the parameter value
        /// </summary>
        public object Value
        {
            get { return this._Value; }
        }
    }
}