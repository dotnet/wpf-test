// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    public class LegalChar : RandomStringProvider
    {
        public LegalChar(Random random)
            : base(random)
        {
            // This is supposed to be "any legal unicode character". I am adding only the first 
            // 255 characters for now. We can add more later if needed.
            this.ranges.Add(new Range('\u0000', '\u00FF'));

            this.overallCount = this.GetOverallCount();
        }
    }
}
