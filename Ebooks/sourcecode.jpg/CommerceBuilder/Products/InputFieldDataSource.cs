namespace CommerceBuilder.Products
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Text;
    using CommerceBuilder.Data;
    using CommerceBuilder.Common;
    using CommerceBuilder.Utility;

    /// <summary>
    /// DataSource class for InputField objects
    /// </summary>
    [DataObject(true)]    
    public partial class InputFieldDataSource
    {        
        /// <summary>
        /// Enumeration that represents the scope of an input field
        /// </summary>
        public enum InputFieldScope
        {
            /// <summary>
            /// This is an input field for all (Merchat as well as Customer)
            /// </summary>
            All, 
            
            /// <summary>
            /// This is an input field for merchants
            /// </summary>
            Merchant, 
            
            /// <summary>
            /// This is an input field for customers
            /// </summary>
            Customer
        }

        /// <summary>
        /// Counts number of input fields for given product template
        /// </summary>
        /// <param name="productTemplateId">Id of the product template to count for</param>
        /// <param name="scope">scope of input fields to consider</param>
        /// <returns>Number of input fields for given product template</returns>
        public static int CountForProductTemplate(int productTemplateId, InputFieldScope scope)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_InputFields WHERE ProductTemplateId = @productTemplateId");
            database.AddInParameter(selectCommand, "@productTemplateId", System.Data.DbType.Int32, productTemplateId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of input fields for given product template
        /// </summary>
        /// <param name="productTemplateId">Id of the product template to load for</param>
        /// <param name="scope">scope of input fields to consider</param>
        /// <returns>A collection of input fields for given product template</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputFieldCollection LoadForProductTemplate(int productTemplateId, InputFieldScope scope)
        {
            return LoadForProductTemplate(productTemplateId, scope, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of input fields for given product template
        /// </summary>
        /// <param name="productTemplateId">Id of the product template to load for</param>
        /// <param name="scope">scope of input fields to consider</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of input fields for given product template</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputFieldCollection LoadForProductTemplate(int productTemplateId, InputFieldScope scope, string sortExpression)
        {
            return LoadForProductTemplate(productTemplateId, scope, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of input fields for given product template
        /// </summary>
        /// <param name="productTemplateId">Id of the product template to load for</param>
        /// <param name="scope">scope of input fields to consider</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of input fields for given product template</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputFieldCollection LoadForProductTemplate(int productTemplateId, InputFieldScope scope, int maximumRows, int startRowIndex)
        {
            return LoadForProductTemplate(productTemplateId, scope, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of input fields for given product template
        /// </summary>
        /// <param name="productTemplateId">Id of the product template to load for</param>
        /// <param name="scope">scope of input fields to consider</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of input fields for given product template</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputFieldCollection LoadForProductTemplate(int productTemplateId, InputFieldScope scope, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "OrderBy";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + InputField.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_InputFields");
            selectQuery.Append(" WHERE ProductTemplateId = @productTemplateId");
            selectQuery.Append(GetScopeClause(scope));
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productTemplateId", System.Data.DbType.Int32, productTemplateId);
            //EXECUTE THE COMMAND
            InputFieldCollection results = new InputFieldCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        InputField inputField = new InputField();
                        InputField.LoadDataReader(inputField, dr);
                        results.Add(inputField);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        private static string GetScopeClause(InputFieldScope scope)
        {
            if (scope == InputFieldScope.Customer) return " AND IsMerchantField = 0";
            if (scope == InputFieldScope.Merchant) return " AND IsMerchantField = 1";
            return string.Empty;
        }
    }
}
