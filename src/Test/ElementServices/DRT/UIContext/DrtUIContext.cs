// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DRT;

public class DrtDispatcher : DrtBase
{
    [STAThread]
    public static void Main(string[] args)
    {
        (new DrtDispatcher()).Run(args);
    }

    public DrtDispatcher()
    {
        this.TeamContact = "WPF";
        this.Contact = "Microsoft";
        this.DrtName = "DrtDispatcher";

        Suites = new DrtTestSuite[]
        {
            new InvokeAsyncTest(),
            new BeginInvokeTest(),
            new DispatcherTimerTest(),
            new ExceptionTests()
        };
    }
}

