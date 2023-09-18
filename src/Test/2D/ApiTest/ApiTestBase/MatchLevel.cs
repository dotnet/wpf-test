// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
  *   Program:   MatchLevels used by all comparators
 ************************************************************/
using System;

namespace Microsoft.Test.Graphics
{
    internal enum MatchLevel
    {
        Exact = 0,          // exactly the same
        Excellent = 1,      // a human cannot tell the difference
        Good = 2,           // acceptable in most cases, reasonable amount of rounding errors won't exceed it
        Poor = 3            // something wrong with code, set this level for won't fix bugs
    }
}
