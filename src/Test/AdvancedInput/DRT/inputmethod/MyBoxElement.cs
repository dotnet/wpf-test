// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;

public class MyBoxElement : FrameworkElement 
{
    public MyBoxElement()
    {
        Focusable = true;
    }

    public Brush Background {get {return _background;} set {_background = value;InvalidateVisual();}}

    protected override void OnRender(DrawingContext ctx)
    {
        ctx.DrawRectangle( 
            _background,
            null,
            new Rect(new Point(), RenderSize));
    }
    
    private Brush _background;
}
