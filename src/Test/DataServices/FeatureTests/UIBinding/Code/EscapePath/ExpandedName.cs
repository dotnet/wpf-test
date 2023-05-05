// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    public class ExpandedName : IRandomStringProvider
    {
        private NCName _ncName;
        private UriEscapePath _uri;
        private Random _random;

        public ExpandedName(Random random) 
        {
            this._random = random;
            _ncName = new NCName(random);
            _uri = new UriEscapePath(random);
        }

        // "{" Uri "}" NCName | NCName
        public string GetRandomString()
        {
            string string1 = "{" + _uri.GetRandomString() + "}" + _ncName.GetRandomString();
            string string2 = _ncName.GetRandomString();
            List<string> list = new List<string>();
            list.Add(string1);
            list.Add(string2);
            return UtilEscapePath.Or(list, _random);
        }
    }
}
