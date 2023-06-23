using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.VisualVerification;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test ToggleButton doesn't show any state
    /// </description>

    /// </summary>
    [Test(0, "ToggleButton", "ToggleButtonRegressionTest11", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions = "4.5.1+")]
    public class ToggleButtonRegressionTest11 : XamlTest
    {
        #region Private Data

        private Snapshot checkedSnapshot;
        private Snapshot uncheckedSnapshot;
        private ToggleButton myTB;

        #endregion

        #region Constructor

        public ToggleButtonRegressionTest11()
            : base(@"ToggleButtonRegressionTest11.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(ToggelButtonTest);
        }

        #endregion

        #region Test Steps

        private TestResult Setup()
        {
            Status("Setup specific for ToggleButtonRegressionTest11");
            myTB = (ToggleButton)RootElement.FindName("myTB");
            Assert.AssertTrue("Failed to load Xaml correctly", myTB != null);

            LogComment("Setup for ToggleButtonRegressionTest11 was successful");
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            checkedSnapshot = null;
            uncheckedSnapshot = null;
            myTB = null;

            return TestResult.Pass;
        }

        private TestResult ToggelButtonTest()
        {
            Status("ToggelButtonTest");

            this.Window.Focus();

            LogComment("Step1:Click ToggleButton to check it then move away to get checkedSnapshot");
            MouseClickThenMove();
            checkedSnapshot = FromControl(myTB);
            Assert.AssertTrue("Failed to get checkedsnapshot", checkedSnapshot != null);
            checkedSnapshot.ToFile("checked.png", ImageFormat.Png);

            LogComment("Step2:Click ToggleButton to uncheck it then move away to get uncheckedsnapshot");
            MouseClickThenMove();
            uncheckedSnapshot = FromControl(myTB);
            Assert.AssertTrue("Failed to get uncheckedsnapshot", uncheckedSnapshot != null);
            uncheckedSnapshot.ToFile("unchecked.png", ImageFormat.Png);

            LogComment("Step3:Compare checkedsnapshot and uncheckedsnapshot");
            SnapshotHistogramVerifier verifier = new SnapshotHistogramVerifier();
            Snapshot diff = checkedSnapshot.CompareTo(uncheckedSnapshot);
            VerificationResult result = verifier.Verify(diff);
            if (result != VerificationResult.Pass)
            {
                LogComment("Pass: The checked and unchecked snapshots of toggelbutton are different, state changed");
                diff.ToFile("diffImage.png", ImageFormat.Png);
                return TestResult.Pass;
            }
            else
            {
                LogComment("Fail: The checked and unchecked snapshots of toggelbutton are the same, state not change");
                return TestResult.Fail;
            }
        }

        #endregion

        #region Helper Methods

        ///<summary>
        ///Click control then move mouse away.
        ///</summary>
        private void MouseClickThenMove()
        {
            Microsoft.Test.Input.Mouse.MoveTo(GetControlPoint(myTB, true));
            DispatcherHelper.DoEvents(500);
            Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
            DispatcherHelper.DoEvents(500);
            Microsoft.Test.Input.Mouse.MoveTo(GetControlPoint(myTB, false));
            DispatcherHelper.DoEvents(500);
        }

        ///<summary>
        ///Get a point in\out of given control.
        ///</summary>
        ///<param name="element"></param>
        ///<param name="IsIn">Is in, or out of control</param>
        ///<returns></returns>
        private System.Drawing.Point GetControlPoint(FrameworkElement element, bool IsIn)
        {
            System.Windows.Point topleft = element.PointToScreen(new System.Windows.Point());
            double width = element.ActualWidth;
            double height = element.ActualHeight;
            if (IsIn == true)
            {
                return new System.Drawing.Point((int)(topleft.X + width / 2), (int)(topleft.Y + height / 2));
            }
            else
            {
                return new System.Drawing.Point((int)(topleft.X - width / 2), (int)(topleft.Y - height / 2));
            }
        }

        /// <summary>
        /// Grabs a Snapshot of a control.
        /// </summary>
        /// <param name="target">The control to capture</param>
        /// <returns>a TestAPI Snapshot</returns>
        private static Snapshot FromControl(FrameworkElement target)
        {
            System.Drawing.Size controlSize = new System.Drawing.Size((int)target.ActualWidth, (int)target.ActualHeight);
            System.Windows.Point targetLocationWindowsPoint = (System.Windows.Point)target.PointToScreen(new System.Windows.Point(0, 0));
            System.Drawing.Point targetLocationDrawingPoint = new System.Drawing.Point((int)targetLocationWindowsPoint.X, (int)targetLocationWindowsPoint.Y);
            System.Drawing.Rectangle targetRectangle = new System.Drawing.Rectangle(targetLocationDrawingPoint, controlSize);
            Snapshot capture = Snapshot.FromRectangle(targetRectangle);
            return capture;
        }

        #endregion

    }
}
