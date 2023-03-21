// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Driver class for selecting between and running N TestSuites.

using System;
using System.Windows;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using System.Threading;

namespace Annotations.Test.Framework
{
    public class RTMDriver
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new TestSuiteDriver().Run(args);
        }
    }
}

