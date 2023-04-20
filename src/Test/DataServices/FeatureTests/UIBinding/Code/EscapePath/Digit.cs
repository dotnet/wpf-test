// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    class Digit : RandomStringProvider
    {
        public Digit(Random random) : base(random)
        {
            this.ranges.Add(new Range('\u0030', '\u0039'));
            this.ranges.Add(new Range('\u0660', '\u0669'));
            this.ranges.Add(new Range('\u06F0', '\u06F9'));
            this.ranges.Add(new Range('\u0966', '\u096F'));
            this.ranges.Add(new Range('\u09E6', '\u09EF'));
            this.ranges.Add(new Range('\u0A66', '\u0A6F'));
            this.ranges.Add(new Range('\u0AE6', '\u0AEF'));
            this.ranges.Add(new Range('\u0B66', '\u0B6F'));
            this.ranges.Add(new Range('\u0BE7', '\u0BEF'));
            this.ranges.Add(new Range('\u0C66', '\u0C6F'));
            this.ranges.Add(new Range('\u0CE6', '\u0CEF'));
            this.ranges.Add(new Range('\u0D66', '\u0D6F'));
            this.ranges.Add(new Range('\u0E50', '\u0E59'));
            this.ranges.Add(new Range('\u0ED0', '\u0ED9'));
            this.ranges.Add(new Range('\u0F20', '\u0F29'));

            this.overallCount = this.GetOverallCount();
        }
    }
}
