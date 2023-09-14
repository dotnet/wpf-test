// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Microsoft.Test.Layout
{
    public class DerivedFlowDocument : FlowDocument
    {
        public DerivedFlowDocument()
        {
        }
    }

    public class SimpleDecoratorForFlowDocument : Decorator
    {
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            return base.ArrangeOverride(arrangeSize);
        }
    }

    public class NullClass
    {

    }
}
