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

namespace CommerceBuilder.Personalization
{
    /// <summary>
    /// DataSource class for PersonalizationPath objects
    /// </summary>
    public partial class PersonalizationPathDataSource
    {
        /// <summary>
        /// Deletes a PersonalizationPath object from the database
        /// </summary>
        /// <param name="personalizationPath">The PersonalizationPath object to delete</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static bool Delete(PersonalizationPath personalizationPath)
        {
            return personalizationPath.Delete();
        }

        /// <summary>
        /// Deletes a PersonalizationPath object with given id from the database
        /// </summary>
        /// <param name="personalizationPathId">Value of PersonalizationPathId of the object to delete.</param>
        /// <returns><b>true</b> if delete operation is successful, <b>false</b> otherwise</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public static bool Delete(Int32 personalizationPathId)
        {
            PersonalizationPath personalizationPath = new PersonalizationPath();
            if (personalizationPath.Load(personalizationPathId)) return personalizationPath.Delete();
            return false;
        }

        /// <summary>
        /// Inserts/Saves a PersonalizationPath object to the database.
        /// </summary>
        /// <param name="personalizationPath">The PersonalizationPath object to insert</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Insert(PersonalizationPath personalizationPath) { return personalizationPath.Save(); }

        /// <summary>
        /// Loads a PersonalizationPath object for given Id from the database.
        /// </summary>
        /// <param name="personalizationPathId">Value of PersonalizationPathId of the object to load.</param>
        /// <returns>If the load is successful the newly loaded PersonalizationPath object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPath Load(Int32 personalizationPathId)
        {
            return PersonalizationPathDataSource.Load(personalizationPathId, true);
        }

        /// <summary>
        /// Loads a PersonalizationPath object for given Id from the database.
        /// </summary>
        /// <param name="personalizationPathId">Value of PersonalizationPathId of the object to load.</param>
        /// <param name="useCache">If true tries to load object from cache first.</param>
        /// <returns>If the load is successful the newly loaded PersonalizationPath object is returned. If load fails null is returned.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPath Load(Int32 personalizationPathId, bool useCache)
        {
            if (personalizationPathId == 0) return null;
            PersonalizationPath personalizationPath = null;
            string key = "PersonalizationPath_" + personalizationPathId.ToString();
            if (useCache)
            {
                personalizationPath = ContextCache.GetObject(key) as PersonalizationPath;
                if (personalizationPath != null) return personalizationPath;
            }
            personalizationPath = new PersonalizationPath();
            if (personalizationPath.Load(personalizationPathId))
            {
                if (useCache) ContextCache.SetObject(key, personalizationPath);
                return personalizationPath;
            }
            return null;
        }

        /// <summary>
        /// Counts the number of PersonalizationPath objects in result if retrieved using the given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the PersonalizationPath objects that should be loaded.</param>
        /// <returns>The number of PersonalizationPath objects matching the criteria</returns>
        public static int CountForCriteria(string sqlCriteria)
        {
            Database database = Token.Instance.Database;
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_PersonalizationPaths" + whereClause);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of PersonalizationPath objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of PersonalizationPath objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPathCollection LoadForCriteria(string sqlCriteria)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of PersonalizationPath objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of PersonalizationPath objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPathCollection LoadForCriteria(string sqlCriteria, string sortExpression)
        {
            return LoadForCriteria(sqlCriteria, 0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of PersonalizationPath objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of PersonalizationPath objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPathCollection LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex)
        {
            return LoadForCriteria(sqlCriteria, maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of PersonalizationPath objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of PersonalizationPath objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPathCollection  LoadForCriteria(string sqlCriteria, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + PersonalizationPath.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_PersonalizationPaths");
            string whereClause = string.IsNullOrEmpty(sqlCriteria) ? string.Empty : " WHERE " + sqlCriteria;
            selectQuery.Append(whereClause);
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            PersonalizationPathCollection results = new PersonalizationPathCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        PersonalizationPath personalizationPath = new PersonalizationPath();
                        PersonalizationPath.LoadDataReader(personalizationPath, dr);
                        results.Add(personalizationPath);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of PersonalizationPath objects associated with the given UserId
        /// </summary>
        /// <param name="userId">The given UserId</param>
        /// <returns>The number of PersonalizationPath objects associated with with the given UserId</returns>
        public static int CountForUser(Int32 userId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_UserPersonalization WHERE UserId = @userId");
            database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, userId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }
        /// <summary>
        /// Loads the PersonalizationPath objects associated with the given UserId
        /// </summary>
        /// <param name="userId">The given UserId</param>
        /// <returns>A collection of PersonalizationPath objects associated with with the given UserId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPathCollection LoadForUser(Int32 userId)
        {
            return PersonalizationPathDataSource.LoadForUser(userId, 0, 0, string.Empty);
        }
        /// <summary>
        /// Loads the PersonalizationPath objects associated with the given UserId
        /// </summary>
        /// <param name="userId">The given UserId</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of PersonalizationPath objects associated with with the given UserId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPathCollection LoadForUser(Int32 userId, string sortExpression)
        {
            return PersonalizationPathDataSource.LoadForUser(userId, 0, 0, sortExpression);
        }
        /// <summary>
        /// Loads the PersonalizationPath objects associated with the given UserId
        /// </summary>
        /// <param name="userId">The given UserId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of PersonalizationPath objects associated with with the given UserId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPathCollection LoadForUser(Int32 userId, int maximumRows, int startRowIndex)
        {
            return PersonalizationPathDataSource.LoadForUser(userId, maximumRows, startRowIndex, string.Empty);
        }
        /// <summary>
        /// Loads the PersonalizationPath objects associated with the given UserId
        /// </summary>
        /// <param name="userId">The given UserId</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of PersonalizationPath objects associated with with the given UserId</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPathCollection LoadForUser(Int32 userId, int maximumRows, int startRowIndex, string sortExpression)
        {
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + PersonalizationPath.GetColumnNames("ac_PersonalizationPaths"));
            selectQuery.Append(" FROM ac_PersonalizationPaths, ac_UserPersonalization");
            selectQuery.Append(" WHERE ac_PersonalizationPaths.PersonalizationPathId = ac_UserPersonalization.PersonalizationPathId");
            selectQuery.Append(" AND ac_UserPersonalization.UserId = @userId");
            selectQuery.Append(" AND StoreId = @storeId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@userId", System.Data.DbType.Int32, userId);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            PersonalizationPathCollection results = new PersonalizationPathCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        PersonalizationPath personalizationPath = new PersonalizationPath();
                        PersonalizationPath.LoadDataReader(personalizationPath, dr);
                        results.Add(personalizationPath);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Counts the number of PersonalizationPath objects for the current store.
        /// </summary>
        /// <returns>The Number of PersonalizationPath objects in the current store.</returns>
        public static int CountForStore()
        {
            int storeId = Token.Instance.StoreId;
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SELECT COUNT(*) AS TotalRecords FROM ac_PersonalizationPaths WHERE StoreId = @storeId");
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            return CommerceBuilder.Utility.AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        /// <summary>
        /// Loads a collection of PersonalizationPath objects for the current store from the database
        /// </summary>
        /// <returns>A collection of PersonalizationPath objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPathCollection LoadForStore()
        {
            return LoadForStore(0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of PersonalizationPath objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of PersonalizationPath objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPathCollection LoadForStore(string sortExpression)
        {
            return LoadForStore(0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of PersonalizationPath objects for the current store from the database.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of PersonalizationPath objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPathCollection LoadForStore(int maximumRows, int startRowIndex)
        {
            return LoadForStore(maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of PersonalizationPath objects for the current store from the database. Sorts using the given sort exrpression.
        /// </summary>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of PersonalizationPath objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersonalizationPathCollection LoadForStore(int maximumRows, int startRowIndex, string sortExpression)
        {
            int storeId = Token.Instance.StoreId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT");
            if (maximumRows > 0) selectQuery.Append(" TOP " + (startRowIndex + maximumRows).ToString());
            selectQuery.Append(" " + PersonalizationPath.GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_PersonalizationPaths");
            selectQuery.Append(" WHERE StoreId = @storeId");
            if (!string.IsNullOrEmpty(sortExpression)) selectQuery.Append(" ORDER BY " + sortExpression);
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, storeId);
            //EXECUTE THE COMMAND
            PersonalizationPathCollection results = new PersonalizationPathCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((maximumRows < 1) || (rowCount < maximumRows)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        PersonalizationPath personalizationPath = new PersonalizationPath();
                        PersonalizationPath.LoadDataReader(personalizationPath, dr);
                        results.Add(personalizationPath);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }

        /// <summary>
        /// Updates/Saves the given PersonalizationPath object to the database.
        /// </summary>
        /// <param name="personalizationPath">The PersonalizationPath object to update/save to database.</param>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011")]
        public static SaveResult Update(PersonalizationPath personalizationPath) { return personalizationPath.Save(); }

    }
}
