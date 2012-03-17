using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;

namespace CommerceBuilder.Personalization
{
    /// <summary>
    /// DataSource class for SharedPersonalization objects
    /// </summary>
    [DataObject(true)]
    public partial class SharedPersonalizationDataSource
    {
        /// <summary>
        /// Loads the shared personalization record for the given path.
        /// </summary>
        /// <param name="path">The path to the file to load data for</param>
        /// <param name="create">If true, the personalization record is created if
        /// it does not exist.</param>
        /// <returns>Thje shared personalization record for the given path</returns>
        public static SharedPersonalization LoadForPath(string path, bool create)
        {
            SharedPersonalization sharedPersonalization = null;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + SharedPersonalization.GetColumnNames("acSP"));
            selectQuery.Append(" FROM ac_SharedPersonalization acSP, ac_PersonalizationPaths acP");
            selectQuery.Append(" WHERE acSP.PersonalizationPathId = acP.PersonalizationPathId AND acP.Path = @path AND acP.StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@path", System.Data.DbType.String, path);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    sharedPersonalization = new SharedPersonalization();
                    SharedPersonalization.LoadDataReader(sharedPersonalization, dr);
                }
                dr.Close();
            }
            if ((sharedPersonalization == null) && create)
            {
                sharedPersonalization = new SharedPersonalization(PersonalizationPathDataSource.LoadForPath(path, true));
            }
            return sharedPersonalization;
        }

        /// <summary>
        /// Returns a collection containing zero or more PersonalizationStateInfo-derived objects based 
        /// on scope and specific query parameters.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public static PersonalizationStateInfoCollection FindState(PersonalizationStateQuery query, int pageIndex, int pageSize, out int totalRecords)
        {
            PersonalizationStateInfoCollection tempResults = new PersonalizationStateInfoCollection();
            //USERNAMES ARE NOT ASSOCIATED WITH SHARED DATA
            //IF A USERNAME WAS SPECIFIED, RETURN AN EMPTY COLLECTION
            totalRecords = 0;
            if (!string.IsNullOrEmpty(query.UsernameToMatch)) return tempResults;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            //CHECK WHETHER TO LOAD ALL PATHS, OR TO FILTER
            selectQuery.Append("SELECT " + SharedPersonalization.GetColumnNames("S") + ",P.Path");
            selectQuery.Append(" FROM ac_SharedPersonalization S, ac_PersonalizationPaths P");
            selectQuery.Append(" WHERE S.PersonalizationPathId = P.PersonalizationPathId AND P.StoreId = @storeId");
            if (!string.IsNullOrEmpty(query.PathToMatch)) selectQuery.Append(" AND P.Path LIKE @pathToMatch");
            selectQuery.Append(" ORDER BY P.Path");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            if (!string.IsNullOrEmpty(query.PathToMatch)) database.AddInParameter(selectCommand, "@pathToMatch", System.Data.DbType.String, query.PathToMatch);
            //EXECUTE THE COMMAND
            int startRowIndex = (pageIndex * pageSize);
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() && ((rowCount < pageSize)))
                {
                    if (thisIndex >= startRowIndex)
                    {
                        SharedPersonalization p = new SharedPersonalization();
                        SharedPersonalization.LoadDataReader(p, dr);
                        SharedPersonalizationStateInfo i = new SharedPersonalizationStateInfo(dr.GetString(5), p.LastUpdatedDate, p.PageSettings.Length, 0, 0);
                        tempResults.Add(i);
                        rowCount++;
                    }
                    thisIndex++;
                }
                dr.Close();
            }
            totalRecords = rowCount;
            //LOOP RESULTS AND COMPILE USER STATISICS
            int size, count;
            PersonalizationStateInfoCollection results = new PersonalizationStateInfoCollection();
            foreach (SharedPersonalizationStateInfo i in tempResults)
            {
                UserPersonalizationDataSource.CountForPath(i.Path, out size, out count);
                results.Add(new SharedPersonalizationStateInfo(i.Path, i.LastUpdatedDate, i.Size, size, count));
            }
            return results;
        }
    }
}
