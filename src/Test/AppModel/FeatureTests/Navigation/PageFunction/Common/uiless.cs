// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
*************************************************************************************
*                                                                                   *
*  Title: UILESS PageFunction Tests                                                 *
*                                                                                   *
*  Description:                                                                     *
*       Uiless PageFunction Tests :                                                 *
*           - UIless PageFunction as StartupUri                                     *
*           - UILess PageFunction Journal Test                                      *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   *
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
        #region UILess PF 

        /************************************************************\
        *                                                            *
        *                 UILess PageFunction Tests                  *
        *                                                            *
        \************************************************************/






        
        // Tests:
        //  1. UILess PageFunction
        //  2. Journaling of UILess PageFunction
        //  3. UILess PageFunction navigating to many children pfs
        
        private void Test_UILessBasic ()
        {
            NavigationHelper.CreateLog("Basic UILess PageFunction test");
            NavigationHelper.Output("Basic test for UILess PageFunction (navigates in its start method)");

            UILessParentPF pfParent = new UILessParentPF ();

            if (pfParent == null)
            {
                NavigationHelper.Fail ("Could not create uiless parent pf");
                return;
            }

            Application.Current.LoadCompleted += new LoadCompletedEventHandler(Load_UILessBasic);
            MainNavWindow.Navigate (pfParent);
        }

        private void Load_UILessBasic (object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output ("Navigation Initiator property is: " + e.IsNavigationInitiator);
            if (e.Navigator is NavigationWindow)
            {
                if (MainNavWindow.Content is UILessParentPF)
                    return;
                stage++;
                switch (stage)
                {
                    case 1:
                        NavigationHelper.Output ("Now verifying the UILess pagefunctions start navigation.");
                        PostVerificationItem (new VerificationDelegate (Verify_UILessBasic));
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

        private void Verify_UILessBasic ()
        {
            if (!TestEventsHT.ContainsKey ("Numtimes_startcalled*uilessparentPF"))
            {
                NavigationHelper.Fail ("Start was not called on the uiless parent pf.");
                return;
            }

            if (((int)TestEventsHT["Numtimes_startcalled*uilessparentPF"]) != 1)
            {
                NavigationHelper.Fail ("Start method was called incorrect number of times on the uiless pf: " + ((int)TestEventsHT["Numtimes_startcalled*uilessparentPF"]) + "\nExpected; 1");
                return;
            }

            if (!(MainNavWindow.Content is ChildPF_Frame_Int))
            {
                NavigationHelper.Fail ("Incorrect pf displayed from uiless pf." + "\nExpected: ChildPF_Frame_Int" + "\nActual: " + MainNavWindow.Content.GetType ().ToString ());
                return;
            }
            else
            {
                NavigationHelper.Output ("Correct pf navigated to from uiless pf");
            }

//            if (Bag.GetJournalCount (MainNavWindow) != 0)
//            {
//                NavigationHelper.Fail ("Incorrect number of items in journal: " + Bag.GetJournalCount (MainNavWindow));
//                return;
//            }
//            else
//            {
//                NavigationHelper.Output ("Correct number of items in journal");
//            }
            
            if (!((ChildPF_Frame_Int)MainNavWindow.Content).CheckState ())
            {
                NavigationHelper.Fail ("Incorrect state of child pf detected");
            }
            else
            {
                NavigationHelper.Output ("Correct state of child pf");
            }
            
            if (!Bag.CheckNavigatorBackFwd(MainNavWindow,false,false) )
            {
                NavigationHelper.Fail ("Incorrect state of CanGoBack/CanGoForward");
            }
            else
            {
                NavigationHelper.Output ("Correct state of CanGoBack/CanGoForward");
            }
                
            NavigationHelper.Pass ("UILess pagefunction basic test finished");
        }


        /*******************************************\
        \*******************************************/
        
        private void Test_UILessJournalFocus ()
        {
            NavigationHelper.CreateLog("UI-less PageFunction journaling test");
            NavigationHelper.Output("Journal test for UILess PageFunction");

            ParentBoolPF pfParent = new ParentBoolPF ();

            if (pfParent == null)
            {
                NavigationHelper.Fail ("Could not create parent pf");
                return;
            }

            Application.Current.LoadCompleted += new LoadCompletedEventHandler(Load_UILessJournalFocus);
            MainNavWindow.Navigate (pfParent);        
        }

        private void Load_UILessJournalFocus (object sender, NavigationEventArgs e)
        {
            if (e.Navigator is NavigationWindow)
            {   
                // don't care about loadcompleted for uiless pf
                if (MainNavWindow.Content is UILessParentPF) 
                {                
                    if (stage == 1 || stage == 2 || stage == 3) 
                    {
                        NavigationHelper.Output("UIless PF loaded at right time: " + stage);
                        if (stage == 2)
                        {
                            NavigationHelper.Output("Checking return value of UILess PageFunction's child");

                            int iret = 
                                ((UILessParentPF)MainNavWindow.Content).IntRetVal;
                            if (iret != DEFAULT_CHILDPF_FRAME_INT_RETVAL)
                            {
                                NavigationHelper.Fail("Return Value validation for UILess PageFunction failed. Expect Int retval: " 
                                + DEFAULT_CHILDPF_FRAME_INT_RETVAL 
                                + " Actual: " + iret);
                                return;
                            }
                        }
                        
                    } else {
                        NavigationHelper.Fail("LoadCompleted of UILess PF at wrong time : " + stage);
                    }
                    return;
                }
                
                stage++;
                
                switch (stage)
                {
                    case 1:
                        NavigationHelper.Output ("Now verifying the state of parent pf and initiating navigation to UILess Pagefunction.");
                        PostVerificationItem (new VerificationDelegate (Verify_UILessJournalFocus));
                        break;
                        
                    case 2:
                        
                        //COMMENT: 
                        //
                        //
                        //
                        NavigationHelper.Output(
                        "Checking the Journal after navigation to a UILess PageFunction");                        
                        PostVerificationItem (new VerificationDelegate (Verify_UILessJournalFocus));
                        break;
                        
                    case 3:
                        NavigationHelper.Output("Checking the Journal after the UILess PageFunction navigates to another child");                        
                        PostVerificationItem (new VerificationDelegate (Verify_UILessJournalFocus));
                        break;
                        
                    case 4:
                        NavigationHelper.Output("Checking Journal after navigating back");
                        PostVerificationItem (new VerificationDelegate (Verify_UILessJournalFocus)); 
                        break;
                        
                    case 5:
                        NavigationHelper.Output("Checking Journal after navigating forward");
                        PostVerificationItem (new VerificationDelegate (Verify_UILessJournalFocus)); 
                        break;

                    default:
                        NavigationHelper.Fail("Should not get here");
                        break;
                }
            }
            else
            {
                NavigationHelper.Output ("Sender is: " + e.Navigator.GetType ().ToString ());
            }
        }
        
        private void Verify_UILessJournalFocus() {
            switch (stage) {
                case 1:
                    ParentBoolPF pf1 = MainNavWindow.Content as ParentBoolPF;
                    
                    if (pf1 == null || !pf1.CheckState())
                    {
                        NavigationHelper.Fail ("Parent pf state incorrect: " + MainNavWindow.Content.GetType ().ToString ());
                        return;
                    }

                    UILessParentPF pfuilesschild = new UILessParentPF();
                    MainNavWindow.Navigate(pfuilesschild);
                    break;
                case 2:
                    if (!Bag.CheckNavigatorBackFwd(MainNavWindow,true,false)) 
                    {
                        NavigationHelper.Fail("Navigator back/fwd not as expected with UILess PF");
                        throw new ApplicationException();
                    }
                    
                    ChildPF_Frame_Int pf2 = MainNavWindow.Content as ChildPF_Frame_Int;
                    
                    if (pf2 == null || !pf2.CheckState())
                    {
                        NavigationHelper.Fail ("Child pf state incorrect: " + MainNavWindow.Content.GetType ().ToString ());
                        return;
                    }

                    NavigationHelper.Output("Finishing child pf of UILess PF");
                    pf2.RequestFinish();
                    break;
                    
                case 3:
                    if (!Bag.CheckNavigatorBackFwd(MainNavWindow,true,false)) 
                    {
                        NavigationHelper.Fail("Navigator back/fwd not as expected with UILess PF");                        
                        return;
                    }

                    ChildPF_String pf3 = MainNavWindow.Content as ChildPF_String;
                    if (pf3 == null || !pf3.CheckState())
                    {
                        NavigationHelper.Fail ("Child pf state incorrect: " + MainNavWindow.Content.GetType ().ToString ());
                        return;
                    }
                    
//                    if (!Bag.CheckJournalCount(MainNavWindow,1)) 
//                    {
//                        NavigationHelper.Fail("Incorrect number of items in Journal");
//                    }

                    NavigationHelper.Output("Now navigating BACK to make sure UILess Pagefunction was not journaled");
                    MainNavWindow.GoBack();                    
                    break;
                    
                case 4:
                    if (!Bag.CheckNavigatorBackFwd(MainNavWindow,false,true)) 
                    {
                        NavigationHelper.Fail("Navigator back/fwd not as expected with UILess PF in the journal");                        
                        return;
                    }

//                    if (!Bag.CheckJournalCount(MainNavWindow,2)) 
//                    {
//                        NavigationHelper.Fail("Incorrect number of items in Journal");
//                    }
                    
                    
                    ParentBoolPF pf4 = MainNavWindow.Content as ParentBoolPF;
                    
                    if (pf4 == null || !pf4.CheckState())
                    {
                        NavigationHelper.Fail ("Parent Pf state incorrect: " + MainNavWindow.Content.GetType ().ToString ());
                        return;
                    }
                    
                    NavigationHelper.Output("Now navigating FORWARD once more to verify UILess Pagefunction was not journaled");
                    MainNavWindow.GoForward();
                    break;
                    
                case 5:                    
                    if (!Bag.CheckNavigatorBackFwd(MainNavWindow,true,false)) 
                    {
                        NavigationHelper.Fail("Navigator back/fwd not as expected");                        
                        return;
                    }

//                    if (!Bag.CheckJournalCount(MainNavWindow,2)) 
//                    {
//                        NavigationHelper.Fail("Incorrect number of items in Journal");
//                    }
                    
                    
                    ChildPF_String pf5 = MainNavWindow.Content as ChildPF_String;
                    
                    if (pf5 == null || !pf5.CheckState())
                    {
                        NavigationHelper.Fail ("Child pf state incorrect: " + MainNavWindow.Content.GetType ().ToString ());
                        return;
                    }
                    
                    NavigationHelper.Pass("Finished test to check that UILess Pagefunction was not journaled");
                    break;
                    
                default:
                    NavigationHelper.Fail("Should not get here");
                    break;
            }
        }
    #endregion

    }
}
