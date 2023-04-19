// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/************************************************************************************
*                                                                                   *
*  Title: Basic Test of Markup PageFunction                                         *          
*                                                                                   *
*  Description:                                                                     *
*      This test will do a basic check of the tree, and Visual verify of a markup   *
*      string pagefunction                                                          *
*                                                                                   *                                                   
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   * 
*************************************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // BasicMarkupPF
    public partial class NavigationTests : Application
    {
        void BasicMarkupPF_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("BasicMarkupPF");
            NavigationHelper.Output("Basic test of a markup string pagefunction");

            NavigationHelper.Output("App Starting Up");
            this.StartupUri = new Uri(MARKUPPF1, UriKind.RelativeOrAbsolute);

            NavigationHelper.SetStage(TestStage.Run);
        }

        void BasicMarkupPF_LoadCompleted(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("Load Completed for Application");

            MarkupPF1 _mkpf = MainWindow.Content as MarkupPF1;
            if (_mkpf == null)
            {
                NavigationHelper.Fail("Wrong content (not string pf markup)");
            }
            else
            {
                NavigationHelper.Output("Correct markup pf content loaded");
            }
            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            NavigationHelper.Output("Doing more tests including Rendering tests");
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(BasicMarkupPF_TestRender), null);
        }

        private object BasicMarkupPF_TestRender(object args)
        {

            MarkupPF1 _mkpf = MainWindow.Content as MarkupPF1;
            if (_mkpf == null)
            {
                NavigationHelper.Fail("Wrong content (not string pf markup)");
            }
            else
            {
                NavigationHelper.Output("Correct markup pf content loaded");
            }
            _mkpf.SelfTest();
            return null;

        }
    }
}
