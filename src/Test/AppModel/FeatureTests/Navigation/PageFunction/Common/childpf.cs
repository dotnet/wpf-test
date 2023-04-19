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
        private void ChildPFDisplayTest ()
        {
            NavigationHelper.CreateLog("NavigateToPFChild");
            NavigationHelper.Output("Basic test of navigation to a child pagefunction");
            Application.Current.LoadCompleted += new LoadCompletedEventHandler (Load_ChildPFDisplayTest);
            ParentBoolPF pfParent = new ParentBoolPF ();
            if (pfParent == null)
            {
                NavigationHelper.Fail ("Could not create parent pf");
            }
            MainNavWindow.Navigate (pfParent);
        }

        private void Load_ChildPFDisplayTest (object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output ("Navigation Initiator property is: " + e.IsNavigationInitiator);
            if (e.Navigator is NavigationWindow)
            {
                stage++;
                switch (stage)
                {
                    case 1:
                        NavigationHelper.Output ("Now navigating to a child pagefunction.");
                        ChildPF_Frame_Int pfchild = new ChildPF_Frame_Int ();
                        if (pfchild == null)
                        {
                            NavigationHelper.Fail ("child pf is null");
                            return;
                        }
                        MainNavWindow.Navigate (pfchild);
                        break;
                    case 2:
                        PostVerificationItem (new VerificationDelegate (Verify_ChildPFDisplayTest));
                        break;

                    default:
                        break;
                }
            }
            else
            {
                NavigationHelper.Output ("Sender is: " + e.Navigator.GetType().ToString());
            }
        }

        private void Verify_ChildPFDisplayTest ()
        {
            ChildPF_Frame_Int pfchild = MainNavWindow.Content as ChildPF_Frame_Int;
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
                NavigationHelper.Pass ("Child pagefunction display verification succeeded");
            }
        }

    #region parent state on navigation to child test
        private void Test_ParentStateOnChildNavigation ()
        {
            Application.Current.LoadCompleted += new LoadCompletedEventHandler (Load_ParentStateOnChildNavigation);

            ParentBoolPF pfParent = new ParentBoolPF ();

            if (pfParent == null)
            {
                NavigationHelper.Fail ("Could not create parent pf");
            }

            // We want to compare the creation times for the parent b/c the parent
            // should be torn down upon navigation.
            TestEventsHT["ParentPFOriginalCreate"] = pfParent.CreateTime;
            MainNavWindow.Navigate (pfParent);
        }

        private void Load_ParentStateOnChildNavigation (object sender, NavigationEventArgs e)
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
                        } else {
                            NavigationHelper.Fail("Could not find the parent pf after first navigation");
                        }
                        break;

                    case 2:
                        PostVerificationItem (new VerificationDelegate (Verify_ParentStateOnChildNavigation));
                        break;
                        
                    case 3:
                        PostVerificationItem (new VerificationDelegate (Verify_ParentStateOnChildNavigation2));
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

        private void Verify_ParentStateOnChildNavigation ()
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
                NavigationHelper.Output ("Child pagefunction display verification succeeded");
            }

            ParentBoolPF pfParent = Application.Current.Properties["ParentPF"] as ParentBoolPF;
            if (pfParent != null)
            {
                NavigationHelper.Output ("Parent pf was not null, which is expected");
                try
                {
                    bool fIsActive = pfParent.IsActive; 
                    NavigationHelper.Output ("IsActive is: " + fIsActive.ToString());
                }
                catch (ObjectDisposedException e_objdisp)
                {
                    NavigationHelper.Output ("Object disposed: " + e_objdisp.Message);
                }
                catch (Exception e_er)
                {
                    NavigationHelper.Output (e_er.GetType ().ToString () + ": " + e_er.Message);
                }
            }

//          if (Bag.GetJournalCount(MainNavWindow) == 1)
//          {
//              NavigationHelper.Output("Correct number of items in journal");
//          }

            NavigationHelper.Output ("Finishing child pagefunction to test the parent pf's state");
            pfchild.RequestFinish ();
        }

        private void Verify_ParentStateOnChildNavigation2 ()
        {
            ParentBoolPF pf = MainNavWindow.Content as ParentBoolPF;
            if (pf == null)
            {
                NavigationHelper.Fail ("Incorrect pf was displayed in the app's window. Currently displaying content is: "
                    + MainNavWindow.Content.GetType().ToString());
            }
            else
            {
                NavigationHelper.Output ("Correct pf (parent) was displayed");
            }
            if (pf.CreateTime > ((DateTime)TestEventsHT["ParentPFOriginalCreate"]))
            {
                NavigationHelper.Pass ("creation time of the parent was greater than the original creation time, signifying that it was unloaded and recreated when the child pf finished." + "\nOriginal creation: " + ((DateTime)TestEventsHT["ParentPFOriginalCreate"]).ToString () + "\nSubsequent creation: " + pf.CreateTime.ToString ());
            }
            else
            {
                NavigationHelper.Fail ("Creation time of the parent was NOT greater than original creation time, signifying that it was NOT unloaded and recreated when the child pf finished (it just stayed around)." 
                    + "\nOriginal creation: " + ((DateTime)TestEventsHT["ParentPFOriginalCreate"]).ToString () 
                    + "\nSubsequent creation: " + pf.CreateTime.ToString ());
            }
        }


    #endregion
    }
}
