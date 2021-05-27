// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// Selector doesn't combine the IVariationGenerator Children as the CombinationGenerator.   
    /// Generate call will return each for the VariationItem for each IVG Children individually.
    /// </summary>
    public class SelectorGenerator : BaseVariationGenerator, IVariationGenerator
	{
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<VariationItem> Generate()
        {
            List<List<VariationItem>> variationListList = base.GenerateIVGChildrenSequential();

            List<VariationItem> selectorList = new List<VariationItem>();

            foreach (List<VariationItem> lvi in variationListList)
            {
                foreach (VariationItem item in lvi)
                {
                    item.Merge(this);
                    selectorList.Add(item);
                }
            }

            return selectorList;
        }
    }
}
