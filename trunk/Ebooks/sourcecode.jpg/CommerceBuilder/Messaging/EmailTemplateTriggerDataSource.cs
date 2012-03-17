using System.ComponentModel;
using System.Text;
using System.Data;
using System.Data.Common;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Stores;

namespace CommerceBuilder.Messaging
{
    /// <summary>
    /// DataSource class for EmailTemplateTrigger objects
    /// </summary>
    [DataObject(true)]
    public partial class EmailTemplateTriggerDataSource
    {
        /// <summary>
        /// Loads a collection of EmailTemplateTrigger objects for the given StoreEvent object
        /// </summary>
        /// <param name="storeEvent">StoreEvent for which to load the EmailTemplateTriggers</param>
        /// <returns>A collection of EmailTemplateTrigger objects for the given StoreEvent object</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PersistentCollection<EmailTemplateTrigger> LoadForStoreEvent(StoreEvent storeEvent)
        {
            PersistentCollection<EmailTemplateTrigger> EmailTemplateTriggerCollection = new PersistentCollection<EmailTemplateTrigger>();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            //GET RECORDS STARTING AT FIRST ROW
            selectQuery.Append("SELECT StoreEventId, EmailTemplateId");
            selectQuery.Append(" FROM ac_EmailTemplateTriggers");
            selectQuery.Append(" WHERE StoreEventId = @storeEventId");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@storeEventId", System.Data.DbType.Int32, (int)storeEvent);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    EmailTemplateTrigger storeEventTrigger = new EmailTemplateTrigger();
                    //SET FIELDS FROM ROW DATA
                    storeEventTrigger.StoreEventId = dr.GetInt32(0);
                    storeEventTrigger.EmailTemplateId = dr.GetInt32(1);
                    storeEventTrigger.IsDirty = false;
                    EmailTemplateTriggerCollection.Add(storeEventTrigger);
                }
                dr.Close();
            }
            return EmailTemplateTriggerCollection;
        }
    }
}
