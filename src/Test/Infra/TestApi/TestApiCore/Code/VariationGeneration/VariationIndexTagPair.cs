// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

﻿
namespace Microsoft.Test.VariationGeneration
{
    /// <summary>
    /// Contains the indices of variation and the corresponding tag.  Used to build the actual Variation.
    /// </summary>
    internal class VariationIndexTagPair
    {
        public int[] Indices { get; set; }
        public object Tag { get; set; }
    }
}
