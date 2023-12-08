﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

namespace WindowsOSBug934077Program
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Windows.Application app = new System.Windows.Application();
            app.Run(new WindowsFormsHostTests.WindowsOSBug934077(new string[0]));
        }
    }
}