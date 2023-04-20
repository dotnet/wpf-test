// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    public class UriEscapePath : IRandomStringProvider
    {
        private Random _random;
        private LegalChar _legalChar;

        public UriEscapePath(Random random)
        {
            this._random = random;
            _legalChar = new LegalChar(random);
        }

        public string GetRandomString()
        {
            return UtilEscapePath.OneOrMore(_legalChar, _random);
        }
    }
}
