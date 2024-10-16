// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Windows.Interop;
using System.Windows;

//
// System.Windows.Interop.HwndHost AutoPME Test
//
public class XHwndHost : XFrameworkElement {
    public XHwndHost(String[] args) : base(args) { }

    //========================================
    // Test Methods
    //========================================
    protected ScenarioResult Dispose(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Handle(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult UpdateWindowPos(TParams p) {
        return new ScenarioResult(true);
    }

}
