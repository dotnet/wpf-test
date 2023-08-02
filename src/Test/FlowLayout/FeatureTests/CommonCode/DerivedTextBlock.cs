// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

using System;
using System.Windows.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

#endregion

namespace Microsoft.Test.Layout
{
    class DerivedTextBlock : TextBlock
    {
        public IEnumerator<IInputElement> getHostedElementsCore()
        {
            return base.HostedElementsCore;
        }

        public IEnumerator getLogicalChildren()
        {
            return base.LogicalChildren;
        }

        public void CallGetRectanglesCore(ContentElement e)
        {
            GetRectanglesCore(e);
        }
        protected override ReadOnlyCollection<Rect> GetRectanglesCore(ContentElement e)
        {
            return base.GetRectanglesCore(e);
        }

        public void CallOnChildDesiredSizeChangedCore(UIElement child)
        {
            OnChildDesiredSizeChangedCore(child);
        }
        protected override void OnChildDesiredSizeChangedCore(UIElement child)
        {
            try { base.OnChildDesiredSizeChangedCore(child); }
            catch { }
        }

        public new Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        public new Size ArrangeOverride(Size arrangeSize)
        {
            return base.ArrangeOverride(arrangeSize);
        }

        public new void OnRender(DrawingContext ctx)
        {
            base.OnRender(ctx);
        }

        public new void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        public new PointHitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return (PointHitTestResult)base.HitTestCore(hitTestParameters);
        }

        public void CallInputHitTestCore(Point p)
        {
            InputHitTestCore(p);
        }

        protected override IInputElement InputHitTestCore(Point p)
        {
            return base.InputHitTestCore(p);
        }

        public void CallHitTestCore(PointHitTestParameters hitTestParameters)
        {
            HitTestCore(hitTestParameters);
        }
    }
}
