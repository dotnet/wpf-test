// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
namespace Microsoft.Test.Globalization
{
    public class PackUriHelper
    {
        public static readonly string UriSchemePack = "pack";
        
        public static void RegisterPackScheme()
        {
            if (!UriParser.IsKnownScheme(UriSchemePack))
            {
                try
                {
                    SecurityPermission permobj = new SecurityPermission(SecurityPermissionFlag.Infrastructure);
                    permobj.Assert(); //BlessedAssert:

                    // Indicate that we want a default hierarchical parser with a registry based authority
                    UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), UriSchemePack, -1);
                }
                finally
                {
                    SecurityPermission.RevertAssert();
                }
            }            
        }
    }
}
