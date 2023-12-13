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

//
// System.Windows.DependencyObject AutoPME Test
//
public class XDependencyObject : XDispatcherObject {
    public XDependencyObject(String[] args) : base(args) { }

    DependencyObject GetDependencyObject(TParams p) {
        if ( p.target is DependencyObject )
            return (DependencyObject)p.target;
        else {
            p.log.WriteLine("target isn't type DependencyObject");
            return null;
        }
    }

    //========================================
    // Test Methods
    //========================================
    protected ScenarioResult get_DependencyObjectType(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsSealed(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult GetValue(TParams p, DependencyProperty dp) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult SetValue(TParams p, DependencyProperty dp, Object value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult SetValue(TParams p, DependencyPropertyKey key, Object value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult ClearValue(TParams p, DependencyProperty dp) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult ClearValue(TParams p, DependencyPropertyKey key) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult CoerceValue(TParams p, DependencyProperty dp) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult InvalidateProperty(TParams p, DependencyProperty dp) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult ReadLocalValue(TParams p, DependencyProperty dp) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult GetLocalValueEnumerator(TParams p) {
        return new ScenarioResult(true);
    }

}
