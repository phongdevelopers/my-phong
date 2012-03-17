using System;
using System.Collections.Generic;

namespace CommerceBuilder.Common
{
    /// <summary>
    /// A generic collection that is sortable
    /// </summary>
    /// <typeparam name="T">Type of the object for this collection</typeparam>
    [Serializable]
    public class SortableCollection<T> : System.Collections.CollectionBase
    {
        /// <summary>
        /// Collection Indexer
        /// </summary>
        /// <param name="index">Index of the object in the collection</param>
        /// <returns>The object at the given index</returns>
        public virtual T this[int index]
        {
            get { return (T)this.List[index]; }
            set { this.List[index] = value; }
        }

        /// <summary>
        /// Returns index of given item in this collection
        /// </summary>
        /// <param name="item">The item for which to return the index</param>
        /// <returns>The index of the given item</returns>
        public virtual int IndexOf(T item)
        {
            return this.List.IndexOf(item);
        }

        /// <summary>
        /// Add the given item to this collection
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>The index where the item is added</returns>
        public virtual int Add(T item)
        {
            return this.List.Add(item);
        }

        /// <summary>
        /// Remove the given object from this collection
        /// </summary>
        /// <param name="item">The object to remove</param>
        public virtual void Remove(T item)
        {
            this.List.Remove(item);
        }

        /// <summary>
        /// Copy all objects of this collection to given array
        /// </summary>
        /// <param name="array">Array to which to copy the objects</param>
        /// <param name="index">Index from where to start copying</param>
        public virtual void CopyTo(Array array, int index)
        {
            this.List.CopyTo(array, index);
        }

        /// <summary>
        /// Add all objects form another SortableCollection to this collection
        /// </summary>
        /// <param name="collection">The collection to add objects from</param>
        public virtual void AddRange(SortableCollection<T> collection)
        {
            this.InnerList.AddRange(collection);
        }

        /// <summary>
        /// Add all objects form another generic collection to this collection
        /// </summary>
        /// <param name="collection">The collection to add objects from</param>
        public virtual void AddRange(T[] collection)
        {
            this.InnerList.AddRange(collection);
        }

        /// <summary>
        /// Add all objects form another generic collection to this collection
        /// </summary>
        /// <param name="collection">The collection to add objects from</param>
        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Does this collection contain the given item?
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>True if the collection contains the given item, false otherwise</returns>
        public virtual bool Contains(T item)
        {
            return this.List.Contains(item);
        }

        /// <summary>
        /// Insert the given item at the given index in this collection
        /// </summary>
        /// <param name="index">Index where the object is to be inserted</param>
        /// <param name="item">The object to insert</param>
        public virtual void Insert(int index, T item)
        {
            this.List.Insert(index, item);
        }

        /// <summary>
        /// Find an object in this collection using the given predicate
        /// </summary>
        /// <param name="match">Predicate to use for matching</param>
        /// <returns>The matched object</returns>
        public T Find(Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentException("match");
            }
            for (int num1 = 0; num1 < this.Count; num1++)
            {
                if (match(this[num1]))
                {
                    return this[num1];
                }
            }
            return default(T);
        }

        /// <summary>
        /// Find all objects in this collection that match the given predicate
        /// </summary>
        /// <param name="match">The predicate to use for matching</param>
        /// <returns>List of objects in this collection that matched the given predicate</returns>
        public List<T> FindAll(Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentException("match");
            }
            List<T> matches = new List<T>();
            for (int num1 = 0; num1 < this.Count; num1++)
            {
                if (match(this[num1]))
                {
                    matches.Add(this[num1]);
                }
            }
            return matches;
        }

        /// <summary>
        /// Sort this collection using the given comparer
        /// </summary>
        /// <param name="comparer">The comparator to use for sorting</param>
        public void Sort(System.Collections.IComparer comparer)
        {
            InnerList.Sort(comparer);
        }

        /// <summary>
        /// Sort this collection using given sort expression and sort direction. 
        /// The GenericComparer is used to create the comparer for sorting. 
        /// </summary>
        /// <param name="sortExpression">The sort expression to use in GenericComparer</param>
        /// <param name="sortDirection">The sort direction to use in GenericComparer</param>
        public void Sort(string sortExpression, GenericComparer.SortDirection sortDirection)
        {
            InnerList.Sort(new GenericComparer(sortExpression, sortDirection));
        }

        /// <summary>
        /// Sort this collection using given sort expression. The GenericComparer is used to create 
        /// the comparer for sorting. 
        /// </summary>
        /// <param name="sortExpression">The sort expression to use in GenericComparer</param>
        public void Sort(string sortExpression)
        {
            InnerList.Sort(new GenericComparer(sortExpression));
        }
    }
}
