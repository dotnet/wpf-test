// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Microsoft.Test.DataServices
{
    public class Letter : IRandomStringProvider
    {
        private BaseChar _baseChar;
        private Ideographic _ideographic;
        private Random _random;

        public Letter(Random random)
        {
            this._random = random;
            _baseChar = new BaseChar(random);
            _ideographic = new Ideographic(random);
        }

        // BaseChar | Ideographic
        public string GetRandomString()
        {
            List<string> list = new List<string>();
            list.Add(_baseChar.GetRandomString());
            list.Add(_ideographic.GetRandomString());
            return UtilEscapePath.Or(list, _random);
        }
    }

}
