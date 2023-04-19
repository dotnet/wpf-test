// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows.Controls;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // PageToMarkupPF
    public partial class NavigationTests : Application
    {
        #region PageToMarkupPF globals
        private int _pageToMarkupPFTest = 0;

        #endregion

        void PageToMarkupPF_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("PageToMarkupPF");
            NavigationHelper.Output("Test of navigation from a page to a markup pf");

            NavigationHelper.Output("App starting up");
            Application.Current.StartupUri = new Uri(LAUNCHPF, UriKind.RelativeOrAbsolute);
            NavigationHelper.SetStage(TestStage.Run);
        }

        void PageToMarkupPF_LoadCompleted(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted for application");

            NonPFLaunchPage nonpf;
            StringPFMarkup _mkpf2;

            switch (_pageToMarkupPFTest++)
            {
                case 0:
                    nonpf = MainWindow.Content as NonPFLaunchPage;
                    if (nonpf == null)
                    {
                        NavigationHelper.Fail("Wrong content in navigationwindow");
                    }
                    else
                    {
                        NavigationHelper.Output("Correct content loaded");
                    }

                    NavigationHelper.Output("Navigating to a pf from a page");
                    Control ctlBtnNext = LogicalTreeHelper.FindLogicalNode(nonpf, "BtnLaunchPF") as Control;
                    if (ctlBtnNext == null)
                    {
                        NavigationHelper.Fail("Could not find button to launch the pf");
                    }
                    else
                    {
                        NavigationHelper.Output("Attempting to launch pf by clicking on the button");
                        AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(ctlBtnNext);
                        IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
                        iip.Invoke();
                    }
                    break;

                case 1:
                    _mkpf2 = MainWindow.Content as StringPFMarkup;
                    if (_mkpf2 == null)
                    {
                        NavigationHelper.Fail("Wrong content (not string pf markup)");
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup pf (string) content loaded");
                    }

                    NavigationHelper.Output("Testing the active state of child string pagefunction (markup string pf)");

                    _mkpf2.SelfTest(true, false, 1);

                    NavigationHelper.Output("Finishing the child pf");
                    Control ctlBtnDone = LogicalTreeHelper.FindLogicalNode(_mkpf2, "LNKDone") as Control;
                    if (ctlBtnDone == null)
                    {
                        NavigationHelper.Fail("Could not find button to finish the child pf");
                    }
                    else
                    {
                        NavigationHelper.Output("Attempting to finish child pf by clicking on the button");
                        AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(ctlBtnDone);
                        IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
                        iip.Invoke();
                    }
                    break;
                case 2:

                    nonpf = MainWindow.Content as NonPFLaunchPage;
                    if (nonpf == null)
                    {
                        NavigationHelper.Fail("Wrong content (not LaunchPage)");
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup loaded after finishing child");
                    }

                    NavigationHelper.Output("Testing the start page after the pf has finished");

                    if (nonpf.SelfCheck_1())
                    {
                        NavigationHelper.Pass("Navigation from page to pagefunctions works.");
                    }
                    else
                    {
                        NavigationHelper.Fail("Navigation from page to pagefunction does not work");
                    }

                    break;
                default:
                    NavigationHelper.Fail("LoadCompleted fired unexpected number of times");
                    break;
            }
        }
    }
}
