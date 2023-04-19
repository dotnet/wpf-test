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
    #region RemoveFromJournalTest
        
        /**********************************************************************\
        *                                                                      *
        *                      Remove from Journal Test                        *
        *                                                                      *
        \**********************************************************************/
        
        /**********************************************************************\
        *                      Remove from Journal false                       *
        \**********************************************************************/
        
        // Remove from journal test will test.
        // 1. Get / Set of RemoveFromJournal Property
        // 2. Default value of RemoveFromJournal Property
        // 3. Test CanGoBack / CanGoForward
        // 4. Test Number of Items in Journal
        // 5. Test Going Back takes you into the finished pagefunction

        
        private void Test_RemoveFromJournalTestFalse ()
        {
            NavigationHelper.CreateLog("PF RemoveFromJournal = false test");
            NavigationHelper.Output("Test of Remove From Journal False");
            Application.Current.LoadCompleted += new LoadCompletedEventHandler(Load_RemoveFromJournalTestFalse);

            ParentBoolPF pfParent = new ParentBoolPF ();
            
            if (pfParent == null)
            {
                NavigationHelper.Fail ("Could not create parent PF");
                return;
            }
            
            if (pfParent.RemoveFromJournal != DEFAULT_REMOVE_FROM_JOURNAL) {
                NavigationHelper.Fail ("Default value of RemoveFromJournal is not as expected");
            }

            MainNavWindow.Navigate (pfParent);
        }

        private void Load_RemoveFromJournalTestFalse (object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output ("Navigation Initiator property is: " + e.IsNavigationInitiator);
            if (e.Navigator is NavigationWindow)
            {
                stage++;
                switch (stage)
                {
                    case 1:
                        NavigationHelper.Output ("Now navigating to a child PF");
                        ParentBoolPF pfPar = MainNavWindow.Content as ParentBoolPF;
                        if (pfPar != null)
                        {
                            NavigationHelper.Output("Navigating to a non-RemoveFromJournal child");
                            pfPar.NavChild (MainNavWindow,false);
                        }
                        break;

                    case 2:
                        PostVerificationItem (new VerificationDelegate (Verify_RemoveFromJournalTest1False));
                        break;

                    case 3:
                        PostVerificationItem (new VerificationDelegate (Verify_RemoveFromJournalTest2False));
                        break;
                    
                    case 4:                    
                        PostVerificationItem (new VerificationDelegate (Verify_RemoveFromJournalTestFalseGoBack));
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

        private void Verify_RemoveFromJournalTest1False ()
        {
            ChildPF_String pfchild = MainNavWindow.Content as ChildPF_String;

            if (pfchild == null)
            {
                NavigationHelper.Fail ("Could not find child PF");
                return;
            }
            
            if (pfchild.RemoveFromJournal) {
                NavigationHelper.Fail("Expected child PF's RemoveFromJournal to be FALSE. Actual: " + pfchild.RemoveFromJournal);
            }

            if (!pfchild.CheckState ())
            {
                NavigationHelper.Fail ("State of child PF is incorrect");
            }
            else
            {
                NavigationHelper.Output ("Child PF display verification succeeded");
            }
            
            if (!MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("Main NavigationWindow's CanGoBack is FALSE (expecting true)");
            } else {
                NavigationHelper.Output("EXPECTED - CanGoBack is TRUE");
            }

            if (MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("Main NavigationWindow's CanGoForward is TRUE (expecting false)");
            } else {
                NavigationHelper.Output("EXPECTED - CanGoForward is FALSE");
            }

//          if (Bag.GetJournalCount (MainNavWindow) == 1)
//          {
//              NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//          } else {
//                NavigationHelper.Fail("Incorrect number of items in Journal");
//            }

            NavigationHelper.Output ("Finishing child PF to test RemoveFromJournal state of the child PF");
            pfchild.RequestFinish ();
        }

        private void Verify_RemoveFromJournalTest2False ()
        {
            ParentBoolPF pf = MainNavWindow.Content as ParentBoolPF;

            if (pf == null)
            {
                NavigationHelper.Fail ("Incorrect PF displayed in the app's window. Currently displaying content is: " + MainNavWindow.Content.GetType ().ToString ());
            }
            else
            {
                NavigationHelper.Output ("Correct PF (parent) was displayed");
            }
            
            if (!pf.CheckState()) {
                NavigationHelper.Fail("Incorrect state of parent PF");
            }
            
            if (!MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("Main NavigationWindow's CanGoBack is FALSE (expecting true)");
            } else {
                NavigationHelper.Output("EXPECTED - CanGoBack is TRUE");
            }

            if (MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("Main NavigationWindow's CanGoForward is TRUE (expecting false)");
            } else {
                NavigationHelper.Output("EXPECTED - CanGoForward is FALSE");
            }
            
//          if (Bag.GetJournalCount (MainNavWindow) == 2)
//          {
//              NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//          }
//          else
//          {
//              NavigationHelper.Fail ("INCORRECT number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//          }
            
            NavigationHelper.Output("Now navigating main window backwards.");
            MainNavWindow.GoBack();
        }

        private void Verify_RemoveFromJournalTestFalseGoBack()
        {
            ChildPF_String pfchild = MainNavWindow.Content as ChildPF_String;

            if (pfchild == null)
            {
                NavigationHelper.Fail("Could not navigate back to the PF that is still supposed to be in the Journal");
                return;
            }

            if (!pfchild.RemoveFromJournal)
            {
                NavigationHelper.Fail("Expected child PF's RemoveFromJournal to be "
                    + "TRUE"
                    + ". Actual: " + pfchild.RemoveFromJournal);
            }

            if (!pfchild.CheckState())
            {
                NavigationHelper.Fail("State of (non-RemoveFromJournal) PF is incorrect");
            }
            else
            {
                NavigationHelper.Output("State of finished non-RemoveFromJournal PF is correct after navigating back");
            }

            if (!MainNavWindow.CanGoBack)
            {
                NavigationHelper.Fail("Main NavigationWindow's CanGoBack is FALSE (expecting true)");
            }
            else
            {
                NavigationHelper.Output("EXPECTED - CanGoBack is TRUE");
            }

            if (!MainNavWindow.CanGoForward)
            {
                NavigationHelper.Fail("Main NavigationWindow's CanGoForward is FALSE (expecting true)");
            }
            else
            {
                NavigationHelper.Output("EXPECTED - CanGoForward is TRUE");
            }

            // if we get to this point without failing, consider it a pass
            NavigationHelper.Pass("All subtests passed");
        }


        
        /**********************************************************************\
        *                      Remove from Journal true                        *
        \**********************************************************************/
        
        // Remove from journal test will test.
        // 1. Get / Set of RemoveFromJournal Property
        // 2. Default value of RemoveFromJournal Property
        // 3. Test CanGoBack / CanGoForward
        // 4. Test Number of Items in Journal
        // 5. Test Going Back does not take you into the finished pagefunction

        
        private void Test_RemoveFromJournalTestTrue ()
        {
            NavigationHelper.CreateLog("PF removeFromJournal = true test");
            NavigationHelper.Output("Test of Remove From Journal True");
            Application.Current.LoadCompleted += new LoadCompletedEventHandler(Load_RemoveFromJournalTestTrue);

            ParentBoolPF pfParent = new ParentBoolPF ();
            
            if (pfParent == null)
            {
                NavigationHelper.Fail ("Could not create parent PF");
                return;
            }            

            MainNavWindow.Navigate (pfParent);

            if (pfParent.RemoveFromJournal != DEFAULT_REMOVE_FROM_JOURNAL) {
                NavigationHelper.Fail ("Default value of RemoveFromJournal is not as expected");
            }        
        }

        private void Load_RemoveFromJournalTestTrue (object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output ("Navigation Initiator property is: " + e.IsNavigationInitiator);
            if (e.Navigator is NavigationWindow)
            {
                stage++;
                switch (stage)
                {
                    case 1:
                        NavigationHelper.Output ("Now navigating to a child PF.");
                        ParentBoolPF pfPar = MainNavWindow.Content as ParentBoolPF;
                        if (pfPar != null)
                        {
                            NavigationHelper.Output("Navigating to a RemoveFromJournal child");
                            pfPar.NavChild (MainNavWindow,true);
                        }
                        break;

                    case 2:
                        PostVerificationItem (new VerificationDelegate (Verify_RemoveFromJournalTest1True));
                        break;

                    case 3:
                        PostVerificationItem (new VerificationDelegate (Verify_RemoveFromJournalTest2True));
                        break;
                    
//                    case 4:                    
//                        PostVerificationItem (new VerificationDelegate (Verify_RemoveFromJournalTestTrueGoBack));
//                          break;

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

        private void Verify_RemoveFromJournalTest1True ()
        {
            ChildPF_String pfchild = MainNavWindow.Content as ChildPF_String;

            if (pfchild == null)
            {
                NavigationHelper.Fail ("Could not find child PF");
                return;
            }
            
            if (!pfchild.RemoveFromJournal) {
                NavigationHelper.Fail("Expected child PF's RemoveFromJournal to be TRUE. Actual: " + pfchild.RemoveFromJournal);
            }

            if (!pfchild.CheckState ())
            {
                NavigationHelper.Fail ("State of child PF is incorrect");
            }
            else
            {
                NavigationHelper.Output ("Child PF verification succeeded");
            }
            
            if (!MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("Main NavigationWindow's CanGoBack is FALSE (expecting true)");
            } else {
                NavigationHelper.Output("EXPECTED - CanGoBack is TRUE");
            }

            if (MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("Main NavigationWindow's CanGoForward is TRUE (expecting false)");
            } else {
                NavigationHelper.Output("EXPECTED - CanGoForward is FALSE");
            }

//            if (Bag.GetJournalCount (MainNavWindow) == 1)
//            {
//                NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            } else {
//                NavigationHelper.Fail("Incorrect number of items in Journal");
//            }

            NavigationHelper.Output ("Finishing child PF to test the RemoveFromJournal state of the child PF");
            pfchild.RequestFinish ();
        }

        private void Verify_RemoveFromJournalTest2True ()
        {
            ParentBoolPF pf = MainNavWindow.Content as ParentBoolPF;

            if (pf == null)
            {
                NavigationHelper.Fail ("Incorrect PF displayed in the app's window. Currently displaying content is: " + MainNavWindow.Content.GetType ().ToString ());
            }
            else
            {
                NavigationHelper.Output ("Correct PF (parent) was displayed");
            }
            
            if (!pf.CheckState()) {
                NavigationHelper.Fail("Incorrect state of parent PF");
            }
            
            if (!MainNavWindow.CanGoBack) {
                NavigationHelper.Output("Main NavigationWindow's CanGoBack is FALSE. Child PF is removed from journal; navigation to parent page is automatic (equiv to GoBack)");
            } else {
                NavigationHelper.Fail("Main NavigationWindow's CanGoBack is TRUE (expecting false)");
            }

            if (MainNavWindow.CanGoForward) {
                NavigationHelper.Fail("Main NavigationWindow's CanGoForward is TRUE (expecting false)");
            } else {
                NavigationHelper.Output("EXPECTED - CanGoForward is FALSE");
            }
            
//            if (Bag.GetJournalCount (MainNavWindow) == 1)
//            {
//                NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            }
//            else
//            {
//                NavigationHelper.Fail ("INCORRECT number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            }
            
            NavigationHelper.Output("Now trying to navigate NavigationWindow backwards.");
            try
            {
                MainNavWindow.GoBack();
            }
            catch (InvalidOperationException)
            {
                NavigationHelper.Pass("Could not go back.");
            }
        }
        
        private void Verify_RemoveFromJournalTestTrueGoBack() 
        {
            ParentBoolPF pf = MainNavWindow.Content as ParentBoolPF;

            if (pf == null)
            {
                NavigationHelper.Fail ("Could not navigate back to PF that is still supposed to be in the journal");
                return;
            }

            if (!pf.CheckState ())
            {
                NavigationHelper.Fail ("State of (non-RemoveFromJournal) PF is incorrect");
            }
            else
            {
                NavigationHelper.Output ("State of finished non-RemoveFromJournal PF is correct after navigating back");
            }
            
            if (MainNavWindow.CanGoBack) {
                NavigationHelper.Fail("Main NavigationWindow's CanGoBack is TRUE (expecting false)");
            } else {
                NavigationHelper.Output("EXPECTED - CanGoBack is FALSE");
            }

            if (MainNavWindow.CanGoForward) {
                NavigationHelper.Output("EXPECTED - CanGoForward is TRUE");
            } else {
                NavigationHelper.Fail("Main NavigationWindow's CanGoForward is FALSE (expecting true)");
            }

//            if (Bag.GetJournalCount (MainNavWindow) == 2)
//            {
//                NavigationHelper.Output ("Correct number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//            } else {
//                NavigationHelper.Fail("Incorrect number of items in Journal");
//            }

        }

    #endregion
    }
}
