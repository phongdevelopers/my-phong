using System;
using System.Collections;

namespace CommerceBuilder.Common
{
    /// <summary>
    /// A generic implementation of IComparer that can be used to compare objects based on a given property
    /// </summary>
    public class GenericComparer : IComparer
    {
        /// <summary>
        /// Enumeration that represents the direction of sorting
        /// </summary>
        public enum SortDirection
        {
            /// <summary>
            /// Sorting in ascending order
            /// </summary>
            ASC = -1,

            /// <summary>
            /// Sorting in descending order
            /// </summary>
            DESC = 1
        }

        string m_SortPropertyName;
        SortDirection m_SortDirection;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sortPropertyName">The property to be used for comparison. 
        /// The property name may include ascending or descending specifier.
        /// For example <b>Name ASC</b> will sort by name property in ascending order
        /// and <b>Name DESC</b> will sort by name property in descending order</param>
        public GenericComparer(string sortPropertyName)
        {
            string[] tokens;
            this.m_SortDirection = SortDirection.ASC;

            if (sortPropertyName.IndexOf("ASC", StringComparison.OrdinalIgnoreCase) > -1  ||
                sortPropertyName.IndexOf("DESC", StringComparison.OrdinalIgnoreCase) > -1) 
            {
                tokens = sortPropertyName.Split(" ".ToCharArray());
                if (tokens.Length > 1)
                {
                    m_SortPropertyName = tokens[0].Trim();
                    if (sortPropertyName.IndexOf("DESC", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        this.m_SortDirection = SortDirection.DESC;
                    }
                }
                else
                {
                    this.m_SortPropertyName = sortPropertyName;                    
                }
            }
            else
            {
                this.m_SortPropertyName = sortPropertyName;                
            }

            // default to ascending order
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sortPropertyName">The property to be used for comparison.</param>
        /// <param name="sortDirection">The direction of sorting; Ascending or Descending.</param>
        public GenericComparer(string sortPropertyName, SortDirection sortDirection)
        {
            this.m_SortPropertyName = sortPropertyName;
            this.m_SortDirection = sortDirection;
        }

        /// <summary>
        /// Implementation of Compare method of ICompare
        /// </summary>
        /// <param name="x">The first object to compare</param>
        /// <param name="y">The second object to compare</param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            // Get the values of the relevant property on the x and y objects

            object valueOfX = x.GetType().GetProperty(m_SortPropertyName).GetValue(x, null);
            object valueOfY = y.GetType().GetProperty(m_SortPropertyName).GetValue(y, null);

            IComparable comp = valueOfY as IComparable;

            // Flip the value from whatever it was to the opposite.
            return Flip(comp.CompareTo(valueOfX));
        }

        private int Flip(int i)
        {
            return (i * (int)m_SortDirection);
        }
    }
}