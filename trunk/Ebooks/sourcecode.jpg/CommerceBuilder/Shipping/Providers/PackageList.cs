using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Common;

namespace CommerceBuilder.Shipping.Providers
{
    /// <summary>
    /// A class that represents a List of Package objects.
    /// </summary>
    public class PackageList : List<Package>
    {
        /// <summary>
        /// Composes the Packages in this list in a way so that their maximum weight 
        /// does not exceed the given value.
        /// </summary>
        /// <param name="maxWeight">The maximum weight that a Package should have</param>
        public void Compose(LSDecimal maxWeight)
        {
            foreach (Package item in this)
            {
                item.Compose(maxWeight);
            }
        }

        /// <summary>
        /// Composes the Packages in this list in a way so that their maximum weight 
        /// does not exceed the given maximum value and is not below the given minimum value.
        /// </summary>
        /// <param name="maxWeight">The maximum weight that a Package should have</param>
        /// <param name="minWeight">The minimum weight that a Package should have</param>
        public void Compose(LSDecimal maxWeight, LSDecimal minWeight)
        {
            foreach (Package item in this)
            {
                item.Compose(maxWeight, minWeight);
            }
        }

        /// <summary>
        /// Ensures no package in the package list is below the minimum weight.
        /// </summary>
        /// <param name="minWeight">The minimum weight that a package should have</param>
        public void EnsureMinimumWeight(LSDecimal minWeight)
        {
            foreach (Package item in this)
            {
                if (item.Weight < minWeight) item.Weight = minWeight;
            }
        }
        
        /// <summary>
        /// For all Package items in this list weight, dimensions and price are converted 
        /// to whole numbers.
        /// </summary>
        public void ConvertAllToWholeNumbers()
        {
            foreach (Package item in this)
            {
                item.ConvertAllToWholeNumbers();
            }
        }

        /// <summary>
        /// For all Package items in this list their weight is converted to a whole number.
        /// </summary>
        public void ConvertWeightToWholeNumber()
        {
            foreach (Package item in this)
            {
                item.ConvertWeightToWholeNumber();
            }
        }

        /// <summary>
        /// For all Package items in this list their dimensions are converted to whole numbers.
        /// </summary>
        public void ConvertDimsToWholeNumbers()
        {
            foreach (Package item in this)
            {
                item.ConvertDimsToWholeNumbers();
            }
        }

        /// <summary>
        /// For all Package items in this list their price is converted to a whole number.
        /// </summary>
        public void ConvertPriceToWholeNumber()
        {
            foreach (Package item in this)
            {
                item.ConvertPriceToWholeNumber();
            }
        }

        /// <summary>
        /// For all Package items in this list their weight and price are converted to a whole numbers.
        /// </summary>
        public void ConvertWeightAndPriceToWholeNumbers()
        {
            foreach (Package item in this)
            {
                item.ConvertWeightAndPriceToWholeNumbers();
            }
        }

        /// <summary>
        /// For all Package items in this list their weight is rounded upto to given dicimals.
        /// </summary>
        /// <param name="decimals">Number of decimals</param>
        public void RoundWeight(int decimals)
        {
            foreach (Package item in this)
            {
                item.RoundWeight(decimals);
            }
        }

        /// <summary>
        /// For all Package items in this list their price is rounded upto to given dicimals.
        /// </summary>
        /// <param name="decimals">Number of decimals</param>
        public void RoundPrice(int decimals)
        {
            foreach (Package item in this)
            {
                item.RoundPrice(decimals);
            }
        }

        /// <summary>
        /// For all Package items in this list their weight and price is rounded upto to given dicimals.
        /// </summary>
        /// <param name="decimals">Number of decimals</param>
        public void RoundWeightAndPrice(int decimals)
        {
            foreach (Package item in this)
            {
                item.RoundWeightAndPrice(decimals);
            }
        }

        /// <summary>
        /// For all Package items in this list their dimensions are rounded upto to given dicimals.
        /// </summary>
        /// <param name="decimals">Number of decimals</param>
        public void RoundDimensions(int decimals)
        {
            foreach (Package item in this)
            {
                item.RoundDimensions(decimals);
            }
        }

        /// <summary>
        /// For all Package items in this list their weight, price and dimensions are rounded upto to given dicimals.
        /// </summary>
        /// <param name="decimals">Number of decimals</param>
        public void RoundAll(int decimals)
        {
            foreach (Package item in this)
            {
                item.RoundAll(decimals);
            }
        }

        /// <summary>
        /// For all Package items in this list their weight is converted to the given weight unit.
        /// </summary>
        /// <param name="to">The weight unit to convert to</param>
        public void ConvertWeight(WeightUnit to)
        {
            foreach (Package item in this)
            {
                item.ConvertWeight(to);
            }
        }

        /// <summary>
        /// For all Package items in this list their dimensions are converted to the given measurement unit.
        /// </summary>
        /// <param name="to">The measurement unit to convert to</param>
        public void ConvertDimensions(MeasurementUnit to)
        {
            foreach (Package item in this)
            {
                item.ConvertDimensions(to);
            }
        }

        /// <summary>
        /// For all Package items in this list their weight is converted to the given weight unit 
        /// and their dimensions are converted to the given measurement unit.
        /// </summary>
        /// <param name="toW">The weight unit to convert to</param>
        /// <param name="toM">The measurement unit to convert to</param>
        public void ConvertBoth(WeightUnit toW, MeasurementUnit toM)
        {
            foreach (Package item in this)
            {
                item.ConvertBoth(toW, toM);
            }
        }

        /// <summary>
        /// Creates a string representation of the contents of this list.
        /// </summary>
        /// <returns>A string representation of the contents of this list.</returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("Package List\r\n");
            foreach (Package item in this)
            {
                result.Append(item.ToString() + "\r\n");
            }
            return result.ToString();
        }

    }
}
