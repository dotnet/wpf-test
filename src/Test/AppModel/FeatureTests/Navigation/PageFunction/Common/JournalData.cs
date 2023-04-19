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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class PageFunctionTestApp 
    {

           /*****************************************\
           *                                         *
           *         JOURNALDATA TESTCASES           *
           *                                         *
           \*****************************************/       
           
        #region VerifySaveCalled
        //verify save is called when you navigate to child
        private void VerifySaveCalledTest()
        {            
            Description = "Verify IJnlDat.Save is called when you navigate to child";
            Application.Current.Properties[PageFunctionTestApp.VPFSAVECALLED] = (int) 0;
            Application.Current.Properties[PageFunctionTestApp.VPFLOADCALLED] = (int)0;
            Application.Current.Properties[PageFunctionTestApp.SAPPSEQ] = "";

            Application.Current.LoadCompleted += new LoadCompletedEventHandler(OnPFLoadComplete_SaveTest);
            
            NavigationHelper.Output("Navigating to a PageFunction that implements IJnlDat");
            
            MainNavWindow.Navigate(new PersistedRefPageFuncObj(DateTime.Now));

        }

        private void OnPFLoadComplete_SaveTest(object sender, NavigationEventArgs e) 
        {
            switch (++stage) 
            {
                case 1:
                    if ((int)(Application.Current.Properties[PageFunctionTestApp.VPFSAVECALLED]) != 0) 
                    {
                        NavigationHelper.Fail("IJnlDat.Save called even when nothing should have been persisted"); 
                    }
                    else if ((int)(Application.Current.Properties[PageFunctionTestApp.VPFLOADCALLED]) != 0) 
                    {
                        NavigationHelper.Fail("IJnlDat.Load called even when nothing should have been persisted");
                    } 
                    else 
                    {
                        NavigationHelper.Output("Load and Save have not been called on the pagefunction at this point as expected.");
                    }
                    
                    PostVerificationItem (
                        new VerificationDelegate (Verify_PFSaveTest));
                                        
                    break;

                case 2:
                    PostVerificationItem (
                        new VerificationDelegate (Verify_PFSaveTest));
                    break;
                default:
                    NavigationHelper.Fail("Should never get here");
                    break;
            }
        }
        
        private void Verify_PFSaveTest () 
        {
            switch (stage) {
                case 1:
                    PersistedRefPageFuncObj pf1 = MainNavWindow.Content as PersistedRefPageFuncObj;

                    if (pf1 == null || !pf1.CheckState())
                    {
                        NavigationHelper.Fail ("Parent pf state incorrect: " + MainNavWindow.Content.GetType ().ToString ());
                        return;
                    }

                    NavigationHelper.Output("Navigating to a child pf");
                    pf1.NavChild(MainNavWindow , false);
                    break;
                case 2:
                    if ((int)(Application.Current.Properties[PageFunctionTestApp.VPFSAVECALLED]) != 1) 
                    {
                        NavigationHelper.Fail("IJnlDat.Save was not called or called incorrect number of times even when the pf should have been persisted: " + 
                            (int)(Application.Current.Properties[PageFunctionTestApp.VPFSAVECALLED]));                     
                    }
                    else if ((int)(Application.Current.Properties[PageFunctionTestApp.VPFLOADCALLED]) != 0) 
                    {
                        NavigationHelper.Fail("IJnlDat.Load called even when the pf has been serialized and is no longer in memory");
                    } 
                    else 
                    {
                        NavigationHelper.Output("Save and Load were called correct number of times");
                        PersistedValPageFuncBool pf2 = MainNavWindow.Content as PersistedValPageFuncBool;

                        if (pf2 == null || !pf2.CheckState())
                        {
                            NavigationHelper.Fail ("Parent pf state incorrect: " + MainNavWindow.Content.GetType ().ToString ());
                            return;
                        }
                        else 
                        {
                            NavigationHelper.Pass("Child PF is loaded correctly");
                        }
                    }
                    break;

                default:
                    NavigationHelper.Fail("Should not get here");
                    break;
            }

        }
        
        #endregion

        #if false
        #region VerifyLoadCalled
        //verify load is called when you finish child pf
        private void VerifyLoadCalledTest()
        {
            //StartupPage="object://PageFunctionPersistencetests/PageFunction.Tests.Persistence.PersistedVoidPF";
            this.LoadCompleted += new LoadCompletedEventHandler(OnPFLoadCompleted_LoadTest);
            NavigationWindow nw = new NavigationWindow();
            nw.Navigate(new PersistedVoidPF(_creationtime));
            nw.Visible = true;
        }

        private void OnPFLoadCompleted_LoadTest(object sender, NavigationEventArgs e) 
        {
            switch (_stage++) 
            {
                case 0:
                    //first (parent) PF loaded
                    if ((bool)(this.Properties[PageFunctionTestApp.VPFSAVECALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Save called even when nothing should have been persisted");                        
                    } 
                    else if ((bool)(this.Properties[PageFunctionTestApp.VPFLOADCALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Load called even when nothing should have been persisted");
                    } 
                    else 
                    {
                        NavigationHelper.Fail(("Load and Save have not been called on the pagefunction at this point as expected.");
                    }
                    _pendingNav = "ps://unp_childpf";
                    PostTestItem(new TestStep(NavPf));
                    break;
                case 1:
                    // navigated to unpersisted child pf
                    if (!(bool)(this.Properties[PageFunctionTestApp.VPFSAVECALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Save was not called even when the pf should have been persisted");                     
                    } 
                    else if ((bool)(this.Properties[PageFunctionTestApp.VPFLOADCALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Load called even when the pf has been serialized and is no longer in memory");
                    } 
                    else 
                    {
                        Properties[PageFunctionTestApp.VPFSAVECALLED] = false;
                        if (MainAppWindow.Content is UnPersistedPF) 
                        {
                            DockPanel dp = ((UnPersistedPF)MainAppWindow.Content).Root.RootElement.FindElement("VoidPFPageRoot") as DockPanel;
                            if (dp !=null) 
                            {
                                NavigationHelper.Fail(("Save was called on the PF upon navigating away");
                            } 
                            else 
                            {
                                FAIL("Could not find root DockPanel within the child pf");
                            }
                        } 
                        else 
                        {
                            NavigationHelper.Fail("Wrong PF is loaded");
                        }
                    }
                    _pendingNav = "ps://done";
                    PostTestItem(new TestStep(NavPf));
                    break;
                case 2:
                    // child pagefunction has finished
                    string [] sequence = (string[])(Properties[PageFunctionTestApp.APPSEQ]);
                    if ((sequence[0] == PageFunctionTestApp.APPSEQ_VPFDEFCTOR) && (sequence[1] == PageFunctionTestApp.APPSEQ_VPFLOAD))  
                    {
                        NavigationHelper.Fail(("Correct sequence of ctor -- load calls:" 
                            + "\nExpected: " + PageFunctionTestApp.APPSEQ_VPFDEFCTOR + " " + PageFunctionTestApp.APPSEQ_VPFLOAD
                            + "\nActual: "   + sequence[0] + " " + sequence[1]);
                    } 
                    else 
                    {
                        NavigationHelper.Fail("Incorrect sequence of ctor -- load calls:" 
                            + "\nExpected: " + PageFunctionTestApp.APPSEQ_VPFDEFCTOR + " " + PageFunctionTestApp.APPSEQ_VPFLOAD
                            + "\nActual: "   + sequence[0] + " " + sequence[1]);
                    }
                    if ((bool)Properties[PageFunctionTestApp.VPFSAVECALLED]) 
                    {
                        NavigationHelper.Fail("Save was called on the parent pf when it was not expected");
                    }
                    if (!(bool)Properties[PageFunctionTestApp.VPFLOADCALLED]) 
                    {
                        NavigationHelper.Fail("Load was not called on the parent pf when it was expected");
                    }

                    if (MainAppWindow.Content is PersistedVoidPF) 
                    {
                        PersistedVoidPF _pf = MainAppWindow.Content as PersistedVoidPF;
                        //_pf.CheckPF(PageFunctionTestApp.SAVEDINT,_creationtime);
                        if (_pf.CheckPF(PageFunctionTestApp.SAVEDINT,_creationtime)) 
                        {
                            NavigationHelper.Output("PageFunction's load was called upon finish of childpf and parent was rehydrated successfully");
                        } 
                        else 
                        {
                            NavigationHelper.Fail("PageFunction was not rehydrated successfully");
                        }

                    } 
                    else 
                    {
                        NavigationHelper.Fail("Wrong PF is loaded");
                    }
                                                                                            {
                    }
                    break;
            }
        }       
        #endregion

        #region VerifyAppBackCallsLoad
        // check that load is called on a persisted pf when you call GoBack();
        private void VerifyLoadCalledOnBackTest()
        {
            //StartupPage="object://PageFunctionPersistencetests/PageFunction.Tests.Persistence.PersistedVoidPF";
            this.LoadCompleted += new LoadCompletedEventHandler(OnPFLoadCompleted_BackTest);
            NavigationWindow nw = new NavigationWindow();
            nw.Navigate(new PersistedVoidPF(_creationtime));
            nw.Visible = true;
        }

        private void OnPFLoadCompleted_BackTest(object sender, NavigationEventArgs e) 
        {
            switch (_stage++) 
            {
                case 0:
                    //first (parent) PF loaded
                    if ((bool)(this.Properties[PageFunctionTestApp.VPFSAVECALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Save called even when nothing should have been persisted");                        
                    } 
                    else if ((bool)(this.Properties[PageFunctionTestApp.VPFLOADCALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Load called even when nothing should have been persisted");
                    } 
                    else 
                    {
                        NavigationHelper.Fail(("Load and Save have not been called on the pagefunction at this point as expected.");
                    }
                    _pendingNav = "ps://unp_childpf";
                    PostTestItem(new TestStep(NavPf));
                    break;
                case 1:
                    // navigated to unpersisted child pf
                    if (!(bool)(this.Properties[PageFunctionTestApp.VPFSAVECALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Save was not called even when the pf should have been persisted");                     
                    } 
                    else if ((bool)(this.Properties[PageFunctionTestApp.VPFLOADCALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Load called even when the pf has been serialized and is no longer in memory");
                    } 
                    else 
                    {
                        Properties[PageFunctionTestApp.VPFSAVECALLED] = false;
                        if (MainAppWindow.Content is UnPersistedPF) 
                        {
                            DockPanel dp = ((UnPersistedPF)MainAppWindow.Content).Root.RootElement.FindElement("VoidPFPageRoot") as DockPanel;
                            if (dp !=null) 
                            {
                                NavigationHelper.Fail(("Save was called on the PF upon navigating away");
                            } 
                            else 
                            {
                                FAIL("Could not find root DockPanel within the child pf");
                            }
                        } 
                        else 
                        {
                            NavigationHelper.Fail("Wrong PF is loaded");
                        }
                    }
                    PostTestItem(new TestStep(MainWinNavBack));
                    break;
                case 2:
                    // Finished going back
                    string [] sequence = (string[])(Properties[PageFunctionTestApp.APPSEQ]);
                    if ((sequence[0] == PageFunctionTestApp.APPSEQ_VPFDEFCTOR) && (sequence[1] == PageFunctionTestApp.APPSEQ_VPFLOAD))  
                    {
                        NavigationHelper.Fail(("Correct sequence of ctor -- load calls:" 
                            + "\nExpected: " + PageFunctionTestApp.APPSEQ_VPFDEFCTOR + " " + PageFunctionTestApp.APPSEQ_VPFLOAD
                            + "\nActual: "   + sequence[0] + " " + sequence[1]);
                    } 
                    else 
                    {
                        NavigationHelper.Fail("Incorrect sequence of ctor -- load calls:" 
                            + "\nExpected: " + PageFunctionTestApp.APPSEQ_VPFDEFCTOR + " " + PageFunctionTestApp.APPSEQ_VPFLOAD
                            + "\nActual: "   + sequence[0] + " " + sequence[1]);
                    }
                    if ((bool)Properties[PageFunctionTestApp.VPFSAVECALLED]) 
                    {
                        NavigationHelper.Fail("Save was called on the parent pf when it was not expected");
                    }
                    if (!(bool)Properties[PageFunctionTestApp.VPFLOADCALLED]) 
                    {
                        NavigationHelper.Fail("Load was not called on the parent pf when it was expected");
                    }

                    if (MainAppWindow.Content is PersistedVoidPF) 
                    {
                        PersistedVoidPF _pf = MainAppWindow.Content as PersistedVoidPF;
                        if (_pf.CheckPF(PageFunctionTestApp.SAVEDINT,_creationtime)) 
                        {
                            NavigationHelper.Output("PageFunction's load was called upon GoBack and pf was rehydrated successfully");
                        } 
                        else 
                        {
                            NavigationHelper.Fail("PageFunction was not rehydrated successfully");
                        }
                    } 
                    else 
                    {
                        NavigationHelper.Fail("Wrong PF is loaded");
                    }
                    break;
            }
        }       
        #endregion

        #region VerifyAppFwdCallsSave
        // check that save is called on a persisted pf when you call GoForward();
        private void VerifyAppFwdCallsSave()
        {
            //StartupPage="object://PageFunctionPersistencetests/PageFunction.Tests.Persistence.PersistedVoidPF";
            this.LoadCompleted += new LoadCompletedEventHandler(OnPFLoadCmp_AppFwdCallsSave);
            NavigationWindow nw = new NavigationWindow();
            nw.Navigate(new PersistedVoidPF(_creationtime));
            nw.Visible = true;
        }

        private void OnPFLoadCmp_AppFwdCallsSave(object sender, NavigationEventArgs e) 
        {
            switch (_stage++) 
            {
                case 0:
                    //first (parent) PF loaded
                    if ((bool)(this.Properties[PageFunctionTestApp.VPFSAVECALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Save called even when nothing should have been persisted");                        
                    } 
                    else if ((bool)(this.Properties[PageFunctionTestApp.VPFLOADCALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Load called even when nothing should have been persisted");
                    } 
                    else 
                    {
                        NavigationHelper.Fail(("Load and Save have not been called on the pagefunction at this point as expected.");
                    }
                    _pendingNav = "ps://unp_childpf";
                    PostTestItem(new TestStep(NavPf));
                    break;
                case 1:
                    // navigated to unpersisted child pf
                    if (!(bool)(this.Properties[PageFunctionTestApp.VPFSAVECALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Save was not called even when the pf should have been persisted");                     
                    } 
                    else if ((bool)(this.Properties[PageFunctionTestApp.VPFLOADCALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Load called even when the pf has been serialized and is no longer in memory");
                    } 
                    Properties[PageFunctionTestApp.VPFSAVECALLED] = false;
                        
                    if (!(MainAppWindow.Content is UnPersistedPF)) 
                    {
                        NavigationHelper.Fail("Wrong PF is loaded. Expected: UnPersistedPF" 
                            + " Actual: " + MainAppWindow.Content.GetType().ToString());
                    }
                                                
                    PostTestItem(new TestStep(MainWinNavBack));
                    break;
                case 2:
                    // Finished going back
                    if (!(bool)Properties[PageFunctionTestApp.VPFLOADCALLED]) 
                    {
                        NavigationHelper.Fail("Load was not called on the persisted parent when you called GoBack");
                    }
                    if (!(MainAppWindow.Content is PersistedVoidPF)) 
                    {
                        NavigationHelper.Fail("Wrong PF is loaded. Expected: PersistedVoidPF" 
                            + " Actual: " + MainAppWindow.Content.GetType().ToString());
                    }
                    
                    Properties[PageFunctionTestApp.VPFLOADCALLED] = false;
                    PostTestItem(new TestStep(MainWinNavFwd));
                    break;

                case 3:
                    string [] sequence = (string[])(Properties[PageFunctionTestApp.APPSEQ]);
                    if ((sequence[0] == PageFunctionTestApp.APPSEQ_VPFDEFCTOR) && (sequence[1] == PageFunctionTestApp.APPSEQ_VPFLOAD) && sequence[2] == PageFunctionTestApp.APPSEQ_VPFSAVE)  
                    {
                        NavigationHelper.Fail(("Correct sequence of ctor -- load/save calls:" 
                            + "\nExpected: " + PageFunctionTestApp.APPSEQ_VPFDEFCTOR + " " + PageFunctionTestApp.APPSEQ_VPFLOAD + " " + PageFunctionTestApp.APPSEQ_VPFSAVE
                            + "\nActual: "   + sequence[0] + " " + sequence[1] + " " + sequence[2]);
                    } 
                    else 
                    {
                        NavigationHelper.Fail("Incorrect sequence of ctor -- load/save calls:" 
                            + "\nExpected: " + PageFunctionTestApp.APPSEQ_VPFDEFCTOR + " " + PageFunctionTestApp.APPSEQ_VPFLOAD + " " + PageFunctionTestApp.APPSEQ_VPFSAVE
                            + "\nActual: "   + sequence[0] + " " + sequence[1] + " " + sequence[2]);
                    }

                    if (!(bool)Properties[PageFunctionTestApp.VPFSAVECALLED]) 
                    {
                        NavigationHelper.Fail("Save was not called on the parent pf when it was expected");
                    }
                    if ((bool)Properties[PageFunctionTestApp.VPFLOADCALLED]) 
                    {
                        NavigationHelper.Fail("Load was called on the parent pf when it was not expected");
                    }

                    if (MainAppWindow.Content is UnPersistedPF) 
                    {
                        DockPanel dp = ((UnPersistedPF)MainAppWindow.Content).Root.RootElement.FindElement("VoidPFPageRoot") as DockPanel;
                        if (dp != null) 
                        {
                            NavigationHelper.Output("PageFunction's save was called upon GoForward and pf was rehydrated successfully");
                        } 
                        else 
                        {
                            NavigationHelper.Fail("Could not find rootelement (dockpanel) of UnPersistedPF");
                        }
                    } 
                    else 
                    {
                        NavigationHelper.Fail("Wrong PF is loaded");
                    }
            
                    break;
            }
        }       
        #endregion

        #region VerifyAppFwdBackCallsLoad
        // check that load is called on a persisted pf when you call GoBack() after loading a chld pf with GoForward();
        // this is diff than when the child is loaded with a navigate call
        private void VerifyAppFwdBackCallsLoad()
        {
            //StartupPage="object://PageFunctionPersistencetests/PageFunction.Tests.Persistence.PersistedVoidPF";
            this.LoadCompleted += new LoadCompletedEventHandler(OnPFLoadCmp_AppFwdBackCallsLoad);
            NavigationWindow nw = new NavigationWindow();
            nw.Navigate(new PersistedVoidPF(_creationtime));
            nw.Visible = true;
        }

        private void OnPFLoadCmp_AppFwdBackCallsLoad(object sender, NavigationEventArgs e) 
        {
            switch (_stage++) 
            {
                case 0:
                    //first (parent) PF loaded
                    if ((bool)(this.Properties[PageFunctionTestApp.VPFSAVECALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Save called even when nothing should have been persisted");                        
                    } 
                    else if ((bool)(this.Properties[PageFunctionTestApp.VPFLOADCALLED])) 
                    {
                        NavigationHelper.Fail("IJnlDat.Load called even when nothing should have been persisted");
                    } 
                    else 
                    {
                        NavigationHelper.Fail(("Load and Save have not been called on the pagefunction at this point as expected.");
                    }
                    _pendingNav = "ps://unp_childpf";
                    PostTestItem(new TestStep(NavPf));
                    break;
                case 1:
                    // navigated to unpersisted child pf
                    if (!(MainAppWindow.Content is UnPersistedPF)) 
                    {
                        NavigationHelper.Fail("Wrong PF is loaded. Expected: UnPersistedPF" 
                            + " Actual: " + MainAppWindow.Content.GetType().ToString());
                    }
                                                
                    PostTestItem(new TestStep(MainWinNavBack));
                    break;
                case 2:
                    // Finished going back
                    if (!(MainAppWindow.Content is PersistedVoidPF)) 
                    {
                        NavigationHelper.Fail("Wrong PF is loaded. Expected: PersistedVoidPF" 
                            + " Actual: " + MainAppWindow.Content.GetType().ToString());
                    }
                    
                    PostTestItem(new TestStep(MainWinNavFwd));
                    break;

                case 3:
                    if (!(MainAppWindow.Content is UnPersistedPF))
                    {
                        NavigationHelper.Fail("Wrong PF is loaded after going forward. Expected: UnPersistedPF" 
                            + " Actual: " + MainAppWindow.Content.GetType().ToString());

                    }
                    PostTestItem(new TestStep(MainWinNavBack));
                    break;
                case 4:
                    string actualseq = (string)(Properties[PageFunctionTestApp.SAPPSEQ]);
                    string expectseq = "AppStart" 
                        + "*" + PageFunctionTestApp.APPSEQ_VPFSAVE 
                        + "*" + PageFunctionTestApp.APPSEQ_VPFDEFCTOR
                        + "*" + PageFunctionTestApp.APPSEQ_VPFLOAD
                        + "*" + PageFunctionTestApp.APPSEQ_VPFSAVE
                        + "*" + PageFunctionTestApp.APPSEQ_VPFDEFCTOR
                        + "*" + PageFunctionTestApp.APPSEQ_VPFLOAD;

                    if (actualseq != expectseq)  
                    {
                        NavigationHelper.Fail("Incorrect sequence of ctor -- load/save calls:" 
                            + "\nExpected: " + expectseq
                            + "\nActual: "   + actualseq);
                    } 
                    else 
                    {
                        NavigationHelper.Fail(("Correct sequence of ctor -- load/save calls:" 
                            + "\nExpected: " + expectseq
                            + "\nActual: "   + actualseq);
                    }

                    if (MainAppWindow.Content is PersistedVoidPF) 
                    {
                        DockPanel dp = ((PersistedVoidPF)MainAppWindow.Content).Root.RootElement.FindElement("VoidPFPageRoot") as DockPanel;
                        if (dp == null) 
                        {
                            NavigationHelper.Fail( "Could not find root element of persisted voidpf");
                        }

                        if (((PersistedVoidPF)MainAppWindow.Content).CheckPF(PageFunctionTestApp.SAVEDINT,_creationtime)) 
                        {
                            NavigationHelper.Output("PageFunction's load was called upon GoForward to child pf and then GoBack. Parent pf was rehydrated successfully");
                        } 
                        else 
                        {
                            NavigationHelper.Fail("Could not find rootelement (dockpanel) of PF");
                        }
                    } 
                    else 
                    {
                        NavigationHelper.Fail("Wrong PF is loaded.\nExpected: PersistedVoidPF.\nActual: " + MainAppWindow.Content.GetType().ToString());
                    }
                    break;
               
            }
        }       
        #endregion

        #region multipersist
        // check that multiple load/save is called for multiple persisted pfs when they navigate to children and back
        private void VerifyMultipersistSaveLoadCalls()
        {
            //StartupPage="object://PageFunctionPersistencetests/PageFunction.Tests.Persistence.PersistedVoidPF";
            this.LoadCompleted += new LoadCompletedEventHandler(OnPFLoadCmp_AppMultiplePersistedPFsSaveLoad);
            NavigationWindow nw = new NavigationWindow();
            nw.Navigate(new PersistedVoidPF(_creationtime));
            nw.Visible = true;
        }

        private void OnPFLoadCmp_AppMultiplePersistedPFsSaveLoad(object sender, NavigationEventArgs e) 
        {
            switch (_stage++) 
            {
                case 0:
                    //first (parent) PF loaded
                    _pendingNav = "ps://childpf";
                    PostTestItem(new TestStep(NavPf));
                    break;
                case 1:
                    // navigated to persisted child pf (string pf)
                    if (!(MainAppWindow.Content is PersistedStringPF)) 
                    {
                        NavigationHelper.Fail("Wrong PF is loaded. Expected: PersistedStringPF" 
                            + " Actual: " + MainAppWindow.Content.GetType().ToString());
                    }
                                                
                    _pendingNav = "ps://unp_childpf";
                    PostTestItem(new TestStep(NavPf));
                    break;
                case 2:
                    // Finished nav to child pf
                    if (!(MainAppWindow.Content is UnPersistedPF)) 
                    {
                        NavigationHelper.Fail("Wrong PF is loaded. Expected: UnPersistedPF" 
                            + " Actual: " + MainAppWindow.Content.GetType().ToString());
                    }
                    //go back 
                    PostTestItem(new TestStep(MainWinNavBack));
                    break;

                case 3:
                    // finished going back 
                    if (!(MainAppWindow.Content is PersistedStringPF))
                    {
                        NavigationHelper.Fail("Wrong PF is loaded after going forward. Expected: PersistedStringPF" 
                            + " Actual: " + MainAppWindow.Content.GetType().ToString());

                    } 
                    else 
                    {
                        if (!((PersistedStringPF)MainAppWindow.Content).CheckPFAfterLoad()) 
                        {
                            NavigationHelper.Fail("IJnlDat.Load for string pf did not do its job");
                        } 
                        else 
                        {
                            NavigationHelper.Fail(("Persisted string pf was rehydrated successfully");
                        }
                    }
                    PostTestItem(new TestStep(MainWinNavBack));
                    break;
                case 4:
                    // finished going back 2nd time
                    string actualseq = (string)(Properties[PageFunctionTestApp.SAPPSEQ]);
                    string expectseq = "AppStart" 
                        + "*" + PageFunctionTestApp.APPSEQ_SPFDEFCTOR
                        + "*" + PageFunctionTestApp.APPSEQ_VPFSAVE
                        + "*" + PageFunctionTestApp.APPSEQ_SPFSAVE
                        + "*" + PageFunctionTestApp.APPSEQ_SPFDEFCTOR
                        + "*" + PageFunctionTestApp.APPSEQ_SPFLOAD
                        + "*" + PageFunctionTestApp.APPSEQ_VPFDEFCTOR
                        + "*" + PageFunctionTestApp.APPSEQ_VPFLOAD
                        + "*" + PageFunctionTestApp.APPSEQ_SPFSAVE;

                    if (actualseq != expectseq)  
                    {
                        NavigationHelper.Fail("Incorrect sequence of ctor -- load/save calls:" 
                            + "\nExpected: " + expectseq
                            + "\nActual: "   + actualseq);
                    } 
                    else 
                    {
                        NavigationHelper.Fail(("Correct sequence of ctor -- load/save calls:" 
                            + "\nExpected: " + expectseq
                            + "\nActual: "   + actualseq);
                    }

                    if (MainAppWindow.Content is PersistedVoidPF) 
                    {
                        DockPanel dp = ((PersistedVoidPF)MainAppWindow.Content).Root.RootElement.FindElement("VoidPFPageRoot") as DockPanel;
                        if (dp == null) 
                        {
                            NavigationHelper.Fail( "Could not find root element of persisted voidpf");
                        }

                        if (((PersistedVoidPF)MainAppWindow.Content).CheckPF(PageFunctionTestApp.SAVEDINT,_creationtime)) 
                        {
                            NavigationHelper.Output("PageFunction's load was called upon GoForward to child pf and then GoBack. Parent pf was rehydrated successfully");
                        } 
                        else 
                        {
                            NavigationHelper.Fail("Could not find rootelement (dockpanel) of PF");
                        }
                    } 
                    else 
                    {
                        NavigationHelper.Fail("Wrong PF is loaded.\nExpected: PersistedVoidPF.\nActual: " + MainAppWindow.Content.GetType().ToString());
                    }
                    break;
                
            }
        }       
        #endregion

        #region navoutcallssave
        // check that navigation away from persisted pf (not to child) calls IJnlDat.Save
        private void VerifyNavOutCallsSave()
        {
            //StartupPage="object://PageFunctionPersistencetests/PageFunction.Tests.Persistence.PersistedVoidPF";
            this.LoadCompleted += new LoadCompletedEventHandler(OnPFLoadCmp_NavOutCallsSave);
            NavigationWindow nw = new NavigationWindow();
            nw.Navigate(new PersistedStringPF());
            nw.Visible = true;
        }

        private void OnPFLoadCmp_NavOutCallsSave(object sender, NavigationEventArgs e) 
        {
            switch (_stage++) 
            {
                case 0:
                    //first (parent) string PF loaded
                    _pendingNav = "ps://navout";
                    PostTestItem(new TestStep(NavPf));
                    break;
                case 1:
                    // navigated out of pagefunction to a page (dockpanel)
                    string actualseq = (string)(Properties[PageFunctionTestApp.SAPPSEQ]);
                    string expectseq = "AppStart" 
                        + "*" + PageFunctionTestApp.APPSEQ_SPFDEFCTOR
                        + "*" + PageFunctionTestApp.APPSEQ_SPFSAVE;

                    if (actualseq != expectseq)  
                    {
                        NavigationHelper.Fail("Incorrect sequence of ctor -- load/save calls:" 
                            + "\nExpected: " + expectseq
                            + "\nActual: "   + actualseq);
                    } 
                    else 
                    {
                        NavigationHelper.Fail(("Correct sequence of ctor -- load/save calls:" 
                            + "\nExpected: " + expectseq
                            + "\nActual: "   + actualseq);
                    }

                    if (!(MainAppWindow.Content is DockPanel)) 
                    {
                        NavigationHelper.Fail("Wrong Content is loaded. Expected: DockPanel" 
                            + " Actual: " + MainAppWindow.Content.GetType().ToString());
                    } 
                    else 
                    {
                        NavigationHelper.Output("Correct content is navigated to, and save was called on the " 
                            + "persistd pf upon navigating to a page from pf");
                    }

                
            }
        }       
        #endregion

        #region navoutandbackcallsload
        // check that navigation away from persisted pf (not to child) and back calls IJnlDat.Save/Load
        // also this testcase uses object: urls
        private void VerifyNavOutAndBackCallsLoad()
        {
            StartupPage="object://PageF.PersistenceTests/PageFunction.Tests.Persistence.PersistedStringPF";
            this.LoadCompleted += new LoadCompletedEventHandler(OnPFLoadCmp_NavOutAndBack);
            
            // we might have to do the following if object: urls go away
            //NavigationWindow nw = new NavigationWindow();
            //nw.Navigate(new PersistedStringPF());
            //nw.Visible = true;
        }

        private void OnPFLoadCmp_NavOutAndBack(object sender, NavigationEventArgs e) 
        {
            switch (_stage++) 
            {
                case 0:
                    //first (parent) string PF loaded
                    _pendingNav = "ps://navout";
                    PostTestItem(new TestStep(NavPf));
                    break;
                case 1:
                    // navigated out of pagefunction to a page (dockpanel)
                    // now navigate back
                    PostTestItem(new TestStep(MainWinNavBack));
                    break;
                case 2:
                    // finished going back 
                    string actualseq = (string)(Properties[PageFunctionTestApp.SAPPSEQ]);
                    string expectseq = "AppStart" 
                        + "*" + PageFunctionTestApp.APPSEQ_SPFDEFCTOR
                        + "*" + PageFunctionTestApp.APPSEQ_SPFSAVE
                        + "*" + PageFunctionTestApp.APPSEQ_SPFDEFCTOR
                        + "*" + PageFunctionTestApp.APPSEQ_SPFLOAD;

                    if (actualseq != expectseq)  
                    {
                        NavigationHelper.Fail("Incorrect sequence of ctor -- load/save calls:" 
                            + "\nExpected: " + expectseq
                            + "\nActual: "   + actualseq);
                    } 
                    else 
                    {
                        NavigationHelper.Fail(("Correct sequence of ctor -- load/save calls:" 
                            + "\nExpected: " + expectseq
                            + "\nActual: "   + actualseq);
                    }

                    if (!(MainAppWindow.Content is PersistedStringPF))
                    {
                        NavigationHelper.Fail("Wrong PF is loaded after going forward. Expected: PersistedStringPF" 
                            + " Actual: " + MainAppWindow.Content.GetType().ToString());
                    } 
                    else 
                    {
                        if (!((PersistedStringPF)MainAppWindow.Content).CheckPFAfterLoad()) 
                        {
                            NavigationHelper.Fail("IJnlDat.Load for string pf did not do its job");
                        } 
                        else 
                        {
                            NavigationHelper.Output("Persisted string pf was rehydrated successfully upon navigating back from non-pf page;"
                                + " Load/Save was called as appropriate");
                        }
                    }
                    break;
            }
        }       
        #endregion

        # region TestSteps
        /// <summary>
        /// requires _pendingNav to be set to whatever ps:// link.
        /// </summary>
        private void NavPf() 
        {
            NavigationHelper.Fail(("Now navigating to a child pf and testing to see if IJnlDat.Save is called on the parent");
            MainAppWindow.Navigate(new BindProductUri(_pendingNav));
        }
        
        private void MainWinNavBack() 
        {
            NavigationHelper.Fail(("Navigating main window of the app back (GoBack)");
            if (!MainAppWindow.GoBack()) 
            {
                NavigationHelper.Fail("Could not navigate main app window back");
            }
        }

        private void MainWinNavFwd() 
        {
            NavigationHelper.Fail(("Navigating main window of the app forward (GoForward)");
            if (!MainAppWindow.GoForward()) 
            {
                NavigationHelper.Fail("Could not navigate main app window forward");
            }
        }

        #endregion
        
        #endif
    }
}
