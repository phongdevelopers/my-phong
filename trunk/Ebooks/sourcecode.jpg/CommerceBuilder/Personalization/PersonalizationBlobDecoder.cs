namespace CommerceBuilder.Personalization
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Web.UI;

    internal static class PersonalizationBlobDecoder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Dictionary<string, PersonalizationInfo> Decode(byte[] data)
        {
            if (data == null || data.Length == 0) return null;
            try
            {
                Dictionary<string, PersonalizationInfo> result = new Dictionary<string, PersonalizationInfo>();
                Queue<object> DataQueue;
                ObjectStateFormatter formatter = new ObjectStateFormatter();
                using (MemoryStream stream = new MemoryStream(data))
                {
                    object[] DataArray = (object[])formatter.Deserialize(stream);
                    DataQueue = new Queue<object>(DataArray);
                }
                int version = (int)DataQueue.Dequeue();
                if (version == 2)
                {
                    int NumberOfParts = (int)DataQueue.Dequeue();
                    for (int PartCounter = 0; PartCounter < NumberOfParts; PartCounter++)
                    {
                        PersonalizationInfo info = PersonalizationInfo.FromObjectQueue(DataQueue);
                        result.Add(info.ControlID, info);
                    }
                }
                return result;
            }
            catch { }
            return null;
        }
    }
}