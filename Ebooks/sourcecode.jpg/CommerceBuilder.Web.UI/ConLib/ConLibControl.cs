using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Web.ConLib
{
    [Serializable]
    public class ConLibControl
    {
        private string _Name;
        private string _Summary;
        Collection<ConLibControlParam> _Params = new Collection<ConLibControlParam>();

        public string Name
        {
            get { return _Name; }
        }
        public string Summary
        {
            get { return _Summary; }
        }
        public Collection<ConLibControlParam> Params
        {
            get { return _Params; }
        }
        public ConLibControl(string name, string filePath)
        {
            _Name = name;
            this.ParseCommentHeader(ReadCommentHeader(filePath));
        }
        public string Usage
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[[ConLib:" + _Name);
                foreach (ConLibControlParam param in _Params)
                {
                    sb.Append(" " + param.Name + "=\"" + param.DefaultValue + "\"");
                }
                sb.Append("]]");
                return sb.ToString();
            }
        }
        private string ReadCommentHeader(string filePath)
        {
            string commentHeader = string.Empty;
            try
            {
                string allText = File.ReadAllText(filePath);
                Match match = Regex.Match(allText, "<conlib>[^\\x00]*?</conlib>", RegexOptions.IgnoreCase);
                if (match.Success) commentHeader = match.ToString();
            }
            catch { }
            return commentHeader;
        }
        private void ParseCommentHeader(string commentHeader)
        {
            try
            {
                Match summaryMatch = Regex.Match(commentHeader, "<summary>([^\\x00]*)</summary>", RegexOptions.IgnoreCase);
                if (summaryMatch.Success)
                    _Summary = summaryMatch.Groups[1].ToString();
                MatchCollection paramMatches = Regex.Matches(commentHeader, "<param name=\"([^\"]+)\"(?: default=\"([^\"]*)\")?>([^\\x00]*?)</param>");
                foreach (Match paramMatch in paramMatches)
                {
                    _Params.Add(new ConLibControlParam(paramMatch.Groups[1].ToString(), paramMatch.Groups[2].ToString(), paramMatch.Groups[3].ToString()));
                }
            }
            catch(Exception ex) 
            {
                Logger.Warn("An error occured while parsing ConLib control xml comments, File Name:" + Name + ".", ex);
            }
        }
    }
}
