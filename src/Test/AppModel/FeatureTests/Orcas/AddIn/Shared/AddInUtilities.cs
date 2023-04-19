// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.Utilities
{
    /// <summary>
    /// Specifies how to merge
    /// </summary>
    public enum MergeAffinity { UsePreviousList };

    public static class ListUtilities
    {
        /// <summary>
        /// Merges two or more sorted lists of the same type
        /// </summary>
        /// <typeparam name="T">Type of Lists to merge, must be IComparable</typeparam>
        /// <param name="lists">Array of Lists to merge</param>
        /// <param name="affinity">MergeListAffinity - Affinity to merge by</param>
        /// <returns></returns>
        public static List<T> Merge<T>(List<T>[] lists, MergeAffinity affinity) where T : System.IComparable
        {
            //Determine the mechanism to merge by
            switch (affinity)
            {
                case MergeAffinity.UsePreviousList:
                default:
                    return MergeListsUsePreviousList<T>(lists);
            }
        }

        private static List<T> MergeListsUsePreviousList<T>(List<T>[] lists) where T : System.IComparable
        {
            List<T> fullList = new List<T>();
            int[] indexes = new int[lists.Length];
            int lastSelectIndex = 0;
            int i = 0;
            int totalObjects = 0;

            int compare = 0;
            int currentLowListIndex = 0;

            for (i = 0; i < indexes.Length; i++)
            {
                indexes[i] = 0;
                totalObjects += lists[i].Count;
            }

            for (i = 0; i < totalObjects; i++)
            {
                //Start with the value from the previous selected list
                if (indexes[lastSelectIndex] < lists[lastSelectIndex].Count)
                {
                    currentLowListIndex = lastSelectIndex;
                }
                else
                {
                    //Nothing available in the previous list, flag to get first valid value
                    currentLowListIndex = -1;
                }

                //Loop through all the lists
                for (int j = 0; j < lists.Length; j++)
                {
                    //Check to see if the list is complete
                    if (indexes[j] < lists[j].Count)
                    {
                        //Determine if there is a currentLowListIndex value
                        if (currentLowListIndex < 0)
                        {
                            //Set the currentLowListIndex value if none was set
                            currentLowListIndex = j;
                        }
                        //Compare the current value to the currentLowListIndex value
                        compare = lists[j][indexes[j]].CompareTo(lists[currentLowListIndex][indexes[currentLowListIndex]]);
                        if (compare < 0)
                        {
                            //if the current value is lower, then set currentLowListIndex to it
                            currentLowListIndex = j;
                        }
                    }
                }
                
                // add the currentLowListIndex to the final list
                fullList.Add(lists[currentLowListIndex][indexes[currentLowListIndex]]);
                
                //set the last selected index
                lastSelectIndex = currentLowListIndex;

                //increment the current index
                indexes[currentLowListIndex]++;
            }

            return fullList;
        }
    }
}
