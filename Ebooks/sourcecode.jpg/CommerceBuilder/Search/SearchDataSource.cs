//-----------------------------------------------------------------------
// <copyright file="SearchDataSource.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using System.Data.Common;
using System.Data;

namespace CommerceBuilder.Search
{
    /// <summary>
    /// DataSource class for admin searching
    /// </summary>
    public partial class SearchDataSource
    {
        /// <summary>
        /// Search for the specified keywords in selected search area
        /// </summary>
        /// <param name="keywords">Keywords to search</param>
        /// <param name="searchArea">objects to be searched</param>
        /// <returns>Search results</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SearchAreaResults Search(String keywords, SearchArea searchArea)
        {
            return Search(keywords, searchArea, 0);
        }

        /// <summary>
        /// Search for the specified keywords in selected search area
        /// </summary>
        /// <param name="keywords">Keywords to search</param>
        /// <param name="searchArea">objects to be searched</param>
        /// <param name="maximumRows">maximum number of search results</param>
        /// <returns>Search results</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SearchAreaResults Search(String keywords, SearchArea searchArea, int maximumRows)
        {
            List<SearchArea> list = new List<SearchArea>();
            list.Add(searchArea);
            List<SearchAreaResults> results = Search(keywords, list, maximumRows);

            if (results.Count > 0) return results[0];
            else return new SearchAreaResults();
        }

        /// <summary>
        /// Search for the specified keywords in selected search areas
        /// </summary>
        /// <param name="keywords">Keywords to search</param>
        /// <param name="searchAreas">objects to be searched</param>
        /// <returns>List of search results each for one search area</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<SearchAreaResults> Search(String keywords, List<SearchArea> searchAreas)
        {
            return Search(keywords, searchAreas, 0);
        }

        /// <summary>
        /// Search for the specified keywords in selected search areas
        /// </summary>
        /// <param name="keywords">Keywords to search</param>
        /// <param name="searchAreas">objects to be searched</param>
        /// <param name="maximumRows">maximum number of search results</param>
        /// <returns>List of search results each for one search area</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<SearchAreaResults> Search(String keywords, List<SearchArea> searchAreas, int maximumRows)
        {
            List<SearchAreaResults> results = new List<SearchAreaResults>();
            String fixedKeywords = keywords;

            if (String.IsNullOrEmpty(keywords)) return results;
            else
            {                
                fixedKeywords = StringHelper.FixSearchPattern(keywords);
            }

            // ADD OBJECTS AND RESPECTIVE DATABASE FIELDS
            List<SearchAreaDetail> searchAreaDetails = new List<SearchAreaDetail>();
            searchAreaDetails.Add(new SearchAreaDetail(SearchArea.Categories, "ac_Categories", "CategoryId", "Name", new string[] { "Name" }));
            searchAreaDetails.Add(new SearchAreaDetail(SearchArea.Webpages, "ac_Webpages", "WebpageId", "Name", new string[] { "Name" }));
            searchAreaDetails.Add(new SearchAreaDetail(SearchArea.Links, "ac_Links", "LinkId", "Name", new string[] { "Name" }));
            searchAreaDetails.Add(new SearchAreaDetail(SearchArea.DigitalGoods, "ac_DigitalGoods", "DigitalGoodId", "Name", new string[] { "Name" }));
            searchAreaDetails.Add(new SearchAreaDetail(SearchArea.Warehouses, "ac_Warehouses", "WarehouseId", "Name", new string[] { "Name" }));
            searchAreaDetails.Add(new SearchAreaDetail(SearchArea.Products, "ac_Products", "ProductId", "Name", new string[] { "Name", "Sku", "Description", "Summary", "ExtendedDescription", "SearchKeywords" }));
            //searchAreaDetails.Add(new SearchAreaDetail(SearchArea.Users, "ac_Users", "UserId", "UserName", new string[] { "LoweredEmail", "LoweredUserName" }));
            
            StringBuilder sqlBuilder;
            Database database = Token.Instance.Database;

            foreach (SearchAreaDetail searchAreaDetail in searchAreaDetails)
            {
                if (searchAreas.Contains(searchAreaDetail.SearchArea) || searchAreas.Contains(SearchArea.All))
                {
                    sqlBuilder = new StringBuilder();
                    if (maximumRows > 0) sqlBuilder.Append("SELECT TOP ").Append(maximumRows).Append(" ");
                    else sqlBuilder.Append("SELECT ");
                    sqlBuilder.Append(searchAreaDetail.KeyField).Append(",").Append( searchAreaDetail.NameField).Append(" FROM ").Append(searchAreaDetail.TableName);
                    sqlBuilder.Append(" WHERE StoreId = @storeId");

                    for (int i = 0; i < searchAreaDetail.SearchFields.Length; i++)
                    {
                        String searchField = searchAreaDetail.SearchFields[i];
                        if (i == 0) sqlBuilder.Append(" AND (");
                        else sqlBuilder.Append(" OR ");
                        sqlBuilder.Append(searchField).Append(" LIKE @keywords");
                        if (i == searchAreaDetail.SearchFields.Length - 1) sqlBuilder.Append(")");
                    }

                    DbCommand selectCommand = database.GetSqlStringCommand(sqlBuilder.ToString());
                    database.AddInParameter(selectCommand, "keywords", DbType.String, fixedKeywords);
                    database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);

                    SearchAreaResults searchAreaResults = new SearchAreaResults();
                    searchAreaResults.SearchArea = searchAreaDetail.SearchArea;
                    //EXECUTE THE COMMAND
                    using (IDataReader dr = database.ExecuteReader(selectCommand))
                    {
                        SearchResult searchResult;
                        while (dr.Read())
                        {
                            searchResult = new SearchResult();
                            searchResult.Id = dr.GetInt32(0);
                            searchResult.Name = dr.GetString(1);
                            searchAreaResults.SearchResults.Add(searchResult);
                        }
                        dr.Close();
                    }
                    if (searchAreaResults.SearchResults.Count > 0)
                    {
                        if (maximumRows > 0 && searchAreaResults.SearchResults.Count >= maximumRows)
                        {
                            // COUNT ALL RESULTS
                            sqlBuilder = new StringBuilder();
                            sqlBuilder.Append("SELECT Count(").Append(searchAreaDetail.KeyField).Append(")");
                            sqlBuilder.Append(" FROM ").Append(searchAreaDetail.TableName);
                            sqlBuilder.Append(" WHERE StoreId = @storeId");
                            for (int i = 0; i < searchAreaDetail.SearchFields.Length; i++)
                            {
                                String searchField = searchAreaDetail.SearchFields[i];
                                if (i == 0) sqlBuilder.Append(" AND (");
                                else sqlBuilder.Append(" OR ");
                                sqlBuilder.Append(searchField).Append(" LIKE @keywords");
                                if (i == searchAreaDetail.SearchFields.Length - 1) sqlBuilder.Append(")");
                            }
                            DbCommand countCommand = database.GetSqlStringCommand(sqlBuilder.ToString());
                            database.AddInParameter(countCommand, "keywords", DbType.String, fixedKeywords);
                            database.AddInParameter(countCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);

                            searchAreaResults.TotalMatches = (int)database.ExecuteScalar(countCommand);

                        }
                        else
                        {
                            searchAreaResults.TotalMatches = searchAreaResults.SearchResults.Count;
                        }
                        results.Add(searchAreaResults);
                    }
                }
            }

            // SEARCH USERS BY NAME IN ADDRESSES TABLE
            bool lastNameOnly = keywords.Contains(" ");
            if (searchAreas.Contains(SearchArea.Users) || searchAreas.Contains(SearchArea.All))
            {
                String firstNamePattern = String.Empty;
                String lastNamePattern = String.Empty;
                sqlBuilder = new StringBuilder();
                if (maximumRows > 0) sqlBuilder.Append("SELECT TOP ").Append(maximumRows);
                else sqlBuilder.Append("SELECT ");
                sqlBuilder.Append(" U.UserId, U.UserName FROM ac_Users U LEFT JOIN ac_Addresses A ON U.PrimaryAddressId = A.AddressId WHERE U.StoreId = @storeId");
                if(keywords.Contains(" "))
                {
                    firstNamePattern = StringHelper.FixSearchPattern(keywords.Substring(0, keywords.LastIndexOf(" ")).Trim());
                    lastNamePattern = StringHelper.FixSearchPattern(keywords.Substring(keywords.LastIndexOf(" ")).Trim());

                    sqlBuilder.Append(" AND ( U.LoweredEmail LIKE @keywords OR U.LoweredUserName LIKE @keywords OR A.FirstName LIKE @firstNamePattern OR A.LastName LIKE @lastNamePattern)");
                }
                else sqlBuilder.Append(" AND (U.LoweredEmail LIKE @keywords OR U.LoweredUserName LIKE @keywords OR A.LastName LIKE @keywords)");

                DbCommand selectCommand = database.GetSqlStringCommand(sqlBuilder.ToString());
                if (keywords.Contains(" "))
                {
                    database.AddInParameter(selectCommand, "@firstNamePattern", DbType.String, firstNamePattern);
                    database.AddInParameter(selectCommand, "@lastNamePattern", DbType.String, lastNamePattern);
                }
                database.AddInParameter(selectCommand, "keywords", DbType.String, fixedKeywords);
                database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
                                
                SearchAreaResults searchAreaResults = new SearchAreaResults();
                searchAreaResults.SearchArea = SearchArea.Users;
                //EXECUTE THE COMMAND
                using (IDataReader dr = database.ExecuteReader(selectCommand))
                {
                    SearchResult searchResult;
                    while (dr.Read())
                    {
                        searchResult = new SearchResult();
                        searchResult.Id = dr.GetInt32(0);
                        searchResult.Name = dr.GetString(1);
                        searchAreaResults.SearchResults.Add(searchResult);
                    }
                    dr.Close();
                }
                if (searchAreaResults.SearchResults.Count > 0)
                {
                    // COUNT ALL RESULTS
                    if (maximumRows > 0 && searchAreaResults.SearchResults.Count >= maximumRows )
                    {
                        sqlBuilder = new StringBuilder();
                        sqlBuilder.Append("SELECT Count(U.UserId)");
                        sqlBuilder.Append(" FROM ac_Users U LEFT JOIN ac_Addresses A ON U.PrimaryAddressId = A.AddressId WHERE U.StoreId = @storeId");
                        if (keywords.Contains(" "))
                        {
                            firstNamePattern = keywords.Substring(0, keywords.LastIndexOf(" ")).Trim();
                            lastNamePattern = keywords.Substring(keywords.LastIndexOf(" ")).Trim();

                            sqlBuilder.Append(" AND ( U.LoweredEmail LIKE @keywords OR U.LoweredUserName LIKE @keywords OR A.FirstName LIKE @firstNamePattern OR A.LastName LIKE @lastNamePattern)");
                        }
                        else sqlBuilder.Append(" AND (U.LoweredEmail LIKE @keywords OR U.LoweredUserName LIKE @keywords OR A.LastName LIKE @keywords)");

                        DbCommand countCommand = database.GetSqlStringCommand(sqlBuilder.ToString());
                        if (keywords.Contains(" "))
                        {
                            database.AddInParameter(countCommand, "@firstNamePattern", DbType.String, firstNamePattern);
                            database.AddInParameter(countCommand, "@lastNamePattern", DbType.String, lastNamePattern);
                        }
                        database.AddInParameter(countCommand, "keywords", DbType.String, fixedKeywords);
                        database.AddInParameter(countCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);

                        searchAreaResults.TotalMatches = (int)database.ExecuteScalar(countCommand);
                    }
                    else searchAreaResults.TotalMatches = searchAreaResults.SearchResults.Count;
                    results.Add(searchAreaResults);
                }
            }

            return results;
        }


        private class SearchAreaDetail
        {
            private SearchArea _SearchArea;
            public SearchArea SearchArea
            {
                get { return _SearchArea; }
                set { _SearchArea = value; }
            }

            private String _TableName;
            public String TableName
            {
                get { return _TableName; }
                set { _TableName = value; }
            }

            private String[] _SearchFields;
            public String[] SearchFields
            {
                get { return _SearchFields; }
                set { _SearchFields = value; }
            }

            private String _KeyField;
            public String KeyField
            {
                get { return _KeyField; }
                set { _KeyField = value; }
            }

            private String _NameField;
            public String NameField
            {
                get { return _NameField; }
                set { _NameField = value; }
            }

            public SearchAreaDetail(SearchArea searchArea, String tableName, String keyField, String nameField, String[] searchFields)
            {
                this.SearchArea = searchArea;
                this.TableName = tableName;
                this.KeyField = keyField;
                this.NameField = nameField;
                this.SearchFields = searchFields;
            }
        }
    }
}
