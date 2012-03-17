using System;
using System.Collections.Generic;
using System.Text;
using CommerceBuilder.Utility;
using CommerceBuilder.Common;

namespace CommerceBuilder.Shipping.Providers
{
    /// <summary>
    /// Class that represents a shipping package
    /// </summary>
    public class Package
    {
        /// <summary>
        /// Weight of the pacakge
        /// </summary>
        public LSDecimal Weight;
        /// <summary>
        /// Weight unit
        /// </summary>
        public WeightUnit WeightUnit;
        /// <summary>
        /// Retail value of the package
        /// </summary>
        public LSDecimal RetailValue;
        /// <summary>
        /// Length of the package
        /// </summary>
        public LSDecimal Length;
        /// <summary>
        /// Width of the package
        /// </summary>
        public LSDecimal Width;
        /// <summary>
        /// Height of the package
        /// </summary>
        public LSDecimal Height;
        /// <summary>
        /// Measurement unit 
        /// </summary>
        public MeasurementUnit MeasurementUnit;

        /// <summary>
        /// Multiplier (number of such packages)
        /// </summary>
        public int Multiplier;
                
        //some providers also expect the item name 
        /// <summary>
        /// Name fo the package
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wu">Weight Unit</param>
        /// <param name="mu">Measurement Unit</param>
        public Package(WeightUnit wu, MeasurementUnit mu)
        {
            this.WeightUnit = wu;
            this.MeasurementUnit = mu;
        }

        /// <summary>
        /// Composes the package such that it is no more than the given maximum weight.
        /// If the weight is greater than the maximum weight, splits this package in multiple
        /// packages. i.e; Multiplier is updated.
        /// </summary>
        /// <param name="maxWeight">Maximum weight that the package should have</param>
        public void Compose(LSDecimal maxWeight)
        {   
            int pkgcount;
            if (this.Weight > maxWeight)
            {
                //split item into multiple items
                pkgcount = (int)Math.Ceiling(((Decimal)Weight / (Decimal)maxWeight));
                Multiplier = Multiplier * pkgcount;
                Weight = Weight / pkgcount;                
            }
        }

        /// <summary>
        /// Composes the package such that it is no more than the given maximum weight.
        /// If the weight is greater than the maximum weight, splits this package in multiple
        /// packages. i.e; Multiplier is updated. If the weight is less than the minimum 
        /// weight it is set equal to the minimum weight
        /// </summary>
        /// <param name="maxWeight">Maximum weight that the package should have</param>
        /// <param name="minWeight">Minimum weight that the package should have</param>
        public void Compose(LSDecimal maxWeight, LSDecimal minWeight)
        {
            int pkgcount;
            if (this.Weight > maxWeight)
            {
                //split item into multiple items
                pkgcount = (int)Math.Ceiling(((Decimal)Weight / (Decimal)maxWeight));
                Multiplier = Multiplier * pkgcount;
                Weight = Weight / pkgcount;
            }
            else if (this.Weight < minWeight)
            {
                this.Weight = minWeight;
            }
        }

        /// <summary>
        /// Converts all dimensions, weight and price of this package to whole numbers
        /// </summary>
        public void ConvertAllToWholeNumbers()
        {            
            Weight = Math.Ceiling((Decimal)Weight);
            RetailValue = Math.Ceiling((Decimal)RetailValue);
            Length = Math.Ceiling((Decimal)Length);
            Width = Math.Ceiling((Decimal)Width);
            Height = Math.Ceiling((Decimal)Height);
        }

        /// <summary>
        /// Converts weight of this package to whole number
        /// </summary>
        public void ConvertWeightToWholeNumber()
        {
            Weight = Math.Ceiling((Decimal)Weight);
        }

        /// <summary>
        /// Converts dimensions of this package to whole numbers
        /// </summary>
        public void ConvertDimsToWholeNumbers()
        {
            Length = Math.Ceiling((Decimal)Length);
            Width = Math.Ceiling((Decimal)Width);
            Height = Math.Ceiling((Decimal)Height);
        }

        /// <summary>
        /// Converts price of this package to whole number
        /// </summary>
        public void ConvertPriceToWholeNumber()
        {
            RetailValue = Math.Ceiling((Decimal)RetailValue);
        }

        /// <summary>
        /// Converts weight and price of this package to whole numbers
        /// </summary>
        public void ConvertWeightAndPriceToWholeNumbers()
        {
            Weight = Math.Ceiling((Decimal)Weight);
            RetailValue = Math.Ceiling((Decimal)RetailValue);
        }

        /// <summary>
        /// Rounds the weight of this package upto given decimals
        /// </summary>
        /// <param name="decimals">number of decimals in fractional part</param>
        public void RoundWeight(int decimals)
        {
            Weight = Math.Round((Decimal)Weight, decimals);
        }

        /// <summary>
        /// Rounds the price of this package upto given decimals
        /// </summary>
        /// <param name="decimals">number of decimals in fractional part</param>
        public void RoundPrice(int decimals)
        {
            RetailValue = Math.Round((Decimal)RetailValue, decimals);
        }

        /// <summary>
        /// Rounds price and weight of this package upto given decimals
        /// </summary>
        /// <param name="decimals">number of decimals in fractional part</param>
        public void RoundWeightAndPrice(int decimals)
        {
            Weight = Math.Round((Decimal)Weight, decimals);
            RetailValue = Math.Round((Decimal)RetailValue, decimals);
        }

        /// <summary>
        /// Rounds the dimensions of this package upto given decimals
        /// </summary>
        /// <param name="decimals">number of decimals in fractional part</param>
        public void RoundDimensions(int decimals)
        {
            Length = Math.Round((Decimal)Length, decimals);
            Width = Math.Round((Decimal)Width, decimals);
            Height = Math.Round((Decimal)Height, decimals);
        }

        /// <summary>
        /// Rounds dimensions, weight and price of this package upto given decimals
        /// </summary>
        /// <param name="decimals">number of decimals in fractional part</param>
        public void RoundAll(int decimals)
        {
            Weight = Math.Round((Decimal)Weight, decimals);
            RetailValue = Math.Round((Decimal)RetailValue, decimals);
            Length = Math.Round((Decimal)Length, decimals);
            Width = Math.Round((Decimal)Width, decimals);
            Height = Math.Round((Decimal)Height, decimals);
        }

        /// <summary>
        /// Converts weight of this package to the given weight unit
        /// </summary>
        /// <param name="to">The weight unit to convert to</param>
        public void ConvertWeight(WeightUnit to)
        {            
            Weight = LocaleHelper.ConvertWeight(this.WeightUnit, Weight, to);
            this.WeightUnit = to;
        }

        /// <summary>
        /// Converts dimensions of this package to the given measurement unit
        /// </summary>
        /// <param name="to">The measurement unit to convert to</param>
        public void ConvertDimensions(MeasurementUnit to)
        {
            Height = LocaleHelper.ConvertMeasurement(this.MeasurementUnit , Height, to);
            Width  = LocaleHelper.ConvertMeasurement(this.MeasurementUnit , Width , to);
            Length = LocaleHelper.ConvertMeasurement(this.MeasurementUnit , Length, to);
            this.MeasurementUnit = to;
        }

        /// <summary>
        /// Converts weight and dimensions of this package to new units
        /// </summary>
        /// <param name="toWunit">The weight unit to convert weight to</param>
        /// <param name="toMunit">The measurement unit to convert </param>
        public void ConvertBoth(WeightUnit toWunit, MeasurementUnit toMunit)
        {
            ConvertWeight(toWunit);
            ConvertDimensions(toMunit);
        }
        
        /// <summary>
        /// Rearranges the dimensions so that length is the largest dimension and height is the smallest
        /// </summary>
        public void RearrangeDimensions()
        {
            //Length is the largest, hight is the smallest
            LSDecimal[] dims = new LSDecimal[3];
            dims[0] = this.Length;
            dims[1] = this.Width;
            dims[2] = this.Height;
            Array.Sort(dims);
            this.Height = dims[0];
            this.Width = dims[1];
            this.Length = dims[2];
        }

        /// <summary>
        /// Creates a string representation the details of this package
        /// </summary>
        /// <returns>A string representation the details of this package</returns>
        public override string ToString()
        {
            return string.Format("Package '{8}' : Weight={0}, WeightUnit={1}, RetailValue={2}, Length={3}, Width={4}, Height={5}, MeasurementUnit={6}, Multiplier={7}", Weight, WeightUnit, RetailValue, Length, Width, Height, MeasurementUnit, Multiplier, Name);
        }
    }

}
