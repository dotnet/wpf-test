// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    public class Property :IRandomStringProvider
    {
        private Axis _axis;
        private NumericalIndex _numericalIndex;
        private ExpandedName _expandedName;
        private Random _random;

        public Property(Random random)
        {
            this._random = random;
            _axis = new Axis(random);
            _numericalIndex = new NumericalIndex(random);
            _expandedName = new ExpandedName(random);
        }

        // Axis ("[" NumericalIndex "]" | "[" ExpandedName "]")?
        public string GetRandomString()
        {
            // zero or one
            int randomNumber = _random.Next(0, 2); // 0, 1
            if (randomNumber == 0)
            {
                return _axis.GetRandomString();
            }
            else
            {
                List<string> list = new List<string>();
                list.Add("[" + _numericalIndex.GetRandomString() + "]");
                list.Add("[" + _expandedName.GetRandomString() + "]");
                string stringOr = UtilEscapePath.Or(list, _random);
                return _axis.GetRandomString() + stringOr;
            }
        }
    }
}
