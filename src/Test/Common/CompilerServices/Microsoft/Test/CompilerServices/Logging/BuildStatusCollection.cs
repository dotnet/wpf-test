using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.CompilerServices.Logging
{
    /// <summary>
    /// A collection of BuildStatus objects used to store
    /// errors and warnings
    /// </summary>
    public class BuildStatusCollection : List<BuildStatus>
    {
        /// <summary>
        /// Override of List.Contains, calls new Contains method
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public new bool Contains(BuildStatus item)
        {
            return Contains(item, false);
        }

        /// <summary>
        /// determines if the current collection contains the provided item
        /// uses the Compare methods defined on BuildStatus
        /// </summary>
        /// <param name="item">item to be compared</param>
        /// <param name="deepComparison">determines the level of comparison</param>
        /// <returns></returns>
        public bool Contains(BuildStatus item, bool deepComparison)
        {
            if (item == null)
            {
                return false;
            }
            for (int i = 0; i < this.Count; i++)
            {
                if (BuildStatus.Compare(item, this[i], deepComparison))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
