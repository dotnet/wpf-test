// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

// Context ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("4A9ECD62-8152-49C5-B0C1-3E070423E4E3")]

//Xmlns defninitions
[assembly: System.Windows.Markup.XmlnsDefinition("http://XamlTestTypes", "Microsoft.Test.Xaml.Types")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://XamlTestTypes", "Microsoft.Test.Xaml.Types.Attributes")]

[assembly: InternalsVisibleTo("XamlWpfTests40, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c1bcd9b7844cd35725b1138216ed5e0fab914676d580860f6f081c8c0e83b92d835ffae76fa968f7637e4b6e32d77fee1c084088cee736ce589d65ad0fa66f086c2f5f755a6f2909d17c643b45c72a906aaac617bfd67491a8fce54784ca98317aea28c0544546ff9123f6f94e59793f2874437bc3d136a40095d0f960e6a6a4")]
[assembly: InternalsVisibleTo("XamlTests35, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c1bcd9b7844cd35725b1138216ed5e0fab914676d580860f6f081c8c0e83b92d835ffae76fa968f7637e4b6e32d77fee1c084088cee736ce589d65ad0fa66f086c2f5f755a6f2909d17c643b45c72a906aaac617bfd67491a8fce54784ca98317aea28c0544546ff9123f6f94e59793f2874437bc3d136a40095d0f960e6a6a4")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
#if VS_BUILD
[assembly: AssemblyVersionAttribute("4.0.0.0")]
#endif
