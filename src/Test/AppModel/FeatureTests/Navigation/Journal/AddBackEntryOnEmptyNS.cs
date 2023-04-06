// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: AddBackEntryOnEmptyNS tests that for NavigationWindows,
//  Frames and IslandFrames:
//    with no StartupUri/Source/object content, a Frame with no Source/
//    object content), if you try setting AddBackEntry on it, this will 
//    throw a InvalidOperationException.
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // AddBackEntryOnEmptyNS
    public partial class NavigationTests : Application
    {

        internal enum AddBackEntryOnEmptyNS_CurrentTest
        {
            UnInit,
            NavigationWindow,
            Frame,
            IslandFrame,
            End
        }

        AddBackEntryOnEmptyNS_CurrentTest _test = AddBackEntryOnEmptyNS_CurrentTest.UnInit;

        void AddBackEntryOnEmptyNS_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("AddBackEntryOnEmptyNS");
            
            // Begin the test
            NavigationHelper.SetStage(TestStage.Run);

            bool isBrowserHosted = BrowserInteropHelper.IsBrowserHosted;
            bool testPassed = true;
            _test = AddBackEntryOnEmptyNS_CurrentTest.NavigationWindow;

            while (_test != AddBackEntryOnEmptyNS_CurrentTest.End)
            {
                switch (_test)
                {
                    case AddBackEntryOnEmptyNS_CurrentTest.NavigationWindow:
                        NavigationHelper.Output("Testing NavigationWindow.NavigationService");
                        if (!isBrowserHosted)
                        {
                            NavigationWindow currNavWin = new NavigationWindow();
                            testPassed = testPassed && AddBackEntryOnEmptyNS_TryToAddBackEntry(currNavWin);
                        }
                        else
                        {
                            NavigationWindow currNavWin = Application.Current.MainWindow as NavigationWindow;
                            testPassed = testPassed && AddBackEntryOnEmptyNS_TryToAddBackEntry(currNavWin);
                        }
                        _test = AddBackEntryOnEmptyNS_CurrentTest.Frame;
                        break;

                    case AddBackEntryOnEmptyNS_CurrentTest.Frame:
                        NavigationHelper.Output("Testing Frame.NavigationService");
                        Frame frame1 = new Frame();
                        testPassed = testPassed && AddBackEntryOnEmptyNS_TryToAddBackEntry(frame1);
                        _test = AddBackEntryOnEmptyNS_CurrentTest.IslandFrame;
                        break;

                    case AddBackEntryOnEmptyNS_CurrentTest.IslandFrame:
                        NavigationHelper.Output("Testing IslandFrame.NavigationService");
                        Frame islandFrame1 = new Frame();
                        islandFrame1.JournalOwnership = JournalOwnership.OwnsJournal;
                        islandFrame1.NavigationUIVisibility = NavigationUIVisibility.Visible;
                        testPassed = testPassed && AddBackEntryOnEmptyNS_TryToAddBackEntry(islandFrame1);
                        _test = AddBackEntryOnEmptyNS_CurrentTest.End;
                        break;
                }
            }

            if (_test == AddBackEntryOnEmptyNS_CurrentTest.End && testPassed)
                NavigationHelper.Pass("NavigationWindow, Frame, IslandFrame tests passed");
            else
                NavigationHelper.Fail("One of the previous subtests failed.");
        }

        private bool AddBackEntryOnEmptyNS_TryToAddBackEntry(ContentControl cc)
        {
            NavigationService ns = null;
            try
            {
                String DUMMYSTRING = "Farmer Ted";
                int DUMMYINT = 1;

                if (cc is Frame)
                {
                    NavigationHelper.Output("Calling Frame.NavigationService.AddBackEntry");
                    ns = ((Frame)cc).NavigationService;
                    ns.AddBackEntry(new AddBackEntryOnEmptyNS_CustomPageState(DUMMYINT, DUMMYSTRING));
                }
                else if (cc is NavigationWindow)
                {
                    NavigationHelper.Output("Calling NavigationWindow.AddBackEntry");
                    ((NavigationWindow)cc).AddBackEntry(new AddBackEntryOnEmptyNS_CustomPageState(DUMMYINT, DUMMYSTRING));
                }
                else
                {
                    NavigationHelper.Output("Control is neither Frame nor NavigationWindow.  Can't call AddBackEntry on it");
                    return false;
                }

                NavigationHelper.Output("InvalidOperationException was not thrown, but should have been");
            }
            catch (InvalidOperationException)
            {
                NavigationHelper.Output("InvalidOperationException was correctly caught");
                return true;
            }
            catch (Exception exp)
            {
                NavigationHelper.Output("Expecting InvalidOperationException but instead got " + exp.ToString());
            }

            return false;
        }

    }


    [Serializable]
    // This is a dummy class that derives from CustomContentState
    class AddBackEntryOnEmptyNS_CustomPageState : CustomContentState
    {
        private int _index;
        private String _name;

        public AddBackEntryOnEmptyNS_CustomPageState(int myIndex, String myName)
        {
            _index = myIndex;
            _name = myName;
        }

        public override string JournalEntryName
        {
            get
            {
               return _name;
            }
        }

        public override void Replay(NavigationService navigationService, NavigationMode mode)
        {
            return;
        }
    }

}
