// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
using System.Windows.Media;

#endregion

namespace Microsoft.Test.Layout
{
    public class DerivedFlowDocumentScrollViewer : FlowDocumentScrollViewer
    {
        public DerivedFlowDocumentScrollViewer()
        {            
        }

        public void CallHitTestCore(PointHitTestParameters hitTestParameters)
        {
            base.HitTestCore(hitTestParameters);
        }

        public new void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
        }

        public new void ParentLayoutInvalidated(UIElement child)
        {
            base.ParentLayoutInvalidated(child);
        }
    }
}
