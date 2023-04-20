// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    public static class UtilEscapePath
    {
        private static string RepeatXToYTimes(IRandomStringProvider provider, int x, int y, Random random)
        {
            string result = "";
            int randomNumber = random.Next(x, y);
            for (int i = x; i < randomNumber; i++)
            {
                result += provider.GetRandomString();
            }
            return result;
        }

        public static string OneOrMore(IRandomStringProvider provider, Random random)
        {
            return RepeatXToYTimes(provider, 1, 4, random); // 1, 2, 3
        }

        public static string ZeroOrMore(IRandomStringProvider provider, Random random)
        {
            return RepeatXToYTimes(provider, 0, 3, random); // 0, 1, 2
        }

        public static string Or(List<string> list, Random random)
        {
            int count = list.Count;
            int randomNumber = random.Next(count);
            return list[randomNumber];
        }
    }
}
