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
    /// Testing Bottomless content change relayout.    
    /// </summary>
    [Test(3, "Bottomless", "BottomlessContentChangeRelayout")]
    public class BottomlessContentChangeRelayout : AvalonTest
    {
        #region Test case members
        
        private Window _w;
        private FlowDocumentScrollViewer _parentFDSV = null;
        private Paragraph _contentParagraph = null;
        private object _smallTextContent = new Paragraph(new Run("this is some small text"));
        private object _largeTextContent =
          new Paragraph(
            new Run(
                "    this is significantly larger text\n"
              + "    Enumerators only allow reading the data in the collection."
              + "    Enumerators cannot be used to modify the underlying collection."
              + "    Initially, the enumerator is positioned before the first element in the collection."
              + "    Reset also brings the enumerator back to this position."
              + "    At this position, calling Current throws an exception."
              + "    Therefore, you must call MoveNext to advance the enumerator to the first element of the collection before reading the value of Current."
            )
          )
        ;        

        #endregion

        #region Constructor
      
        public BottomlessContentChangeRelayout()
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
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentPageViewer and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {                        
            _w = new Window();
         
            _contentParagraph = new Paragraph();
            _contentParagraph.Background = Brushes.Yellow;
            _contentParagraph.Margin = new Thickness(20);

            _parentFDSV = new FlowDocumentScrollViewer();
            _parentFDSV.Document = new FlowDocument(_contentParagraph);
            _parentFDSV.Background = Brushes.Pink;
           
            Canvas c = new Canvas();
            c.Children.Add(_parentFDSV);

            _w.Content = c;
            _w.Width = 300;
            _w.Height = 300;
            _w.Top = 0;
            _w.Left = 0;
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
            GlobalLog.LogStatus("Starting tests...");
            UIHelper parentFDSVHelper = new UIHelper(_parentFDSV);
            UIHelper contentParagraphHelper = new UIHelper(_contentParagraph);

            //See if parentFDSV increases when content is added
            GlobalLog.LogStatus("Adding Content to Empty FDSV");
            parentFDSVHelper.RemoveContent();
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            parentFDSVHelper.GetSize();
            parentFDSVHelper.SetContent(_largeTextContent);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TestLog log1 = new TestLog("ValidateSizeIncreased  - content added to parent");
            parentFDSVHelper.ValidateSizeIncreased(log1);
            log1.Close();

            //See if parentFDSV decreases when content is removed
            GlobalLog.LogStatus("Emptying FlowDocumentScrollViewer with Content");
            parentFDSVHelper.SetContent(_largeTextContent);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            parentFDSVHelper.GetSize();
            parentFDSVHelper.RemoveContent();
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TestLog log2 = new TestLog("ValidateSizeReduced - emptying content");
            parentFDSVHelper.ValidateSizeReduced(log2);
            log2.Close();

            //See if parentFDSV increases when content changes from small text to large text
            GlobalLog.LogStatus("Increasing Content of Non Empty FDSV");
            parentFDSVHelper.SetContent(_smallTextContent);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            parentFDSVHelper.GetSize();
            parentFDSVHelper.SetContent(_largeTextContent);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TestLog log3 = new TestLog("ValidateSizeIncreased  - text size changes");
            parentFDSVHelper.ValidateSizeIncreased(log3);
            log3.Close();

            //See if parentFDSV decreases when content changes from large text to small text
            GlobalLog.LogStatus("Reducing Content of Non Empty FDSV");
            parentFDSVHelper.SetContent(_largeTextContent);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            parentFDSVHelper.GetSize();
            parentFDSVHelper.SetContent(_smallTextContent);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TestLog log4 = new TestLog("ValidateSizeReduced - text size changes");
            parentFDSVHelper.ValidateSizeReduced(log4);
            log4.Close();

            //See if parentFDSV increases when content's content increases (should not)
            GlobalLog.LogStatus("Increasing Contents of FDSVs Content");
            contentParagraphHelper.RemoveContent();
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            parentFDSVHelper.SetContent(_contentParagraph);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            parentFDSVHelper.GetSize();
            contentParagraphHelper.SetContent(_largeTextContent);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TestLog log5 = new TestLog("ValidateNoSizeChange - Increase child's content");
            parentFDSVHelper.ValidateNoSizeChange(log5);
            log5.Close();

            //See if parentFDSV decreases when content's content decreases (should not)
            GlobalLog.LogStatus("Reducing Contents of FDSVs Content");
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            contentParagraphHelper.SetContent(_largeTextContent);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            parentFDSVHelper.GetSize();
            contentParagraphHelper.SetContent(_smallTextContent);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TestLog log6 = new TestLog("ValidateNoSizeChange - Decrease child's content");
            parentFDSVHelper.ValidateNoSizeChange(log6);
            log6.Close();

            return TestResult.Pass;
        }
        #endregion
       
        public static void ShowDifference(TestLog log, Size a, Size b)
        {            
            log.LogEvidence("\nOld size " + a + " New size " + b);
        }
    }

    public class UIHelper
    {
        object _element = null;
        Size _computedSize;
        const double FloatingPointError = 1e-14; //CLR doubles have about 15 to 16 digits of precision

        public UIHelper(object f)
        {
            _element = f;
        }

        public void SetContent(object content)
        {
            RemoveContent();
            AddContent(content);
        }

        public void AddContent(object content)
        {
            if (content != null)
            {
                if (content is string)
                {
                    if (_element is FlowDocumentScrollViewer)
                    {
                        (_element as FlowDocumentScrollViewer).Document.Blocks.Add(new Paragraph(new Run(content as string)));
                    }
                    else if (_element is Paragraph)
                    {
                        (_element as Paragraph).Inlines.Add(new Run(content as string));
                    }
                }
                else
                {
                    if (content is Paragraph)
                    {
                        if (_element is FlowDocumentScrollViewer)
                        {
                            (_element as FlowDocumentScrollViewer).Document.Blocks.Add(content as Paragraph);
                        }
                        else if (_element is Paragraph)
                        {
                            if ((_element as Paragraph).Inlines.FirstInline != null)
                            {
                                (_element as Paragraph).Inlines.Add(new LineBreak());
                                (_element as Paragraph).Inlines.Add((content as Paragraph).Inlines.FirstInline);
                            }                     
                        }
                    }
                    else
                    {
                        if (content is UIElement)
                        {
                            if (_element is FlowDocumentScrollViewer)
                            {
                                (_element as FlowDocumentScrollViewer).Document.Blocks.Add(new Paragraph(new InlineUIContainer(content as UIElement)));
                            }
                            else if (_element is Paragraph)
                            {
                                (_element as Paragraph).Inlines.Add(new InlineUIContainer(content as UIElement));
                            }
                        }
                    }
                }
            }
        }

        public void RemoveContent()
        {
            if (_element is FlowDocumentScrollViewer)
            {
                FlowDocumentScrollViewer fdsv = (FlowDocumentScrollViewer)_element;
                fdsv.Document.Blocks.Clear();
                return;
            }

            if (_element is TextBlock)
            {
                ((TextBlock)_element).Text = String.Empty;
                return;
            }

            if (_element is Paragraph)
            {
                ((Paragraph)_element).Inlines.Clear();
                return;
            }

            throw new InvalidOperationException("Dont know how to remove the content for objects of type " + _element.GetType());
        }

        public void GetSize()
        {
            if (_element is FlowDocumentScrollViewer)
            {
                _computedSize = (_element as FlowDocumentScrollViewer).RenderSize;
            }
        }

        public void ValidateSizeIncreased(TestLog log)
        {
            Size s = new Size();
            if (_element is FlowDocumentScrollViewer)
            {
                s = (_element as FlowDocumentScrollViewer).RenderSize;
            }

            bool wIncreased = GreaterThan(s.Width, _computedSize.Width);
            bool hIncreased = GreaterThan(s.Height, _computedSize.Height);
            bool wReduced = LessThan(s.Width, _computedSize.Width);
            bool hReduced = LessThan(s.Height, _computedSize.Height);

            bool result = (wIncreased || hIncreased) && !(wReduced || hReduced);
            if (result == false)
            {
                BottomlessContentChangeRelayout.ShowDifference(log, _computedSize, s);
                log.LogEvidence("The panels size did not increase as expected");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
        }

        public void ValidateSizeReduced(TestLog log)
        {
            Size s = s = new Size();
            if (_element is FlowDocumentScrollViewer)
            {
                s = (_element as FlowDocumentScrollViewer).RenderSize;
            }

            bool wIncreased = GreaterThan(s.Width, _computedSize.Width);
            bool hIncreased = GreaterThan(s.Height, _computedSize.Height);
            bool wReduced = LessThan(s.Width, _computedSize.Width);
            bool hReduced = LessThan(s.Height, _computedSize.Height);

            bool result = (wReduced || hReduced) && !(wIncreased || hIncreased);
            if (result == false)
            {
                BottomlessContentChangeRelayout.ShowDifference(log, _computedSize, s);
                log.LogEvidence("The panels size did not reduce as expected");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
        }

        public void ValidateNoSizeChange(TestLog log)
        {
            Size s = s = new Size();
            if (_element is FlowDocumentScrollViewer)
            {
                s = (_element as FlowDocumentScrollViewer).RenderSize;
            }

            bool result = Equal(s.Width, _computedSize.Width) && Equal(s.Height, _computedSize.Height);
            if (result == false)
            {
                BottomlessContentChangeRelayout.ShowDifference(log, _computedSize, s);
                log.LogEvidence("The panels size remain the same as expected");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
        }
       
        bool Equal(double a, double b)
        {
            return Math.Abs(a - b) < FloatingPointError;
        }

        bool LessThan(double a, double b)
        {
            return !Equal(a, b) && (a < b);
        }

        bool GreaterThan(double a, double b)
        {
            return !Equal(a, b) && (a > b);
        }

        bool LessThanOrEqual(double a, double b)
        {
            return !GreaterThan(a, b);
        }

        bool GreaterThanOrEqual(double a, double b)
        {
            return !LessThan(a, b);
        }
    }
}
