// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Threading;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // NoReturnEventHandlerPF
    public partial class NavigationTests : Application
    {
        #region NoReturnEventHandlerPF globals
        private int _noReturnEventHandlerPFTest = 0;

        #endregion

        void NoReturnEventHandlerPF_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("NoReturnEventHandlerPF");
            NavigationHelper.Output("Testing child PageFunction with no Return eventhandler");

            NavigationHelper.Output("App starting up");
            Application.Current.StartupUri = new Uri(MARKUPSTRPF1, UriKind.RelativeOrAbsolute);

            NavigationHelper.SetStage(Microsoft.Test.Logging.TestStage.Run);
        }

        void NoReturnEventHandlerPF_LoadCompleted(object sender, NavigationEventArgs e)
        {
            StringPFMarkup _mkpf;

            NavigationHelper.Output("Load Completed for Application");

            switch (_noReturnEventHandlerPFTest++)
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
                    Control ctlLink = LogicalTreeHelper.FindLogicalNode(_mkpf, "LNKNextNoAttach") as Control;
                    if (ctlLink == null)
                    {
                        NavigationHelper.Fail("Could not find link to child PF with no Return eventhandler");
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
                        new DispatcherOperationCallback(NoReturnEventHandlerPF_DoVisualVerifyChild), null);

                    break;
                case 2:
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
                        return;
                    }

                    if (_mkpf.SCheck_ChildFinish_TC21_Void_45_456AE_44(0, 0, 0, 0))
                    {
                        NavigationHelper.Output("Returned object from child markup object PF, and the parent PF are in correct state");
                    }

                    NavigationHelper.Output("Doing rendering test on parent PF after the child PF has finished");

                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                        new DispatcherOperationCallback(NoReturnEventHandlerPF_DoVisualVerifyOnParentResume), null);
                    break;

                default:
                    NavigationHelper.Fail("LoadCompleted fired incorrect number of times");
                    break;
            }

        }

        private object NoReturnEventHandlerPF_DoVisualVerifyChild(object args)
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


        private object NoReturnEventHandlerPF_DoVisualVerifyOnParentResume(object args)
        {
            StringPFMarkup _mkpf2 = MainWindow.Content as StringPFMarkup;

            NavigationHelper.Pass("Finished test.");
            return null;
        }
    }
}
