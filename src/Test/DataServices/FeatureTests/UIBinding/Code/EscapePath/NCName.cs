// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    class NCName : IRandomStringProvider
    {
        private Letter _letter;
        private string _underscore;
        private NCNameChar _ncNameChar;
        private Random _random;

        public NCName(Random random)
        {
            this._random = random;
            _letter = new Letter(random);
            _underscore = "_";
            _ncNameChar = new NCNameChar(random);
        }

        //(Letter | '_')(NCNameChar)*
        public string GetRandomString()
        {
            string result = "";

            //(Letter | '_')
            List<string> list = new List<string>();
            list.Add(_letter.GetRandomString());
            list.Add(_underscore);
            result = UtilEscapePath.Or(list, _random);

            //(Letter | '_')(NCNameChar)*
            result += UtilEscapePath.ZeroOrMore(_ncNameChar, _random);

            return result;
        }
    }
}
