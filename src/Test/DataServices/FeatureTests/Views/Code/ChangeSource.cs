// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Collections;
using Microsoft.Test;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// This test has a binding to a CollectionViewSource. When the Source of
    /// the CVS changes, the binding should be updated accordingly. This was working before for the first
    /// time but not for consecutive times.
	/// </description>
	/// <relatedBugs>

    /// </relatedBugs>
	/// </summary>


    // The problem was with binding to a CollectionViewSource and changing that 
    // CollectionViewSource�s Source collection.
    // When the source on the CVS changes, the CVS raises a change notification to the Binding, 
    // telling it that the View is changed.  The first time, the Binding will hear this notification 
    // from the CVS and pick up data from the new view.  The bug was that subsequent changes to the 
    // source of the CollectionViewSource (which causes the View to be changed and notification to be 
    // raised) were not noticed by the Binding.
    // In the example I had in the bug, the second CVS had its Source property bound to a property on 
    // the current item of view of the first CVS.  This meant that when the currency on the first CVS 
    // changed, the second CVS got a new list as its Source.   One could also create other repro cases 
    // for the Source property being changed, e.g. through a binding to anything or through any code.  
    // That master detail scenario just happened to be a convenient example for me to write down.

    [Test(2, "Views", "ChangeSource")]
    public class ChangeSource : XamlTest
    {
        ListBox _detail;
        ListBox _master;
        ListLeagueList _leagueList;

        public ChangeSource()
            : base(@"ChangeSource.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(VerifyElements);
            RunSteps += new TestStep(ChangeCurrencyVerifyElements);
            RunSteps += new TestStep(ChangeCurrencyVerifyElementsAgain);
        }

        private TestResult Setup()
        {
            Status("Setup");
            _detail = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "detail"));
            _master = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "master"));
            _leagueList = (ListLeagueList)(this.RootElement.Resources["leagueList"]);
            return TestResult.Pass;
        }

        private TestResult VerifyElements()
        {
            Status("VerifyElements");
            WaitForPriority(DispatcherPriority.Render);

            List<Team> expectedTeams = _leagueList[0].Divisions[0].Teams;
            if (!VerifyElements(expectedTeams))
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult ChangeCurrencyVerifyElements()
        {
            Status("ChangeCurrencyVerifyElements");
            _master.Items.MoveCurrentToNext();
            List<Team> expectedTeams = _leagueList[0].Divisions[1].Teams;
            if (!VerifyElements(expectedTeams))
            {
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult ChangeCurrencyVerifyElementsAgain()
        {
            Status("ChangeCurrencyVerifyElementsAgain");
            _master.Items.MoveCurrentToNext();
            List<Team> expectedTeams = _leagueList[0].Divisions[2].Teams;
            if (!VerifyElements(expectedTeams))
            {
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        bool VerifyElements(List<Team> expectedTeams)
        {
            // verify count
            int expectedCount = expectedTeams.Count;
            int actualCount = _detail.Items.Count;

            if (expectedCount != actualCount)
            {
                LogComment("Fail - Expected count:" + expectedCount + ". Actual: " + actualCount);
                return false;
            }

            // verify items
            for (int i = 0; i < expectedCount; i++)
            {
                if (_detail.Items[i] != expectedTeams[i])
                {
                    LogComment("Fail - Expected item:" + ((Team)(expectedTeams[i])).Name + ". Actual: " + ((Team)(_detail.Items[i])).Name);
                    return false;
                }
            }
            return true;
        }
    }
}
