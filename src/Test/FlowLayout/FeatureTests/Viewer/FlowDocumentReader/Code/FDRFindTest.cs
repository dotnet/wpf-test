// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Automation;
using System.Windows.Interop;

using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using Microsoft.Test.Input;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.RenderingVerification;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing Find feature in FlowDocumentReader.  
    /// </summary>
    [Test(0, "Viewer.FlowDocumentReader", "FindTest", MethodName = "Run")]
    public class FindTest : AvalonTest
    {
        private string _xamlFile;
        private string _testName;
        private string _viewerName;
        private NavigationWindow _window;
        private FrameworkElement _viewer;
        private FlowDocument _document;
        private string _findText;
        private FrameworkElement _findToolBar;
        private TextBox _textBox;
        private Button _findNext;
        private Button _findPrevious;
        private AutomationElement _rootAutomationElement;
        private bool _continueFindingText;
        private int _findCount;
        private IntPtr _hwnd;
        private ToggleButton _findButton;
        private const string standardFontFamily = "Georgia";
        private const int standardFontSize = 12;
        private System.Drawing.Bitmap _initialStateImage = null;
        private System.Drawing.Bitmap _afterFindStateImage = null;     

        [Variation("FlowDocumentSample1x.xaml", "FindIconUI", "Reader.Page", "their")]
        [Variation("FlowDocumentSample1x.xaml", "FindIconUI", "Reader.TwoPage", "their")]
        [Variation("FlowDocumentSample1x.xaml", "FindIconUI", "Reader.Scroll", "their")]
        [Variation("FlowDocumentSample1x.xaml", "PressF3Key", "Reader.Page", "their")]
        [Variation("FlowDocumentSample1x.xaml", "PressF3Key", "Reader.TwoPage", "their", Keywords = "MicroSuite")]
        [Variation("FlowDocumentSample1x.xaml", "PressF3Key", "Reader.Scroll", "their")]
        [Variation("FlowDocumentSample1x.xaml", "PressF3Key", "PageViewer", "their")]
        [Variation("FlowDocumentSample1x.xaml", "PressF3Key", "Reader.Page", " 6 ")]
        [Variation("FlowDocumentSample1x.xaml", "InvokeFindNext", "Reader.Page", "snake")]
        [Variation("FlowDocumentSample1x.xaml", "InvokeFindNext", "Reader.TwoPage", "snake", Keywords = "MicroSuite")]
        [Variation("FlowDocumentSample1x.xaml", "InvokeFindNext", "Reader.Scroll", "snake")]
        [Variation("FlowDocumentSample1x.xaml", "InvokeFindNext", "PageViewer", "snake")]
        [Variation("FlowDocumentSample1x.xaml", "InvokeFindNext", "Reader.TwoPage", "6")]
        [Variation("FlowDocumentSample1x.xaml", "InvokeFindPrevious", "Reader.Page", "Python")]
        [Variation("FlowDocumentSample1x.xaml", "InvokeFindPrevious", "Reader.TwoPage", "Python")]
        [Variation("FlowDocumentSample1x.xaml", "InvokeFindPrevious", "Reader.Scroll", "Python")]
        [Variation("FlowDocumentSample1x.xaml", "InvokeFindPrevious", "PageViewer", "Python")]
        [Variation("FlowDocumentSample1x.xaml", "InvokeFindPrevious", "Reader.Scroll", "6")]
        [Variation("FlowDocumentSample1x.xaml", "InvalidTextSearch", "Reader.Page", "ThisTextCantPossiblyBeFound")]
        [Variation("FlowDocumentSample1x.xaml", "InvalidTextSearch", "Reader.TwoPage", "ThisTextCantPossiblyBeFound")]
        [Variation("FlowDocumentSample1x.xaml", "InvalidTextSearch", "Reader.Scroll", "ThisTextCantPossiblyBeFound")]
        [Variation("FlowDocumentSample1x.xaml", "InvalidTextSearch", "PageViewer", "ThisTextCantPossiblyBeFound")]
        [Variation("FlowDocumentSample1x.xaml", "VisualVerifyTheHighlight", "Reader.Page", "Reptiles")]
        [Variation("FlowDocumentSample1x.xaml", "VisualVerifyTheHighlight", "Reader.TwoPage", "Reptiles")]
        [Variation("FlowDocumentSample1x.xaml", "VisualVerifyTheHighlight", "Reader.Scroll", "Reptiles")]
        [Variation("FlowDocumentSample1x.xaml", "VisualVerifyTheHighlight", "PageViewer", "Reptiles")]
        public FindTest(string xamlFile, string testName, string viewerName, string findText)
            : base()
        {
            this._xamlFile = xamlFile;
            this._testName = testName;
            this._viewerName = viewerName;
            this._findText = findText;

            _continueFindingText = true;
            _findCount = 0;

            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);

            switch (testName)
            {
                case "FindIconUI":
                    {
                        RunSteps += FindIconUISetupStep;
                        RunSteps += FindIconUIRunStep;
                        RunSteps += FindIconUIVerifyStep;
                        break;
                    }

                case "PressF3Key":
                    {
                        RunSteps += PressF3KeySetupStep;
                        RunSteps += PressF3KeyRunStep;
                        RunSteps += VerifyMatchingWordsStep;
                        break;
                    }

                case "InvokeFindNext":
                    {
                        RunSteps += InvokeFindNextSetupStep;
                        RunSteps += InvokeFindNextRunStep;
                        RunSteps += VerifyMatchingWordsStep;
                        break;
                    }

                case "InvokeFindPrevious":
                    {
                        RunSteps += InvokeFindPreviousSetupStep;
                        RunSteps += InvokeFindPreviousRunStep;
                        RunSteps += VerifyMatchingWordsStep;
                        break;
                    }

                case "InvalidTextSearch":
                    {
                        RunSteps += InvalidTextSearchSetupStep;
                        RunSteps += PressF3KeyRunStep;
                        RunSteps += VerifyMatchingWordsStep;
                        break;
                    }

                case "VisualVerifyTheHighlight":
                    {                       
                        RunSteps += InvokeFindNextSetupStep;
                        RunSteps += InvokeFindNextRunStep;
                        RunSteps += VerifyHighlightStep;
                        break;
                    }

                default:
                    {
                        throw new ApplicationException(string.Format("Unknown test name '{0}'", testName));
                    }
            }
        }

        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentReader and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            _window = new NavigationWindow();
            object content = CreateViewer();
            _window.Content = content;
            _window.SizeToContent = SizeToContent.WidthAndHeight;
            _window.Left = 0;
            _window.Top = 0;
            _window.Topmost = true;
            _window.ShowsNavigationUI = false;

            if (content is FrameworkElement)
            {
                // Setting Window.Content size to ensure same size of root element over all themes.  
                // Different themes have different sized window chrome which will cause property dump 
                // and vscan failures even though the rest of the content is the same.
                // 784x564 is the content size of a 800x600 window in Aero theme.
                ((FrameworkElement)content).Height = 564;
                ((FrameworkElement)content).Width = 784;
            }
            _window.Show();
            Status("Window created and populated");

            //When running under Test Center, need to bring focus to the test Window (from the console window of the test harness)
            Status("Bring focus to main window");
            UserInput.MouseLeftDown(_window, 1, 1);
            UserInput.MouseLeftUp(_window, 1, 1);

            WaitForPriority(DispatcherPriority.SystemIdle);

            //Get the handle to the window when find is done and then close it.
            _hwnd = new WindowInteropHelper(_window).Handle;
            if (_hwnd == IntPtr.Zero)
            {
                LogComment("Cannot get Win32 hwnd from Avalon Window");
                return TestResult.Fail;
            }
            Status("HWND obtained from Window");

            //Converts a HWND to an AutomationElement
            //but performs the conversion in another thread
            _rootAutomationElement = Convert<IntPtr, AutomationElement>(_hwnd, AutomationElement.FromHandle);

            if (_rootAutomationElement == null)
            {
                LogComment("Cannot get AutomationElement from win32 HWND");
                return TestResult.Fail;
            }
            Status("Automation Element obtained from HWND");

            Automation.AddAutomationEventHandler(
                WindowPattern.WindowOpenedEvent,
                _rootAutomationElement,
                TreeScope.Descendants,
                CloseModalMessageBox
            );

            Status("WindowOpened event handler registerd for Automation Element descenants");
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _window.Close();
            return TestResult.Pass;
        }

        private FrameworkElement CreateViewer()
        {
            _document = (FlowDocument)XamlReader.Load(File.OpenRead(_xamlFile));

            if (_viewerName == "PageViewer")
            {
                FlowDocumentPageViewer pageViewer = new FlowDocumentPageViewer();
                pageViewer.Document = _document;
                pageViewer.FontFamily = new FontFamily(standardFontFamily);
                pageViewer.FontSize = standardFontSize;
                _viewer = pageViewer;
            }
            else
            {
                FlowDocumentReader reader = new FlowDocumentReader();
                reader.FontFamily = new FontFamily(standardFontFamily);
                reader.FontSize = standardFontSize;
                reader.ViewingMode = FlowDocumentReaderViewingMode.Page;
                reader.Document = _document;
                _viewer = reader;
            }

            switch (_viewerName)
            {
                case "Reader.Page":
                    {                       
                        ((FlowDocumentReader)_viewer).ViewingMode = FlowDocumentReaderViewingMode.Page;
                        break;
                    }
                case "Reader.TwoPage":
                    {                       
                        ((FlowDocumentReader)_viewer).ViewingMode = FlowDocumentReaderViewingMode.TwoPage;
                        break;
                    }
                case "Reader.Scroll":
                    {                      
                        ((FlowDocumentReader)_viewer).ViewingMode = FlowDocumentReaderViewingMode.Scroll;
                        break;
                    }              
            }

            //Reducing visual noise in test by removing localized UI (reduces # of vscan masters needed)
            RectangleGeometry rectGeometry1 = new RectangleGeometry(new Rect(0, 0, 762, 530));
            RectangleGeometry rectGeometry2 = new RectangleGeometry(new Rect(0, 530, 350, 200));
            CombinedGeometry combinedGeometry = new CombinedGeometry(GeometryCombineMode.Union, rectGeometry1, rectGeometry2);
            _viewer.Clip = combinedGeometry;

            return _viewer;
        }

        private TestResult FindIconUISetupStep()
        {
            if (_viewerName == "PageViewer")
            {
                return TestResult.Pass;
            }

            _findButton = TestHelperW.FindtheFindButtonVisual(_viewer) as ToggleButton;
            if (_findButton == null)
            {
                LogComment("Cannot find the Find button");
                return TestResult.Fail;
            }
            //mark the button so we can later retrieve it with UIAutomation
            _findButton.Name = "MyFindButton";
            return TestResult.Pass;
        }

        private TestResult FindIconUIRunStep()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);

            if (_viewerName == "PageViewer")
            {
                //Need to invoke the Find Dialog w/ F3 in a FlowDocumentPageViewer                
                UserInput.KeyDown("F3");
                UserInput.KeyUp("F3");
            }
            else
            {
                UserInput.MouseLeftClickCenter(_findButton);
            }
            WaitForPriority(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }

        private TestResult FindIconUIVerifyStep()
        {
            WaitFor(500);

            _findToolBar = TestHelperW.FindtheFindToolBar(_viewer) as FrameworkElement;
            if (_findToolBar == null)
            {
                LogComment("The Find button was not clicked, no FindToolBar found");
                return TestResult.Fail;
            }
            else
            {
                Status("The Find button was found and clicked correctly");
                return TestResult.Pass;
            }
        }

        private TestResult PressF3KeySetupStep()
        {
            ApplicationCommands.Find.Execute(null, _viewer);
            WaitFor(500);

            if (FindTheVisuals())
            {
                _textBox.Text = _findText;                               
                return TestResult.Pass;
            }
            return TestResult.Fail;
        }

        private TestResult InvokeFindNextSetupStep()
        {
            ApplicationCommands.Find.Execute(null, _viewer);
            WaitFor(500);

            if (FindTheVisuals())
            {
                _textBox.Text = _findText;
                _findNext = TestHelperW.FindtheFindNextButton(_findToolBar) as Button;
                if (_findNext == null)
                {
                    LogComment("Cannot find the 'Find Next' button");
                    return TestResult.Fail;
                }

                // If test is VisualVerifyTheHighlight take an initial snapshot of the document
                if (_testName == "VisualVerifyTheHighlight")
                {                   
                    WaitFor(1000);
                    _initialStateImage = ImageUtility.CaptureElement(_viewer);
                }

                return TestResult.Pass;
            }
            return TestResult.Fail;
        }

        private TestResult InvokeFindNextRunStep()
        {
            while (_continueFindingText)
            {
                UserInput.MouseLeftClickCenter(_findNext);
                WaitFor(300);

                // Only want to do a single find for the hightlight test.
                if (_testName == "VisualVerifyTheHighlight")
                {
                    break;
                }

                if (_continueFindingText)
                {
                    _findCount++;
                }
            }
            return TestResult.Pass;
        }

        private TestResult InvokeFindPreviousSetupStep()
        {
            WaitFor(2000);            
            _document.Blocks.LastBlock.BringIntoView();
            WaitFor(1000);

            UserInput.MouseLeftClickCenter((FrameworkElement)_document.FindName("DOCUMENT_END"));
            WaitForPriority(DispatcherPriority.SystemIdle);

            ApplicationCommands.Find.Execute(null, _viewer);
            WaitFor(500);

            if (FindTheVisuals())
            {
                _textBox.Text = _findText;
                _findPrevious = TestHelperW.FindtheFindPreviousButton(_findToolBar) as Button;
                if (_findPrevious == null)
                {
                    LogComment("Cannot find the 'Find Previous' button");
                    return TestResult.Fail;
                }
                return TestResult.Pass;
            }
            return TestResult.Fail;
        }

        private TestResult InvokeFindPreviousRunStep()
        {
            while (_continueFindingText)
            {
                Status("Invoking Find Previous Button");
                UserInput.MouseLeftClickCenter(_findPrevious);
                WaitFor(300);

                if (_continueFindingText)
                {
                    _findCount++;
                }
            }
            return TestResult.Pass;
        }

        private TestResult InvalidTextSearchSetupStep()
        {
            Status("Executing find command");
            ApplicationCommands.Find.Execute(null, _viewer);
            WaitFor(500);

            if (FindTheVisuals())
            {
                _textBox.Text = _findText;
                return TestResult.Pass;
            }
            return TestResult.Fail;
        }

        private TestResult HighlightSetupStep()
        {
            //disable viewer chrome so that the system theme doesnt affect the captured bitmap
            _window.Resources.MergedDictionaries.Add(GenericStyles.LoadAllStyles());
            WaitForPriority(DispatcherPriority.SystemIdle);

            Status("Executing find command");

            ApplicationCommands.Find.Execute(null, _viewer);
            WaitFor(500);

            if (FindTheVisuals())
            {
                _textBox.Text = _findText;
                return TestResult.Pass;
            }
            return TestResult.Fail;
        }

        ///<summary>Press F3 until all instances of search word have been found</summary>
        private TestResult PressF3KeyRunStep()
        {
            while (_continueFindingText)
            {
                Status("pressing F3");
                UserInput.KeyDown("F3");
                UserInput.KeyUp("F3");

                WaitFor(300);

                if (_continueFindingText)
                {
                    _findCount++;
                }
            }
            return TestResult.Pass;
        }

        ///<summary>Press F3 once in order to highlight a word</summary>
        private TestResult PressF3KeyOnceRunStep()
        {
            if (_continueFindingText)
            {
                Status("pressing F3");
                UserInput.KeyDown("F3");
                UserInput.KeyUp("F3");

                WaitFor(300);
                return TestResult.Pass;
            }
            else
            {
                LogComment("Cannot proceede with search, reached end of document");
                return TestResult.Fail;
            }
        }

        ///<summary>Verify that the number of matched words in document are equal to number of words that exist in document</summary>        
        private TestResult VerifyMatchingWordsStep()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);

            int expectedCount = GetSubstringCount(_textBox.Text, new TextRange(_document.ContentStart, _document.ContentEnd).Text, StringComparison.CurrentCultureIgnoreCase);

            LogComment(string.Format("The Find api has found {0} of {1} occurrences of \"{2}\"", _findCount, expectedCount, _textBox.Text));

            return (_findCount == expectedCount) ? TestResult.Pass : TestResult.Fail;
        }

        /// <summary>Visually verify that find highlight is rendered by comparing to a bitmap master</summary>
        private TestResult VerifyHighlightStep()
        {
            UserInput.MouseMove(0, 0);
            WaitFor(300);
            _afterFindStateImage = ImageUtility.CaptureElement(_viewer);

            ImageAdapter beforeAdapter = new ImageAdapter(_initialStateImage);
            ImageAdapter afterAdapter = new ImageAdapter(_afterFindStateImage);

            ImageComparator comparator = new ImageComparator();
            bool matches = comparator.Compare(beforeAdapter, afterAdapter, false);

            if (!matches)
            {
                return TestResult.Pass;
            }
            else
            {
                LogComment("Test failed comparison.  Images should not match after a Find is performed.");                             
                return TestResult.Fail;
            }
        }

        ///<summary>Closes the message box that appears when no more matching items are found</summary>
        private void CloseModalMessageBox(object sender, AutomationEventArgs e)
        {
            _continueFindingText = false;

            //Dismiss the MessageBox
            UserInput.KeyDown("Enter");
            UserInput.KeyUp("Enter");
        }

        ///<summary>Find visuals of all the elements needed for FindTest.</summary>
        private bool FindTheVisuals()
        {
            _findToolBar = TestHelperW.FindtheFindToolBar(_viewer) as FrameworkElement;
            if (_findToolBar == null)
            {
                LogComment("Cannot find the FindToolBar");
                return false;
            }
            _textBox = TestHelperW.FindtheTextBox(_findToolBar) as TextBox;
            if (_textBox == null)
            {
                LogComment("Cannot find the FindToolbar TextBox");
                return false;
            }
            return true;
        }

        //Ahh the joys of concurrency
        //AutomationElement AutomationElement.FromHandle(IntPtr) is implemented via the the blocking win32 messaging API (SendMessage)
        //But our test case application, the recipient of the message, needs to return control to the dispatcher in order to process the message
        //The solution... call AutomationElement.FromHandle in a background thread and push a dispatcher frame in the ui thread until AutomationElement.FromHandle completes
        //The below method is a generic implementation of the above solution
        private TOutput Convert<TInput, TOutput>(TInput input, Converter<TInput, TOutput> converter)
        {
            DispatcherFrame frame = new DispatcherFrame(true);
            IAsyncResult asyncResult = converter.BeginInvoke(input, ConvertCompletedCallback, frame);
            Dispatcher.PushFrame(frame);
            return converter.EndInvoke(asyncResult);
        }

        private void ConvertCompletedCallback(IAsyncResult result)
        {
            DispatcherFrame frame = (DispatcherFrame)(result.AsyncState);
            frame.Continue = false;
        }

        private int GetSubstringCount(string substring, string input, StringComparison comparison)
        {
            if (substring.Length < 1)
            {
                return 0;
            }

            int start = 0;
            int foundIndex = 0;
            int count = 0;
            do
            {
                foundIndex = input.IndexOf(substring, start, comparison);
                if (foundIndex < start)
                {
                    break;
                }
                count++;
                start = foundIndex + substring.Length;
            }
            while (true);

            return count;
        }
    }
}
