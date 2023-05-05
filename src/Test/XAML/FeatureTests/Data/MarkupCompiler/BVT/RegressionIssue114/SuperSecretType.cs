// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WpfApplication1, PublicKey=00240000048000009400000006020000002400005253413100040000010001001d46bc0630cddc7b03685d9a57dde67054474ebeb9a94709a6d4ced4d28ff76e3b7b8b62fb7e27d1674d45552e1e7af2cc029035b4fd852fead2d94212cb42ae63aeb78dbcd66d442ce0c746fe785589db551d2c77868090b98906fd8a229133ad7409a33c052930cc8434b41177fca0022bacaaab021c016ef24a043846259b")]

namespace InternalTypes
{
    internal class SuperSecretType
    {
        public string Name
        {
            get
            {
                return "Foo";
            }
        }
    }
}
