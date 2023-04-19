// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Resources;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;

namespace Microsoft.Test.AddIn
{
    public class AddInApplication : Application
    {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        public static void Main()
        {
            AddInApplication app = new AddInApplication();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            GlobalLog.LogStatus("Get TestDefinition parameter \'TestFile\' to get the filename");
            string fileName = DriverState.DriverParameters["TestFile"];

            GlobalLog.LogStatus("TestFile=\'" + fileName + "\'");
            
            WebClient client = new WebClient();
            Uri uri = new Uri("pack://siteoforigin:,,,/" + fileName, UriKind.RelativeOrAbsolute);

            StreamResourceInfo info = Application.GetRemoteStream(uri);
            Stream stream = info.Stream;

            BasicAddInTest test = new BasicAddInTest(stream, 0);
            test.Run();

            GlobalLog.LogStatus("Shutting down the xbap");
            ApplicationMonitor.NotifyStopMonitoring();
        }

    }
}
