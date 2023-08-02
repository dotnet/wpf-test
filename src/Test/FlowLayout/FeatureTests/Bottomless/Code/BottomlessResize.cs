// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing Bottomless resize.   
    /// </summary>
    [Test(0, "Bottomless", "BottomlessResize")]
    public class BottomlessResize : AvalonTest
    {
        #region Test case members

        private Window _w;
        private FlowDocumentScrollViewer _theFDSV = null;
        private FrameworkElement _theContent = null;
        private FrameworkElement _theParent = null;
        private const double FloatingPointError = 1e-10; //CLR doubles have about 15 to 16 digits of precision

        #endregion

        #region Constructor

        public BottomlessResize()
            : base()
        {
            CreateLog = false;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);           
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initialize: setup the test
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            _w = new Window();
            _w.Width = 640;
            _w.Height = 480;
            _w.Top = 0;
            _w.Left = 0;

            Canvas c = new Canvas();
            c.Width = 250;
            c.Height = 250;
            c.Background = Brushes.Cyan;

            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument t = fdsv.Document = new FlowDocument();
            fdsv.Background = Brushes.LightGreen;

            Paragraph para = new Paragraph();
            t.Blocks.Add(para);

            TextBlock child = new TextBlock();
            child.Text =
                "This is some random text content\n"
              + "Abandon Hope All Ye Who Enter!!\n"
              + "Or at least wipe your feet,\n"
              + "you're bringing all the dirt in!";
            para.Inlines.Add(new InlineUIContainer(child));
            c.Children.Add(fdsv);

            _theParent = c;
            _theContent = child;
            _theFDSV = fdsv;

            _w.Content = c;            
            _w.Show();

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// RunTests: Run tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
                       
            SetSize(new Size(350, 350));
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TestLog log1 = new TestLog("ValidateLargerThanParent - Large Width, Large Height");
            log1.LogStatus("Large Width, Large Height");
            ValidateLargerThanParent(log1);
            log1.Close();
                        
            SetMaxSize(new Size(400, 400));
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TestLog log2 = new TestLog("ValidateLargerThanContent - Large Max Width, Large Max Height");
            log2.LogStatus("Large Max Width, Large Max Height");
            ValidateLargerThanContent(log2);
            log2.Close();
            
            //Min Width/Height Cases//////////////////////////            
            SetMinSize(new Size(18, 18));
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TestLog log3 = new TestLog("ValidateLargerThanContent - Small Min Width, Small Min Height");
            log3.LogStatus("Small Min Width, Small Min Height");
            ValidateLargerThanContent(log3);
            log3.Close();
            
            SetMinSize(new Size(400, 400));
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TestLog log4 = new TestLog("ValidateLargerThanParent - Large Min Width, Large Min Height");
            log4.LogStatus("Large Min Width, Large Min Height");
            ValidateLargerThanParent(log4);
            log4.Close();

            return TestResult.Pass;
        }

        #endregion

        #region Test Helpers

        private void SetSize(object o)
        {
            Size s = (Size)o;
            _theFDSV.Width = s.Width;
            _theFDSV.Height = s.Height;

            _theFDSV.MinWidth = 0.0;
            _theFDSV.MinHeight = 0.0;
            _theFDSV.MaxWidth = Double.PositiveInfinity;
            _theFDSV.MaxHeight = Double.PositiveInfinity;
        }

        private void SetMinSize(object o)
        {
            Size s = (Size)o;
            _theFDSV.MinWidth = s.Width;
            _theFDSV.MinHeight = s.Height;

            _theFDSV.Width = double.NaN;
            _theFDSV.Height = double.NaN;
            _theFDSV.MaxWidth = Double.PositiveInfinity;
            _theFDSV.MaxHeight = Double.PositiveInfinity;
        }

        private void SetMaxSize(object o)
        {
            Size s = (Size)o;
            _theFDSV.MaxWidth = s.Width;
            _theFDSV.MaxHeight = s.Height;

            _theFDSV.Width = double.NaN;
            _theFDSV.Height = double.NaN;
            _theFDSV.MinWidth = 0.0;
            _theFDSV.MinHeight = 0.0;
        }
      
        private void ValidateLargerThanParent(TestLog log)
        {
            bool w = GreaterThanOrEqual(_theFDSV.RenderSize.Width, _theParent.RenderSize.Width);
            bool h = GreaterThanOrEqual(_theFDSV.RenderSize.Height, _theParent.RenderSize.Height);

            bool result = w && h;

            if (!result)
            {
                log.LogEvidence("FlowDocumentScrollViewer Size " + _theFDSV.RenderSize + " Parent Size " + _theParent.RenderSize);
                log.LogEvidence("The FlowDocumentScrollViewer is not larger than its parent");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
        }       

        private void ValidateLargerThanContent(TestLog log)
        {
            bool w = GreaterThanOrEqual(_theFDSV.RenderSize.Width, _theContent.RenderSize.Width);
            bool h = GreaterThanOrEqual(_theFDSV.RenderSize.Height, _theContent.RenderSize.Height);

            bool result = w && h;

            if (!result)
            {
                log.LogEvidence("FlowDocumentScrollViewer Size " + _theFDSV.RenderSize + " Content Size " + _theContent.RenderSize);
                log.LogEvidence("The FlowDocumentScrollViewer is not larger than its content");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
        }        
        
        private bool Equal(double a, double b)
        {
            return Math.Abs(a - b) < FloatingPointError;
        }

        private bool LessThan(double a, double b)
        {
            return !Equal(a, b) && (a < b);
        }

        private bool GreaterThan(double a, double b)
        {
            return !Equal(a, b) && (a > b);
        }

        private bool LessThanOrEqual(double a, double b)
        {
            return !GreaterThan(a, b);
        }

        private bool GreaterThanOrEqual(double a, double b)
        {
            return !LessThan(a, b);
        }

        #endregion
    }
}
