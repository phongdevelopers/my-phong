//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Products;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Products
{
    /// <summary>
    /// This class represents a ReviewerProfile object in the database.
    /// </summary>
    public partial class ReviewerProfile : IPersistable
    {

#region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ReviewerProfile() { }

        /// <summary>
        /// Constructor with primary key
        /// <param name="reviewerProfileId">Value of ReviewerProfileId.</param>
        /// </summary>
        public ReviewerProfile(Int32 reviewerProfileId)
        {
            this.ReviewerProfileId = reviewerProfileId;
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
          columnNames.Add(prefix + "ReviewerProfileId");
          columnNames.Add(prefix + "Email");
          columnNames.Add(prefix + "DisplayName");
          columnNames.Add(prefix + "Location");
          columnNames.Add(prefix + "EmailVerified");
          columnNames.Add(prefix + "EmailVerificationCode");
          return string.Join(",", columnNames.ToArray());
        }

        /// <summary>
        /// Loads the given ReviewerProfile object from the given database data reader.
        /// </summary>
        /// <param name="reviewerProfile">The ReviewerProfile object to load.</param>
        /// <param name="dr">The database data reader to read data from.</param>
        public static void LoadDataReader(ReviewerProfile reviewerProfile, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA
            reviewerProfile.ReviewerProfileId = dr.GetInt32(0);
            reviewerProfile.Email = dr.GetString(1);
            reviewerProfile.DisplayName = dr.GetString(2);
            reviewerProfile.Location = NullableData.GetString(dr, 3);
            reviewerProfile.EmailVerified = dr.GetBoolean(4);
            reviewerProfile.EmailVerificationCode = NullableData.GetGuid(dr, 5);
            reviewerProfile.IsDirty = false;
        }

#endregion

        private Int32 _ReviewerProfileId;
        private String _Email = string.Empty;
        private String _DisplayName = string.Empty;
        private String _Location = string.Empty;
        private Boolean _EmailVerified;
        private Guid _EmailVerificationCode;
        private bool _IsDirty;

        /// <summary>
        /// ReviewerProfileId
        /// </summary>
        [DataObjectField(true, true, false)]
        public Int32 ReviewerProfileId
        {
            get { return this._ReviewerProfileId; }
            set
            {
                if (this._ReviewerProfileId != value)
                {
                    this._ReviewerProfileId = value;
                    this.IsDirty = true;
                    this.EnsureChildProperties();
                }
            }
        }

        /// <summary>
        /// Email
        /// </summary>
        public String Email
        {
            get { return this._Email; }
            set
            {
                if (this._Email != value)
                {
                    this._Email = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// DisplayName
        /// </summary>
        public String DisplayName
        {
            get { return this._DisplayName; }
            set
            {
                if (this._DisplayName != value)
                {
                    this._DisplayName = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Location
        /// </summary>
        public String Location
        {
            get { return this._Location; }
            set
            {
                if (this._Location != value)
                {
                    this._Location = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// EmailVerified
        /// </summary>
        public Boolean EmailVerified
        {
            get { return this._EmailVerified; }
            set
            {
                if (this._EmailVerified != value)
                {
                    this._EmailVerified = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// EmailVerificationCode
        /// </summary>
        public Guid EmailVerificationCode
        {
            get { return this._EmailVerificationCode; }
            set
            {
                if (this._EmailVerificationCode != value)
                {
                    this._EmailVerificationCode = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether this ReviewerProfile object has changed since it was loaded from the database.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                if (this._IsDirty) return true;
                if (this.ProductReviewsLoaded && this.ProductReviews.IsDirty) return true;
                return false;
            }
            set { this._IsDirty = value; }
        }

        /// <summary>
        /// Ensures that child objects of this ReviewerProfile are properly associated with this ReviewerProfile object.
        /// </summary>
        public virtual void EnsureChildProperties()
        {
            if (this.ProductReviewsLoaded) { foreach (ProductReview productReview in this.ProductReviews) { productReview.ReviewerProfileId = this.ReviewerProfileId; } }
        }

#region Children
        private ProductReviewCollection _ProductReviews;

        /// <summary>
        /// A collection of ProductReview objects associated with this ReviewerProfile object.
        /// </summary>
        public ProductReviewCollection ProductReviews
        {
            get
            {
                if (!this.ProductReviewsLoaded)
                {
                    this._ProductReviews = ProductReviewDataSource.LoadForReviewerProfile(this.ReviewerProfileId);
                }
                return this._ProductReviews;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal bool ProductReviewsLoaded { get { return (this._ProductReviews != null); } }

#endregion

        /// <summary>
        /// Deletes this ReviewerProfile object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_ReviewerProfiles");
            deleteQuery.Append(" WHERE ReviewerProfileId = @ReviewerProfileId");
            Database database = Token.Instance.Database;
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {
                database.AddInParameter(deleteCommand, "@ReviewerProfileId", System.Data.DbType.Int32, this.ReviewerProfileId);
                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            return (recordsAffected > 0);
        }


        /// <summary>
        /// Load this ReviewerProfile object from the database for the given primary key.
        /// </summary>
        /// <param name="reviewerProfileId">Value of ReviewerProfileId of the object to load.</param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(Int32 reviewerProfileId)
        {
            bool result = false;
            this.ReviewerProfileId = reviewerProfileId;
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ReviewerProfiles");
            selectQuery.Append(" WHERE ReviewerProfileId = @reviewerProfileId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@reviewerProfileId", System.Data.DbType.Int32, reviewerProfileId);
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
        /// Saves this ReviewerProfile object to the database.
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of the save operation.</returns>
        protected SaveResult BaseSave()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                bool recordExists = true;
                
                if (this.ReviewerProfileId == 0) recordExists = false;

                if (recordExists) {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT COUNT(*) As RecordCount FROM ac_ReviewerProfiles");
                    selectQuery.Append(" WHERE ReviewerProfileId = @ReviewerProfileId");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {
                        database.AddInParameter(selectCommand, "@ReviewerProfileId", System.Data.DbType.Int32, this.ReviewerProfileId);
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
                    updateQuery.Append("UPDATE ac_ReviewerProfiles SET ");
                    updateQuery.Append("Email = @Email");
                    updateQuery.Append(", DisplayName = @DisplayName");
                    updateQuery.Append(", Location = @Location");
                    updateQuery.Append(", EmailVerified = @EmailVerified");
                    updateQuery.Append(", EmailVerificationCode = @EmailVerificationCode");
                    updateQuery.Append(" WHERE ReviewerProfileId = @ReviewerProfileId");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {
                        database.AddInParameter(updateCommand, "@ReviewerProfileId", System.Data.DbType.Int32, this.ReviewerProfileId);
                        database.AddInParameter(updateCommand, "@Email", System.Data.DbType.String, this.Email);
                        database.AddInParameter(updateCommand, "@DisplayName", System.Data.DbType.String, this.DisplayName);
                        database.AddInParameter(updateCommand, "@Location", System.Data.DbType.String, NullableData.DbNullify(this.Location));
                        database.AddInParameter(updateCommand, "@EmailVerified", System.Data.DbType.Boolean, this.EmailVerified);
                        database.AddInParameter(updateCommand, "@EmailVerificationCode", System.Data.DbType.Guid, NullableData.DbNullify(this.EmailVerificationCode));
                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_ReviewerProfiles (Email, DisplayName, Location, EmailVerified, EmailVerificationCode)");
                    insertQuery.Append(" VALUES (@Email, @DisplayName, @Location, @EmailVerified, @EmailVerificationCode)");
                    insertQuery.Append("; SELECT Scope_Identity()");
                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {
                        database.AddInParameter(insertCommand, "@ReviewerProfileId", System.Data.DbType.Int32, this.ReviewerProfileId);
                        database.AddInParameter(insertCommand, "@Email", System.Data.DbType.String, this.Email);
                        database.AddInParameter(insertCommand, "@DisplayName", System.Data.DbType.String, this.DisplayName);
                        database.AddInParameter(insertCommand, "@Location", System.Data.DbType.String, NullableData.DbNullify(this.Location));
                        database.AddInParameter(insertCommand, "@EmailVerified", System.Data.DbType.Boolean, this.EmailVerified);
                        database.AddInParameter(insertCommand, "@EmailVerificationCode", System.Data.DbType.Guid, NullableData.DbNullify(this.EmailVerificationCode));
                        //RESULT IS NEW IDENTITY;
                        result = AlwaysConvert.ToInt(database.ExecuteScalar(insertCommand));
                        this._ReviewerProfileId = result;
                    }
                }
                this.SaveChildren();

                //OBJECT IS DIRTY IF NO RECORDS WERE UPDATED OR INSERTED
                this.IsDirty = (result == 0);
                if (this.IsDirty) { return SaveResult.Failed; }
                else { return (recordExists ? SaveResult.RecordUpdated : SaveResult.RecordInserted); }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

        /// <summary>
        /// Saves that child objects associated with this ReviewerProfile object.
        /// </summary>
        public virtual void SaveChildren()
        {
            this.EnsureChildProperties();
            if (this.ProductReviewsLoaded) this.ProductReviews.Save();
        }

     }
}
