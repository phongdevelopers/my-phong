using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CommerceBuilder.Data;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Search
{
    /// <summary>
    /// Provides order number range parsing and query criteria services
    /// </summary>
    public class IdRangeParser
    {
        private string _UniqueId = string.Empty;
        private string _FieldName;
        private List<IdRange> _IdRanges;
        private static Regex _RangeTokenRegex = new Regex(@"^(\d+)(?:-(\d+))?$");
        public static Regex RangeRegex = new Regex(@"^\d+( *- *\d+)?( *, *\d+(-\d+)?)*$");

        /// <summary>
        ///  Returns the count of ranges parsed
        /// </summary>
        public int RangeCount
        {
            get {
                if (_IdRanges == null) return 0;
                return _IdRanges.Count; 
            }
        }

        /// <summary>
        ///  Creates a new instance of IdRangeParser
        /// </summary>
        /// <param name="fieldName">The name of the field being matched.</param>
        /// <param name="idRange">The order range to parse</param>
        /// <remarks>
        /// Valid range examples:
        /// 12: Order 12
        /// 12,15: Order 12 and 15
        /// 12-15: Order 12 through 15
        /// 12-15,20: Order 12 through 15 and 20
        /// </remarks>
        public IdRangeParser(string fieldName, string idRange) : this(fieldName, idRange, string.Empty) { }

        /// <summary>
        ///  Creates a new instance of IdRangeParser
        /// </summary>
        /// <param name="fieldName">The name of the field being matched.</param>
        /// <param name="idRange">The order range to parse</param>
        /// <param name="uniqueId">A unique ID to assign to this parser, if more than one is used in a search.</param>
        /// <remarks>
        /// Valid range examples:
        /// 12: Order 12
        /// 12,15: Order 12 and 15
        /// 12-15: Order 12 through 15
        /// 12-15,20: Order 12 through 15 and 20
        /// </remarks>
        public IdRangeParser(string fieldName, string idRange, string uniqueId)
        {
            if (!string.IsNullOrEmpty(idRange) && IdRangeParser.RangeRegex.IsMatch(idRange))
            {
                _FieldName = fieldName;
                ParseRange(idRange);
                _UniqueId = uniqueId;
            }
        }

        private void ParseRange(string idRange)
        {
            _IdRanges = new List<IdRange>();
            // REMOVE ALL SPACES
            string cleanIdRange = idRange.Replace(" ", string.Empty);
            // SPLIT INTO RANGE TOKENS ON , DELIMITER
            string[] idRangeTokens = cleanIdRange.Split(",".ToCharArray());
            foreach (string idRangeToken in idRangeTokens)
            {
                // GET THE RANGE
                Match m = _RangeTokenRegex.Match(idRangeToken);
                if (m.Success)
                {
                    int rangeStart = AlwaysConvert.ToInt(m.Groups[1].Value);
                    int rangeEnd;
                    if (string.IsNullOrEmpty(m.Groups[2].Value)) rangeEnd = rangeStart;
                    else rangeEnd = AlwaysConvert.ToInt(m.Groups[2].Value);
                    if (rangeStart > rangeEnd)
                    {
                        int rangeTemp = rangeStart;
                        rangeStart = rangeEnd;
                        rangeEnd = rangeTemp;
                    }
                    _IdRanges.Add(new IdRange(rangeStart, rangeEnd));
                }
            }
        }

        /// <summary>
        /// Returns the appropriate parameterized SQL criteria
        /// </summary>
        /// <returns>A parameterized SQL criteria</returns>
        /// <remarks>
        /// For Example, Range is 12-15,20.
        /// Method should return:
        /// ***
        /// ((OrderNumber &gt;= @r1b AND OrderNumber &lt;= @r1e)
        /// OR OrderNumber = @o20)
        /// ***
        /// Where r1b = range 1 begin, r1e = range 1 end, o20 = order 20
        /// </remarks>
        public string GetSqlString(string joinWord)
        {
            if (_IdRanges != null && _IdRanges.Count > 0)
            {
                StringBuilder sql = new StringBuilder();
                if (!string.IsNullOrEmpty(joinWord)) sql.Append(" " + joinWord + " ");
                for(int i = 0; i < _IdRanges.Count; i++)
                {
                    IdRange range = _IdRanges[i];
                    if (i > 0) sql.Append(" OR ");
                    sql.Append(range.GetSqlString(_UniqueId, _FieldName, i));
                }
                return sql.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Add parameters to the specified command for the range criteria
        /// </summary>
        /// <param name="database">The databae object</param>
        /// <param name="command">The command object to populate with parameters</param>
        public void AddParameters(Database database, DbCommand command)
        {
            if (_IdRanges != null)
            {
                for (int i = 0; i < _IdRanges.Count; i++)
                {
                    _IdRanges[i].AddParameter(database, command, _UniqueId, i);
                }
            }
        }

        private class IdRange
        {
            private int _RangeStart;
            private int _RangeEnd;
            public int RangeStart { get { return _RangeStart;}}
            public int RangeEnd { get {return _RangeEnd;}}
            public IdRange (int rangeStart, int rangeEnd)
            {
                this._RangeStart = rangeStart;
                this._RangeEnd = rangeEnd;
            }
            public string GetSqlString(string uniqueId, string fieldName, int parameterCount)
            {
                if (this._RangeStart == this._RangeEnd)
                {
                    return string.Format("{0} = @r" + uniqueId + "_{1}", fieldName, parameterCount);
                }
                else
                {
                    return string.Format("({0} >= @r" + uniqueId + "_s{1} AND {0} <= @r" + uniqueId + "_e{1})", fieldName, parameterCount);
                }
            }
            public void AddParameter(Database database, DbCommand command, string uniqueId, int parameterCount)
            {
                if (_RangeStart == _RangeEnd)
                {
                    database.AddInParameter(command, "@r" + uniqueId + "_" + parameterCount.ToString(), DbType.Int32, _RangeStart);
                }
                else
                {
                    database.AddInParameter(command, "@r" + uniqueId + "_s" + parameterCount.ToString(), DbType.Int32, _RangeStart);
                    database.AddInParameter(command, "@r" + uniqueId + "_e" + parameterCount.ToString(), DbType.Int32, _RangeEnd);
                }
            }
        }
    }
}
