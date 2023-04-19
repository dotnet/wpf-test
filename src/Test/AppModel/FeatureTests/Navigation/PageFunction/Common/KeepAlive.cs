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


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class PageFunctionTestApp
    {
        #region basic KeepAlive Test

        /*****************************************************\
        *                                                     *
        *               KEEP ALIVE TESTS                      *
        *                                                     *
        \*****************************************************/
        // test methods:
        // This test is only setting keepalive on the parent pagefunction, and 
        //       is only testing for keepalive when a chid pagefunction finishes
        // This app tests for:
        // 1. Saved data in the parent pf (foodata, IsAlive)
        // 2. Parent PF creation time (also indirectly checks to see if constructor 
        //          is not called upon return)
        // 3. Saved data in the element tree of the parent pf (in a control that does not
        //           implement Journal DP, i.e. not serialized to Journal)
        // 4. Set and Get of KeepAlive property
        // 5. KeepAlive property after child pagefunction finishes.
        // 6. Number of items in Journal
        



        private void Test_KeepAlive ()
        {
            NavigationHelper.CreateLog("Basic PF KeepAlive test");
            Application.Current.LoadCompleted += new LoadCompletedEventHandler(Load_KeepAlive);

            ParentBoolPF pfParent = new ParentBoolPF ();

            if (pfParent == null)
            {
                NavigationHelper.Fail ("Could not create parent pf");
            }

            TestEventsHT["ParentPF"] = pfParent;
            TestEventsHT["ParentPFOriginalCreate"] = pfParent.CreateTime;

            TextBlock _textcontent = pfParent.tt;

            if (Bag.SupportsSerializationToJournal(_textcontent)) {
                NavigationHelper.Fail("Text element now implements serializaton to Journal (Journal  metadata flag set on DP), so this testcase needs to be changed.");    
            }
            
            pfParent.tt.Text="KeepAlivePageFunction";
            
            // Setting and Getting KeepAlive property.
            NavigationHelper.Output ("Setting the KeepAlive property of the parent pagefunction to True");
            JournalEntry.SetKeepAlive(pfParent, true);
            if (!JournalEntry.GetKeepAlive(pfParent)) {
                NavigationHelper.Fail("Setting and Getting KeepAlive property of the app failed");
            }

            MainNavWindow.Navigate (pfParent);
        }

        private void Load_KeepAlive (object sender, NavigationEventArgs e)
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
                            pfPar.NavChild (MainNavWindow);
                        }
                        
                        
                        break;

                    case 2:
                        PostVerificationItem (new VerificationDelegate (Verify_KeepAlive1));
                        break;

                    case 3:
                        PostVerificationItem (new VerificationDelegate (Verify_KeepAlive2));
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

        private void Verify_KeepAlive1 ()
        {
            ChildPF_String pfchild = MainNavWindow.Content as ChildPF_String;

            if (pfchild == null)
            {
                NavigationHelper.Fail ("Could not find child pf");
            }

            if (!pfchild.CheckState ())
            {
                NavigationHelper.Fail ("State of chld pf is incorrect");
            }
            else
            {
                NavigationHelper.Output ("Child pagefunction display verification succeeded");
            }

//            if (Bag.GetJournalCount (MainNavWindow) == 1)
//            {
//                NavigationHelper.Output ("Correct number of items in journal");
//            }

            ParentBoolPF parentpf = TestEventsHT["ParentPF"] as ParentBoolPF;
            if (parentpf == null)
            {
                NavigationHelper.Fail ("Parent pf is null for keepalive pf!");
            }

            if (parentpf.foodata != 5 || !parentpf.IsActive )
            {
                NavigationHelper.Fail ("Parent pf state is incorrect!");
            }
            
            if (parentpf.tt.Text == null || parentpf.tt.Text != "KeepAlivePageFunction") {
                NavigationHelper.Fail("KeepAlive PageFunction did not save the contents of an element that does not implement serialization to Journal (Journal); i.e. the keepalive pagefunction was not kept alive");
            } else {
                NavigationHelper.Output("Successfully preserved value in a non-journal-serialized control");
            }


            if (parentpf.CreateTime != ((DateTime)TestEventsHT["ParentPFOriginalCreate"]))
            {
                NavigationHelper.Fail ("ParentPF create time is different, which shouldn't happen for KEEPALIVE!");
                NavigationHelper.Output ("Expected: " + ((DateTime)TestEventsHT["ParentPFOriginalCreate"]).ToString()
                    + " Actual: " + parentpf.CreateTime.ToString());
            }
            
            if (!JournalEntry.GetKeepAlive(parentpf)) {
                NavigationHelper.Fail("Parent's KeepAlive property is not as expected");
            }
            
            NavigationHelper.Output ("ParentPF keepalive state is correct so far (when navigating to child)");
            NavigationHelper.Output ("Finishing child pagefunction to test the keepalive parent pf's state");
            pfchild.RequestFinish ();
        }

        private void Verify_KeepAlive2 ()
        {
            ParentBoolPF pf = MainNavWindow.Content as ParentBoolPF;

            if (pf == null)
            {
                NavigationHelper.Fail ("Incorrect pf was displayed in the app's window. Currently displaying content is: " + MainNavWindow.Content.GetType ().ToString ());
            }
            else
            {
                NavigationHelper.Output ("Correct pf (keepalive parent) was displayed");
            }

            if (pf.tt.Text == null || pf.tt.Text != "KeepAlivePageFunction") {
                NavigationHelper.Fail("KeepAlive PageFunction did not save the contents of an element that does not implement serialization to Journal (Journal); i.e. the keepalive pagefunction was not kept alive");
            } else {
                NavigationHelper.Output("Successfully preserved value in a non-serialized-to-journal control");
            }
            
            if (pf.CreateTime != ((DateTime)TestEventsHT["ParentPFOriginalCreate"]))
            {
                NavigationHelper.Fail ("Creation time of the parent was not equal the original creation time, signifying that it was unloaded and recreated when the child pf finished." 
                    + "\n This is incorrect for keepalive pfs"  
                    + "\nOriginal creation: " 
                    + ((DateTime)TestEventsHT["ParentPFOriginalCreate"]).ToString () 
                    + "\nSubsequent creation: " + pf.CreateTime.ToString ());
            }
            else
            {
                NavigationHelper.Pass ("Creation time of the keepalive parent was NOT greater than original creation time, signifying that it was NOT unloaded and recreated when the child pf finished (it just stayed around)." 
                    + "\nOriginal creation: " 
                    + ((DateTime)TestEventsHT["ParentPFOriginalCreate"]).ToString () 
                    + "\nSubsequent creation: " 
                    + pf.CreateTime.ToString ());
            }
        }
        #endregion

    }
}
