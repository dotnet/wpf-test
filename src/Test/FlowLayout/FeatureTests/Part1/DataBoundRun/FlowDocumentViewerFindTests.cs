// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Logging;
using Mti = Microsoft.Test.Input;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Coverage for performing text searches on the content of Runs with bound text source in FlowDocument viewers.    
    /// </summary>
    [Test(1, "Part1.DataBoundRun", "Find in Bound Content ", MethodName = "Run", Keywords = "Localization_Suite")]
    public class FlowDocumentViewerFindTests : AvalonTest
    {
        private Window _testWindow;
        private string _viewerType;         
        private FrameworkElement _documentViewer;
        private string _findString = "FlowDocument";
        private DataBoundRunCommon _dataBoundRunCommon;
           
        [Variation("FlowDocumentReader", false)]
        [Variation("FlowDocumentPageViewer", false)]
        [Variation("FlowDocumentScrollViewer", false)]
        [Variation("FlowDocumentReader", true)]
        [Variation("FlowDocumentPageViewer", true)]
        [Variation("FlowDocumentScrollViewer", true)] 
        public FlowDocumentViewerFindTests(string viewerType, bool updateBindingSource)
        {
            this._viewerType = viewerType;

            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(FindContentInBoundedContent);                       
            if (updateBindingSource)
            {
                RunSteps += new TestStep(UpdateBindingSource);
                RunSteps += new TestStep(FindContentInBoundedContent);                   
            }
            RunSteps += new TestStep(VerifyTest);
        }

        /// <summary>
        /// Creates content for the test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            _testWindow = new Window();
            _testWindow.Topmost = true;
            _testWindow.Title = DriverState.TestName;                        
            _testWindow.Show();
           
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Mti.Input.MoveToAndClick(_testWindow);
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            _dataBoundRunCommon = new DataBoundRunCommon(_testWindow);
            _documentViewer = _dataBoundRunCommon.CreateDocumentViewerWithBoundedRun(_viewerType, "This test verifies that a Document can Find content from a Run with a bound Text property. ", " This is content after the Bounded Run.");
            _testWindow.Content = _documentViewer;
            WaitForPriority(DispatcherPriority.ApplicationIdle);
                        
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _testWindow.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// user32 function GetKeyboardLayout
        /// supplying threadID = 0 means the current thread
        /// </summary>
        /// <param name="threadID">thread id, 0 means current thread</param>
        /// <returns>return hkl for the thread</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern System.IntPtr GetKeyboardLayout(System.IntPtr threadID);

        /// <summary>
        /// Loads a new input locale identifier (formerly called the
        /// keyboard layout) into the system.
        /// </summary>
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern System.IntPtr LoadKeyboardLayout(string pwszKLID, int flags);

        /// <summary>
        /// Sets the input locale identifier (formerly called the keyboard
        /// layout handle) for the calling thread or the current process.
        /// </summary>
        [System.Runtime.InteropServices.DllImport("user32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern System.IntPtr ActivateKeyboardLayout(System.IntPtr hkl, int uFlags);

        /// <summary>
        /// Attempt to find text content that is in the bound Run.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult FindContentInBoundedContent()
        {            
            switch (_viewerType)
            {
                case "FlowDocumentReader":
                    {
                        ((FlowDocumentReader)_documentViewer).Find();
                        break;
                    }
                case "FlowDocumentPageViewer":
                    {
                        ((FlowDocumentPageViewer)_documentViewer).Find();
                        break;
                    }
                case "FlowDocumentScrollViewer":
                    {
                        ((FlowDocumentScrollViewer)_documentViewer).Find();
                        break;
                    }
            }

            System.IntPtr currentKeyboardLayoutIntPtr = GetKeyboardLayout(System.IntPtr.Zero);
            string currentKeyboardLayout = ((uint)(currentKeyboardLayoutIntPtr)).ToString("x8", System.Globalization.CultureInfo.InvariantCulture);
            TestLog.Current.LogStatus("Current Keyboard Layout = {0}", currentKeyboardLayout);

            if (currentKeyboardLayout != "04090409")
            {
                TestLog.Current.LogStatus("Making sure that we get English output when typing...");
                System.IntPtr hkl = LoadKeyboardLayout("04090409", 0);
                ActivateKeyboardLayout(hkl, 0);
                if (hkl == System.IntPtr.Zero)
                {
                    TestLog.Current.LogStatus("Failed to Set and Activate the keyboard to English locale!! This test may fail!!");
                }
            }
           
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TypeText(_findString);

            Mti.UserInput.KeyPress("Enter");            
            WaitForPriority(DispatcherPriority.ApplicationIdle);
          
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that the Find was successful.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            TextSelection textSelection = _dataBoundRunCommon.GetDocumentSelection(_documentViewer);     
            if (!textSelection.Text.Contains(_findString))
            {
                TestLog.Current.LogEvidence("Failed to find the content that was expected in the Bounded Run!");
                TestLog.Current.LogEvidence(string.Format("Looking for Selection containing: '{0}'", _findString));
                TestLog.Current.LogEvidence(string.Format("Instead Selection contains: '{0}'", textSelection.Text));
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }            
        }

        /// <summary>
        /// Change the content of the Window title so that the source of the bounded Run is updated.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult UpdateBindingSource()
        {
            _findString = "Window";
            _testWindow.Title = "Window Title has been updated";
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        /// <summary>
        /// Sends a string of keyboard input.
        /// </summary>        
        private void TypeText(string text)
        {
            char[] letters = text.ToCharArray();
            foreach (char letter in letters)
            {                
                Mti.UserInput.KeyPress(letter.ToString().ToUpperInvariant());
                WaitForPriority(DispatcherPriority.ApplicationIdle);
            }
        }
    }
}
