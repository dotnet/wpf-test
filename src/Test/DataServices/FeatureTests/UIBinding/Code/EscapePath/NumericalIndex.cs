// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    public class NumericalIndex :IRandomStringProvider
    {
        private Digit _digit;
        private Random _random;

        public NumericalIndex(Random random)
        {
            this._random = random;
            _digit = new Digit(random);
        }

        public string GetRandomString()
        {
            return UtilEscapePath.OneOrMore(_digit, _random);
        }
    }
}
