// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Threading;

using System.Windows;

namespace XamlPadEdit
{
    public partial class XamlPadApp : Application
    {
#if !XamlPadExpressApp
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            this.Startup += new System.Windows.StartupEventHandler(this.AppStartup);
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
#endif
        
        #region Event Handlers

        void AppStartup(object sender, StartupEventArgs args)
        {
#if !XamlPadExpressApp
            MainWindow.Content = LoadComponent(new System.Uri("XamlPadPage.xaml", System.UriKind.Relative));
            // Get the arguments specified on the command line
            XamlHelper.CommandLineArgs = args.Args;
#endif
        }

        #endregion Event Handlers

    }
}