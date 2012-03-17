//Generated by DataSourceBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// DataSource class for InputChoice objects
    /// </summary>
    public partial class InputChoiceDataSource
    {
        /// <summary>
        /// Deletes a InputChoice object from the database
        /// </summary>
        /// <param name="inputChoice">The InputChoice object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(InputChoice inputChoice)
        {
            return inputChoice.Delete();
        }

        /// <summary>
        /// Deletes a InputChoice object with given id from the database
        /// </summary>
        /// <param name="inputChoiceId">Value of InputChoiceId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 inputChoiceId)
        {
            InputChoice inputChoice = new InputChoice();
            if (inputChoice.Load(inputChoiceId)) return inputChoice.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a InputChoice object to the database.
        /// </summary>
        /// <param name="inputChoice">The InputChoice object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(InputChoice inputChoice) { return inputChoice.Save(); }

        /// <summary>
        /// Loads a InputChoice object for given Id from the database.
        /// </summary>
        /// <param name="inputChoiceId">Value of InputChoiceId of the object to load.</param>
        /// <returns>If the load is successful the newly loaded InputChoice object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputChoice Load(Int32 inputChoiceId)
        {
            return InputChoiceDataSource.Load(inputChoiceId, true);
        }

        /// <summary>
        /// Loads a InputChoice object for given Id from the database.
        /// </summary>
        /// <param name="inputChoiceId">Value of InputChoiceId of the object to load.</param>
        /// <param name="useCache">If true tries to load object from cache first.</param>
        /// <returns>If the load is successful the newly loaded InputChoice object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputChoice Load(Int32 inputChoiceId, bool useCache)
        {
            if (inputChoiceId == 0) return null;
            InputChoice inputChoice = null;
            string key = "InputChoice_" + inputChoiceId.ToString();
            if (useCache)
            {
                inputChoice = ContextCache.GetObject(key) as InputChoice;
                if (inputChoice != null) return inputChoice;
            }
            inputChoice = new InputChoice();
            if (inputChoice.Load(inputChoiceId))
            {
                if (useCache) ContextCache.SetObject(key, inputChoice);
                return inputChoice;
            }
            return null;
        }

        /// <summary>
        /// Counts the number of InputChoice objects in result if retrieved using the given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the InputChoice objects that should be loaded.</param>
        /// <returns>The number of InputChoice objects matching the criteria</returns>
        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_InputChoices" + whereClause);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of InputChoice objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of InputChoice objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputChoiceCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of InputChoice objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of InputChoice objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputChoiceCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of InputChoice objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of InputChoice objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputChoiceCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of InputChoice objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of InputChoice objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputChoiceCollection  LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "OrderBy";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + InputChoice.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_InputChoices");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            InputChoiceCollection results = new InputChoiceCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        InputChoice inputChoice = new InputChoice();
                        InputChoice.LoadDataReader(inputChoice, dr);
                        results.Add(inputChoice);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of InputChoice objects for the given InputFieldId in the database.
        /// <param name="inputFieldId">The given InputFieldId</param>
        /// </summary>
        /// <returns>The Number of InputChoice objects for the given InputFieldId in the database.</returns>
        public static int CountForInputField(Int32 inputFieldId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_InputChoices WHERE InputFieldId = @inputFieldId");
            database.AddInParameter(selectCommand, "@inputFieldId", System.Data.DbType.Int32, inputFieldId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of InputChoice objects for the given InputFieldId from the database
        /// </summary>
        /// <param name="inputFieldId">The given InputFieldId</param>
        /// <returns>A collection of InputChoice objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputChoiceCollection LoadForInputField(Int32 inputFieldId)
        {
            return LoadForInputField(inputFieldId, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of InputChoice objects for the given InputFieldId from the database
        /// </summary>
        /// <param name="inputFieldId">The given InputFieldId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of InputChoice objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputChoiceCollection LoadForInputField(Int32 inputFieldId, string sortExpression)
        {
            return LoadForInputField(inputFieldId, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of InputChoice objects for the given InputFieldId from the database
        /// </summary>
        /// <param name="inputFieldId">The given InputFieldId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of InputChoice objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputChoiceCollection LoadForInputField(Int32 inputFieldId, int maximumRows, int startRowIndex)
        {
            return LoadForInputField(inputFieldId, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of InputChoice objects for the given InputFieldId from the database
        /// </summary>
        /// <param name="inputFieldId">The given InputFieldId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of InputChoice objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static InputChoiceCollection LoadForInputField(Int32 inputFieldId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //DEFAULT SORT EXPRESSION
            if (string.IsNullOrEmpty(sortExpression)) sortExpression = "OrderBy";
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + InputChoice.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_InputChoices");
            selectQuery.Append(" WHERE InputFieldId = @inputFieldId");
            selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@inputFieldId", System.Data.DbType.Int32, inputFieldId);
            //EXECUTE THE COMMAND
            InputChoiceCollection results = new InputChoiceCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        InputChoice inputChoice = new InputChoice();
                        InputChoice.LoadDataReader(inputChoice, dr);
                        results.Add(inputChoice);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Gets the next value of the OrderBy field for InputChoice objects.
        /// </summary>
        /// <param name="inputFieldId">The InputFieldId for which to get the next OrderBy value</param>
        /// <returns>The next value of the OrderBy field for InputChoice objects</returns>
        public static short GetNextOrderBy(Int32 inputFieldId)
        {
            Database database = Token.Instance.Database;
            using (DbCommand selectCommand = database.GetSqlStringCommand("SELECT (Max(OrderBy) + 1) AS NextOrderBy FROM ac_InputChoices WHERE InputFieldId = @inputFieldId"))
            {
                database.AddInParameter(selectCommand, "@inputFieldId", System.Data.DbType.Int32, inputFieldId);
                object result = database.ExecuteScalar(selectCommand);
                if (result.Equals(DBNull.Value)) return 1;
                return (short)(int)result;
            }
        }

        /// <summary>
        /// Updates/Saves the given InputChoice object to the database.
        /// </summary>
        /// <param name="inputChoice">The InputChoice object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(InputChoice inputChoice) { return inputChoice.Save(); }

    }
}