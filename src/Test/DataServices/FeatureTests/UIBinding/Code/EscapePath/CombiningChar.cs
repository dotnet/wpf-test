// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    class CombiningChar :RandomStringProvider
    {
        public CombiningChar(Random random) : base(random)
        {
            #region Add ranges
            this.ranges.Add(new Range('\u0300', '\u0345'));
            this.ranges.Add(new Range('\u0360', '\u0361'));
            this.ranges.Add(new Range('\u0483', '\u0486'));
            this.ranges.Add(new Range('\u0591', '\u05A1'));
            this.ranges.Add(new Range('\u05A3', '\u05B9'));

            this.ranges.Add(new Range('\u05BB', '\u05BD'));
            this.ranges.Add(new Range('\u05BF', '\u05BF'));
            this.ranges.Add(new Range('\u05C1', '\u05C2'));
            this.ranges.Add(new Range('\u05C4', '\u05C4'));
            this.ranges.Add(new Range('\u064B', '\u0652'));

            this.ranges.Add(new Range('\u0670', '\u0670'));
            this.ranges.Add(new Range('\u06D6', '\u06DC'));
            this.ranges.Add(new Range('\u06DD', '\u06DF'));
            this.ranges.Add(new Range('\u06E0', '\u06E4'));
            this.ranges.Add(new Range('\u06E7', '\u06E8'));

            this.ranges.Add(new Range('\u06EA', '\u06ED'));
            this.ranges.Add(new Range('\u0901', '\u0903'));
            this.ranges.Add(new Range('\u093C', '\u093C'));
            this.ranges.Add(new Range('\u093E', '\u094C'));
            this.ranges.Add(new Range('\u094D', '\u094D'));

            this.ranges.Add(new Range('\u0951', '\u0954'));
            this.ranges.Add(new Range('\u0962', '\u0963'));
            this.ranges.Add(new Range('\u0981', '\u0983'));
            this.ranges.Add(new Range('\u09BC', '\u09BC'));
            this.ranges.Add(new Range('\u09BE', '\u09BE'));

            this.ranges.Add(new Range('\u09BF', '\u09BF'));
            this.ranges.Add(new Range('\u09C0', '\u09C4'));
            this.ranges.Add(new Range('\u09C7', '\u09C8'));
            this.ranges.Add(new Range('\u09CB', '\u09CD'));
            this.ranges.Add(new Range('\u09D7', '\u09D7'));

            this.ranges.Add(new Range('\u09E2', '\u09E3'));
            this.ranges.Add(new Range('\u0A02', '\u0A02'));
            this.ranges.Add(new Range('\u0A3C', '\u0A3C'));
            this.ranges.Add(new Range('\u0A3E', '\u0A3E'));
            this.ranges.Add(new Range('\u0A3F', '\u0A3F'));

            this.ranges.Add(new Range('\u0A40', '\u0A42'));
            this.ranges.Add(new Range('\u0A47', '\u0A48'));
            this.ranges.Add(new Range('\u0A4B', '\u0A4D'));
            this.ranges.Add(new Range('\u0A70', '\u0A71'));
            this.ranges.Add(new Range('\u0A81', '\u0A83'));

            this.ranges.Add(new Range('\u0ABC', '\u0ABC'));
            this.ranges.Add(new Range('\u0ABE', '\u0AC5'));
            this.ranges.Add(new Range('\u0AC7', '\u0AC9'));
            this.ranges.Add(new Range('\u0ACB', '\u0ACD'));
            this.ranges.Add(new Range('\u0B01', '\u0B03'));

            this.ranges.Add(new Range('\u0B3C', '\u0B3C'));
            this.ranges.Add(new Range('\u0B3E', '\u0B43'));
            this.ranges.Add(new Range('\u0B47', '\u0B48'));
            this.ranges.Add(new Range('\u0B4B', '\u0B4D'));
            this.ranges.Add(new Range('\u0B56', '\u0B57'));

            this.ranges.Add(new Range('\u0B82', '\u0B83'));
            this.ranges.Add(new Range('\u0BBE', '\u0BC2'));
            this.ranges.Add(new Range('\u0BC6', '\u0BC8'));
            this.ranges.Add(new Range('\u0BCA', '\u0BCD'));
            this.ranges.Add(new Range('\u0BD7', '\u0BD7'));

            this.ranges.Add(new Range('\u0C01', '\u0C03'));
            this.ranges.Add(new Range('\u0C3E', '\u0C3E'));
            this.ranges.Add(new Range('\u0C46', '\u0C48'));
            this.ranges.Add(new Range('\u0C4A', '\u0C4D'));
            this.ranges.Add(new Range('\u0C55', '\u0C56'));

            this.ranges.Add(new Range('\u0C82', '\u0C83'));
            this.ranges.Add(new Range('\u0CBE', '\u0CC4'));
            this.ranges.Add(new Range('\u0CC6', '\u0CC8'));
            this.ranges.Add(new Range('\u0CCA', '\u0CCD'));
            this.ranges.Add(new Range('\u0CD5', '\u0CD6'));

            this.ranges.Add(new Range('\u0D02', '\u0D03'));
            this.ranges.Add(new Range('\u0D3E', '\u0D43'));
            this.ranges.Add(new Range('\u0D46', '\u0D48'));
            this.ranges.Add(new Range('\u0D4A', '\u0D4D'));
            this.ranges.Add(new Range('\u0D57', '\u0D57'));

            this.ranges.Add(new Range('\u0E31', '\u0E31'));
            this.ranges.Add(new Range('\u0E34', '\u0E3A'));
            this.ranges.Add(new Range('\u0E47', '\u0E4E'));
            this.ranges.Add(new Range('\u0EB1', '\u0EB1'));
            this.ranges.Add(new Range('\u0EB4', '\u0EB9'));

            this.ranges.Add(new Range('\u0EBB', '\u0EBC'));
            this.ranges.Add(new Range('\u0EC8', '\u0ECD'));
            this.ranges.Add(new Range('\u0F18', '\u0F19'));
            this.ranges.Add(new Range('\u0F35', '\u0F35'));
            this.ranges.Add(new Range('\u0F37', '\u0F37'));

            this.ranges.Add(new Range('\u0F39', '\u0F39'));
            this.ranges.Add(new Range('\u0F3E', '\u0F3E'));
            this.ranges.Add(new Range('\u0F3F', '\u0F3F'));
            this.ranges.Add(new Range('\u0F71', '\u0F84'));
            this.ranges.Add(new Range('\u0F86', '\u0F8B'));

            this.ranges.Add(new Range('\u0F90', '\u0F95'));
            this.ranges.Add(new Range('\u0F97', '\u0F97'));
            this.ranges.Add(new Range('\u0F99', '\u0FAD'));
            this.ranges.Add(new Range('\u0FB1', '\u0FB7'));
            this.ranges.Add(new Range('\u0FB9', '\u0FB9'));

            this.ranges.Add(new Range('\u20D0', '\u20DC'));
            this.ranges.Add(new Range('\u20E1', '\u20E1'));
            this.ranges.Add(new Range('\u302A', '\u302F'));
            this.ranges.Add(new Range('\u3099', '\u3099'));
            this.ranges.Add(new Range('\u309A', '\u309A'));
            #endregion

            this.overallCount = this.GetOverallCount();
        }
    }
}
