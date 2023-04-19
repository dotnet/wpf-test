// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows.Automation.Peers;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // FinishChildMarkupPF
    public partial class NavigationTests : Application
    {
        #region FinishChildMarkupPF globals
        private int _finishChildMarkupPF_CurrentTest = 0;

        #endregion

        void FinishChildMarkupPF_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("FinishChildMarkupPF");
            NavigationHelper.Output("Testing Finish of child PageFunction");

            NavigationHelper.Output("App starting up");
            Application.Current.StartupUri = new Uri(MARKUPSTRPF1, UriKind.RelativeOrAbsolute);
            NavigationHelper.SetStage(TestStage.Run);
        }

        void FinishChildMarkupPF_LoadCompleted(object sender, NavigationEventArgs e)
        {
            StringPFMarkup _mkpf;

            NavigationHelper.Output("Load Completed for Application");
            switch (_finishChildMarkupPF_CurrentTest++)
            {
                case 0:
                    _mkpf = MainWindow.Content as StringPFMarkup;
                    if (_mkpf == null)
                    {
                        NavigationHelper.Fail("Wrong content (not string pf markup)");
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup pf content loaded");
                    }
                    Control ctlLink = LogicalTreeHelper.FindLogicalNode(_mkpf, "LNKNext") as Control;
                    if (ctlLink == null)
                    {
                        NavigationHelper.Fail("Could not find link to child pf");
                    }
                    else
                    {
                        NavigationHelper.Output("Attempting navigation to child pagefunction");
                        AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(ctlLink);
                        IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
                        iip.Invoke();
                    }
                    NavigationWindow nw = MainWindow as NavigationWindow;
                    break;

                case 1:
                    ObjectPFMarkup1 _mkpf2 = MainWindow.Content as ObjectPFMarkup1;
                    if (_mkpf2 == null)
                    {
                        NavigationHelper.Fail("Wrong content (not obj pf markup)");
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup child pf (object pf) loaded");
                    }
                    if (_mkpf2.SelfTest(true, false, 1))
                    {
                        NavigationHelper.Output("Successfully navigated to Markup Child PageFunction");
                    }

                    NavigationHelper.Output("Doing rendering test on this PageFunction and afterwards finishing the child pagefunction");

                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                        new DispatcherOperationCallback(FinishChildMarkupPF_DoVisualVerifyChild), null);
                    break;

                case 2:
                    _mkpf = MainWindow.Content as StringPFMarkup;
                    if (_mkpf == null)
                    {
                        NavigationHelper.Fail("Wrong content (not string pf markup)");
                        return;
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup pf content loaded");
                    }

                    if (_mkpf.SelfTest(false, false, 1))
                    {
                        NavigationHelper.Output("Initial checks of pf are good");
                    }
                    else
                    {
                        NavigationHelper.Fail("State of parent pf after finishing child markup pf is incorrect");
                        return;
                    }

                    if (_mkpf.SCheck_ChildFinish_TC21_Void_45_456AE_44(1, 0, 0, 0))
                    {
                        NavigationHelper.Output("Returned object from child markup object pf, and the parent pf are in correct state");
                    }

                    NavigationHelper.Output("Doing rendering test on parent PageFunction after the child pagefunction has finished");

                    /// Even though Visual Verification is deprecated, I have to call this funtion because 
                    /// The "Pass" API is called from within it. Will repair that as a quality issues later. 
                    /// Filed Work Item# 
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                        new DispatcherOperationCallback(FinishChildMarkupPF_DoVisualVerifyOnParentResume), null);
                    break;

                default:
                    NavigationHelper.Fail("LoadCompleted fired incorrect number of times");
                    break;
            }
        }


        private object FinishChildMarkupPF_DoVisualVerifyChild(object args)
        {
            ObjectPFMarkup1 _mkpf2 = MainWindow.Content as ObjectPFMarkup1;

            NavigationHelper.Output("Now attempting to finish child.");

            Control ctlBtnDone = LogicalTreeHelper.FindLogicalNode(_mkpf2, "BtnDone") as Control;
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
            return null;
        }


        private object FinishChildMarkupPF_DoVisualVerifyOnParentResume(object args)
        {
            NavigationHelper.Pass("Finished test.  All tests passed");
            return null;
        }
    }
}
