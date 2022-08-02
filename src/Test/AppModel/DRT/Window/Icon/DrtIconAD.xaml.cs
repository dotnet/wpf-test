// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using System.IO;

using System.Net;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Markup;

namespace DrtIcon
{
    public partial class DrtIconApp : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            bool hold = false;
            if ((e.Args.Length > 0) && 
                (e.Args[0].ToLower().Equals("-hold") || e.Args[0].ToLower().Equals("/hold")))
            {
                hold = true;
            }
            
            this.Hold = hold;
            base.OnStartup(e);
        }

        public bool Hold
        {
            get { return _hold;}
            set { _hold = value;}
        }
        
        protected override void OnExit(ExitEventArgs e)
        {
            if (_success == 1)
            {
                Log("Test PASSED");
            }
            else
            {
                Log("Test FAILED");
            }

            base.OnExit(e);
        }


        private void Log(string message, params object[] args)
        {
            _logger.Log(message, args);
        }

        private bool   _hold;
        private int    _success    = 1;
        // never used private Window _win  = null;
        private DRT.Logger _logger  = new DRT.Logger("DrtIcon", "Microsoft", "Testing Window.Icon property");
    }
}
