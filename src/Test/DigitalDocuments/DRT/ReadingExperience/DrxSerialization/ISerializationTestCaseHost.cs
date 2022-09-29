// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//  This interface provides the services that a running serialization test case
//  needs.
//
//



namespace MS.Internal.WppDrt.EDocsUx
{
    internal interface ISerializationTestCaseHost
    {
        /// <summary>
        ///   The mode in which the DRT is run. See the SerializationTestCaseRunMode
        ///   enumeration for details.
        /// </summary>
        SerializationTestCaseRunMode Mode { get; }

        /// <summary>
        ///   Directory from which baseline comparison files are read or to which they
        ///   are written.
        /// </summary>
        string BaselineDirectory { get; }
       
    }
}

