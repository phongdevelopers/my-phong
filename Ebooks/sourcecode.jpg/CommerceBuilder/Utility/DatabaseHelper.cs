using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CommerceBuilder.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Caching;
using System.Web.Configuration;
using System.IO;
using System.Web;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Utility class for database operations
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Check the integrity of the database
        /// </summary>
        public static void CheckIntegrity()
        {
            CheckTaxRuleIntegrity();
        }

        //Verify that ac_TaxRule.ProvinceId is null or a valid value
        private static void CheckTaxRuleIntegrity()
        {
        }

        /// <summary>
        /// Checks if a given field exists in the given table. 
        /// Note: It is assumed that the table already exists.
        /// </summary>
        /// <param name="tableName">The name of the table in which to check the field</param>
        /// <param name="fieldName">The field name to check for existance</param>
        /// <returns>true if field exists, false otherwise</returns>
        public static bool TableFieldExists(string tableName, string fieldName)
        {
            Database database = Token.Instance.Database;
            DataSet ds = database.ExecuteDataSet(CommandType.Text, "SELECT TOP 1 * FROM " + tableName);
            return ds.Tables[0].Columns.Contains(fieldName);
        }

        /// <summary>
        /// Determines if a table exists in the database
        /// </summary>
        /// <param name="tableName">Name of the table to check for</param>
        /// <returns>True if the table exists, false otherwise.</returns>
        public static bool TableExists(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName", "Table name is required and cannot be an empty string");
            Database database = Token.Instance.Database;
            string sql = "SELECT COUNT(*) AS NumRows FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName";
            DbCommand command = database.GetSqlStringCommand(sql);
            database.AddInParameter(command, "tableName", DbType.String, tableName);
            return (AlwaysConvert.ToInt(database.ExecuteScalar(command)) != 0);
        }

        /// <summary>
        /// Determines if a column exists within a database table
        /// </summary>
        /// <param name="tableName">Name of the table to look in</param>
        /// <param name="columnName">Name of the column to check for</param>
        /// <returns>True if the column exists within the table, false otherwise.</returns>
        /// <remarks>This method assumes the table exists.</remarks>
        public static bool ColumnExists(string tableName, string columnName)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName", "Table name is required and cannot be an empty string");
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName", "Column name is required and cannot be an empty string");
            Database database = Token.Instance.Database;
            string sql = "SELECT COUNT(*) AS NumRows FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName";
            DbCommand command = database.GetSqlStringCommand(sql);
            database.AddInParameter(command, "tableName", DbType.String, tableName);
            database.AddInParameter(command, "columnName", DbType.String, columnName);
            return (AlwaysConvert.ToInt(database.ExecuteScalar(command)) != 0);
        }

        /// <summary>
        /// Determines if a constraint exists for a database table
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="constraintName">Name of the constraint to check for</param>
        /// <returns>True if the constraint exists for the table, false otherwise.</returns>
        /// <remarks>This method assumes the table exists.</remarks>
        public static bool ConstraintExists(string tableName, string constraintName)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName", "Table name is required and cannot be an empty string");
            if (string.IsNullOrEmpty(constraintName)) throw new ArgumentNullException("constraintName", "Constraint name is required and cannot be an empty string");
            Database database = Token.Instance.Database;
            string sql = "SELECT COUNT(*) AS NumRows FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE TABLE_NAME = @tableName AND CONSTRAINT_NAME = @constraintName";
            DbCommand command = database.GetSqlStringCommand(sql);
            database.AddInParameter(command, "tableName", DbType.String, tableName);
            database.AddInParameter(command, "constraintName", DbType.String, constraintName);
            return (AlwaysConvert.ToInt(database.ExecuteScalar(command)) != 0);
        }

        /// <summary>
        /// Drops a table
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        public static void DropTable(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName", "Table name is required and cannot be an empty string");
            Database database = Token.Instance.Database;
            string sql = "DROP TABLE " + tableName;
            DbCommand command = database.GetSqlStringCommand(sql);
            database.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Drops a column from a table
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="columnName">Name of the column to drop</param>
        public static void DropColumn(string tableName, string columnName)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName", "Table name is required and cannot be an empty string");
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName", "Column name is required and cannot be an empty string");
            Database database = Token.Instance.Database;
            string sql = "ALTER TABLE " + tableName + " DROP COLUMN " + columnName;
            DbCommand command = database.GetSqlStringCommand(sql);
            database.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Drops a constraint from a table
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="constraintName">Name of the constraint to drop</param>
        public static void DropConstraint(string tableName, string constraintName)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName", "Table name is required and cannot be an empty string");
            if (string.IsNullOrEmpty(constraintName)) throw new ArgumentNullException("constraintName", "Constraint name is required and cannot be an empty string");
            Database database = Token.Instance.Database;
            string sql = "ALTER TABLE " + tableName + " DROP CONSTRAINT " + constraintName;
            DbCommand command = database.GetSqlStringCommand(sql);
            database.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Gets the name of the current database
        /// </summary>
        /// <returns>The name of the current database</returns>
        internal static string GetCurrentDatabaseName()
        {
            Database database = Token.Instance.Database;
            string sql = "SELECT db_name()";
            DbCommand command = database.GetSqlStringCommand(sql);
            return AlwaysConvert.ToString(database.ExecuteScalar(command));
        }

        /// <summary>
        /// Update the database connection string
        /// </summary>
        /// <param name="connectionString">new connection string</param>
        /// <param name="encrypt"></param>
        /// <returns></returns>
        public static void UpdateConnectionString(string connectionString, bool encrypt) 
        {
            HttpRequest request = HttpContextHelper.SafeGetRequest();
            if (request != null)
            {
                try
                {
                    System.Configuration.Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(request.ApplicationPath);
                    ConnectionStringsSection section = config.GetSection("connectionStrings") as ConnectionStringsSection;
                    if (section.SectionInformation.IsProtected) section.SectionInformation.UnprotectSection();
                    section.ConnectionStrings.Remove("LocalSqlServer");
                    section.ConnectionStrings["AbleCommerce"].ConnectionString = connectionString;
                    if (encrypt)
                    {
                        if (!section.SectionInformation.IsProtected) section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    }
                    else
                    {
                        if (section.SectionInformation.IsProtected) section.SectionInformation.UnprotectSection();
                    }
                    config.Save();
                }
                catch
                {
                    //IF MS API FAILS, WRITE THE FILE CLEARTEXT
                    StringBuilder textBuilder = new StringBuilder();
                    textBuilder.AppendLine("<connectionStrings>");
                    textBuilder.AppendFormat("\t<add name=\"AbleCommerce\" connectionString=\"{0}\" providerName=\"System.Data.SqlClient\" />\r\n", connectionString);
                    textBuilder.AppendLine("</connectionStrings>");

                    string configPath = request.MapPath("~/App_Data/database.config");
                    File.WriteAllText(configPath, textBuilder.ToString());
                }
            }
        }
    }
}