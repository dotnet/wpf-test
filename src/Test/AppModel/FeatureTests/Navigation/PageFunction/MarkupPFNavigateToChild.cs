// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // MarkupPFNavigateToChild
    public partial class NavigationTests : Application
    {
        #region MarkupPFNavigateToChild globals
        private int _markupPFNavigateToChildTest = 0;

        #endregion

        void MarkupPFNavigateToChild_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("MarkupPFNavigateToChild");
            NavigationHelper.Output("Markup PageFunction navigation to child");

            NavigationHelper.Output("App starting up");
            Application.Current.StartupUri = new Uri(MARKUPSTRPF1, UriKind.RelativeOrAbsolute);
            NavigationHelper.SetStage(Microsoft.Test.Logging.TestStage.Run);
        }

        void MarkupPFNavigateToChild_LoadCompleted(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("Load Completed for Application");
            switch (_markupPFNavigateToChildTest++)
            {
                case 0:
                    StringPFMarkup _mkpf = MainWindow.Content as StringPFMarkup;
                    if (_mkpf == null)
                    {
                        NavigationHelper.Fail("Wrong content (not string pf markup)");
                    }
                    else
                    {
                        NavigationHelper.Output("Correct markup pf content loaded");
                    }
                    Control ctlLink = LogicalTreeHelper.FindLogicalNode(_mkpf, "LNKNextNoAttach") as Control;
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
                        NavigationHelper.Pass("Child Markup PageFunction Test passed (no return handler)");
                    }
                    break;
            }
        }
    }
}
