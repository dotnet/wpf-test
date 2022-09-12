// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//  Enumeration of the different modes in which the DRT for serialization
//  of DRX classes can run.
//
//

namespace MS.Internal.WppDrt.EDocsUx
{
    /// <summary>
    ///   Enumeration of the different modes in which the DRT for serialization
    ///   of EDocs UX classes can run 
    /// </summary>
    internal enum SerializationTestCaseRunMode
    {
        /// <summary>
        ///   Run the DRT test cases.
        /// </summary>
        RunDrt,

        /// <summary>
        ///   Regenerate the baseline comparison files against which the results
        ///   of the serialization are compared.
        /// </summary>
        Rebaseline
    }
}
