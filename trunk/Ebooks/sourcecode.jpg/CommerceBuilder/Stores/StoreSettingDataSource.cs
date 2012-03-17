using CommerceBuilder.Common;
using CommerceBuilder.Data;
using System.ComponentModel;

namespace CommerceBuilder.Stores
{
    /// <summary>
    /// DataSource class for StoreSetting objects
    /// </summary>
    [DataObject(true)]
    public partial class StoreSettingDataSource
    {
        /// <summary>
        /// Loads the collection of store settings for the current store
        /// </summary>
        /// <returns>Collection of store settings indexed with the setting name</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public virtual PersistentDictionary<string, StoreSetting> LoadDictionaryForStore()
        {
            PersistentCollection<StoreSetting> settingCollection = StoreSettingDataSource.LoadForStore();
            PersistentDictionary<string, StoreSetting> settingDictionary = new PersistentDictionary<string, StoreSetting>();
            foreach (StoreSetting setting in settingCollection)
            {
                settingDictionary.Add(setting.FieldName, setting);
            }
            return settingDictionary;
        }
    }
}
