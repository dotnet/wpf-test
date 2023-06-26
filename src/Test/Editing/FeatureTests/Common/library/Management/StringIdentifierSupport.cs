// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides an interface to provide support for value identifiers.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Management/StringIdentifierSupport.cs $")]

namespace Test.Uis.Management
{
    /// <summary>
    /// Interface implemented by all classes that support providing
    /// a string identifier.
    /// </summary>
    interface IStringIdentifierSupport
    {
        /// <summary>Non-null string identifier for this instance.</summary>
        /// <remarks>
        /// The string identifier obeys most rules of a C# identifier:
        /// no leading numbers, no whitespace, no punctuation, no
        /// symbols, no control characters. It is also non-zero length.
        /// </remarks>
        string StringIdentifier { get; }
    }
}
