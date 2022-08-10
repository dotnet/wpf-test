// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

public class DrtCommandingBox : UIElement
{
    protected override int VisualChildrenCount
    {
        get
        {
            return (_commandTarget!=null ? 1 : 0);
        }
    }
        
    protected override Visual GetVisualChild(int index)
    {
        if(index != 0) throw new Exception("Index out of Bound");
            
        return _commandTarget;
    }
        
    public DrtCommandingBox()
    {
        _commandTarget = new DrtCommandingElement("CommandTarget");
        this.AddVisualChild(_commandTarget);
    }

    public DrtCommandingElement TargetControl { get { return _commandTarget; } }

    protected override Size MeasureCore(Size constraint)
    {
        Size childConstraint = new Size(constraint.Width/25, constraint.Height/2);
        _commandTarget.Measure(childConstraint);

        return constraint;
    }

    protected override void ArrangeCore(Rect arrangeBounds)
    {
        base.ArrangeCore(arrangeBounds);

        double cx = arrangeBounds.Width/2;
        double cy = arrangeBounds.Height/2;

        _commandTarget.Arrange(new Rect(new Point(), new Size(cx, cy)));
    }

    private DrtCommandingElement _commandTarget;
}
 



