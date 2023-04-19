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
    // RemoveFromJournalFinishChildMarkupPF
    public partial class NavigationTests : Application
    {
        #region RemoveFromJournalFinishChildMarkupPF
        private int _removeFromJournalFinishChildPFTest = 0;

        #endregion


        void RemoveFromJournalFinishChildMarkupPF_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("RemoveFromJournalFinishChildMarkupPF");
            NavigationHelper.Output("Test of Remove From Journal when finishing Child Markup PFs");
            NavigationHelper.Output("App starting up");
            Application.Current.StartupUri = new Uri(MARKUPBOOLPF1, UriKind.RelativeOrAbsolute);

            NavigationHelper.SetStage(TestStage.Run);
        }

        void RemoveFromJournalFinishChildMarkupPF_LoadCompleted(object sender, NavigationEventArgs e)
        {
            BoolPFMarkup1 _mkpf;
            IntPFMarkup1 _mkpf2;

            NavigationHelper.Output("Load Completed for Application");
            switch (_removeFromJournalFinishChildPFTest++)
            {
                case 0:
                    _mkpf = MainWindow.Content as BoolPFMarkup1;
                    if (_mkpf == null)
                    {
                        NavigationHelper.Fail("Wrong content (not bool PF markup)");
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup PF content loaded");
                    }
                    NavigationHelper.Output("Testing the start pagefunction (markup bool pf) which has default value for Remove From Journal");
                    _mkpf.SelfTest(false, false, 1, 1, true /*default value is TRUE*/);

                    NavigationHelper.Output("Navigating to child PF");
                    Control ctlBtnNext = LogicalTreeHelper.FindLogicalNode(_mkpf, "BtnNextChildInt") as Control;
                    if (ctlBtnNext == null)
                    {
                        NavigationHelper.Fail("Could not find button to launch the child (int) PF");
                    }
                    else
                    {
                        NavigationHelper.Output("Attempting to launch child PF by clicking on the button");
                        AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(ctlBtnNext);
                        IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
                        iip.Invoke();
                    }

                    break;
                case 1:
                    _mkpf2 = MainWindow.Content as IntPFMarkup1;
                    if (_mkpf2 == null)
                    {
                        NavigationHelper.Fail("Wrong content (not int PF markup)");
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup PF content loaded");
                    }

                    NavigationHelper.Output("Testing the active state of child int pagefunction (markup int PF)");

                    _mkpf2.SelfTest(true, false, 1, 1, true);

                    NavigationHelper.Output("Finishing the child int PF with RemoveFromJournal = true");
                    Control ctlBtnDone = LogicalTreeHelper.FindLogicalNode(_mkpf2, "BtnDone") as Control;
                    if (ctlBtnDone == null)
                    {
                        NavigationHelper.Fail("Could not find button to finish the child (int) PF");
                    }
                    else
                    {
                        NavigationHelper.Output("Attempting to finish child PF by clicking on the button");
                        AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(ctlBtnDone);
                        IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
                        iip.Invoke();
                    }
                    break;
                case 2:

                    _mkpf = MainWindow.Content as BoolPFMarkup1;
                    if (_mkpf == null)
                    {
                        NavigationHelper.Fail("Wrong content (not bool PF markup)");
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup PF content loaded after finishing child");
                    }
                    NavigationHelper.Output("Testing the start PF after the RemoveFromJournal child has finished");

                    _mkpf.SelfTest(false, false, 1, 1, true);

                    if (_mkpf.SCheck_RmvJnl_TC21_IntChildFinish_45_456AE_46())
                    {
                        NavigationHelper.Pass("RemoveFromJournal for child PF worked as expected");
                    }
                    else
                    {
                        NavigationHelper.Fail("Incorrect result when RemoveFromJournal child finishes");
                    }

                    break;
                default:
                    NavigationHelper.Fail("LoadCompleted fired unexpected number of times");
                    break;
            }
        }
    }
}
