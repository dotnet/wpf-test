// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

﻿
namespace Microsoft.Test.VariationGeneration
{
    /// <summary>
    /// Pairs a combination in consideration for addition to a variation with how many combinations it will cover
    /// </summary>
    internal class CandidateCoverage
    {
        public ValueCombination Value { get; set; }
        public int CoverageCount { get; set; }
    }
}
