// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
*************************************************************************************
*                                                                                   *
*  Title:                                                                           *
*                                                                                   *
*  Description:                                                                     *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   * 
*************************************************************************************
*/

#define use_tools
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Markup;
using System.Collections;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
using System.Globalization;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{ 
    public partial class PageFunctionTestApp
    {
    #region verify call to Start on initialization
        private void Test_StartMethodBasic ()
        {
            NavigationHelper.CreateLog("PageFunction Start method test");
            NavigationHelper.Output("Basic test to see that Start method on PageFunction is called");
            StartMethodTestParentPF pfParent = new StartMethodTestParentPF ();
            if (pfParent == null)
            {
                NavigationHelper.Fail ("Could not create parent pf");
                return;
            }
            Application.Current.LoadCompleted += new LoadCompletedEventHandler(Load_StartMethodBasic);
            MainNavWindow.Navigate (pfParent);

        }

        private void Load_StartMethodBasic (object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output ("Navigation Initiator property is: " + e.IsNavigationInitiator);
            if (e.Navigator is NavigationWindow)
            {
                stage++;
                switch (stage)
                {
                    case 1:
                        NavigationHelper.Output ("Now verifying start method was called.");
                        PostVerificationItem (new VerificationDelegate (Verify_StartMethodBasic));
                        break;

                    default:
                        break;
                }
            }
            else
            {
                NavigationHelper.Output ("Sender is: " + e.Navigator.GetType ().ToString ());
            }
        }

        private void Verify_StartMethodBasic ()
        {
            if (!TestEventsHT.ContainsKey ("Numtimes_startcalled"))
            {
                NavigationHelper.Fail ("Start was not called on the parent pf.");
                return;
            }

            if (((int)TestEventsHT["Numtimes_startcalled"]) != 1)
            {
                NavigationHelper.Fail ("Start method was called incorrect number of times: " 
                + ((int)TestEventsHT["Numtimes_startcalled"])
                + "\nExpected; 1");
                return;
            }
            NavigationHelper.Pass ("Start method was called as expected (and the correct number ot times).");
        }

    #endregion
    #region verify start does not get called on return from child
        private void Test_StartMethodChildFinish ()
        {
            Description = "Basic test to see that Start method on PageFunction is not called when child pf finishes";
            StartMethodTestParentPF pfParent = new StartMethodTestParentPF ();
            if (pfParent == null)
            {
                NavigationHelper.Fail ("Could not create parent pf");
                return;
            }

            Application.Current.LoadCompleted += new LoadCompletedEventHandler(Load_StartMethodChildFinish);
            MainNavWindow.Navigate (pfParent);
        }

        private void Load_StartMethodChildFinish (object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output ("Navigation Initiator property is: " + e.IsNavigationInitiator);
            if (e.Navigator is NavigationWindow)
            {
                stage++;
                switch (stage)
                {
                    case 1:
                        NavigationHelper.Output ("Now navigating to child pagefunction.");
                        StartMethodTestParentPF pfPar = MainNavWindow.Content as StartMethodTestParentPF;
                        if (pfPar != null)
                        {
                            pfPar.NavChild(MainNavWindow);
                        }                       
                        break;
                    case 2:
                        NavigationHelper.Output ("Now finishing child pagefunction.");
                        ChildPF_Frame_Int pfchild = MainNavWindow.Content as ChildPF_Frame_Int;
                        if (pfchild == null)
                        {
                            NavigationHelper.Fail ("Could not find child pf");
                            return;
                        }
                        pfchild.RequestFinish ();
                        break;
                    case 3:
                        NavigationHelper.Output ("Now verifying Start method on parent was only called once");
                        PostVerificationItem (new VerificationDelegate (Verify_StartMethodChildFinish));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                NavigationHelper.Output ("Sender is: " + e.Navigator.GetType ().ToString ());
            }
        }

        private void Verify_StartMethodChildFinish ()
        {
            if (!TestEventsHT.ContainsKey ("Numtimes_startcalled"))
            {
                NavigationHelper.Fail ("Start was not called on the parent pf.");
                return;
            }

            if (((int)TestEventsHT["Numtimes_startcalled"]) != 1)
            {
                NavigationHelper.Fail ("Start method was called incorrect number of times when finishing the child pf: " + ((int)TestEventsHT["Numtimes_startcalled"]) + "\nExpected; 1");
                return;
            }

            NavigationHelper.Pass ("Start method was called as expected (and the correct number ot times -- 1).");
        }

    #endregion
    #region verify start does not get called on return from child (Journaling serialization impl)
        //
    #endregion

    #region verify start does not get called on history navigations (back)
        private void Test_StartMethodHistoryNavBack ()
        {
            NavigationHelper.CreateLog("PF Start method not called on back navigation");
            NavigationHelper.Output("Basic test to see that Start method on PageFunction is not called on history navigation (back))");

            StartMethodTestParentPF pfParent = new StartMethodTestParentPF ();

            if (pfParent == null)
            {
                NavigationHelper.Fail ("Could not create parent pf");
                return;
            }

            Application.Current.LoadCompleted += new LoadCompletedEventHandler(Load_StartMethodHistoryNavBack);
            MainNavWindow.Navigate (pfParent);
        }

        private void Load_StartMethodHistoryNavBack (object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output ("Navigation Initiator property is: " + e.IsNavigationInitiator);
            if (e.Navigator is NavigationWindow)
            {
                stage++;
                switch (stage)
                {
                    case 1:
                        NavigationHelper.Output ("Now navigating to child pagefunction.");

                        StartMethodTestParentPF pfPar = MainNavWindow.Content as StartMethodTestParentPF;

                        if (pfPar != null)
                        {
                            pfPar.NavChild (MainNavWindow);
                        }

                        break;

                    case 2:

                        ChildPF_Frame_Int pfchild = MainNavWindow.Content as ChildPF_Frame_Int;

                        if (pfchild == null)
                        {
                            NavigationHelper.Fail ("Could not find child pf");
                            return;
                        }

                        NavigationHelper.Output ("Now going back.");
                        MainNavWindow.GoBack();
                        break;

                    case 3:
                        NavigationHelper.Output ("Now verifying Start method on parent was only called once");
                        PostVerificationItem (new VerificationDelegate (Verify_StartMethodHistoryNavBack));
                        break;

                    default:
                        break;
                }
            }
            else
            {
                NavigationHelper.Output ("Sender is: " + e.Navigator.GetType ().ToString ());
            }
        }

        private void Verify_StartMethodHistoryNavBack ()
        {
            if (!TestEventsHT.ContainsKey ("Numtimes_startcalled"))
            {
                NavigationHelper.Fail ("Start was not called on the parent pf.");
                return;
            }

            if (((int)TestEventsHT["Numtimes_startcalled"]) != 1)
            {
                NavigationHelper.Fail ("Start method was called incorrect number of times for history navigation (back): " + ((int)TestEventsHT["Numtimes_startcalled"]) + "\nExpected: 1");
                return;
            }

            NavigationHelper.Pass ("Start method was called as expected (and the correct number of times -- 1).");
        }

    #endregion
    #region verify start does not get called on history navigations (forward)
        private void Test_StartMethodHistoryNavFwd ()
        {
            NavigationHelper.CreateLog("PF Start method is not called on forward navigation");
            NavigationHelper.Output("Basic test to see that Start method on PageFunction is not called on history navigation (fwd)");

            StartMethodTestParentPF pfParent = new StartMethodTestParentPF ();

            if (pfParent == null)
            {
                NavigationHelper.Fail ("Could not create parent pf");
                return;
            }

            Application.Current.LoadCompleted += new LoadCompletedEventHandler(Load_StartMethodHistoryNavFwd);
            MainNavWindow.Navigate (pfParent);
        }

        private void Load_StartMethodHistoryNavFwd (object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output ("Navigation Initiator property is: " + e.IsNavigationInitiator);
            if (e.Navigator is NavigationWindow)
            {
                stage++;
                switch (stage)
                {
                    case 1:
                        NavigationHelper.Output ("Now navigating to child pagefunction.");

                        StartMethodTestParentPF pfPar = MainNavWindow.Content as StartMethodTestParentPF;

                        if (pfPar != null)
                        {
                            pfPar.NavChild (MainNavWindow);
                        }

                        break;

                    case 2:
                        ChildPF_Frame_Int pfchild = MainNavWindow.Content as ChildPF_Frame_Int;

                        if (pfchild == null)
                        {
                            NavigationHelper.Fail ("Could not find child pf");
                            return;
                        }

                        NavigationHelper.Output ("Now going back (to parentpf).");
                        MainNavWindow.GoBack ();
                        break;

                    case 3:

                        NavigationHelper.Output ("Now going fwd (to child).");
                        MainNavWindow.GoForward ();
                        break;

                    case 4:
                        NavigationHelper.Output ("Now verifying Start method on parent was only called once");
                        PostVerificationItem (new VerificationDelegate (Verify_StartMethodHistoryNavFwd));
                        break;

                    default:
                        break;
                }
            }
            else
            {
                NavigationHelper.Output ("Sender is: " + e.Navigator.GetType ().ToString ());
            }
        }

        private void Verify_StartMethodHistoryNavFwd ()
        {
            if (!TestEventsHT.ContainsKey ("Numtimes_startcalled"))
            {
                NavigationHelper.Fail ("Start was not called on the parent pf.");
                return;
            }

            if (((int)TestEventsHT["Numtimes_startcalled"]) != 1)
            {
                NavigationHelper.Fail ("Start method was called incorrect number of times on parent pf when going back: " + ((int)TestEventsHT["Numtimes_startcalled"]) + "\nExpected: 1");
                return;
            }

            if (!TestEventsHT.ContainsKey ("Numtimes_startcalled_*ChildPF_Frame_Int"))
            {
                NavigationHelper.Fail ("Start method was not called on the child pf");
                return;
            }

            if ((int)TestEventsHT["Numtimes_startcalled_*ChildPF_Frame_Int"] != 1)
            {
                NavigationHelper.Fail ("Start method was called incorrect number of times on child pf when going fwd: " + ((int)TestEventsHT["Numtimes_startcalled_ * ChildPF_Frame_Int"]) + "\nExpected: 1");
                return;
            }

            NavigationHelper.Pass ("Start method was called as expected (and the correct number of times on the parent and child on back/forward -- 1).");
        }

    #endregion

    }
}
