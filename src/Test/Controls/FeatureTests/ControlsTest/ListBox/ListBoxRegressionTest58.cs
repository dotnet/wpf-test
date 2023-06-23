using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ListBoxRegressionTest58
    /// </summary>
    [Test(0, "ListBox", "ListBoxRegressionTest58", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions = "4.5.1+")]
    public class ListBoxRegressionTest58 : XamlTest
    {
        #region Private Members

        private ListBox listbox;
        public ObservableCollection<string> myItems { get; set; }

        #endregion

        #region Constructor

        public ListBoxRegressionTest58()
            : base(@"ListBoxRegressionTest58.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        #endregion

        #region Test Steps

        public TestResult Setup()
        {
            Status("Setup");

            listbox = (ListBox)RootElement.FindName("listbox");

            if (listbox == null)
            {
                LogComment("Can not find ListBox.");
                return TestResult.Fail;
            }
            myItems = new ObservableCollection<string> { "Tim", "Jim" };

            this.Window.DataContext = this;

            //ListBox.AnchorItem Property: Gets or sets the item that is initially selected when SelectionMode is Extended.
            listbox.SelectionMode = SelectionMode.Extended;
            listbox.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            SelectFirstListBoxItemUsingMouse selectFirstListBoxItemUsingMouse = new SelectFirstListBoxItemUsingMouse();
            selectFirstListBoxItemUsingMouse.Do(listbox, null);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            listbox = null;
            return TestResult.Pass;
        }

        public TestResult Repro()
        {
            Status("Repro");
            this.Window.DataContext = null;
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            object anchorItem = GetAnchorItem(listbox);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            Assert.AssertTrue(string.Format("\n_anchorItem should be null or not pointing to any of objects."), anchorItem == null);

            return TestResult.Pass;
        }

        private object GetAnchorItem(ListBox listBox)
        {
            return typeof(ListBox).GetProperty("AnchorItem", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(listBox, null);
        }

        #endregion
    }
}

