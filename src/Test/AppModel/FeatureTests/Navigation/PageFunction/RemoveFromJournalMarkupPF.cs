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
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // RemoveFromJournalMarkupPF
    public partial class NavigationTests : Application
    {
        #region RemoveFromJournalMarkupPF globals
        private int _removeFromJournalMarkupPFTest = 0;

        #endregion

        void RemoveFromJournalMarkupPF_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("RemoveFromJournalMarkupPF");
            NavigationHelper.Output("Test of Remove From Journal default and typeconverters from Markup PFs");
            NavigationHelper.Output("App Starting Up");
            Application.Current.StartupUri = new Uri(MARKUPBOOLPF1, UriKind.RelativeOrAbsolute);

            NavigationHelper.SetStage(TestStage.Run);
        }

        void RemoveFromJournalMarkupPF_LoadCompleted(object sender, NavigationEventArgs e)
        {
            BoolPFMarkup1 _mkpf;
            IntPFMarkup1 _mkpf2;

            NavigationHelper.Output("Load Completed for Application. Test: " + _removeFromJournalMarkupPFTest);
            switch (_removeFromJournalMarkupPFTest++)
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
                    NavigationHelper.Output("Testing the start PF (markup bool PF) which has default value for RemoveFromJournal");
                    _mkpf.SelfTest(false, false, 1, 1, true /*default value is TRUE*/);

                    NavigationHelper.Output("Navigating to child pf");
                    Control ctlBtnNext = LogicalTreeHelper.FindLogicalNode(_mkpf, "BtnNextChildInt") as Control;
                    if (ctlBtnNext == null)
                    {
                        NavigationHelper.Fail("Could not find button to launch the child (int) PF");
                    }
                    else
                    {
                        NavigationHelper.Output("Attempting to launch child pf by clicking on the button");
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

                    NavigationHelper.Output("Finishing the child int PF with RemoveFromJournal == true");
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
                        NavigationHelper.Fail("Wrong content (not bool pf markup)");
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
                        NavigationHelper.Fail("Failed.");
                    }
                    break;
                default:
                    NavigationHelper.Fail("LoadCompleted fired unexpected number of times");
                    break;
            }
        }
    }
}
