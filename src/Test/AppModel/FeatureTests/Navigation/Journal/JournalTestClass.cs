// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// The JournalTestClass is instantiated by each specific Journal navigation test
    /// and provides the following members and methods common to all tests that 
    /// exercise Journal API.
    /// </summary>
    public class JournalTestClass : NavigationBaseClass
    {

        #region globals
        //List<String> backStack = null;
        //List<String> fwdStack = null;

        //bool isBackEnabled = false;
        //bool isFwdEnabled = false; 

        private const String JOURNALPAGE = @"JournalTestPage.xaml";
        #endregion

        public JournalTestClass(String userGivenTestName)
            : base(userGivenTestName)
        {
            Application.Current.StartupUri = new Uri(JOURNALPAGE, UriKind.RelativeOrAbsolute);

            // Begin the test
            NavigationHelper.SetStage(TestStage.Run);
        }

        public JournalTestClass()
            : this("DefaultJournalTestName")
        {
        }
    }
}
