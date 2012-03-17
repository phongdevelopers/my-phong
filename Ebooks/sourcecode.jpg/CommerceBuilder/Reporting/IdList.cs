using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceBuilder.Reporting
{
    /// <summary>
    /// Class that represents a list of integer Ids
    /// </summary>
    [Serializable]
    public class IdList
    {
        private List<int> InnerList;

        /// <summary>
        /// Default constructor
        /// </summary>
        public IdList() {
            this.InnerList = new List<int>();
        }

        /// <summary>
        /// Add a given Id to this list
        /// </summary>
        /// <param name="id">Id to add to this list</param>
        public void Add(int id)
        {
            this.InnerList.Add(id);
        }

        /// <summary>
        /// Counts the number of objects in this list
        /// </summary>
        public int Count
        {
            get
            {
                return this.InnerList.Count;
            }
        }

        /// <summary>
        /// Gets the item at the given index
        /// </summary>
        /// <param name="index">Index of the item to get</param>
        /// <returns></returns>
        public int this[int index]
        {
            get
            {
                return this.InnerList[index];
            }
        }
        
        /// <summary>
        /// Clears this list
        /// </summary>
        public void Clear()
        {
            this.InnerList.Clear();
        }

        /// <summary>
        /// Removes a item from from the given index in this list
        /// </summary>
        /// <param name="index">Index of the item to remove</param>
        public void RemoveAt(int index)
        {
            this.InnerList.RemoveAt(index);
        }

        /// <summary>
        /// Gets the index of a given item in this list
        /// </summary>
        /// <param name="id">Id of the item to get index for</param>
        /// <returns>Index of the required item in this list</returns>
        public int IndexOf(int id)
        {
            return this.InnerList.IndexOf(id);
        }

        /// <summary>
        /// Creates a coma separated list of Ids
        /// </summary>
        /// <returns>A coma separated list of Ids</returns>
        public string ToList()
        {
            return this.ToList(",");
        }

        /// <summary>
        /// Creates a list of Ids separated by given separator
        /// </summary>
        /// <param name="separator">Separator to use for separating the list items</param>
        /// <returns>A separator separated list of Ids</returns>
        public string ToList(string separator)
        {
            if (InnerList.Count == 0) return string.Empty;
            string[] list = new string[InnerList.Count];
            for (int i = 0; i < InnerList.Count; i++)
            {
                list[i] = InnerList[i].ToString();
            }
            return String.Join(separator, list);
        }
    }
}
