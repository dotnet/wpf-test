// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    public class Extender : RandomStringProvider
    {
        public Extender(Random random)
            : base(random)
        {
            this.ranges.Add(new Range('\u00B7', '\u00B7'));
            this.ranges.Add(new Range('\u02D0', '\u02D0'));
            this.ranges.Add(new Range('\u02D1', '\u02D1'));
            this.ranges.Add(new Range('\u0387', '\u0387'));
            this.ranges.Add(new Range('\u0640', '\u0640'));
            this.ranges.Add(new Range('\u0E46', '\u0E46'));
            this.ranges.Add(new Range('\u0EC6', '\u0EC6'));
            this.ranges.Add(new Range('\u3005', '\u3005'));
            this.ranges.Add(new Range('\u3031', '\u3035'));
            this.ranges.Add(new Range('\u309D', '\u309E'));
            this.ranges.Add(new Range('\u30FC', '\u30FE'));

            this.overallCount = this.GetOverallCount();
        }
    }
}
