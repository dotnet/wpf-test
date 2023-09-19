// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Windows;
using System.Windows.Media;

//
// System.Windows.Media.Visual AutoPME Test
//
public class XVisual : XDependencyObject {
    public XVisual(String[] args) : base(args) { }

    //========================================
    // Test Methods
    //========================================
    protected ScenarioResult IsAncestorOf(TParams p, DependencyObject descendant) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult IsDescendantOf(TParams p, DependencyObject ancestor) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult FindCommonVisualAncestor(TParams p, DependencyObject otherVisual) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult TransformToAncestor(TParams p, Visual ancestor) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult TransformToDescendant(TParams p, Visual descendant) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult TransformToVisual(TParams p, Visual visual) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult PointToScreen(TParams p, Point point)
    {
        return new ScenarioResult(true);
    }

    protected ScenarioResult PointFromScreen(TParams p, Point point)
    {
        return new ScenarioResult(true);
    }

}
