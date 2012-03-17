using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using CommerceBuilder.Common;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// Class that represents a scriptlet
    /// </summary>
    public partial class Scriptlet : IPersistable
    {
        private string _BaseDir;
        private string _ThemeId = string.Empty;
        private ScriptletType _ScriptletType = ScriptletType.Unspecified;
        private string _Identifier = string.Empty;
        private bool _IsCustom = false;
        private string _Description = string.Empty;
        private string _HeaderData = string.Empty;
        private string _ScriptletData = string.Empty;
        private bool _IsDirty = false;
        //VARIABLES TO STORE THE LOADED STATE OF THE SCRIPTLET
        private string _LoadedIdentifier = string.Empty;
        private ScriptletType _LoadedScriptletType = ScriptletType.Unspecified;
        private bool _LoadedIsCustom = false;
        //CREATE EXPRESSIONS FOR PARSING FILES
        private static Regex identifierRegex = new Regex(@"[^A-Z0-9_ ]", RegexOptions.IgnoreCase);
        private static Regex scriptletRegex = new Regex(@"^(?:<!--([^\x00]+?)-->)?([^\x00]*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static Regex descriptionRegex = new Regex(@"<Description>([^\x00]*)</Description>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static Regex headerRegex = new Regex(@"<HeaderData>([^\x00]*)</HeaderData>", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        /// <summary>
        /// Default constructor
        /// </summary>
        public Scriptlet()
        {
            //INITIALIZE THE BASE DIRECTORY
            _BaseDir = AppDomain.CurrentDomain.BaseDirectory;
            _ThemeId = string.Empty;
        }

        /// <summary>
        /// Theme Name/Identifier
        /// </summary>
        public string ThemeId
        {
            get { return _ThemeId; }
            set { _ThemeId = value; }
        }

        /// <summary>
        /// Scriptlet type
        /// </summary>
        public ScriptletType ScriptletType
        {
            get { return this._ScriptletType; }
            set
            {
                if (this._ScriptletType != value)
                {
                    this._ScriptletType = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Scriptlet identifier
        /// </summary>
        public string Identifier
        {
            get { return this._Identifier; }
            set
            {
                string filteredValue = identifierRegex.Replace(value, string.Empty);
                if (this._Identifier != filteredValue)
                {
                    this._Identifier = filteredValue;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Is this a custom scriptlet?
        /// </summary>
        public bool IsCustom
        {
            get { return this._IsCustom; }
            set
            {
                if (this._IsCustom != value)
                {
                    this._IsCustom = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Description of this scriptlet
        /// </summary>
        public string Description
        {
            get { return this._Description; }
            set
            {
                if (this._Description != value)
                {
                    this._Description = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Header data
        /// </summary>
        public string HeaderData
        {
            get { return this._HeaderData; }
            set
            {
                if (this._HeaderData != value)
                {
                    this._HeaderData = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Scriptlet data
        /// </summary>
        public string ScriptletData
        {
            get { return this._ScriptletData; }
            set
            {
                if (this._ScriptletData != value)
                {
                    this._ScriptletData = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Has this scriptlet object been modified?
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

        /// <summary>
        /// Gets the file path of this scriptlet
        /// </summary>
        /// <returns>The file path of this scriptlet</returns>
        private string GetFilePath()
        {
            if (this.Identifier == string.Empty) this.Identifier = Guid.NewGuid().ToString("n");            
            return GetFilePath(this.ThemeId, this.Identifier, this.ScriptletType, this.IsCustom);
        }

        /// <summary>
        /// Gets the file path for given scriptlet details
        /// </summary>
        /// <param name="identifier">The scriptlet identifier</param>
        /// <param name="scriptletType">The scriptlet type</param>
        /// <param name="custom">Whether the scriptlet is a custom scriptlet</param>
        /// <returns>The file path of the scriptlet</returns>
        private static string GetFilePath(string identifier, ScriptletType scriptletType, bool custom)
        {
            return GetFilePath(string.Empty, identifier, scriptletType, custom);
        }

        /// <summary>
        /// Gets the file path for given scriptlet details
        /// </summary>
        /// <param name="themeId">Name/Identifier of the theme</param>
        /// <param name="identifier">The scriptlet identifier</param>
        /// <param name="scriptletType">The scriptlet type</param>
        /// <param name="custom">Whether the scriptlet is a custom scriptlet</param>
        /// <returns>The file path of the scriptlet</returns>
        private static string GetFilePath(string themeId, string identifier, ScriptletType scriptletType, bool custom)
        {
            if (string.IsNullOrEmpty(themeId))
            {
                return "App_Data\\Scriptlets" + (custom ? "\\Custom\\" : "\\Default\\") + scriptletType.ToString() + "\\" + identifier + ".htm";
            }
            else
            {
                string pathPart = "App_Themes\\" + themeId + "\\Scriptlets";
                if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pathPart)))
                {
                    return "App_Themes\\" + themeId + "\\Scriptlets" + (custom ? "\\Custom\\" : "\\Default\\") + scriptletType.ToString() + "\\" + identifier + ".htm";
                }
                else
                {
                    return "App_Data\\Scriptlets" + (custom ? "\\Custom\\" : "\\Default\\") + scriptletType.ToString() + "\\" + identifier + ".htm";
                }
            }
        }

        /// <summary>
        /// Deletes this scriptlet
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            string filePath = GetFilePath();
            FileInfo fi = new FileInfo(Path.Combine(_BaseDir, filePath));
            if (fi.Exists)
            {
                try
                {
                    fi.Delete();
                    return true;
                }
                catch(Exception ex) {
                    Logger.Warn("Could not delete scriptlet file '" + filePath + "'.", ex);
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Loads this scriptlet object for given scriptlet identifier and scriptlet type
        /// </summary>
        /// <param name="themeId">Name/Identifier of the theme</param>
        /// <param name="identifier">criptlet identifier</param>
        /// <param name="scriptletType">The scriptlet type</param>
        /// <returns></returns>
        public bool Load(string themeId, string identifier, ScriptletType scriptletType)
        {
            return Load(themeId, identifier, scriptletType, BitFieldState.Any);
        }

        /// <summary>
        /// Loads this scriptlet object for given scriptlet identifier and scriptlet type
        /// </summary>
        /// <param name="themeId">Name/Identifier of the theme</param>
        /// <param name="identifier">criptlet identifier</param>
        /// <param name="scriptletType">The scriptlet type</param>
        /// <param name="custom">Is it a custom scriptlet, default script or any of the two</param>
        /// <returns></returns>
        public bool Load(string themeId, string identifier, ScriptletType scriptletType, BitFieldState custom)
        {
            //VALIDATE INPUT PARAMETERS
            if (string.IsNullOrEmpty(identifier)) throw new ArgumentNullException("Identifier must be specified.", "identifier");
            identifier = identifierRegex.Replace(identifier, string.Empty);
            if (scriptletType == ScriptletType.Unspecified) throw new ArgumentOutOfRangeException("Scriptlet type must be specified.", "scriptletType");
            //DETERMINE THE FILE PATH TO LOAD
            string baseDir = _BaseDir;
            string filePath;
            bool isCustom = false;
            switch (custom)
            {
                case BitFieldState.Any:
                    filePath = GetFilePath(themeId, identifier, scriptletType, true);
                    if (File.Exists(Path.Combine(baseDir, filePath))) isCustom = true;
                    else filePath = GetFilePath(identifier, scriptletType, false);
                    break;
                case BitFieldState.True:
                    filePath = GetFilePath(themeId, identifier, scriptletType, true);
                    isCustom = true;
                    break;
                default:
                    filePath = GetFilePath(themeId, identifier, scriptletType, false);
                    break;
            }
            //LOAD THE FILE
            FileInfo fi = new FileInfo(Path.Combine(baseDir, filePath));
            if (fi.Exists)
            {
                this.ThemeId = themeId;
                this.Identifier = identifier;
                _LoadedIdentifier = identifier;
                this.ScriptletType = scriptletType;
                _LoadedScriptletType = scriptletType;
                this.IsCustom = isCustom;
                _LoadedIsCustom = isCustom;
                try
                {
                    bool parsed = this.ParseScriptletFile(File.ReadAllText(fi.FullName));
                    if (parsed)
                    {
                        this.IsDirty = false;
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Logger.Warn("Could not read scriptlet file '" + filePath + "'.", ex);
                }
            }
            return false;
        }

        internal void ClearLoadedState()
        {
            _LoadedIdentifier = string.Empty;
            _LoadedIsCustom = false;
            _LoadedScriptletType = ScriptletType.Unspecified;
        }

        /// <summary>
        /// Saves this scriptlet
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            return Save(this.ThemeId);
        }

        /// <summary>
        /// Saves this scriptlet
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save(string themeId)
        {
            if (this.IsDirty)
            {
                //CHECK WHETHER THE LOADED SCRIPTLET FILE WAS CUSTOM
                //AND IT IS BEING SAVED AS A CUSTOM SCRIPTLET
                if (_LoadedIsCustom && this.IsCustom)
                {
                    if (_LoadedIdentifier != this.Identifier)
                    {
                        //THIS IS A FILE RENAME
                        //DELETE THE OLD FILE IF IT EXISTS
                        string oldFilePath = GetFilePath(themeId, _LoadedIdentifier, _LoadedScriptletType, true);
                        FileInfo oldFile = new FileInfo(Path.Combine(_BaseDir, oldFilePath));
                        if (oldFile.Exists)
                        {
                            try
                            {
                                oldFile.Delete();
                            }
                            catch (Exception ex)
                            {
                                Logger.Warn("Could not delete scriptlet file '" + oldFilePath + "'.", ex);
                            }
                        }
                    }
                }

                //GENERATE THE CURRENT FILE PATH
                string filePath = GetFilePath();
                FileInfo fi = new FileInfo(Path.Combine(_BaseDir, filePath));
                //DELETE EXISTING SCRIPTLET BEFORE SAVING NEW CONTENT
                bool update = false;
                if (fi.Exists)
                {
                    if (!this.Delete()) return SaveResult.Failed;
                    update = true;
                }
                try
                {
                    StringBuilder fileContent = new StringBuilder();
                    fileContent.Append(this.BuildCommentHeader());
                    fileContent.Append(this.ScriptletData);
                    //build the header data
                    File.WriteAllText(fi.FullName, fileContent.ToString());
                }
                catch (Exception ex)
                {
                    Logger.Warn("Could not write scriptlet file '" + filePath + "'.", ex);
                }
                return (update ? SaveResult.RecordUpdated : SaveResult.RecordInserted);
            }
            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

        /// <summary>
        /// Creates a copy of the given scriptlet
        /// </summary>
        /// <param name="identifier">Identifier of scriptlet to create copy of</param>
        /// <param name="scriptletType">Type of scriptlet to create copy of</param>
        /// <returns>Copy of the given scriptlet</returns>
        public static Scriptlet Copy(string identifier, ScriptletType scriptletType)
        {
            return Copy(string.Empty, identifier, scriptletType);
        }

        /// <summary>
        /// Creates a copy of the given scriptlet
        /// </summary>
        /// <param name="themeId">Name/Identifier of the theme</param>
        /// <param name="identifier">Identifier of scriptlet to create copy of</param>
        /// <param name="scriptletType">Type of scriptlet to create copy of</param>
        /// <returns>Copy of the given scriptlet</returns>
        public static Scriptlet Copy(string themeId, string identifier, ScriptletType scriptletType)
        {
            Scriptlet copy = ScriptletDataSource.Load(themeId, identifier, scriptletType, false);
            if (copy != null)
            {
                copy.Identifier = "Copy of " + copy.Identifier;
                copy.IsCustom = true;
                copy.ClearLoadedState();
                return copy;
            }
            return null;
        }

        private bool ParseScriptletFile(string fileData)
        {
            Match match = scriptletRegex.Match(fileData);
            if (match.Success)
            {
                string commentHeader = match.Groups[1].ToString();
                //LOOK FOR DESCRIPTION
                Match match2 = descriptionRegex.Match(commentHeader);
                if (match2.Success)
                {
                    //HAVE TO REPLACE ANY ENCODED COMMENT END TAGS
                    this.Description = match2.Groups[1].ToString().Trim(" \r\n\t".ToCharArray()).Replace("--&gt;", "-->");
                }
                match2 = headerRegex.Match(commentHeader);
                if (match2.Success)
                {
                    //HAVE TO REPLACE ANY ENCODED COMMENT END TAGS
                    this.HeaderData = match2.Groups[1].ToString().Trim(" \r\n\t".ToCharArray()).Replace("--&gt;", "-->");
                }
                this.ScriptletData = match.Groups[2].ToString().Trim(" \r\n\t".ToCharArray());
                return true;
            }
            return false;
        }

        private string BuildCommentHeader()
        {
            bool hasDescription = !string.IsNullOrEmpty(this.Description);
            bool hasHeaderData = !string.IsNullOrEmpty(this.HeaderData);
            //COMMENT HEADER NOT NEEDED IF DESCRIPTION AND HEADER DATA ARE EMPTY
            if (!hasDescription && !hasHeaderData) return string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("<!--\r\n");
            if (hasDescription)
            {
                //HAVE TO ENCODE ANY COMMENT END TAGS
                sb.Append("<Description>\r\n" + this.Description.Trim(" \r\n\t".ToCharArray()).Replace("-->", "--&gt;") + "\r\n</Description>\r\n");
            }
            if (hasHeaderData)
            {
                //HAVE TO ENCODE ANY COMMENT END TAGS
                sb.Append("<HeaderData>\r\n" + this.HeaderData.Trim(" \r\n\t".ToCharArray()).Replace("-->", "--&gt;") + "\r\n</HeaderData>\r\n");
            }
            sb.Append("-->\r\n");
            return sb.ToString();
        }
    }
}
