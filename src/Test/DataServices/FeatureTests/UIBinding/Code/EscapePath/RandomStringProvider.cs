// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    public abstract class RandomStringProvider : IRandomStringProvider
    {
        protected List<Range> ranges;
        protected int overallCount;
        protected Random random;

        public RandomStringProvider(Random random)
        {
            ranges = new List<Range>();
            overallCount = 0;
            this.random = random;
        }

        public string GetRandomString()
        {
            int randomIndex = random.Next(0, overallCount); // 0 is inclusive, overallCount is exclusive

            int countSoFar = 0;
            foreach (Range range in ranges)
            {
                countSoFar += range.Count;
                if (randomIndex < countSoFar)
                {
                    return new String(new char[] { range.GetCharInIndex(randomIndex - (countSoFar - range.Count)) });
                }
            }

            throw new InvalidOperationException("Random char not found in range");
        }

        protected int GetOverallCount()
        {
            int overallCount = 0;
            foreach (Range range in ranges)
            {
                overallCount += range.Count;
            }
            return overallCount;
        }
    }
}
