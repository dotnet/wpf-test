// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Threading;
using Microsoft.Test.Logging;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // FinishPFWithNonEmptyFwdJournal
    public partial class NavigationTests : Application
    {
        #region FinishPFWithNonEmptyFwdJournal globals
        private int _finishPFWithNonEmptyFwdJournalTest = 0;
        private String _finishPFWithNonEmptyFwdJournalExtFile = IMAGEPAGE;
        #endregion

        void FinishPFWithNonEmptyFwdJournal_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("FinishPFWithNonEmptyFwdJournal");
            NavigationHelper.Output("Testing finishing a child PF which has entries in fwd journal");

            NavigationHelper.Output("App starting up");
            Application.Current.StartupUri = new Uri(MARKUPSTRPF1, UriKind.RelativeOrAbsolute);

            NavigationHelper.SetStage(TestStage.Run);
        }

        void FinishPFWithNonEmptyFwdJournal_LoadCompleted(object sender, NavigationEventArgs e)
        {
            StringPFMarkup _mkpf;
            NavigationWindow nw = MainWindow as NavigationWindow;

            NavigationHelper.Output("Load Completed for Application");
            switch (_finishPFWithNonEmptyFwdJournalTest++)
            {
                // Creating start PF and launching the rest of the test...
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
                    break;

                // After launching child PF...
                case 1:
                    ObjectPFMarkup1 _mkpf2 = MainWindow.Content as ObjectPFMarkup1;
                    if (_mkpf2 == null)
                    {
                        NavigationHelper.Fail("Wrong content (not obj PF markup)");
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup child pf (object PF) loaded");
                    }
                    if (_mkpf2.SelfTest(true, false, 1))
                    {
                        NavigationHelper.Output("Successfully navigated to child PF");
                    }

                    NavigationHelper.Output("Doing rendering test on this PF and afterwards navigating to a different page");

                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                        new DispatcherOperationCallback(FinishPFWithNonEmptyFwdJournal_DoNavigateToAnotherPage), null);
                    break;

                // After navigating to page from child PF...
                case 2:
                    // Check that we are on the correct page...
                    if (!_finishPFWithNonEmptyFwdJournalExtFile.Equals(nw.Source.ToString()))
                    {
                        NavigationHelper.Fail("MainWindow is not at expected page (" + _finishPFWithNonEmptyFwdJournalExtFile + ").  MainWindow is at " + nw.Source);
                    }

                    NavigationHelper.Output("EXPECTED - MainWindow is at " + _finishPFWithNonEmptyFwdJournalExtFile);

                    if (nw.CanGoBack)
                    {
                        NavigationHelper.Output("GoBack to the child PF.");
                        nw.GoBack();
                    }
                    else
                    {
                        NavigationHelper.Fail("Cannot GoBack to child PF after navigating away to XAML page.  Nothing in the back stack");
                    }
                    break;

                // After calling GoBack, we should be in the child PF again...
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
                    if (_mkpf3.SelfTest(true, true, 1))
                    {
                        NavigationHelper.Output("Successfully navigated to child PF");
                    }

                    NavigationHelper.Output("Doing rendering test on this PF and afterwards finishing the child PF");

                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                        new DispatcherOperationCallback(FinishPFWithNonEmptyFwdJournal_DoVisualVerifyChild), null);
                    break;

                // After returning from child PF back to its parent PF...
                case 4:
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

                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                        new DispatcherOperationCallback(FinishPFWithNonEmptyFwdJournal_DoVisualVerifyOnParentResume), null);
                    break;

                default:
                    NavigationHelper.Fail("LoadCompleted Fired incorrect number of times");
                    break;
            }

        }

        private object FinishPFWithNonEmptyFwdJournal_DoNavigateToAnotherPage(object args)
        {
            ObjectPFMarkup1 _mkpf2 = MainWindow.Content as ObjectPFMarkup1;
            NavigationWindow nw = MainWindow as NavigationWindow;

            NavigationHelper.Output("Navigating to another non-PF page");
            nw.Navigate(new Uri(_finishPFWithNonEmptyFwdJournalExtFile, UriKind.RelativeOrAbsolute));

            return null;
        }

        private object FinishPFWithNonEmptyFwdJournal_DoVisualVerifyChild(object args)
        {
            ObjectPFMarkup1 _mkpf2 = MainWindow.Content as ObjectPFMarkup1;
            NavigationWindow nw = MainWindow as NavigationWindow;

            NavigationHelper.Output("Checking that child PF has forward stack entry");
            if (!nw.CanGoForward)
            {
                NavigationHelper.Fail("Expecting forward entry for " + _finishPFWithNonEmptyFwdJournalExtFile + ".  No forward entries in journal");
            }

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


        private object FinishPFWithNonEmptyFwdJournal_DoVisualVerifyOnParentResume(object args)
        {
            NavigationHelper.Pass("Finished test.");
            return null;
        }

    }
}
