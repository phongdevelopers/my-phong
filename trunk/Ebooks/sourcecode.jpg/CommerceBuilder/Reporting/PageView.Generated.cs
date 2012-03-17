//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Stores;
using CommerceBuilder.Users;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// This class represents a PageView object in the database.
    /// </summary>
    public partial class PageView : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PageView() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="pageViewId">Value of PageViewId.</param>
        /// </summary>
        public PageView(Int32 pageViewId)
        {
            this.PageViewId = pageViewId;
        }

        /// <summary>
        /// Returns a coma separated list of column names in this database object.
        /// </summary>
        /// <param name="prefix">Prefix to use with column names. Leave null or empty for no prefix.</param>
        /// <returns>A coman separated list of column names for this database object.</returns>
        public static string GetColumnNames(string prefix)
        {
          if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
          else prefix = prefix + ".";
          List<string> columnNames = new List<string>();
          columnNames.Add(prefix + "PageViewId");
          columnNames.Add(prefix + "StoreId");
          columnNames.Add(prefix + "ActivityDate");
          columnNames.Add(prefix + "RemoteIP");
          columnNames.Add(prefix + "RequestMethod");
          columnNames.Add(prefix + "UserId");
          columnNames.Add(prefix + "UriStem");
          columnNames.Add(prefix + "UriQuery");
          columnNames.Add(prefix + "TimeTaken");
          columnNames.Add(prefix + "UserAgent");
          columnNames.Add(prefix + "Referrer");
          columnNames.Add(prefix + "CatalogNodeId");
          columnNames.Add(prefix + "CatalogNodeTypeId");
          columnNames.Add(prefix + "Browser");
          columnNames.Add(prefix + "BrowserName");
          columnNames.Add(prefix + "BrowserPlatform");
          columnNames.Add(prefix + "BrowserVersion");
          columnNames.Add(prefix + "AffiliateId");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given PageView object from the given database data reader.
        /// </summary>
        /// <param name="pageView">The PageView object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(PageView pageView, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            pageView.PageViewId = dr.GetInt32(0);
            pageView.StoreId = dr.GetInt32(1);
            pageView.ActivityDate = LocaleHelper.ToLocalTime(dr.GetDateTime(2));
            pageView.RemoteIP = dr.GetString(3);
            pageView.RequestMethod = dr.GetString(4);
            pageView.UserId = dr.GetInt32(5);
            pageView.UriStem = dr.GetString(6);
            pageView.UriQuery = NullableData.GetString(dr, 7);
            pageView.TimeTaken = dr.GetInt32(8);
            pageView.UserAgent = NullableData.GetString(dr, 9);
            pageView.Referrer = NullableData.GetString(dr, 10);
            pageView.CatalogNodeId = NullableData.GetInt32(dr, 11);
            pageView.CatalogNodeTypeId = NullableData.GetInt16(dr, 12);
            pageView.Browser = dr.GetString(13);
            pageView.BrowserName = dr.GetString(14);
            pageView.BrowserPlatform = dr.GetString(15);
            pageView.BrowserVersion = dr.GetString(16);
            pageView.AffiliateId = dr.GetInt32(17);
            pageView.IsDirty = false;
        }

#endregion

        private Int32 _PageViewId;
        private Int32 _StoreId;
        private DateTime _ActivityDate;
        private String _RemoteIP = string.Empty;
        private String _RequestMethod = string.Empty;
        private Int32 _UserId;
        private String _UriStem = string.Empty;
        private String _UriQuery = string.Empty;
        private Int32 _TimeTaken;
        private String _UserAgent = string.Empty;
        private String _Referrer = string.Empty;
        private Int32 _CatalogNodeId;
        private Int16 _CatalogNodeTypeId;
        private String _Browser = string.Empty;
        private String _BrowserName = string.Empty;
        private String _BrowserPlatform = string.Empty;
        private String _BrowserVersion = string.Empty;
        private Int32 _AffiliateId;
        private bool _IsDirty;

        /// <summary>
        /// PageViewId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 PageViewId
        {
            get { return this._PageViewId; }
            set
            {
                if (this._PageViewId != value)
                {
                    this._PageViewId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// StoreId
        /// </summary>
        public Int32 StoreId
        {
            get { return this._StoreId; }
            set
            {
                if (this._StoreId != value)
                {
                    this._StoreId = value;
                    this.IsDirty = true;
                    this._Store = null;
                }
            }
        }

        /// <summary>
        /// ActivityDate
        /// </summary>
        public DateTime ActivityDate
        {
            get { return this._ActivityDate; }
            set
            {
                if (this._ActivityDate != value)
                {
                    this._ActivityDate = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// RemoteIP
        /// </summary>
        public String RemoteIP
        {
            get { return this._RemoteIP; }
            set
            {
                if (this._RemoteIP != value)
                {
                    this._RemoteIP = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// RequestMethod
        /// </summary>
        public String RequestMethod
        {
            get { return this._RequestMethod; }
            set
            {
                if (this._RequestMethod != value)
                {
                    this._RequestMethod = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// UserId
        /// </summary>
        public Int32 UserId
        {
            get { return this._UserId; }
            set
            {
                if (this._UserId != value)
                {
                    this._UserId = value;
                    this.IsDirty = true;
                    this._User = null;
                }
            }
        }

        /// <summary>
        /// UriStem
        /// </summary>
        public String UriStem
        {
            get { return this._UriStem; }
            set
            {
                if (this._UriStem != value)
                {
                    this._UriStem = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// UriQuery
        /// </summary>
        public String UriQuery
        {
            get { return this._UriQuery; }
            set
            {
                if (this._UriQuery != value)
                {
                    this._UriQuery = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// TimeTaken
        /// </summary>
        public Int32 TimeTaken
        {
            get { return this._TimeTaken; }
            set
            {
                if (this._TimeTaken != value)
                {
                    this._TimeTaken = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// UserAgent
        /// </summary>
        public String UserAgent
        {
            get { return this._UserAgent; }
            set
            {
                if (this._UserAgent != value)
                {
                    this._UserAgent = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Referrer
        /// </summary>
        public String Referrer
        {
            get { return this._Referrer; }
            set
            {
                if (this._Referrer != value)
                {
                    this._Referrer = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// CatalogNodeId
        /// </summary>
        public Int32 CatalogNodeId
        {
            get { return this._CatalogNodeId; }
            set
            {
                if (this._CatalogNodeId != value)
                {
                    this._CatalogNodeId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// CatalogNodeTypeId
        /// </summary>
        public Int16 CatalogNodeTypeId
        {
            get { return this._CatalogNodeTypeId; }
            set
            {
                if (this._CatalogNodeTypeId != value)
                {
                    this._CatalogNodeTypeId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Browser
        /// </summary>
        public String Browser
        {
            get { return this._Browser; }
            set
            {
                if (this._Browser != value)
                {
                    this._Browser = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// BrowserName
        /// </summary>
        public String BrowserName
        {
            get { return this._BrowserName; }
            set
            {
                if (this._BrowserName != value)
                {
                    this._BrowserName = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// BrowserPlatform
        /// </summary>
        public String BrowserPlatform
        {
            get { return this._BrowserPlatform; }
            set
            {
                if (this._BrowserPlatform != value)
                {
                    this._BrowserPlatform = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// BrowserVersion
        /// </summary>
        public String BrowserVersion
        {
            get { return this._BrowserVersion; }
            set
            {
                if (this._BrowserVersion != value)
                {
                    this._BrowserVersion = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// AffiliateId
        /// </summary>
        public Int32 AffiliateId
        {
            get { return this._AffiliateId; }
            set
            {
                if (this._AffiliateId != value)
                {
                    this._AffiliateId = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this PageView object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

#region Parents
        private Store _Store;
        private User _User;

        /// <summary>
        /// The Store object that this PageView object is associated with
        /// </summary>
        public Store Store
        {
            get
            {
                if (!this.StoreLoaded)
                {
                    this._Store = StoreDataSource.Load(this.StoreId);
                }
                return this._Store;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool StoreLoaded { get { return ((this._Store != null) && (this._Store.StoreId == this.StoreId)); } }

        /// <summary>
        /// The User object that this PageView object is associated with
        /// </summary>
        public User User
        {
            get
            {
                if (!this.UserLoaded)
                {
                    this._User = UserDataSource.Load(this.UserId);
                }
                return this._User;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool UserLoaded { get { return ((this._User != null) && (this._User.UserId == this.UserId)); } }

#endregion

        /// <summary>
        /// Deletes this PageView object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_PageViews");
            deleteQuery.Append(" WHERE PageViewId = @PageViewId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@PageViewId", System.Data.DbType.Int32, this.PageViewId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this PageView object from the database for the given primary key.
        /// </summary>
        /// <param name="pageViewId">Value of PageViewId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 pageViewId)
        {
            bool result = false;
            this.PageViewId = pageViewId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_PageViews");
            selectQuery.Append(" WHERE PageViewId = @pageViewId");
            selectQuery.Append(" AND StoreId = @storeId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@pageViewId", System.Data.DbType.Int32, pageViewId);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    result = true;
                    LoadDataReader(this, dr);;
                }
                dr.Close();
            }
            return result;
        }

        /// <summary>
        /// Saves this PageView object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;
                
                //SET EMPTY STOREID TO CURRENT CONTEXT
                if (this.StoreId == 0) this.StoreId = Token.Instance.StoreId;
                if (this.PageViewId == 0) recordExists = false;

                //SET DEFAULT FOR DATE FIELD
                if (this.ActivityDate == System.DateTime.MinValue) this.ActivityDate = LocaleHelper.LocalNow;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_PageViews");
                    selectQuery.Append(" WHERE PageViewId = @PageViewId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@PageViewId", System.Data.DbType.Int32, this.PageViewId);
                        if ((int)database.ExecuteScalar(selectCommand) == 0)
                        {
                            recordExists = false;
                        }
                    }
                }

                int result = 0;
                if (recordExists)
                {
                    //UPDATE
                    StringBuilder updateQuery = new StringBuilder();
                    updateQuery.Append("UPDATE ac_PageViews SET ");
                    updateQuery.Append("StoreId = @StoreId");
                    updateQuery.Append(", ActivityDate = @ActivityDate");
                    updateQuery.Append(", RemoteIP = @RemoteIP");
                    updateQuery.Append(", RequestMethod = @RequestMethod");
                    updateQuery.Append(", UserId = @UserId");
                    updateQuery.Append(", UriStem = @UriStem");
                    updateQuery.Append(", UriQuery = @UriQuery");
                    updateQuery.Append(", TimeTaken = @TimeTaken");
                    updateQuery.Append(", UserAgent = @UserAgent");
                    updateQuery.Append(", Referrer = @Referrer");
                    updateQuery.Append(", CatalogNodeId = @CatalogNodeId");
                    updateQuery.Append(", CatalogNodeTypeId = @CatalogNodeTypeId");
                    updateQuery.Append(", Browser = @Browser");
                    updateQuery.Append(", BrowserName = @BrowserName");
                    updateQuery.Append(", BrowserPlatform = @BrowserPlatform");
                    updateQuery.Append(", BrowserVersion = @BrowserVersion");
                    updateQuery.Append(", AffiliateId = @AffiliateId");
                    updateQuery.Append(" WHERE PageViewId = @PageViewId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@PageViewId", System.Data.DbType.Int32, this.PageViewId);
                        database.AddInParameter(updateCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(updateCommand, "@ActivityDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.ActivityDate));
                        database.AddInParameter(updateCommand, "@RemoteIP", System.Data.DbType.String, this.RemoteIP);
                        database.AddInParameter(updateCommand, "@RequestMethod", System.Data.DbType.String, this.RequestMethod);
                        database.AddInParameter(updateCommand, "@UserId", System.Data.DbType.Int32, this.UserId);
                        database.AddInParameter(updateCommand, "@UriStem", System.Data.DbType.String, this.UriStem);
                        database.AddInParameter(updateCommand, "@UriQuery", System.Data.DbType.String, NullableData.DbNullify(this.UriQuery));
                        database.AddInParameter(updateCommand, "@TimeTaken", System.Data.DbType.Int32, this.TimeTaken);
                        database.AddInParameter(updateCommand, "@UserAgent", System.Data.DbType.String, NullableData.DbNullify(this.UserAgent));
                        database.AddInParameter(updateCommand, "@Referrer", System.Data.DbType.String, NullableData.DbNullify(this.Referrer));
                        database.AddInParameter(updateCommand, "@CatalogNodeId", System.Data.DbType.Int32, NullableData.DbNullify(this.CatalogNodeId));
                        database.AddInParameter(updateCommand, "@CatalogNodeTypeId", System.Data.DbType.Int16, NullableData.DbNullify(this.CatalogNodeTypeId));
                        database.AddInParameter(updateCommand, "@Browser", System.Data.DbType.String, this.Browser);
                        database.AddInParameter(updateCommand, "@BrowserName", System.Data.DbType.String, this.BrowserName);
                        database.AddInParameter(updateCommand, "@BrowserPlatform", System.Data.DbType.String, this.BrowserPlatform);
                        database.AddInParameter(updateCommand, "@BrowserVersion", System.Data.DbType.String, this.BrowserVersion);
                        database.AddInParameter(updateCommand, "@AffiliateId", System.Data.DbType.Int32, this.AffiliateId);
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_PageViews (StoreId, ActivityDate, RemoteIP, RequestMethod, UserId, UriStem, UriQuery, TimeTaken, UserAgent, Referrer, CatalogNodeId, CatalogNodeTypeId, Browser, BrowserName, BrowserPlatform, BrowserVersion, AffiliateId)");
                    insertQuery.Append(" VALUES (@StoreId, @ActivityDate, @RemoteIP, @RequestMethod, @UserId, @UriStem, @UriQuery, @TimeTaken, @UserAgent, @Referrer, @CatalogNodeId, @CatalogNodeTypeId, @Browser, @BrowserName, @BrowserPlatform, @BrowserVersion, @AffiliateId)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@PageViewId", System.Data.DbType.Int32, this.PageViewId);
                        database.AddInParameter(insertCommand, "@StoreId", System.Data.DbType.Int32, this.StoreId);
                        database.AddInParameter(insertCommand, "@ActivityDate", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.ActivityDate));
                        database.AddInParameter(insertCommand, "@RemoteIP", System.Data.DbType.String, this.RemoteIP);
                        database.AddInParameter(insertCommand, "@RequestMethod", System.Data.DbType.String, this.RequestMethod);
                        database.AddInParameter(insertCommand, "@UserId", System.Data.DbType.Int32, this.UserId);
                        database.AddInParameter(insertCommand, "@UriStem", System.Data.DbType.String, this.UriStem);
                        database.AddInParameter(insertCommand, "@UriQuery", System.Data.DbType.String, NullableData.DbNullify(this.UriQuery));
                        database.AddInParameter(insertCommand, "@TimeTaken", System.Data.DbType.Int32, this.TimeTaken);
                        database.AddInParameter(insertCommand, "@UserAgent", System.Data.DbType.String, NullableData.DbNullify(this.UserAgent));
                        database.AddInParameter(insertCommand, "@Referrer", System.Data.DbType.String, NullableData.DbNullify(this.Referrer));
                        database.AddInParameter(insertCommand, "@CatalogNodeId", System.Data.DbType.Int32, NullableData.DbNullify(this.CatalogNodeId));
                        database.AddInParameter(insertCommand, "@CatalogNodeTypeId", System.Data.DbType.Int16, NullableData.DbNullify(this.CatalogNodeTypeId));
                        database.AddInParameter(insertCommand, "@Browser", System.Data.DbType.String, this.Browser);
                        database.AddInParameter(insertCommand, "@BrowserName", System.Data.DbType.String, this.BrowserName);
                        database.AddInParameter(insertCommand, "@BrowserPlatform", System.Data.DbType.String, this.BrowserPlatform);
                        database.AddInParameter(insertCommand, "@BrowserVersion", System.Data.DbType.String, this.BrowserVersion);
                        database.AddInParameter(insertCommand, "@AffiliateId", System.Data.DbType.Int32, this.AffiliateId);
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._PageViewId = result;
                    }
                }

                //OBJECT IS DIRTY IF NO RECORDS WERE UPDATED OR INSERTED
                this.IsDirty = (result == 0);
                if (this.IsDirty) { return SaveResult.Failed; }
                else { return (recordExists ? SaveResult.RecordUpdated : SaveResult.RecordInserted); }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

     }
}
