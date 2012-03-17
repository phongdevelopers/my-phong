//-----------------------------------------------------------------------
// <copyright file="OrderFilterDataSource.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace CommerceBuilder.Search
{
    using System;
    using System.ComponentModel;
    using System.Data.Common;
    using CommerceBuilder.Common;
    using CommerceBuilder.Data;
    using CommerceBuilder.Orders;
    using CommerceBuilder.Utility;

    /// <summary>
    /// Datasource used to bridge ASP.NET object datasources to the order filter class
    /// </summary>
    public static class OrderFilterDataSource
    {
        /// <summary>
        /// Counts the number of Order objects in result if retrieved using the given SQL criteria
        /// </summary>
        /// <param name="filter">The order filter that determines the Order objects that should be loaded.</param>
        /// <returns>The number of Order objects matching the criteria</returns>
        public static int Count(OrderFilter filter)
        {
            return filter.Count();
        }

        /// <summary>
        /// Loads a collection of Order objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="filter">The order filter that determines the Order objects that should be loaded.</param>
        /// <returns>A collection of Order objects</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public static OrderCollection Load(OrderFilter filter)
        {
            return filter.Load(0, 0, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Order objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="filter">The order filter that determines the Order objects that should be loaded.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Order objects</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public static OrderCollection Load(OrderFilter filter, string sortExpression)
        {
            return filter.Load(0, 0, sortExpression);
        }

        /// <summary>
        /// Loads a collection of Order objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="filter">The order filter that determines the Order objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>A collection of Order objects</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public static OrderCollection Load(OrderFilter filter, int maximumRows, int startRowIndex)
        {
            return filter.Load(maximumRows, startRowIndex, string.Empty);
        }

        /// <summary>
        /// Loads a collection of Order objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="filter">The order filter that determines the Order objects that should be loaded.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <returns>A collection of Order objects</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public static OrderCollection Load(OrderFilter filter, int maximumRows, int startRowIndex, string sortExpression)
        {
            return filter.Load(maximumRows, startRowIndex, sortExpression);
        }
    }
}
