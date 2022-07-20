// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Threading;

using System.Windows.Media;
using System.Xml;
using System.Text;
using System.Windows.Interop;
using System.Runtime.InteropServices;

using MS.Win32;
using MS.Internal;

namespace DRT
{
    public class DrtMultiTouch : DrtBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            (new DrtMultiTouch()).Run(args);
        }

        public DrtMultiTouch()
        {
            this.TeamContact = "WPF";
            this.Contact = "Microsoft";
            this.DrtName = "MultiTouch";

            this.BlockInput = true;
            this.WindowTitle = "DrtMultiTouch";

            Suites = new DrtTestSuite[]
            {
                new ManipulationSimulationSuite(),
                new InputSimulationSuite(),
            };
        }

        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            return base.HandleCommandLineArgument(arg, option, args, ref k);
        }

        public TestWindow OpenTestWindow()
        {
            TestWindow window = new TestWindow();
            window.Show();
            MainWindow = (HwndSource)PresentationSource.FromVisual(window);
            InitializeMainWindow();

            return window;
        }

        public void CloseTestWindow(TestWindow window)
        {
            UninitializeMainWindow();
            MainWindow = null;
            window.Close();
        }
    }
}
