// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using Microsoft.Test.Logging;

namespace MarkupCompiler.UidManagerTest
{
    /// <summary>
    /// Test UidManager task in PresentationBuildTasks
    /// Call Check, Update and Remove tasks and verify the output
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            CheckUidPage checkUidPage = new CheckUidPage();
            UpdateUidPage updateUidPage = new UpdateUidPage();
            RemoveUidPage removeUidPage = new RemoveUidPage();

            GlobalLog.LogDebug("checkUidPage.Uid = " + checkUidPage.Uid);
            GlobalLog.LogDebug("updateUidPage.Uid = " + updateUidPage.Uid);
            GlobalLog.LogDebug("removeUidPage.Uid = " + removeUidPage.Uid);

            // Update task auto generates Uid name "Page_1" for root level page
            if (checkUidPage.Uid == "Uid_Check" && updateUidPage.Uid == "Page_1" && removeUidPage.Uid == String.Empty)
            {
                Application.Current.Shutdown(0);
            }
            else
            {
                Application.Current.Shutdown(-1);
            }
        }
    }
}
