// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Implements WeightList type.
 *
********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Test.Win32;

namespace Microsoft.Test.Collections.Generics
{
    /// <summary>
    /// Represents an uneven distribution of items according 
    /// to a given weight value. The probability of retrieving any
    /// item using a random index value (0.00 &lt;= value &lt;= 1.00)
    /// is proportional to the item's weight relative to the total
    /// weight of the list.
    /// </summary>
    /// <typeparam name="T">The Type of the list items.</typeparam>
    /// <example>
    /// // Creates a list with 1/3 of the total weight holding value=100
    /// // and 2/3 of the total weight holding value=200.
    /// WeightedList&lt;object> list = new WeightedList&lt;object>();
    /// list.Add(1, new int(100));
    /// list.Add(2, new int(200));
    /// 
    /// // val will be 200 since 0.40 (40%) retrieves the item from
    /// // the upper 2/3 of the total weight.
    /// int val = list[0.40];
    /// </example>
    public class WeightedList<T>
    {
        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="itemWeight">The weight of the item.</param>
        /// <param name="item">The item. The item does not have to be unique. If a single item is added more than once, the weights are effectively added.</param>
        public void Add(int itemWeight, T item)
        {
            if (itemWeight < 1)
            {
                return;
            }

            // Get the group of items with the same weight
            // as the new item.  Create the group if the weight
            // is new.
            List<T> itemGroup;
            if (_itemGroups[itemWeight] == null)
            {
                itemGroup = new List<T>();
                _itemGroups[itemWeight] = itemGroup;
            }
            else
            {
                itemGroup = (List<T>)_itemGroups[itemWeight];
            }

            itemGroup.Add(item);

            _totalWeight += itemWeight;
        }
        /// <summary>
        /// Retrieves a random item from the list.
        /// </summary>
        /// <returns>A single item.</returns>
        public T GetRandomItem()
        {
            return this[new Random().NextDouble()];
        }
        /// <summary>
        /// Retrieves an item from the list at the given distribution point.
        /// </summary>
        /// <param name="weightIndex">The distribution point. Must be between 0 and 1.</param>
        /// <returns>A single item.</returns>
        /// <remarks>
        /// The same weightIndex will always return the same item in the list
        /// as long as the list is unchanged.  Changing the list also changes
        /// the distribution because the total weight of the list changes.
        /// </remarks>
        public T this[double weightIndex]
        {
            get
            {
                if (weightIndex < 0.0 || weightIndex > 1.0)
                {
                    throw new ArgumentOutOfRangeException("weightIndex", "Value must be 0.0 <= x <= 1.0.");
                }

                if (this.TotalWeight == 0)
                {
                    throw new InvalidOperationException("There are no items in the list.");
                }

                if (weightIndex == 0.0)
                {
                    weightIndex = 0.0001;
                }


                double targetWeight = Math.Ceiling(this.TotalWeight * weightIndex);

                List<T> itemGroup = null;
                double currentWeight = 0;
                int itemWeight = 0;

                // Loop through each group until the current total
                // weight reaches the target weight.
                foreach (int key in _itemGroups.Keys)
                {
                    itemWeight = key;

                    itemGroup = (List<T>)_itemGroups[itemWeight];

                    double groupWeight = (double)(itemWeight * itemGroup.Count);

                    if (currentWeight + groupWeight >= targetWeight)
                        break;

                    currentWeight += groupWeight;
                }

                // Adjust scope of targetWeight to the current group.
                targetWeight -= currentWeight;

                // Adjust targetWeight to nearest ceiling multiple of itemWeight.
                double mod = targetWeight % itemWeight;
                if(mod > 0.0)
                    targetWeight += (itemWeight - mod);

                // Calculate index into group.
                // Must add 0.01 because Math.Round() rounds 0.5 
                // down instead of up.
                double dbindex = 0.01 + ((targetWeight - itemWeight) / (double)itemWeight);
                int index = (int)Math.Round(dbindex);

                T item = itemGroup[index];

                return item;
            }
        }
        /// <summary>
        /// Returns the total weight of all items.
        /// </summary>
        public int TotalWeight
        {
            get
            {
                return _totalWeight;
            }
        }

        // Items are grouped by weight.  Thos groups are sorted
        // by weight in this list.
        private SortedList _itemGroups = new SortedList();

        // Cache of total weight count.
        private int _totalWeight = 0;
    }
}
