// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    class NCNameChar : IRandomStringProvider
    {
        private Letter _letter;
        private Digit _digit;
        private string _dot;
        private string _dash;
        private string _underscore;
        private CombiningChar _combiningChar;
        private Extender _extender;
        private Random _random;

        public NCNameChar(Random random)
        {
            _letter = new Letter(random);
            _digit = new Digit(random);
            _dot = ".";
            _dash = "-";
            _underscore = "_";
            _combiningChar = new CombiningChar(random);
            _extender = new Extender(random);
            this._random = random;
        }

        // Letter | Digit | '.' | '-' | '_' | CombiningChar | Extender
        public string GetRandomString()
        {
            List<string> list = new List<string>();
            list.Add(_letter.GetRandomString());
            list.Add(_digit.GetRandomString());
            list.Add(_dot);
            list.Add(_dash);
            list.Add(_underscore);
            list.Add(_combiningChar.GetRandomString());
            list.Add(_extender.GetRandomString());
            return UtilEscapePath.Or(list, _random);
        }
    }
}
