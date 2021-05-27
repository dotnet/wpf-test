// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;

namespace DRT
{
    // An enumeration of the security levels for the tests
    internal enum SecurityLevel {
        Unrestricted = 0, // FullTrust or no partial trust
        PartialTrust = 1, // defined as IntranetZone today
        Default = PartialTrust,
    }

    internal class TestedSecurityLevelAttribute : Attribute {

        // The constructor is called when the attribute is set.
        public TestedSecurityLevelAttribute(SecurityLevel level)
        {
            _level = level;
        }

        // Keep a variable internally ...
        protected SecurityLevel _level = SecurityLevel.Default;

        // .. and show a copy to the outside world.
        public SecurityLevel TestedLevel
        {
            get
            {
                return _level;
            }
            set {
                _level = value;
            }
        }
    }
}
