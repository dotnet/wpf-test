using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Controls
{
    [Test(0, "DataGrid", "SelectiveScrollingOrientation", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class SelectiveScrollingGridSelectiveScrollingOrientation : XamlTest
    {
        [Variation("SelectiveScrollingGridSelectiveScrollingOrientation.xaml", SelectiveScrollingOrientation.None)]
        [Variation("SelectiveScrollingGridSelectiveScrollingOrientation.xaml", SelectiveScrollingOrientation.Horizontal)]
        [Variation("SelectiveScrollingGridSelectiveScrollingOrientation.xaml", SelectiveScrollingOrientation.Vertical)]
        [Variation("SelectiveScrollingGridSelectiveScrollingOrientation.xaml", SelectiveScrollingOrientation.Both)]
        public SelectiveScrollingGridSelectiveScrollingOrientation(string fileName, SelectiveScrollingOrientation selectiveScrollingOrientation)
            : base(fileName)
        {
            this.selectiveScrollingOrientation = selectiveScrollingOrientation;
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(VerifySelectiveScrollingOrientation);            
        }

        private Panel panel;
        private SelectiveScrollingOrientation selectiveScrollingOrientation;

        public TestResult Setup()
        {
            Status("Setup for SelectiveScrollingGridSelectiveScrollingOrientation");

            panel = (Panel)RootElement.FindName("stackpanel1");
            Assert.AssertTrue("Unable to find stackpanel", panel != null);

            panel.SetValue(SelectiveScrollingGrid.SelectiveScrollingOrientationProperty, selectiveScrollingOrientation);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            panel = null;
            return TestResult.Pass;
        }

        public TestResult VerifySelectiveScrollingOrientation()
        {
            Assert.AssertTrue("Set/Get SelectiveScrollingOrientationProperty fail", selectiveScrollingOrientation == (SelectiveScrollingOrientation)panel.GetValue(SelectiveScrollingGrid.SelectiveScrollingOrientationProperty));

            return TestResult.Pass;
        }
    }
}