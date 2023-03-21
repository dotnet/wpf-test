// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $")]

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

