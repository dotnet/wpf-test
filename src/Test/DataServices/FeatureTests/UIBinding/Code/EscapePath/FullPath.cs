// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.Test.DataServices
{
    public class FullPath : IRandomStringProvider
    {
        private Property _property;
        private Random _random;

        public FullPath(Random random)
        {
            this._random = random;
            _property = new Property(random);
        }

        // Property ("." Property)*
        public string GetRandomString()
        {
            string result = _property.GetRandomString();
            int randomNumber = _random.Next(0, 3);
            for (int i = 0; i < randomNumber; i++)
            {
                result += "." + _property.GetRandomString();
            }
            return result;
        }
    }
}
