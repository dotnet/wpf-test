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
using System.Windows.Threading;

//
// System.Windows.Threading.DispatcherObject AutoPME Test
//
public class XDispatcherObject : XObject {
    public XDispatcherObject(String[] args) : base(args) { }

    protected override Type Class {
        get { return typeof(DispatcherObject); }
    }

    protected override Object CreateObject(TParams p) {
        return new Object();
    }

    DispatcherObject GetDispatcherObject(TParams p) {
        if ( p.target is DispatcherObject )
            return (DispatcherObject)p.target;
        else {
            p.log.WriteLine("target isn't type DispatcherObject");
            return null;
        }
    }

    //========================================
    // Test Methods
    //========================================
    protected ScenarioResult get_Dispatcher(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult CheckAccess(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult VerifyAccess(TParams p) {
        return new ScenarioResult(true);
    }

}
