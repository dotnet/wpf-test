// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Test Inline element's basic property values
//
//              Checks Italic, Bold, Underline, Inline, SuperScript, and Subscript
//
// Verification: Basic API validation.  Visual verification is not used.
// Created by:  Microsoft
//////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Markup;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing commands in FlowDocumentPageViewer.    
    /// </summary>
    [Test(0, "Inline", "PropertyCheckInlineTest", MethodName = "Run")]
    public class PropertyCheckInlineTest : AvalonTest
    {
        #region Test case members

        private FlowDocumentScrollViewer _eRoot;
        private Window _w;
        private string _inputXaml;
        private int _inputID;
        
        #endregion

        #region Constructor
       
        [Variation("BVT_PropertyCheck_Inline.xaml", 1)]
        [Variation("BVT_PropertyCheck_Inline.xaml", 2)]
        [Variation("BVT_PropertyCheck_Inline.xaml", 3)]
        [Variation("BVT_PropertyCheck_Inline.xaml", 4)]
        [Variation("BVT_PropertyCheck_Inline.xaml", 5)]
        [Variation("BVT_PropertyCheck_Inline.xaml", 6)]

        public PropertyCheckInlineTest(string xamlFile, int testID)
            : base()
        {
            CreateLog = false;
            _inputXaml = xamlFile;
            _inputID = testID;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }

        #endregion

        #region Test Steps
      
        /// <summary>
        /// Initialize: setup the test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            // navWin = new NavigationWindow();
            _w = new Window();
           
            Status("Initialize");

            _eRoot = (FlowDocumentScrollViewer)XamlReader.Load(File.OpenRead(_inputXaml));
            _w.Content = _eRoot;
            _w.Width = 800;
            _w.Height = 600;
            _w.Left = 0;
            _w.Top = 0;
            _w.Topmost = true;
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
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            switch (_inputID)
            {
                case 1: VerifyInlinePropertyValues("Italic", "Italic", "Normal", "Normal", null);
                    break;
                case 2: VerifyInlinePropertyValues("Bold", "Normal", "Bold", "Normal", null);
                    break;
                case 3: VerifyInlinePropertyValues("Underline", "Normal", "Normal", "Normal", TextDecorations.Underline);
                    break;
                case 4: VerifyInlinePropertyValues("Inline", "Normal", "Normal", "Normal", null);
                    break;
                case 5: VerifyInlinePropertyValues("Superscript", "Normal", "Normal", "Superscript", null);
                    break;
                case 6: VerifyInlinePropertyValues("Subscript", "Normal", "Normal", "Subscript", null);
                    break;
            }
            return TestResult.Pass;
        }

        #endregion

        private void VerifyInlinePropertyValues(string id, string fs, string fw, string fv, TextDecorationCollection td)
        {
            Inline testInline = LogicalTreeHelper.FindLogicalNode(_eRoot, id) as Inline;

            // Verify FontStyle
            TestLog log = new TestLog("Verify FontStyle");
            if (testInline.FontStyle.ToString() != fs)
            {
                log.LogEvidence("Failed: " + id + " FontStyle is incorrect.  Expected " + fs + ", Received " + testInline.FontStyle.ToString());
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            // Verify FontWeight
            log = new TestLog("Verify FontWeight");
            if (testInline.FontWeight.ToString() != fw)
            {
                log.LogEvidence("Failed: " + id + " FontWeight is incorrect.  Expected " + fw + ", Received " + testInline.FontWeight.ToString());
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            // Verify Typography.Variants
            log = new TestLog("Verify Typography.Variants");
            if (testInline.Typography.Variants.ToString() != fv)
            {
                log.LogEvidence("Failed: " + id + " Typography.Variants is incorrect.  Expected " + fv + ", Received " + testInline.Typography.Variants.ToString());
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            // Verify TextDecorations
            log = new TestLog("Verify TextDecorations");
            if (IsNone(testInline.TextDecorations) && IsNone(td))
            {
                log.LogStatus("Pass: " + id + " TextDecorations is correct");
                log.Result = TestResult.Pass;
            }
            else if (!AreTextDecorationsEqual(testInline.TextDecorations, td))
            {
                log.LogEvidence("Failed: " + testInline.ToString() + " TextDecorations is incorrect.  Expected " + TextDecorationsToString(td) + ", Received " + TextDecorationsToString(testInline.TextDecorations));
                log.Result = TestResult.Fail;
            }
            else
            {
                log.LogStatus("Pass: " + id + " TextDecorations are equal");
                log.Result = TestResult.Pass;
            }
            log.Close();
        }

        // Make sure the locations of the TextDecoration are the same at least 
        private bool AreTextDecorationsEqual(TextDecorationCollection left, TextDecorationCollection right)
        {
            if (left == null)
            {
                return right == null;
            }

            if (right != null)
            {
                if (left.Count != right.Count)
                {
                    return false;
                }

                for (int i = 0; i < left.Count; i++)
                {
                    if (left[i].Location != right[i].Location)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private bool IsNone(TextDecorationCollection textDecors)
        {
            return (null == textDecors || 0 == textDecors.Count);
        }

        private string TextDecorationsToString(TextDecorationCollection textDecors)
        {
            if (IsNone(textDecors))
            {
                return "None";
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (TextDecoration decor in textDecors)
            {
                sb.Append(decor.Location.ToString());
                sb.Append(',');
            }

            return sb.ToString().Trim(',');
        }
    }
}
