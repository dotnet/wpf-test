using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Code coverage test for SelectiveScrollingGrid 
    /// </description>
    /// </summary>
    [Test(0, "DataGrid", "SelectiveScrollingGridBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class SelectiveScrollingGridBVT : StepsTest
    {
        [Variation(SelectiveScrollingOrientation.None)]
        [Variation(SelectiveScrollingOrientation.Horizontal)]
        [Variation(SelectiveScrollingOrientation.Vertical)]
        [Variation(SelectiveScrollingOrientation.Both)]
        public SelectiveScrollingGridBVT(SelectiveScrollingOrientation expectedOrientation)
        {
            this.expectedOrientation = expectedOrientation;
            RunSteps += new TestStep(TestSelectiveScrollingGrid);
        }

        private SelectiveScrollingOrientation expectedOrientation;

        /// <summary>
        /// SetSelectiveScrollingOrientation and GetSelectiveScrollingOrientation
        /// Validate the actual SelectiveScrollingOrientation equals to expected SelectiveScrollingOrientation
        /// </summary>
        private TestResult TestSelectiveScrollingGrid()
        {
            LogComment(String.Format("Test SelectiveScrollingGrid on {0}", expectedOrientation));

            DependencyObject dObj = new DependencyObject();
            SelectiveScrollingGrid.SetSelectiveScrollingOrientation(dObj, expectedOrientation);
            SelectiveScrollingOrientation actualOrientation = SelectiveScrollingGrid.GetSelectiveScrollingOrientation(dObj);
            if (actualOrientation != expectedOrientation)
            {
                LogComment(String.Format("#Actual SelectiveScrollingOrientation is {0} does not equal to expected SelectiveScrollingOrientation is {1}", actualOrientation, expectedOrientation));
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }                
    }
}
