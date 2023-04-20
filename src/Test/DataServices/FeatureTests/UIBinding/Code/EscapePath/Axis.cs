// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    public class Axis : IRandomStringProvider
    {
        private Random _random;

        public Axis(Random random)
        {
            this._random = random;
        }

        // "Element" | "Elements" | "Descendant" | "Descendants" | "Attribute" | "Attributes" | "Nodes"
        public string GetRandomString()
        {
            List<string> list = new List<string>();
            list.Add("Element");
            list.Add("Elements");
            list.Add("Descendant");
            list.Add("Descendants");
            list.Add("Attribute");
            list.Add("Attributes");
            list.Add("Nodes");
            return UtilEscapePath.Or(list, _random);
        }
    }
}
