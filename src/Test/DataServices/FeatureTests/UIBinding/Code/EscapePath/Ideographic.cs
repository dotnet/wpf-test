// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    public class Ideographic : RandomStringProvider
    {
        public Ideographic(Random random)
            : base(random)
        {
            this.ranges.Add(new Range('\u4E00', '\u9FA5'));
            this.ranges.Add(new Range('\u3007', '\u3007'));
            this.ranges.Add(new Range('\u3021', '\u3029'));

            this.overallCount = this.GetOverallCount();
        }
    }
}
