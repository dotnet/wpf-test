// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;
using System.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections;
using Microsoft.Test.Logging;
using Microsoft.Test.Globalization;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// BVT that has tests the RemoveBackEntry() api. Verifies that the correct
    /// journal entry (first entry on the backstack) is removed on calling RemoveBackEntry
    /// and that an exception is thrown when we try to RemoveBackEntry when the stack is empty
    /// </summary>
    
    // RemoveBackEntry
    public partial class NavigationTests : Application
    {
        NavigationWindow _removeBackEntry_navWin = null;

        enum RemoveBackEntry_CurrentTest
        {
            UnInit,
            Init,
            NavToPage2,
            NavToPage3,
            NavToString,
            NavToNull,
            RemoveBackEntries
        };

        #region RemoveBackEntry globals
        RemoveBackEntry_CurrentTest _currentState = RemoveBackEntry_CurrentTest.UnInit;
        String _page1JournalEntry = "Page1";
        String _page2JournalEntry = "Lipsum, For The Masses";
        String _page3JournalEntry = "ImagePage";
        #endregion

        void RemoveBackEntry_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("RemoveBackEntry");

            Application.Current.StartupUri = new Uri(CONTROLSPAGE, UriKind.RelativeOrAbsolute);

            NavigationHelper.SetStage(TestStage.Run);
            _currentState = RemoveBackEntry_CurrentTest.Init;
        }

        void RemoveBackEntry_LoadCompleted(object sender, NavigationEventArgs e)
        {
            switch (_currentState)
            {
                case RemoveBackEntry_CurrentTest.Init:
                    _removeBackEntry_navWin = Application.Current.MainWindow as NavigationWindow;
                    _removeBackEntry_navWin.ContentRendered += new EventHandler(RemoveBackEntry_ContentRendered);
                    break;
            }
        }

        void RemoveBackEntry_ContentRendered(object sender, EventArgs e)
        {
            switch (_currentState)
            {
                case RemoveBackEntry_CurrentTest.Init:
                    _currentState = RemoveBackEntry_CurrentTest.NavToPage2;
                    _removeBackEntry_navWin.Navigate(new Uri(FLOWDOCPAGE, UriKind.RelativeOrAbsolute));
                    break;

                case RemoveBackEntry_CurrentTest.NavToPage2:
                    _currentState = RemoveBackEntry_CurrentTest.NavToPage3;
                    _removeBackEntry_navWin.Navigate(new Uri(IMAGEPAGE, UriKind.RelativeOrAbsolute));
                    break;

                case RemoveBackEntry_CurrentTest.NavToPage3:
                    _currentState = RemoveBackEntry_CurrentTest.NavToString;
                    _removeBackEntry_navWin.Navigate("This is some string");
                    break;

                case RemoveBackEntry_CurrentTest.NavToString:
                    _currentState = RemoveBackEntry_CurrentTest.NavToNull;
                    _removeBackEntry_navWin.Navigate(null);
                    break;

                case RemoveBackEntry_CurrentTest.NavToNull:
                    _currentState = RemoveBackEntry_CurrentTest.RemoveBackEntries;
                    RemoveAndVerifyBackEntries();
                    break;
            }
        }

        void RemoveTopEntryAndVerify(String expectedJournalEntry)
        {
            JournalEntry removedEntry = _removeBackEntry_navWin.RemoveBackEntry();
            if (expectedJournalEntry.Equals(removedEntry.Name))
            {
                NavigationHelper.Output("Expected and removed journal entry match");
            }
            else
            {
                NavigationHelper.Fail("Expected and removed journal entry don't match - expected = " + expectedJournalEntry
                                + " actual = " + removedEntry.Name);
            }
        }

        void RemoveAndVerifyBackEntries()
        {
            string untitled = "Untitled";
            Assembly presentationFramework = null;
            
            try
            {
                presentationFramework = Assembly.Load("PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                untitled = Extract.GetExceptionString("Untitled", presentationFramework);
            }
            catch (Exception e)
            {
                NavigationHelper.Output("Couldn't get resource string for Untitled: " + e.ToString());
            }

            RemoveTopEntryAndVerify(untitled);
            RemoveTopEntryAndVerify(_page3JournalEntry);
            RemoveTopEntryAndVerify(_page2JournalEntry);
            RemoveTopEntryAndVerify(_page1JournalEntry);

            Object o = _removeBackEntry_navWin.RemoveBackEntry();
            if (o == null)
            {
                NavigationHelper.Pass("Trying to remove back entry when back stack is empty returned null");
            }
            else
            {
                NavigationHelper.Fail("Trying to remove back entry when back stack is empty did not return null");
            }

            NavigationHelper.SetStage(TestStage.Cleanup);
        }
    }
}
