// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Navigation;
using System.Reflection;
using System.Text ;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace DrtPagefunctionTest
{
    public class DrtPagefunction
    {
        [STAThread]
        public static int Main(string[] args)
        {
            try
            {
                PageFunctionTest pt = new PageFunctionTest();

                pt.Run() ;

                Log("PASSED");

                return 0 ;

            }
            catch( Exception e )
            {
                Log("ERROR: {0}", e.Message);
                Log("ERROR: Exception thrown at {0}", e.StackTrace);
                return 1;
            }
        }

        internal static void Log(String message)
        {
            s_logger.Log(message);
        }

        internal static void Log(string message, params object[] args)
        {
            s_logger.Log(message, args);
        }

        private static DRT.Logger s_logger = new DRT.Logger("DrtPageFunction", @"Microsoft", "PageFunction navigation tests");
    }

//
// PageFunctionTest does the following.
//
// Tests navigating pagefunctions amongst themselves
//
//        0 - Create a NavApp, override OnStartingUp to create a custom navigaiton window.
//
// Test A ( Basic pagefunction invocation, and returning results)
//
//        1- Create PageFunction1 ( has a child element, i.e. has UI)
//            - Navigate to that PageFunction in the Navigation Window.
//        2- Create a Pagefunction2 - of type <string>. ( also has a child element).
//        3- Attach as a listener from the first pagefunction to the second.
//        4- Navigate to the second pagefunction.
//        5- In the second pagefunction - call "OnReturn"
//        6- Check that the first pagefunction is resumed, and he gets the returned object passed in 5.
//           a. Check that NavigationService and the host Window can be access in the Return event handler.
//
// Test B ( Test UI-less Pagefunction invocation, and going back with UI-less)
//          - Subtest: Fragment navigation within a PageFunction
//
//        ( starting at PageFunction1)
//
//        1- Create a UI-less PageFunction, PageFunction3
//        2- Navigate to Pagefunction3
//        3- From the start of PageFunction3, create child pagefunction4 that has UI, call Navigate on it
//        4.1- Sub-test: Do fragment navigation to "#bottomTarget" (it's synchronous), then GoBack, 
//            and verify nw.Source. PageFunction4.Start() should not get called on fragment navigation.
//        4.2- Call NavWindow.GoBack
//        5- From the Navigated event Handler, check that PageFunction1 is the current PageFunction.
//        6- Call NavWindow.GoForward.
//        7- From the Navigated event handler, check that PageFunction4 is the current PageFunction
//        7.1- Complete the fragment navigation sub-test by doing GoForward & GoBack again and checking
//          the journal state.
//        8- From PageFunction4 - call OnReturn().
//        9- From PageFunction3's listener to PageFunction4, call OnReturn
//        10- Check that we're back at PageFunction1.
//
//  Test C Multiple finishes with JournalEntry.KeepAlive=true (1007658)
//
//      State: C_Start
//          Close existing NavWin, make a new NavWin
//          Navigate to PfParent
//      State: C_AtParent0
//          Verify current page
//          PfParent launches PfChild
//      State: C_AtChild
//          Verify current page
//          PfChild launches PfGrandchild
//      State: C_AtGrandchild0
//          Verify current page
//          Finish PfGrandchild, which also finishes PfChild and navigates back to PfParent
//          (new navigation recorded in the journal)
//      State: C_AtParent1
//          Verify current page
//          Verify parent return has been called once
//          Verify that backstack contains PfParent, PfChild, PfGrandchild
//          Call NavWin.GoBack
//      State: C_AtGrandchild1
//          Verify current page
//          Finish PfGrandchild, which also finishes PfChild and navigates back to PfParent
//          (new navigation recorded in the journal; overwrites previous entry for PfParent)
//      State: C_AtParent2
//          Verify current page
//          Verify parent return has been called twice
//          Verify that backstack contains PfParent, PfChild, PfGrandchild
//
//  Test D (piggybacks on C) Odd cases...
//      - Verify that a PF with RemoveFromJournal=true causes another PF on the
//        fwd stack to be removed from the journal as well and stops it from being journaled again
//        (NavigationService._doNotJournalCurrentContent).
//      - Calling a PF without attaching a Return event handler 
//        It should still return normally.
//      - PF in a frame. To make the framework do more work, the PF is not keep-alive and 
//        neither is the parent (PfChildWithFrame). Also, RemoveFromJournal is set on the PF in the frame. 
//        This should not prevent the frame from remembering its content when moving away and back to the 
//        parent page.
//      - PF calling OnReturn() when there is no parent (invoking) page to return to.
//        (The first PF in the journal has nowhere to return to. The omni-evil NW.RemoveBackEntry()
//         can also be a cause.)
//      - New navigation started from a Return event handler on a non-PF parent page. The case is
//        distinct and significant because the Return event is raised much later in the navigation 
//        sequence than for a PF parent.
//
//      State: D_Start
//          Verify PfParent is the current page
//          Navigate to PfChildWithFrame w/o attaching Return event handler
//      State: D_AtPfChildWithFrame
//          Navigate the frame to an instance of PfChild
//      State: D_FrameNavigated
//          Navigate the NavWin to PfGrandchild
//      State: D_AtPfGrandchildAgain
//          NavWin.GoBack
//      State: D_BackToPfChildWithFrame
//          Verify back stack has 4 entries (PfParent, PfChild, PfGrandchild, PfParent) 
//          and fwd stack has 1.
//          Verify the frame's content is PfChild. Make it return. Nothing should happen.
//          Set RemoveFromJournal on PfChildWithFrame 
//          PfChildWithFrame returns
//      State: D_ReturnedToParent
//          Verify back stack has 3 entries and fwd stack is empty.
//          PfParent calls OnReturn()
//      State: D_ReturnedToNowhere
//          Verify the current page is still PfParent and no new navigation is started.
//          Because PfParent has RemoveFromJournal (new default), the journal will be
//          emptied. (PfParent's first instance is at index 0.)
//          Navigate to a Page (class OrdinaryPage)
//      State: D_NavAfterReturnToNowhere
//          Verify back stack is still empty. (PfParent has RemoveFromJournal, so it
//          should not be re-journaled)
//          Start PageFunction1.
//      State: D_Pf1Started:
//          Cause PageFunction1 to return. The return handler, on OrdinaryPage, starts a 
//          new navigation.
//      State: D_NavigatedFromReturnHandler:
//          Current page should be PfChild; backstack should have OrdinaryPage.
//          Make PfChild return. This leaves the state expected by the next test.
//  
//  Test E: Try a bit of recursive PF invocation, do some fragment navigation
//          - Use the same PF classes as the previous tests but with different KeepAlive &
//            RemoveFromJournal flags.
//          - Along the way, test journaling of a DP within the element tree marked with 
//            FrameworkPropertyMetadataOptions.Journal. This is PfParent._TextBox.Text.
//          - Some fragment navigation is done within PfParent, which is journaled by type in 
//            this test, so slightly different code is exercised than in Test B. 
//            (JournalEntryPageFunctionXX.Navigate().)
//          - Finally, return to a non-PF parent (the OrdinaryPage left from the previous test).
//      State: E_Start
//          navWin.Content is OrdinaryPage (from last test). Journal is empty.
//          Navigate to PfParent, KeepAlive=False (, RemoveFromJournal=true - default)
//      State: E_AtParent
//          Navigate to PfChild, RemoveFromJournal=false (set in ctor)
//      State: E_AtChild
//          Try navigating again to the PfParent instance created at E_Start. This should throw
//          InvalidOperationException. (We don't allow this, because there is only a single 
//          ReturnEventSaver, so PageFunction is not "reentrant".)
//          Then Navigate to a _new_ instance of PfParent.
//      State: E_AtNestedParent
//          Return from PfParent
//      State: E_ReturnedToChild
//          Verify the current page is PfChild, and the backstack has 2 entries (OrdinaryPage, PfParent)
//          Return from PfChild
//      State: E_ReturnedToFirstParent
//          Verify the current page is PfParent, and the backstack has 3 entries (OrdinaryPage, 
//          PfParent, PfChild). (PfChild.RemoveFromJournal=false, so a new navigation to its parent
//          is recorded.)
//          Do a fragment navigation to "#fragment" (a named element within PfParent). While doing
//          fragment navigation, the current state is E_FragNavTest, to prevent infinite recursion.
//          Last, navigate to a new Page. 
//      State: E_NavToAnotherPage:
//          The backstack should contain 4 entries. (Only the "exit" entry from the JournalEntryGroup
//          created for PfParent should be considered navigable.)
//      State: E_BackToFirstParentSecondJournalInstance:
//          We should be at PfParent, navWin.Source="#fragment".
//          Do GoBack. Should still be at PfParent, but navWin.Source=null.
//          Return (to a non-PF parent)
//      State: E_ReturnedFromFirstParent
//          The current content should be the OrdinaryPage, and the backstack should be empty.
//
//
//      State: F_Start  
//             Navigated from an instance of MarkupPF1,
//             Check nw.Source and Back/Forward states.
//             Start a navigation with pure fragment.
//
//     State.F_PureFragment
//              Check journal status, Source setting and TextBox setting.
//              Change the value in TextBox in MarkupPF1.     
//              Launch a child PageFunction MarkupPF2.
//     State.State.F_Navigate_MarkupPF2 
//              Check the Back/Forward status,
//              Return from MarkupPF2
//
//     State.F_MarkupPF2_Return:
//              Check the Back/Forward status.
//              Source setting, It should create a differnt instance of MarkupPF1 when it is resumed.
//              Check the return value from MarkupPF2
//              Check how many times the Journalabl DP TextBox's value was set.
//              Navigate back.
//
//     State.F_BackToPurePF1
//              Start a Pure markup navigation for MarkupPF1
//  
//     State.F_Navigate_PureMarkup_PF1
//              Validate the Source setting, TextBox setting.
//              Update the text in TextBox element.
//              Launch child Page MarkupPF2             
//
//     State.F_PF2_Created_From_Pure_MarkupPF1
//  
//              Check the Back/Forward status.
//              verify the nw.Content.
//              Cancel this Page to return to its parent with differnt value.
//
//     State.F_PF2_Cancelled
//  
//              Check the Back/Forward status, 
//              Verify the curent nw.Content, make sure a new instance of MarkupPF1 is created.
//              Check the TextBox setting and How many times the Text value  is changed.
//              Check the return value from MarkupPF2.
//              Start a back navigation.
//               
//     State.F_Navigate_PureMarkup_PF1_GoBack
//              Check Back/Forward status and Go Back
//
//     State.F_Navigate_PureMarkup_GoBack_To_OriginalPage
//              Check Back/Forwas staus and Close Window.
//

     public enum State
    {
        Start,
        NavigatedFirst,
        CalledSecond,
        GotSecond,
        CalledThird,
        CalledFourth,
        FourthStarted, Fourth_FragNavTest, 
        FourthGoBack,
        FirstGoForward,
        FourthEnded,
        ThirdEnded,
        C_Start, C_AtParent0, C_AtChild, C_AtGrandchild0, C_AtParent1, C_AtGrandchild1, C_AtParent2,
        D_Start, D_AtPfChildWithFrame, D_FrameNavigated, D_AtPfGrandchildAgain, D_BackToPfChildWithFrame,
            D_ReturnedToParent, D_ReturnedToNowhere, D_NavAfterReturnToNowhere,
            D_Pf1Started, D_NavigatedFromReturnHandler,
        E_Start, E_AtParent, E_AtChild, E_AtNestedParent, E_ReturnedToChild, E_ReturnedToFirstParent,
            E_FragNavTest, E_NavToAnotherPage, E_BackToFirstParentSecondJournalInstance, 
            E_ReturnedFromFirstParent,

        F_Start, F_PureFragment, 
                 F_Navigate_MarkupPF2, 
                 F_MarkupPF2_Return, 
                 F_BackToPurePF1, 
                 F_Navigate_PureMarkup_PF1, 
                 F_PF2_Created_From_Pure_MarkupPF1,
                 F_PF2_Cancelled, 
                 F_Navigate_PureMarkup_PF1_GoBack, 
                 F_Navigate_PureMarkup_GoBack_To_OriginalPage,
        Done
    }


    internal class PageFunctionTest    : Application
    {
        static internal PageFunctionTest g_pst;

        private MarkupPF1                _savedMarkupPF1;

        internal PageFunctionTest( )
        {
            g_pst = this;
        }


        public State State
        {
            get
            {
                return _state;
            }
            set
            {
                if ( _verbose )
                {
                    DumpState( _state, value ) ;
                }

                _state = value;
            }
        }


        public static PageFunctionTest MyApplication
        {
            get
            {
                return (( PageFunctionTest) Application.Current ) ;
            }
        }

        public bool Verbose
        {
            get
            {
                return _verbose;
            }
        }

        protected  override void OnStartup(StartupEventArgs e)
        {
            for( int i = 0;i < e.Args.Length ;  i++ )
            {
                switch( e.Args[i].ToLower() )
                {
                    case "-v" :
                    case "-verbose":
                    case "/v":
                    case "/verbose":
                        _verbose = true;
                        break;

                    case "-?" :
                    case "/?" :
                        ShowHelp();
                        Shutdown();
                        break;
                }
            }

            this.ShutdownMode = ShutdownMode.OnLastWindowClose;

            NavigationWindow nw = new NavigationWindow();

            nw.Show();

            // Step 1 - create a pf and navigate to it.

            DrtPagefunction.Log("Test A: Basic page function invocation and returning a result");
            this.State = State.NavigatedFirst ;

            PageFunction1 pf = new PageFunction1();
            nw.Navigate( (FrameworkElement) pf ) ;


            // ideally we would just call the 2nd pagefunction directly here.
            // however - we need to await synchronous navigation being implemented
            // The test continues in OnNavigated().

            //
            // 

        }

        internal void Assert(bool condition)
        {
            Assert(condition, null);
        }
        internal void Assert(bool condition, string msg)
        {
            if (!condition)
                throw new ApplicationException("At state "+_state+": "+msg);
        }

        protected override void OnNavigated(NavigationEventArgs e)
        {
            NavigationWindow nw = (NavigationWindow)Application.Current.Windows[0];

            switch (_state)
            {
                case State.NavigatedFirst:
                    {
                        CheckBackForwardState(false, false);
                        // Step 2 - create a 2nd pf

                        PageFunction2 pf2 = new PageFunction2();

                        PageFunction1 pf1 = nw.Content as PageFunction1;

                        pf2.Return += new ReturnEventHandler<string>(pf1.OnPageFunction2Returned);

                        State = State.CalledSecond;
                        nw.Navigate((UIElement)pf2);
                    }
                    break;

                case State.CalledSecond:
                    {
                        CheckBackForwardState(true, false);
                        VerifyBackStack(nw, 1, "After starting PageFunction2");
                    }
                    break;

                case State.GotSecond:
                    {
                        VerifyBackStack(nw, 0, "After PageFunction2 returns (RemoveFromJournal defaults to true)");
                    }
                    break;

                case State.CalledThird:
                    {
                        DrtPagefunction.Log("Test B: UI-less page function. Fragment navigation sub-test");

                        CheckBackForwardState(true, false);

                        PageFunction3 pf3 = nw.Content as PageFunction3;

                        if (pf3 == null)
                            throw new ApplicationException("Did not successfully navigate to PageFunction3");

                        pf3.Next();

                    }
                    break;

                case State.CalledFourth:
                    {
                        CheckBackForwardState(true, false);
                        VerifyBackStack(nw, 1,
                            "After starting PF3 and then PF4 (PF3 is UI-less, so its journal entry is filtered out)");

                        // Do the first part of the fragment navigation sub-test (within a KeepAlive PF)
                        PageFunctionTest.MyApplication.State = State.Fourth_FragNavTest;
                        nw.Navigate(new Uri("#bottomTarget", UriKind.Relative));
                        Assert(nw.Source.ToString() == "#bottomTarget");
                        nw.GoBack();
                        Assert(nw.Source == null);
                        CheckBackForwardState(true, true);
                        // The fragment navigation test continues at state FirstGoForward.

                        // After returning from here, PageFunction4.Start() will be called.
                        // [This order is according to the spec.] It will call navWin.GoBack().
                        // The interruption is needed in order to verify that PageFunction4.Start()
                        // is called exactly once, only when the PF is first navigated to (started). 
                        PageFunctionTest.MyApplication.State = State.CalledFourth;
                    }
                    break;

                case State.Fourth_FragNavTest:
                    // Verification done within PageFunction4.Start() and the FirstGoForward block below.
                    Assert(e.Content is PageFunction4);
                    break;

                case State.FourthGoBack:
                    {
                        CheckBackForwardState(false, true);

                        PageFunction1 pf1 = nw.Content as PageFunction1;
                        if (pf1 == null)
                            throw new ApplicationException("Did not successfully navigate to PageFunction1");

                        State = State.FirstGoForward;
                        nw.GoForward();

                    }
                    break;

                case State.FirstGoForward:
                    {
                        PageFunction4 pf4 = nw.Content as PageFunction4;
                        if (pf4 == null)
                            throw new ApplicationException("Did not successfully navigate to PageFunction4 on GoForward call");

                        // Complete the fragment navigation sub-test started at state CalledFourth.
                        PageFunctionTest.MyApplication.State = State.Fourth_FragNavTest;
                        CheckBackForwardState(true, true/*because of the fragment navigation entry*/);
                        Assert(nw.Source == null);
                        nw.GoForward(); //(Fragment navigation is synchronous.)
                        Assert(nw.Source.ToString() == "#bottomTarget");
                        CheckBackForwardState(true, false);

                        PageFunctionTest.MyApplication.State = State.FirstGoForward;

                        DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
                        timer.Interval = TimeSpan.FromMilliseconds(2000);
                        timer.Tick += delegate(object unused1, EventArgs unused2)
                        {
                            timer.IsEnabled = false;
                            Dispatch(pf4);
                        };
                        timer.Start();
                    }
                    break;

                case State.ThirdEnded:
                    {
                        // Both PF3 and PF4 are RemoveFromJournal and have returned.
                        CheckBackForwardState(false, false);
                    }
                    break;

                // Test C  -----------------------------------------------------------------

                case State.C_Start:
                    {
                        DrtPagefunction.Log("Test C: Multiple finishes with JournalEntry.KeepAlive=true");

                        NavigationWindow newnw = new NavigationWindow();
                        newnw.Show();

                        nw.Close();

                        this.State = State.C_AtParent0;
                        PfParent pf = new PfParent();
                        pf.KeepAlive = true;
                        newnw.Navigate(pf);
                    }
                    break;

                case State.C_AtParent0:
                    {
                        VerifyBackStack(nw, 0, "After loading parent");
                        this.State = State.C_AtChild;
                        PfParent pf = (PfParent)nw.Content;
                        pf.LaunchNext();
                    }
                    break;

                case State.C_AtChild:
                    {
                        VerifyBackStack(nw, 1, "After loading child");
                        this.State = State.C_AtGrandchild0;
                        PfChild pf = (PfChild)nw.Content;
                        pf.LaunchNext();
                    }
                    break;

                case State.C_AtGrandchild0:
                    {
                        VerifyBackStack(nw, 2, "After loading grandchild");
                        this.State = State.C_AtParent1;
                        PfGrandchild pf = (PfGrandchild)nw.Content;
                        pf.End();
                    }
                    break;

                case State.C_AtParent1:
                    {
                        this.State = State.C_AtGrandchild1;
                        VerifyBackStack(nw, 3, "After finishing grandchild once");
                        PfParent pf = (PfParent)nw.Content;
                        if (pf.ReturnCount != 1)
                        {
                            throw new ApplicationException("Parent return handler has not been called once: got " + pf.ReturnCount);
                        }
                        nw.GoBack();
                    }
                    break;

                case State.C_AtGrandchild1:
                    {
                        VerifyBackStack(nw, 2, "After going back to grandchild");
                        this.State = State.C_AtParent2;
                        PfGrandchild pf = (PfGrandchild)nw.Content;
                        pf.End();
                    }
                    break;

                case State.C_AtParent2:
                    {
                        VerifyBackStack(nw, 3, "After finishing grandchild twice");
                        this.State = State.Done;
                        PfParent pf = (PfParent)nw.Content;
                        if (pf.ReturnCount != 2)
                        {
                            throw new ApplicationException("Parent return handler has not been called twice: got " + pf.ReturnCount);
                        }
                    }
                    goto case State.D_Start;

                // Test D  -----------------------------------------------------------------

                case State.D_Start:
                    DrtPagefunction.Log("Test D: Odd cases...");

                    nw.Navigate(new PfChildWithFrame()); // no Return handler attached
                    this.State = State.D_AtPfChildWithFrame;
                    break;

                case State.D_AtPfChildWithFrame:
                    {
                        VerifyBackStack(nw, 4, "After starting PfChildWithFrame");
                        PfChildWithFrame pf = (PfChildWithFrame)nw.Content;
                        Assert(!pf.KeepAlive);

                        pf.Frame.Navigate(new PfChild());
                        this.State = State.D_FrameNavigated;

                    }
                    break;

                case State.D_FrameNavigated:
                    {
                        VerifyBackStack(nw, 4, "After navigating the frame in PfChildWithFrame");
                        PfChildWithFrame pf = (PfChildWithFrame)nw.Content;
                        Assert(!pf.KeepAlive);

                        Frame f = pf.Frame;
                        Assert(f.Content is PfChild, "The Frame was not navigated to PfChild.");
                        PfChild pfInFrame = (PfChild)f.Content;
                        pfInFrame.RemoveFromJournal = true; //(See test overview.)

                        pf.LaunchNext();
                        this.State = State.D_AtPfGrandchildAgain;
                    }
                    break;

                case State.D_AtPfGrandchildAgain:
                    {
                        VerifyBackStack(nw, 5, "After PfChild has started PfGrandchild");
                        PfGrandchild pf = (PfGrandchild)nw.Content;
                        nw.GoBack();
                        this.State = State.D_BackToPfChildWithFrame;
                    }
                    break;

                case State.D_BackToPfChildWithFrame:
                    {
                        // In the current implementation the Frame re-navigation is started before the
                        // navigation to PfChildWithFrame, so it be finished first. When e.Navigator is
                        // the NavWin, the child Frame should have been loaded. This order should not be
                        // strictly enforced, though. If major implementation changes occur, the test can
                        // be adjusted as appropriate.
                        if (e.Navigator == nw)
                        {
                            VerifyBackStack(nw, 4, "Back to PfChildWithFrame");
                            CheckBackForwardState(true, true);
                            PfChildWithFrame pf = (PfChildWithFrame)nw.Content;

                            Assert(pf.Frame.Content is PfChild,
                                "The frame's content was not restored (expected PfChild).");
                            PfChild pfInFrame = (PfChild)pf.Frame.Content;

                            // pfInFrame calling OnReturn(). It has no parent to return to, but the
                            // forward stack will still be cleared.
                            pfInFrame.RemoveFromJournal = true;
                            pfInFrame.End();
                            VerifyBackStack(nw, 4, "PF in Frame returning was not expected to change the back stack.");
                            CheckBackForwardState(true, false);

                            pf.RemoveFromJournal = true; //(Set to false by ctor.) 
                            pf.End();
                            this.State = State.D_ReturnedToParent;
                        }
                        else
                            Assert(e.Navigator is Frame);
                    }
                    break;

                case State.D_ReturnedToParent:
                    {
                        VerifyBackStack(nw, 3, "Back to PfParent after PfChild returns with RemoveFromJournal set");
                        CheckBackForwardState(true, false);
                        PfParent pf = (PfParent)nw.Content;
                        // No Return event handler attached to PfChild, so ReturnCount doesn't change.
                        Assert(pf.ReturnCount == 2, "PfParent.ReturnCount expected to be still 2; it is " + pf.ReturnCount);

                        // Now try returning from PfParent. There's nowhere to return to, but because it 
                        // has RemoveFromJournal, all journal entries starting from PfParent's first instance
                        // will be removed. So, this will leave the journal empty.

                        Assert(pf.RemoveFromJournal); // new default value
                        pf.End();
                        this.State = State.D_ReturnedToNowhere;

                        NavigationService ns = NavigationService.GetNavigationService(pf);
                        string navStatus = GetNavigationStatus(ns);
                        Assert(navStatus == "Idle" || navStatus == "Navigated",
                            "No new navigation should have been started, because there is no parent page to return to.");

                        CheckBackForwardState(false, false);

                        // Verify PfParent won't be re-journaled
                        nw.Navigate(new OrdinaryPage());
                        this.State = State.D_NavAfterReturnToNowhere;
                    }
                    break;

                case State.D_NavAfterReturnToNowhere:
                    {
                        VerifyBackStack(nw, 0, "No journal entry should be created for a RemoveFromJournal PF");
                        CheckBackForwardState(false, false);
                        OrdinaryPage page = (OrdinaryPage)nw.Content;

                        page.StartPf1AndDoNewNavOnReturn(nw);
                        this.State = State.D_Pf1Started;
                    }
                    break;

                case State.D_Pf1Started:
                    {
                        CheckBackForwardState(true, false);
                        PageFunction1 pf = (PageFunction1)nw.Content;
                        pf.RemoveFromJournal = true;
                        pf.End();
                        // The Return handler will start a new navigation to PfChild. 
                        this.State = State.D_NavigatedFromReturnHandler;
                    }
                    break;

                case State.D_NavigatedFromReturnHandler:
                    {
                        Assert(e.Content is PfChild,
                            "The event handler for PageFunction1.Return should have navigated to PfChild, " +
                            "and the navigation back to OrdinaryPage should not have completed.");
                        VerifyBackStack(nw, 1, "Only OrdinaryPage should be in the backstack at this state.");
                        PfChild pf = (PfChild)nw.Content;

                        // Get rid of PfChild to leave the state expected by the next test.
                        pf.RemoveFromJournal = true;
                        pf.End();
                        this.State = State.E_Start;
                    }
                    break;

                // Test E  -----------------------------------------------------------------

                case State.E_Start:
                    {
                        DrtPagefunction.Log("Test E: Recursive PF invocation. PF journaling by type. Fragment navigation");

                        CheckBackForwardState(false, false);
                        OrdinaryPage page = (OrdinaryPage)nw.Content;
                        page.StartPfParent(nw);
                        this.State = State.E_AtParent;
                    }
                    break;

                case State.E_AtParent:
                    {
                        VerifyBackStack(nw, 1, "Only the OrdinaryPage should be in the backstack");
                        _TestE_1stPfParent = (PfParent)nw.Content;

                        // Test journaling of a DP with FrameworkPropertyMetadataOptions.Journal.
                        // But see the hack in PfParent's ctor.
                        _TestE_1stPfParent._TextBox.Text = "Test E";

                        PfChild pf = new PfChild();
                        Assert(!pf.KeepAlive && !pf.RemoveFromJournal/*cleared by ctor*/);
                        nw.Navigate(pf);
                        State = State.E_AtChild;
                    }
                    break;

                case State.E_AtChild:
                    {
                        try
                        {
                            nw.Navigate(_TestE_1stPfParent);
                            Assert(false, "Trying to restart a PageFunction before it has returned was not prevented");
                        }
                        catch (InvalidOperationException)
                        {
                        }
                        _TestE_1stPfParent = null;

                        nw.Navigate(new PfParent());
                        this.State = State.E_AtNestedParent;
                    }
                    break;

                case State.E_AtNestedParent:
                    {
                        VerifyBackStack(nw, 3, "After starting nested PfParent");
                        CheckBackForwardState(true, false);
                        PfParent pf = (PfParent)nw.Content;
                        Assert(pf.RemoveFromJournal); // default
                        pf.End();
                        this.State = State.E_ReturnedToChild;
                    }
                    break;

                case State.E_ReturnedToChild:
                    {
                        VerifyBackStack(nw, 2, "After returning to PfChild");
                        CheckBackForwardState(true, false);
                        PfChild pf = (PfChild)nw.Content;
                        Assert(!pf.KeepAlive && !pf.RemoveFromJournal);
                        pf.End();
                        this.State = State.E_ReturnedToFirstParent;
                    }
                    break;

                case State.E_ReturnedToFirstParent:
                    {
                        VerifyBackStack(nw, 3, "After returning from PfChild [RFJ=false]");
                        CheckBackForwardState(true, false);
                        PfParent pf = (PfParent)nw.Content;
                        Assert(!pf.KeepAlive && pf.RemoveFromJournal);
                        Assert(pf._TextBox.Text == "Test E", "TextBox.Text was not journaled.");

                        // Start the fragment navigation sub-test
                        this.State = State.E_FragNavTest;
                        nw.Navigate(new Uri("#fragment", UriKind.Relative));
                        Assert(nw.Content == pf && nw.Source.ToString() == "#fragment");

                        nw.Content = new Page();
                        this.State = State.E_NavToAnotherPage;
                    }
                    break;

                case State.E_FragNavTest:
                    // Verification in the E_ReturnedToFirstParent block. (Fragment navigation is synchronous.)
                    Assert(e.Content is PfParent);
                    break;

                case State.E_NavToAnotherPage:
                    {
                        Page p = (Page)nw.Content;
                        VerifyBackStack(nw, 4, "After fragment navigation in PfParent and then navigation to a new Page");
                        // See the explanation of why 4 in the test overview.

                        nw.GoBack();
                        this.State = State.E_BackToFirstParentSecondJournalInstance;

                    }
                    break;

                case State.E_BackToFirstParentSecondJournalInstance:
                    {
                        VerifyBackStack(nw, 4, "After returning to PfParent#fragment");
                        CheckBackForwardState(true, true);
                        PfParent pf = (PfParent)nw.Content;
                        Assert(!pf.KeepAlive && pf.RemoveFromJournal);
                        Assert(pf._TextBox.Text == "Test E", "TextBox.Text was not journaled.");

                        // Complete fragment navigation testing
                        this.State = State.E_FragNavTest;
                        pf._TextBox.Text = "Test E - frag nav";
                        nw.GoBack();  // Fragment navigation 
                        Assert(pf._TextBox.Text == "Test E - frag nav",
                            "Fragment navigation should not change controls state.");
                        Assert(nw.Content == pf && nw.Source == null);
                        nw.GoForward();
                        Assert(nw.Content == pf && nw.Source.ToString() == "#fragment");
                        VerifyBackStack(nw, 4, "After journal-renavigating to PfParent#fragment");
                        nw.GoBack();

                        pf.End();
                        this.State = State.E_ReturnedFromFirstParent;
                    }
                    break;

                case State.E_ReturnedFromFirstParent:
                    {
                        OrdinaryPage p = (OrdinaryPage)nw.Content;
                        Assert(p._PfParentReturned, "PfParent's Return handler was not called.");
                        CheckBackForwardState(false, false);

                        _savedMarkupPF1 = new MarkupPF1();
                        nw.Navigate(_savedMarkupPF1);
                        this.State = State.F_Start;
                    }
                    break;

                case State.F_Start:
                    {
                        DrtPagefunction.Log("Test F: PageFunction is implemented from Xaml file and Directly Navigated from Markup.");


                        Assert(nw.Source == null, "NavigationWindow's Source property should be null when MarkupPF1 instance is navigated");

                        CheckBackForwardState(true, false);

                        this.State = State.F_PureFragment;
                        nw.Navigate(new Uri("#testTextBox", UriKind.Relative));
                    }

                    break;

                case State.F_PureFragment:
                    {
                        CheckBackForwardState(true, false);

                        MarkupPF1 markupPF1 = (MarkupPF1)nw.Content;

                        Assert(markupPF1 == _savedMarkupPF1, "The first MarkupPF1 should not be changed after a pure fragment navigation.");

                        TestTextBox testTextBox = markupPF1.TestTextBox;

                        Assert(testTextBox.CountTextSet == 1, "The Text should be set once in TestTextBox element.");
                        Assert(String.IsNullOrEmpty(testTextBox.PreviousText), "The Old text in TestTextBox should be null");
                        Assert(testTextBox.NewText == "This is original Text", "A wrong value is set in TestTextBox.");
                        Assert(nw.Source.OriginalString == "#testTextBox", "Source Uri should be a pure fragment.");

                        //
                        // Update the text content in TestTextBox and navigate to PF2.
                        //

                        markupPF1.TestTextBox.Text = "The new Text for Journaling.";
                        markupPF1.LaunchMarkupPF2(nw);
                        this.State = State.F_Navigate_MarkupPF2;
                    }
                    break;

                case State.F_Navigate_MarkupPF2:
                    {
                        CheckBackForwardState(true, false);
                        MarkupPF2 markupPF2 = (MarkupPF2)nw.Content;

                        markupPF2.End();

                        this.State = State.F_MarkupPF2_Return;
                    }

                    break;

                case State.F_MarkupPF2_Return:
                    {
                        CheckBackForwardState(true, false);

                        MarkupPF1 newMarkupPF1 = (MarkupPF1)nw.Content;

                        Assert(newMarkupPF1 != null && newMarkupPF1 != _savedMarkupPF1, "Resume MarkupPF1 should create a new instance of the parent PageFunction.");
                        Assert(nw.Source.OriginalString == "#testTextBox", "Source Uri should still be a pure fragment after PF1 is resumed.");

                        TestTextBox testTextBox = newMarkupPF1.TestTextBox;

                        Assert(testTextBox.CountTextSet == 1, "The Text should be set once in TestTextBox element after PF1 is resumed.");
                        Assert(String.IsNullOrEmpty(testTextBox.PreviousText), "The Old text in TestTextBox should be null");
                        Assert(testTextBox.NewText == "The new Text for Journaling.", "A wrong value is set in TestTextBox.");

                        Assert(newMarkupPF1.ReturnTextFromChild == "The PF2 returns to its parent PageFunction", "MarkupPF2 returns a wrong value to its parent page.");

                        this.State = State.F_BackToPurePF1;

                        nw.GoBack();

                    }

                    break;

                case State.F_BackToPurePF1:
                    {
                        CheckBackForwardState(true, true);

                        Uri markupUri = new Uri("markuppf1.xaml", UriKind.Relative);

                        nw.Navigate(markupUri);

                        this.State = State.F_Navigate_PureMarkup_PF1;
                    }

                    break;


                case State.F_Navigate_PureMarkup_PF1:
                    {
                        CheckBackForwardState(true, false);

                        _savedMarkupPF1 = (MarkupPF1)nw.Content;

                        Assert(_savedMarkupPF1 != null, "The instance of MarkupPF1 should be created.");
                        Assert(nw.Source.OriginalString == "markuppf1.xaml", " MarkupPF1.xaml should be set in nw.Source.");

                        TestTextBox testTextBox = _savedMarkupPF1.TestTextBox;

                        Assert(testTextBox.CountTextSet == 1, "The Text should be set once in TestTextBox element.");
                        Assert(String.IsNullOrEmpty(testTextBox.PreviousText), "The Old text in TestTextBox should be null");
                        Assert(testTextBox.NewText == "This is original Text", "A wrong value is set in TestTextBox.");

                        //
                        // Update the text content in TestTextBox and Launch PF2.
                        //

                        testTextBox.Text = "The new Text for Journaling. in pure Markup navigation";

                        _savedMarkupPF1.LaunchMarkupPF2(nw);

                        this.State = State.F_PF2_Created_From_Pure_MarkupPF1;
                    }

                    break;


                case State.F_PF2_Created_From_Pure_MarkupPF1:
                    {
                        CheckBackForwardState(true, false);

                        MarkupPF2 markupPF2 = (MarkupPF2)nw.Content;

                        markupPF2.Cancel();

                        this.State = State.F_PF2_Cancelled;
                    }

                    break;

                case State.F_PF2_Cancelled:
                    {
                        CheckBackForwardState(true, false);

                        Assert(nw.Source.OriginalString == "markuppf1.xaml", " MarkupPF1.xaml should be set in nw.Source.");

                        MarkupPF1 newMarkupPF1 = (MarkupPF1)nw.Content;

                        Assert(newMarkupPF1 != null && newMarkupPF1 != _savedMarkupPF1, "Resume MarkupPF1 from Uri should create a new instance of PageFunction.");

                        TestTextBox testTextBox = newMarkupPF1.TestTextBox;

                        Assert(testTextBox.CountTextSet == 1, "The Text should be set once in TestTextBox element after MarkupPF1 is resumed from its Uri.");
                        Assert(String.IsNullOrEmpty(testTextBox.PreviousText), "The Old text in TestTextBox should be null");
                        Assert(testTextBox.NewText == "The new Text for Journaling. in pure Markup navigation", "A wrong value is set in TestTextBox.");

                        nw.GoBack();

                        this.State = State.F_Navigate_PureMarkup_PF1_GoBack;
                    }
                    break;


                case State.F_Navigate_PureMarkup_PF1_GoBack:
                    {
                        CheckBackForwardState(true, true);

                        nw.GoBack();

                        this.State = State.F_Navigate_PureMarkup_GoBack_To_OriginalPage;
                    }
                    break;

                case State.F_Navigate_PureMarkup_GoBack_To_OriginalPage:
                    {
                        CheckBackForwardState(false, true);
                        VerifyBackStack(nw, 0, "After go back to the original page ");

                        // DONE
                        this.State = State.Done;
                        nw.Close();
                    }

                    break;
                    
             }
        }

        PfParent _TestE_1stPfParent;

        string GetNavigationStatus(NavigationService ns)
        {
            PropertyInfo navStatusProp = ns.GetType().GetProperty(
                "NavStatus", BindingFlags.Instance | BindingFlags.NonPublic);
            object navStatus = navStatusProp.GetValue(ns, null);
            return navStatus.ToString();
        }

        public void VerifyBackStack(NavigationWindow navWin, int expected, string message)
        {
            IEnumerator entries = ((IEnumerable)navWin.GetValue(NavigationWindow.BackStackProperty)).GetEnumerator();

            for (int actual = 0; actual < expected; ++actual)
                Assert(entries.MoveNext(), message + ": Journal only had " + actual + " entries, expected " + expected);

            Assert(!entries.MoveNext(), message + ": Journal had too many entries, expected " + expected);
        }

        private object Dispatch(object obj)
        {
            PageFunction4 pf4 = (PageFunction4) obj ;

            pf4.End();

            return null;
        }

        private void CheckBackForwardState(bool canGoBack, bool canGoForward)
        {
            NavigationWindow window = Application.Current.Windows[0] as NavigationWindow;

            Assert(window.CanGoBack == canGoBack, "CanGoBack expected to be " + canGoBack);
            Assert(window.CanGoForward == canGoForward, "CanGoForward expected to be " + canGoForward);
        }

        private void DumpState( State fromState, State toState )
        {
            StringBuilder theBuilder = new StringBuilder() ;

            theBuilder.Append("  Changing state from: " ) ;
            theBuilder.Append( StateToString( fromState ) ) ;

            theBuilder.Append(" to: " ) ;
            theBuilder.Append( StateToString( toState ) ) ;

            if ( _verbose )
                DrtPagefunction.Log(theBuilder.ToString());
		}

        internal static void CheckState( State expected, State actual, String place  )
        {
            if ( expected != actual )
            {
                StringBuilder theBuilder = new StringBuilder() ;

                theBuilder.Append("*** In unexpected State at " + place ) ;
                theBuilder.Append(" expected: " ) ;
                theBuilder.Append( expected ) ;

                theBuilder.Append(" actual state: " ) ;
                theBuilder.Append( StateToString( actual ) ) ;

                throw new ApplicationException( theBuilder.ToString() ) ;
            }
        }

        private void ShowHelp()
        {
            DrtPagefunction.Log("DrtPageFunction [-v] [-?]");
            DrtPagefunction.Log("        -v  Verbose Mode On");
            DrtPagefunction.Log("        -?  Shows this text");
		}

        private static string StateToString( State inState )
        {
            return inState.ToString();
        }

        private State _state;
        private bool _verbose;
        public int StyleSizeState;
        public double StyleHeight1;
    }

    public class CrossElement : UIElement
    {
        public CrossElement()
        {
            _children = new VisualCollection(this);        
        }
            
        protected override void OnRender(DrawingContext ctx)
        {
            ctx.DrawLine(
                new Pen(new SolidColorBrush(Color), 2.0f),
                new Point(0, 0),
                new Point(RenderSize.Width, RenderSize.Height));
            ctx.DrawLine(
                new Pen(new SolidColorBrush(Color), 2.0f),
                new Point(0, RenderSize.Height),
                new Point(RenderSize.Width, 0));
        }

        public void MyAddChild(Visual v)
        {
            _children.Add(v);
        }

        protected override Size MeasureCore(Size constraint)
        {
            PageFunctionTest pst = PageFunctionTest.g_pst;

            switch (pst.StyleSizeState)
            {
                case 0:
                    pst.StyleHeight1 = constraint.Height;
                    pst.StyleSizeState = 1;
                    break;
                case 1:
                    if (constraint.Height - pst.StyleHeight1 < 1.0)
                    {
                        // Style did not work.
                        //throw new ApplicationException("PageFunctionStyle did not work correctly.");
                    }
                    pst.StyleSizeState = 2;
                    break;
                default:
                    break;
            }
            return base.MeasureCore(constraint);
        }

        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: 
        ///       During this virtual call it is not valid to modify the Visual tree. 
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {            
            if(_children == null)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }
            if(index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            return _children[index];
        }
        
        /// <summary>
        ///  Derived classes override this property to enable the Visual code to enumerate 
        ///  the Visual children. Derived classes need to return the number of children
        ///  from this method.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: During this virtual method the Visual tree must not be modified.
        /// </summary>        
        protected override int VisualChildrenCount
        {           
            get 
            { 
                if(_children == null)
                {
                    throw new ArgumentOutOfRangeException("_children is null");
                }                
                return _children.Count; 
            }
        }    


        public Color Color = Color.FromRgb(0x00, 0xff, 0x00);
        private VisualCollection _children;                
        
    }

    class PageFunction1 : PageFunction<int>
    {
        public static Style CreateStyle(string s)
        {
            Color darkPurple = Color.FromScRgb(1.0F, .95F, .24F, .76F);
            Color lightPurple = Color.FromScRgb(1.0F, .94F, .92F, .85F);
            LinearGradientBrush brushPurpleGradient = new LinearGradientBrush(darkPurple, lightPurple, 0);

/*
            ColorAnimation canim = new ColorAnimation(darkPurple, Colors.DarkGreen, 1000);
            canim.Begin = 0;
            canim.RepeatDuration = 100;
            canim.AutoReverse = true;
            canim.Freeze();
            brushPurpleGradient.GradientStops[0].GetAnimations(GradientStop.ColorProperty).Add(canim);
            brushPurpleGradient.Freeze();
*/
            FrameworkElementFactory fefTitleBorder = new FrameworkElementFactory(typeof(Border));

            fefTitleBorder.SetValue(Border.BorderThicknessProperty, new Thickness(0));
            fefTitleBorder.SetValue(Border.BackgroundProperty, brushPurpleGradient);
            fefTitleBorder.SetValue(DockPanel.DockProperty, Dock.Top);

            FrameworkElementFactory fefTitleBar = new FrameworkElementFactory(typeof(TextBlock));
            fefTitleBar.SetValue(TextBlock.TextProperty, s);
            fefTitleBorder.AppendChild(fefTitleBar);

            FrameworkElementFactory fefChild = new FrameworkElementFactory(typeof(ContentPresenter));
            Binding binding = new Binding("Content");
            binding.Mode = BindingMode.TwoWay;
            binding.RelativeSource = RelativeSource.TemplatedParent;
            fefChild.SetBinding(ContentPresenter.ContentProperty, binding);
            fefChild.SetValue(ClipToBoundsProperty, true);

            FrameworkElementFactory fefDockPanel = new FrameworkElementFactory(typeof(DockPanel));
            fefDockPanel.AppendChild(fefTitleBorder);
            fefDockPanel.AppendChild(fefChild);

            Style style = new Style(typeof(PageFunctionBase));

            ControlTemplate template = new ControlTemplate(typeof(PageFunctionBase));
            template.VisualTree = fefDockPanel;
            style.Setters.Add(new Setter(PageFunctionBase.TemplateProperty, template));

            return style;
        }

        public PageFunction1()
        {
            Style = PageFunction1.CreateStyle("This is pagefunction 1");
            CrossElement ce = new CrossElement();
            Content = ce;
        }

        public void OnPageFunction2Returned(object sender, ReturnEventArgs<string> ra )
        {
            if ( !(sender is PageFunction2) || ra.Result != "Hello World")
                throw new ApplicationException("Did not get expected result on PageFunction2 return");

            NavigationWindow nw = ( NavigationWindow) Application.Current.Windows[0];

            PageFunctionTest.CheckState( State.GotSecond, PageFunctionTest.MyApplication.State, "PageFunction1::OnPageFunction2Returned" ) ;

            // Check that NavigationService and the host Window can be accessed in the Return event handler.
            // This is not trivial, because we rely on inheritable DPs, but this PF is not attached to the tree at this point.
            // NavigationService.FireChildPageFunctionReturnEvent() temporarily sets local values of these DPs.
            PageFunctionTest.g_pst.Assert(Window.GetWindow(this) == nw && this.NavigationService == nw.NavigationService);

            // Create Third PageFunction - call navigate on it.

            PageFunction3 pf3 = new PageFunction3() ;
            pf3.Return += new ReturnEventHandler<int>( this.OnPageFunction3Returned ) ;

            PageFunctionTest.MyApplication.State = State.CalledThird;
            nw.Navigate( pf3 ) ;

        }

        public void OnPageFunction3Returned(object sender, ReturnEventArgs<int> ra)
        {
            PageFunctionTest.CheckState( State.ThirdEnded, PageFunctionTest.MyApplication.State, "OnPageFunction3Returned");

            if ( ra.Result != - 99 )
                throw new ApplicationException("Did not get expected result from pagefunction3");

            if ( PageFunctionTest.MyApplication.Verbose )
                DrtPagefunction.Log("Got expected result from PageFunction3 return");

            PageFunctionTest.MyApplication.State = State.C_Start;
        }

        public void End()
        {
            OnReturn(new ReturnEventArgs<int>(1));
        }
    }

    internal class PageFunction2 : PageFunction<string>
    {
        public PageFunction2(  )
        {
        }

        protected override void Start()
        {
            PageFunctionTest.MyApplication.State = State.GotSecond;

            //
            // Check to see that we are the top-most element in the NavigationWindow
            //

            NavigationWindow nw = ( NavigationWindow) Application.Current.Windows[0];
            object content = nw.Content;
            if ( content != this || Parent != nw )
                throw new ApplicationException("Topmost element in NavigationWindow is not this");

            ReturnEventArgs<string> ra = new ReturnEventArgs<string>();
            ra.Result = "Hello World";


            OnReturn( ra );
        }

    }

    internal class PageFunction3 : PageFunction<int>
    {
        public PageFunction3()
        {
        }

        // Navigate to next PageFunction.
        internal void Next()
        {
            PageFunctionTest.CheckState( State.CalledThird , PageFunctionTest.MyApplication.State, "PageFunction3::Next" ) ;

            PageFunction4 pf = new PageFunction4( );

            pf.Return += new ReturnEventHandler<string>( this.OnPageFunction4Returned ) ;

            PageFunctionTest.MyApplication.State = State.CalledFourth ;

            NavigationService.Navigate( pf ) ;
        }

        public void OnPageFunction4Returned(object sender, ReturnEventArgs<string> args )
        {
            PageFunctionTest.CheckState( State.FourthEnded, PageFunctionTest.MyApplication.State, "PageFunction3::OnPageFunction4Returned" );

            PageFunctionTest.MyApplication.State = State.ThirdEnded ;

            OnReturn( new ReturnEventArgs<int>(-99) );
        }
    }

    internal class PageFunction4 : PageFunction<string>
    {
        public PageFunction4()
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            panel.VerticalAlignment = VerticalAlignment.Stretch; panel.HorizontalAlignment = HorizontalAlignment.Stretch;

            CrossElement ce = new CrossElement();
            panel.Children.Add(ce);
            // Note: Now that the CrossElement is shown with a StackPanel, it doesn't seem to measure
            // itself properly. But this doesn't affect the test logic.

            // Add a named element to test fragment navigation
            Label label = new Label();
            label.VerticalAlignment = VerticalAlignment.Bottom; label.Height = 16;
            label.Content = "Bootom target";
            label.Name = "bottomTarget";
            panel.Children.Add(label);

            Content = panel;

            JournalEntry.SetKeepAlive(this, true);
        }

        private bool _startCalled;

        protected override void Start()
        {
            PageFunctionTest.CheckState( State.CalledFourth, PageFunctionTest.MyApplication.State, "PageFunction4.Start" ) ;
            PageFunctionTest.g_pst.Assert(!_startCalled, "PageFunction4.Start() was already called.");
            _startCalled = true;

            PageFunctionTest.MyApplication.State = State.FourthGoBack;
            try
            {
                NavigationService.GoBack();
            }
            catch (InvalidOperationException e)
            {
                if (PageFunctionTest.MyApplication.Verbose)
                    DrtPagefunction.Log("GoBack failed ", e.Message);
            }
        }

        public void End()
        {
            PageFunctionTest.CheckState( State.FirstGoForward, PageFunctionTest.MyApplication.State, "PageFunction4.End");
            PageFunctionTest.g_pst.Assert(_startCalled, "PageFunction4.Start() was never called.");

            PageFunctionTest.MyApplication.State = State.FourthEnded ;

            OnReturn( null ) ;
        }
    }

    internal class PfParent : PageFunction<string>
    {
        public PfParent()
        {
            // Set Content to a StackPanel containing a Label and a TextBox.

            StackPanel panel = new StackPanel();
            Label label = new Label();
            label.Content = "PfParent";
            panel.Children.Add(label);

            // A TextBox is added to test DP journaling. 
            // Also, its Name is used as a target for fragment navigation - Test E.
            _TextBox = new TextBox();
            _TextBox.Width = 100;
            _TextBox.Name = "fragment";
            panel.Children.Add(_TextBox);
            // Hack: To enable DP journaling, the PersistId needs to be set.
            // This is normally done by the BAML/XAML parser. -- 

            MethodInfo m = _TextBox.GetType().GetMethod("SetPersistId", BindingFlags.NonPublic | BindingFlags.Instance);
            m.Invoke(_TextBox, new object[] { 7 });

            Content = panel;

            //JournalEntry.SetKeepAlive(this, true); - set by creator as needed
        }

        internal TextBox _TextBox;

        public void LaunchNext()
        {
            PfChild child = new PfChild();
            child.KeepAlive = true;
            child.Return += new ReturnEventHandler<string>(child_Return);
            _NavWindow.Navigate(child);
        }

        public void End()
        {
            OnReturn(new ReturnEventArgs<string>("Returning from PfParent"));
        }

        private NavigationWindow _NavWindow
        {
            get { return (NavigationWindow)Window.GetWindow(this); }
        }

        void child_Return(object sender, ReturnEventArgs<string> e)
        {
            ++_returnCount;
        }

        public int ReturnCount
        {
            get { return _returnCount; }
        }

        private int _returnCount;
    }

    internal class PfChild : PageFunction<string>
    {
        public PfChild()
        {
            base.RemoveFromJournal = false;
            Label label = new Label();
            label.Content = "PfChild";
            Content = label; //  new CrossElement();
            //JournalEntry.SetKeepAlive(this, keepAlive); - to be set by creator as needed
        }

        public void LaunchNext()
        {
            PfGrandchild child = new PfGrandchild();
            child.Return +=new ReturnEventHandler<string>(child_Return);
            _NavWindow.Navigate(child);
        }

        private NavigationWindow _NavWindow
        {
            get { return (NavigationWindow)Window.GetWindow(this); }
        }

        void child_Return(object sender, ReturnEventArgs<string> e)
        {
            OnReturn(new ReturnEventArgs<string>(e.Result + ":C"));
        }

        public void End()
        {
            OnReturn(new ReturnEventArgs<string>("PfChild returning"));
        }
    }

    internal class PfChildWithFrame : PfChild
    {
        public PfChildWithFrame()
        {
            Frame f = new Frame();
            Content = f;
            // Same hack as in PfParent's ctor... Here the PersistId is need to be able
            // to journal the frame's current content when its parent navigates.
            MethodInfo m = f.GetType().GetMethod("SetPersistId", BindingFlags.NonPublic | BindingFlags.Instance);
            m.Invoke(f, new object[] { 8 });
        }

        public Frame Frame { get { return (Frame)Content; } }
    }

    internal class PfGrandchild : PageFunction<string>
    {
        public PfGrandchild()
        {
            base.RemoveFromJournal = false;
            Label label = new Label();
            label.Content = "PfGrandchild";
            Content = label; //  new CrossElement();
            JournalEntry.SetKeepAlive(this, true);
        }

        public void End()
        {
            OnReturn(new ReturnEventArgs<string>("GC"));
        }

        private NavigationWindow _NavWindow
        {
            get { return (NavigationWindow)Window.GetWindow(this); }
        }
    }

    class OrdinaryPage : Page
    {
        internal bool _PfParentReturned;

        internal void StartPfParent(NavigationWindow nw)
        {
            PfParent pf = new PfParent();
            PageFunctionTest.g_pst.Assert(!pf.KeepAlive && pf.RemoveFromJournal);
            pf.Return += new ReturnEventHandler<string>(OnPfParentReturn);
            nw.Navigate(pf);
        }

        void OnPfParentReturn(object sender, ReturnEventArgs<string> e)
        {
            PageFunctionTest.g_pst.Assert(!_PfParentReturned, "PfParent was expected to return only once.");
            PageFunctionTest.g_pst.Assert(e.Result == "Returning from PfParent", 
                "PfParent did not return the expected result.");
            _PfParentReturned = true;
        }

        internal void StartPf1AndDoNewNavOnReturn(NavigationWindow nw)
        {
            PageFunction1 pf = new PageFunction1();
            pf.Return += new ReturnEventHandler<int>(OnPf1Return);
            nw.Navigate(pf);
        }

        void OnPf1Return(object sender, ReturnEventArgs<int> e)
        {
            PageFunctionTest.g_pst.Assert(sender is PageFunction1 && e.Result == 1);
            NavigationService.GetNavigationService((DependencyObject)sender).Navigate(new PfChild());
        }

    };
}


