// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Data;

namespace Microsoft.Test.DataServices
{
    public class MyCompositeCollection : CompositeCollection
    {
        public MyCompositeCollection()
        {
            this.Add(new Places());
            this.Add(new GreekGods());
        }
    }
}
