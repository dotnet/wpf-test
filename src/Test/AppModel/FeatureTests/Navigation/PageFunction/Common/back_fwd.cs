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
*                                                     *
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

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class PageFunctionTestApp
    {
        #region BackTest
        
        /*****************************************************\
        *                                                     *
        *                    BACK TESTS                       *
        *                                                     *
        \*****************************************************/
        
        /*****************************************************\
        *          Back Test with Child PageFunction          *
        \*****************************************************/
        

        private void Test_BackTest ()
        {
            NavigationHelper.CreateLog("PFBackTest");
            NavigationHelper.Output("Navigate Back into a pagefunction from a child pf");
            Application.Current.LoadCompleted += new LoadCompletedEventHandler (Load_BackTest);
            ParentBoolPF pfParent = new ParentBoolPF();
            if (pfParent == null)
            {
                NavigationHelper.Fail("Could not create parent pf");
            }

            MainNavWindow.Navigate (pfParent);
        }

        private void Load_BackTest (object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigation Initiator property is: " + e.IsNavigationInitiator);
            if (e.Navigator is NavigationWindow)
            {
                stage++;
                switch (stage)
                {
                    case 1:
                        NavigationHelper.Output("Now navigating to a child pagefunction.");

                        ParentBoolPF pfPar = MainNavWindow.Content as ParentBoolPF;

                        if (pfPar != null)
                        {
                            pfPar.NavChild (MainNavWindow, false);
                        }

                        break;

                    case 2:
                        PostVerificationItem (new VerificationDelegate (Verify_BackTest));
                        break;
                    case 3:
                        PostVerificationItem (new VerificationDelegate (Verify_BackTest2));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                NavigationHelper.Output("Sender is: " + e.Navigator.GetType ().ToString ());
            }
        }

        private void Verify_BackTest ()
        {
            ChildPF_String pfchild = MainNavWindow.Content as ChildPF_String;

            if (pfchild == null)
            {
                NavigationHelper.Fail ("Could not find child pf");
                return;
            }

            if (!pfchild.CheckState ())
            {
                NavigationHelper.Fail ("State of child pf is incorrect");
            }
            else
            {
                NavigationHelper.Output ("Child pagefunction display verification succeeded");
            }
            
            if (!MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("can go back of window is false");
            } else {
                NavigationHelper.Output("CanGoBack is as expected");
            }

            if (MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("can go Forward of window is true");
            } else {
                NavigationHelper.Output("CanGoForward is as expected");
            }

//          if (Bag.GetJournalCount (MainNavWindow) == 1)
//          {
//              NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//          }
//          else
//          {
//              NavigationHelper.Fail ("Incorrect number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//          }

            NavigationHelper.Output ("Now going back in the app to the parent pf");
            MainNavWindow.GoBack ();
        }

        private void Verify_BackTest2 ()
        {
            ParentBoolPF pf = MainNavWindow.Content as ParentBoolPF;

            if (pf == null)
            {
                NavigationHelper.Fail ("Incorrect pf was displayed in the app's window. Currently displaying content is: " + MainNavWindow.Content.GetType ().ToString ());
            }
            else
            {
                NavigationHelper.Output ("Correct pf (parent) was displayed");
            }

            if (MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("can go back of window is true");
            } else {
                NavigationHelper.Output("CanGoBack is as expected");
            }

            if (!MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("can go Forward of window is false");
            } else {
                NavigationHelper.Output("CanGoForward is as expected");
            }

//          if (Bag.GetJournalCount (MainNavWindow) == 2)
//          {
//              NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//          }
//          else
//          {
//              NavigationHelper.Fail ("INCORRECT number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//          }

            if (!pf.CheckState ())
            {
                NavigationHelper.Fail ("Incorrect state of parent pf detected");
            }
            else
            {
                NavigationHelper.Output ("Back functionality between parent and child pfs functioned as expected");
            }
            
            NavigationHelper.Pass("PageFunction Back test passed");
        }


    #endregion


        #region FwdTest
        /*****************************************************\
        *                                                     *
        *                   FORWARD TESTS                     *
        *                                                     *
        \*****************************************************/
        
        /*****************************************************\
        *            Navigate Back and Forward                *
        \*****************************************************/
        

        
        private void Test_FwdTest ()
        {
            NavigationHelper.CreateLog("PFForwardTest");
            NavigationHelper.Output("Navigate Forward into a pagefunction from a parent pf");
            Application.Current.LoadCompleted += new LoadCompletedEventHandler (Load_FwdTest);

            ParentBoolPF pfParent = new ParentBoolPF ();

            if (pfParent == null)
            {
                NavigationHelper.Fail ("Could not create parent pf");
            }

            MainNavWindow.Navigate (pfParent);
        }

        private void Load_FwdTest (object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output ("Navigation Initiator property is: " + e.IsNavigationInitiator);
            if (e.Navigator is NavigationWindow)
            {
                stage++;
                switch (stage)
                {
                    case 1:
                        NavigationHelper.Output ("Now navigating to a child pagefunction.");
                        
                        ParentBoolPF pfPar = MainNavWindow.Content as ParentBoolPF;

                        if (pfPar != null)
                        {
                            pfPar.NavChild (MainNavWindow, false);
                        }

                        break;

                    case 2:
                        PostVerificationItem (new VerificationDelegate (Verify_FwdTest1));
                        break;

                    case 3:
                        PostVerificationItem (new VerificationDelegate (Verify_FwdTest2));
                        break;

                    case 4:
                        PostVerificationItem (new VerificationDelegate (Verify_FwdTest3));
                        break;

                    default:
                        NavigationHelper.Fail("Should not get here.");
                        break;
                }
            }
            else
            {
                NavigationHelper.Output ("Sender is: " + e.Navigator.GetType ().ToString ());
            }
        }

        private void Verify_FwdTest1 ()
        {
            ChildPF_String pfchild = MainNavWindow.Content as ChildPF_String;

            if (pfchild == null)
            {
                NavigationHelper.Fail ("Could not find child pf");
                return;
            }

            if (!pfchild.CheckState ())
            {
                NavigationHelper.Fail ("State of chld pf is incorrect");
            }
            else
            {
                NavigationHelper.Output ("Child pagefunction verification succeeded");
            }

//          if (Bag.GetJournalCount (MainNavWindow) == 1)
//          {
//              NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            } else {
//                NavigationHelper.Fail("Incorrect number of items in journal: Expect: 1" + ". Actual: " + Bag.GetJournalCount (MainNavWindow));
//            }
            
            if (!MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("CanGoBack of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoBack is as expected");
            }

            if (MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("CanGoForward of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoForward is as expected");
            }
            

            NavigationHelper.Output ("Navigating the window backwards to the parent pagefunction");
            
            MainNavWindow.GoBack();
        }

        private void Verify_FwdTest2 ()
        {
            ParentBoolPF pf = MainNavWindow.Content as ParentBoolPF;

            if (pf == null)
            {
                NavigationHelper.Fail ("Incorrect pf was displayed in the app's window. Currently displaying content is: " + MainNavWindow.Content.GetType ().ToString ());
            }
            else
            {
                NavigationHelper.Output ("Correct pf (parent) was displayed");
            }

//            if (Bag.GetJournalCount (MainNavWindow) == 2)
//            {
//                NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            }
//            else
//            {
//                NavigationHelper.Fail ("INCORRECT number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            }
            
            if (MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("CanGoBack of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoBack is as expected");
            }

            if (!MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("CanGoForward of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoForward is as expected");
            }
            
            NavigationHelper.Output("Now Navigating forward into the child pagefunction");
            MainNavWindow.GoForward();
        }

        private void Verify_FwdTest3 ()
        {
            ChildPF_String pfchild = MainNavWindow.Content as ChildPF_String;

            if (pfchild == null)
            {
                NavigationHelper.Fail ("Could not find child pf");
                return;
            }

            if (!pfchild.CheckState ())
            {
                NavigationHelper.Fail ("State of chld pf is incorrect");
            }
            else
            {
                NavigationHelper.Output ("Child pagefunction verification succeeded");
            }

//            if (Bag.GetJournalCount (MainNavWindow) == 2)
//            {
//                NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            } else {
//                NavigationHelper.Fail("Incorrect number of items in journal: Expect: 2" + ". Actual: " + Bag.GetJournalCount (MainNavWindow));
//            }
            
            if (!MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("CanGoBack of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoBack is as expected");
            }

            if (MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("CanGoForward of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoForward is as expected");
            }

            NavigationHelper.Pass("PageFunction Forward test passed");
        }


    #endregion


    #region combined back and forward tests
        /****************************************************************\
        *    Navigate Back and Forward and in and out of a Finished PF   *
        \****************************************************************/
        
        // Goes back and forward multiple times
        
        
        private void Test_BackFwdTestFinishedChild ()
        {
            NavigationHelper.CreateLog("PFBackForwardTest");
            NavigationHelper.Output("Navigate Back and Forward into a pagefunction that has already finished");
            Application.Current.LoadCompleted += new LoadCompletedEventHandler (Load_BackFwdTestFinishedChild);

            ParentBoolPF pfParent = new ParentBoolPF ();

            if (pfParent == null)
            {
                NavigationHelper.Fail ("Could not create parent pf");
            }

            MainNavWindow.Navigate (pfParent);
        }

        private void Load_BackFwdTestFinishedChild (object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output ("Navigation Initiator property is: " + e.IsNavigationInitiator);
            if (e.Navigator is NavigationWindow)
            {
                stage++;
                switch (stage)
                {
                    case 1:
                        NavigationHelper.Output ("Now navigating to a child pagefunction.");
                        
                        ParentBoolPF pfPar = MainNavWindow.Content as ParentBoolPF;

                        if (pfPar != null)
                        {
                            pfPar.NavChild (MainNavWindow, false);
                        }

                        break;

                    case 2:
                        NavigationHelper.Output("Verification after Navigation to child");
                        PostVerificationItem (new VerificationDelegate (Verify_BackFwdTestFinishedChild1));
                        break;

                    case 3:
                        NavigationHelper.Output("Verification after child pf has finished");
                        PostVerificationItem (new VerificationDelegate (Verify_BackFwdTestFinishedChild2));
                        break;

                    case 4:
                        NavigationHelper.Output("Verification after going back once");
                        PostVerificationItem (new VerificationDelegate (Verify_BackFwdTestFinishedChild3));
                        break;

                    case 5:
                        NavigationHelper.Output("Verification after going back again");
                        PostVerificationItem (new VerificationDelegate (Verify_BackFwdTestFinishedChild4));
                        break;

                    case 6:
                        NavigationHelper.Output("Verification after going forward once");
                        PostVerificationItem (new VerificationDelegate (Verify_BackFwdTestFinishedChild5));
                        break;

                    case 7:
                        NavigationHelper.Output("Verification after going forward again");
                        PostVerificationItem (new VerificationDelegate (Verify_BackFwdTestFinishedChild6));
                        break;

                    default:
                        NavigationHelper.Fail("Should not get here.");
                        break;
                }
            }
            else
            {
                NavigationHelper.Output ("Sender is: " + e.Navigator.GetType ().ToString ());
            }
        }

        private void Verify_BackFwdTestFinishedChild1 ()
        {
            ChildPF_String pfchild = MainNavWindow.Content as ChildPF_String;

            if (pfchild == null)
            {
                NavigationHelper.Fail ("Could not find child pf");
                return;
            }

            if (!pfchild.CheckState ())
            {
                NavigationHelper.Fail ("State of chld pf is incorrect");
            }
            else
            {
                NavigationHelper.Output ("Child pagefunction verification succeeded");
            }

//            if (Bag.GetJournalCount (MainNavWindow) == 1)
//            {
//                NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            } else {
//                NavigationHelper.Fail("Incorrect number of items in journal: Expect: 1" + ". Actual: " + Bag.GetJournalCount (MainNavWindow));
//            }
            
            if (!MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("CanGoBack of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoBack is as expected");
            }

            if (MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("CanGoForward of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoForward is as expected");
            }
            

            NavigationHelper.Output ("Finishing the child PageFunction");
            
            pfchild.RequestFinish();
        }

        private void Verify_BackFwdTestFinishedChild2 ()
        {
            ParentBoolPF pf = MainNavWindow.Content as ParentBoolPF;

            if (pf == null)
            {
                NavigationHelper.Fail ("Incorrect pf was displayed in the app's window. Currently displaying content is: " + MainNavWindow.Content.GetType ().ToString ());
            }
            else
            {
                NavigationHelper.Output ("Correct pf (parent) was displayed");
            }
            
            if(!pf.CheckState()) {
                NavigationHelper.Fail("Incorrect state of parent pf");
            } else {
                NavigationHelper.Output("Correct state of parent pf");
            }

            NavigationHelper.Output("Checking Journal after the child pf has finished.");

//            if (Bag.GetJournalCount (MainNavWindow) == 2)
//            {
//                NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            }
//            else
//            {
//                NavigationHelper.Fail ("INCORRECT number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            }
            
            if (!MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("CanGoBack of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoBack is as expected");
            }

            if (MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("CanGoForward of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoForward is as expected");
            }
            
            NavigationHelper.Output("Now Navigating back into the child pagefunction");
            MainNavWindow.GoBack();
        }

        private void Verify_BackFwdTestFinishedChild3 ()
        {
            ChildPF_String pfchild = MainNavWindow.Content as ChildPF_String;

            if (pfchild == null)
            {
                NavigationHelper.Fail ("Could not find child pf");
                return;
            }

            if (!pfchild.CheckState ())
            {
                NavigationHelper.Fail ("State of chld pf is incorrect");
            }
            else
            {
                NavigationHelper.Output ("Child pagefunction verification succeeded");
            }

            NavigationHelper.Output("Checking journal after navigating back to finished child");

//            if (Bag.GetJournalCount (MainNavWindow) == 3)
//            {
//                NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            } else {
//                NavigationHelper.Fail("Incorrect number of items in journal: Expect: 2" + ". Actual: " + Bag.GetJournalCount (MainNavWindow));
//            }
            
            if (!MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("CanGoBack of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoBack is as expected");
            }

            if (!MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("CanGoForward of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoForward is as expected");
            }
                
            NavigationHelper.Output("Navigating back once more.");    
            MainNavWindow.GoBack();
            
        }

        private void Verify_BackFwdTestFinishedChild4 ()
        {
            ParentBoolPF pf = MainNavWindow.Content as ParentBoolPF;

            if (pf == null)
            {
                NavigationHelper.Fail ("Incorrect pf was displayed in the app's window. Currently displaying content is: " + MainNavWindow.Content.GetType ().ToString ());
            }
            else
            {
                NavigationHelper.Output ("Correct pf (parent) was displayed");
            }
            
            if(!pf.CheckState()) {
                NavigationHelper.Fail("Incorrect state of parent pf");
            } else {
                NavigationHelper.Output("Correct state of parent pf");
            }
            
            NavigationHelper.Output("Checking Journal after navigating back to the parent from a finished child pf.");

//            if (Bag.GetJournalCount (MainNavWindow) == 3)
//            {
//                NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            }
//            else
//            {
//                NavigationHelper.Fail ("INCORRECT number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            }
            
            if (MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("CanGoBack of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoBack is as expected");
            }

            if (!MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("CanGoForward of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoForward is as expected");
            }
            
            NavigationHelper.Output("Now Navigating Forward into the (finished) child pagefunction");
            MainNavWindow.GoForward();
        }
        
        private void Verify_BackFwdTestFinishedChild5 ()
        {
            ChildPF_String pfchild = MainNavWindow.Content as ChildPF_String;

            if (pfchild == null)
            {
                NavigationHelper.Fail ("Could not find child pf");
                return;
            }

            if (!pfchild.CheckState ())
            {
                NavigationHelper.Fail ("State of chld pf is incorrect");
            }
            else
            {
                NavigationHelper.Output ("Child pagefunction verification succeeded");
            }

            NavigationHelper.Output("Checking journal after navigating forward into finished child");

//            if (Bag.GetJournalCount (MainNavWindow) == 3)
//            {
//                NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            } else {
//                NavigationHelper.Fail("Incorrect number of items in journal: Expect: 2" + ". Actual: " + Bag.GetJournalCount (MainNavWindow));
//            }
            
            if (!MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("CanGoBack of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoBack is as expected");
            }

            if (!MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("CanGoForward of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoForward is as expected");
            }
                
            NavigationHelper.Output("Navigating forward once more.");    
            MainNavWindow.GoForward();
            
        }
        
        private void Verify_BackFwdTestFinishedChild6 ()
        {
            ParentBoolPF pf = MainNavWindow.Content as ParentBoolPF;

            if (pf == null)
            {
                NavigationHelper.Fail ("Incorrect pf was displayed in the app's window. Currently displaying content is: " + MainNavWindow.Content.GetType ().ToString ());
            }
            else
            {
                NavigationHelper.Output ("Correct pf (parent) was displayed");
            }
            
            if(!pf.CheckState()) {
                NavigationHelper.Fail("Incorrect state of parent pf");
            } else {
                NavigationHelper.Output("Correct state of parent pf");
            }
            
            NavigationHelper.Output("Checking Journal after navigating forward to the parent from a finished child pf.");

//            if (Bag.GetJournalCount (MainNavWindow) == 3)
//            {
//                NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            }
//            else
//            {
//                NavigationHelper.Fail ("INCORRECT number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            }
            
            if (!MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("CanGoBack of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoBack is as expected");
            }

            if (MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("CanGoForward of window is not as expected");
                return;
            } else {
                NavigationHelper.Output("CanGoForward is as expected");
            }

            NavigationHelper.Pass("PageFunction Back/Forward test passed");
        }        

    #endregion
    }
}
