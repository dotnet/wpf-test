// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Text;
using System.Windows.Threading;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Launch a test based on cmd-line args
    /// </summary>
    public class Cgt3d
    {
        /// <summary/>
        [STAThread]
        public static void Main(string[] args)
        {
            TestLauncher.Launch(args, typeof(Cgt3d).Assembly);
        }

        /// <summary/>
        public static void Launch()
        {
            char[] tokens={' '};
            TestLauncher.Launch(DriverState.DriverParameters["Args"].Split(tokens), typeof(Cgt3d).Assembly);
        }
    }
}