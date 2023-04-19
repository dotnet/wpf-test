// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // ReuseReturnedPF
    public partial class NavigationTests : Application
    {
        #region ReuseReturnedPF globals
        private int _reuseReturnedPFTest = 0;
        private bool _reuseReturnedPFIsFirstTime = true;

        #endregion

        void ReuseReturnedPF_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("ReuseReturnedPF");
            NavigationHelper.Output("Testing reusing a PageFunction that has previously returned");

            NavigationHelper.Output("App starting up");
            Application.Current.StartupUri = new Uri(MARKUPSTRPF1, UriKind.RelativeOrAbsolute);

            NavigationHelper.SetStage(Microsoft.Test.Logging.TestStage.Run);
        }

        void ReuseReturnedPF_LoadCompleted(object sender, NavigationEventArgs e)
        {
            StringPFMarkup _mkpf;

            NavigationHelper.Output("Load Completed for Application");
            switch (_reuseReturnedPFTest++)
            {
                case 0:
                    _mkpf = MainWindow.Content as StringPFMarkup;
                    if (_mkpf == null)
                    {
                        NavigationHelper.Fail("Wrong content (not string PF markup)");
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup PF content loaded");
                    }
                    Control ctlLink = LogicalTreeHelper.FindLogicalNode(_mkpf, "LNKNext") as Control;
                    if (ctlLink == null)
                    {
                        NavigationHelper.Fail("Could not find link to child PF");
                    }
                    else
                    {
                        NavigationHelper.Output("Attempting navigation to child PF");
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
                        NavigationHelper.Fail("Wrong content (not obj PF markup)");
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup child PF (object PF) loaded");
                    }
                    if (_mkpf2.SelfTest(true, false, 1))
                    {
                        NavigationHelper.Output("Successfully navigated to Markup child PF");
                    }

                    NavigationHelper.Output("Doing rendering test on this PageFunction and afterwards finishing the child pagefunction");

                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                        new DispatcherOperationCallback(ReuseReturnedPF_DoVisualVerifyChild), null);
                    break;

                case 2:
                case 4:
                    _mkpf = MainWindow.Content as StringPFMarkup;
                    if (_mkpf == null)
                    {
                        NavigationHelper.Fail("Wrong content (not string PF markup)");
                        return;
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup PF content loaded");
                    }

                    if (_mkpf.SelfTest(false, false, 1))
                    {
                        NavigationHelper.Output("Initial checks of parent PF are good");
                    }
                    else
                    {
                        NavigationHelper.Fail("State of parent PF after finishing child markup pf is incorrect");
                    }

                    if (_mkpf.SCheck_ChildFinish_TC21_Void_45_456AE_44(1, 0, 0, 0))
                    {
                        NavigationHelper.Output("Returned object from child markup object PF, and the parent PF are in correct state");
                    }

                    NavigationHelper.Output("Doing rendering test on parent PF after the child PF has finished");

                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                        new DispatcherOperationCallback(ReuseReturnedPF_DoVisualVerifyOnParentResume), null);
                    break;

                case 3:
                    ObjectPFMarkup1 _mkpf3 = MainWindow.Content as ObjectPFMarkup1;
                    if (_mkpf3 == null)
                    {
                        NavigationHelper.Fail("Wrong content (not obj PF markup)");
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup child PF (object PF) loaded");
                    }
                    if (_mkpf3.SelfTest(true, false, 1))
                    {
                        NavigationHelper.Output("Successfully navigated to Markup child PF (x2)");
                    }

                    NavigationHelper.Output("Doing rendering test on this PF and afterwards finishing the child PF");

                    Dispatcher.CurrentDispatcher.BeginInvoke(
                       DispatcherPriority.Background,
                        new DispatcherOperationCallback(ReuseReturnedPF_DoVisualVerifyChild), null);
                    break;

                default:
                    NavigationHelper.Fail("LoadCompleted fired incorrect number of times");
                    break;
            }

        }

        private object ReuseReturnedPF_DoVisualVerifyChild(object args)
        {
            ObjectPFMarkup1 _mkpf2 = MainWindow.Content as ObjectPFMarkup1;

            NavigationHelper.Output("Now attempting to finish child.");

            Control ctlBtnDone = LogicalTreeHelper.FindLogicalNode(_mkpf2, "BtnDone") as Control;
            if (ctlBtnDone == null)
            {
                NavigationHelper.Fail("Could not find button to finish the child PF");
            }
            else
            {
                NavigationHelper.Output("Attempting to finish child PF by clicking on the button");
                AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(ctlBtnDone);
                IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
                iip.Invoke();
            }

            return null;
        }


        private object ReuseReturnedPF_DoVisualVerifyOnParentResume(object args)
        {
            StringPFMarkup _mkpf2 = MainWindow.Content as StringPFMarkup;

            if (_reuseReturnedPFIsFirstTime)
            {
                NavigationHelper.Output("Attempting to reuse/restart this PF's child");
                Control ctlLink = LogicalTreeHelper.FindLogicalNode(_mkpf2, "LNKNext") as Control;

                if (ctlLink == null)
                {
                    NavigationHelper.Fail("Could not find link to child PF");
                }
                else
                {
                    NavigationHelper.Output("Attempting navigation to child PF");
                    AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(ctlLink);
                    IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
                    iip.Invoke();
                }

                _reuseReturnedPFIsFirstTime = false;
            }
            else
            {
                NavigationHelper.Pass("Finished test.");
            }

            return null;
        }

    }

}
