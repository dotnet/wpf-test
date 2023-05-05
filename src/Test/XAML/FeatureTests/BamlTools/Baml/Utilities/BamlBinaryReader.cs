// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;

namespace Microsoft.Test.Baml.Utilities
{

    public class BamlBinaryReader: BinaryReader
    {
        public BamlBinaryReader(Stream stream)
            :base(stream)
        {
        }

        public BamlBinaryReader(Stream stream, Encoding code)
            :base(stream, code)
        {
        }

        public new int Read7BitEncodedInt()
        {
            return base.Read7BitEncodedInt();
        }
    }
}
