// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Test List StartIndex property for invalid values (non positive)
//
// Verification: Basic API validation.  Visual verification is not used.
// Created by:  Microsoft
//////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Markup;
using System.IO;
using System.Windows.Controls.Primitives;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>  
    /// Testing basic section properties    
    /// </summary>
    [Test(0, "Section", "BasicSectionPropertiesTest", MethodName = "Run", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    public class BasicSectionPropertiesTest : AvalonTest
    {
        #region Test case members
       
        private Window _w;

        #endregion

        #region Constructor

        public BasicSectionPropertiesTest()
            : base()
        {
            CreateLog = false;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunPropertyTests);
            RunSteps += new TestStep(VisuallyVerifyTest);
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
            object root = XamlReader.Load(File.OpenRead("SectionBVT.xaml"));

            DocumentPageView dpv = new DocumentPageView();
            dpv.DocumentPaginator = ((IDocumentPaginatorSource)root).DocumentPaginator;

            _w.Content = dpv;
            _w.Top = 0;
            _w.Left = 0;
            _w.Width = 850;
            _w.Height = 600;
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
        private TestResult RunPropertyTests()
        {
            TestInheritables((DependencyObject)_w.Content);
            return TestResult.Pass;
        }

        /// <summary>
        /// VerifyTest: after running the test this verifies the test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VisuallyVerifyTest()
        {
            // Now test if it really looks okay - Vscan
            VScanCommon vscan = new VScanCommon(_w, this);
            // Compare First Page  
            GlobalLog.LogStatus("Comparing");                    
            if (vscan.CompareImage())
            {
                return TestResult.Pass;           
            }
            else
            {
                return TestResult.Fail;
            }            
        }

        #endregion

        private void TestInheritables(Object root)
        {
            DependencyObject dobj = root as DependencyObject;
            if (dobj == null) return;

            if (dobj.GetType().Equals(typeof(DocumentPageView)))
            {
                DocumentPageView pgView = dobj as DocumentPageView;
                DocumentPaginator paginator = pgView.DocumentPaginator;
                dobj = paginator.Source as FlowDocument;               
            }
           
            foreach (object obj in LogicalTreeHelper.GetChildren(dobj))
            {
                if (obj.GetType().Equals(typeof(Section)))
                {
                    TestSectionInheritables(obj as Section);
                }
                TestInheritables(obj);
            }           
        }

        private void TestSectionInheritables(Section sec)
        {
            // section defaults
            sec.LineHeight = double.NaN;
            sec.FlowDirection = FlowDirection.LeftToRight;
            sec.TextAlignment = TextAlignment.Left;
            sec.FontFamily = new FontFamily("Verdana");
            sec.FontStretch = FontStretches.Normal;
            sec.FontStyle = FontStyles.Normal;
            sec.FontWeight = FontWeights.Normal;
            CommonFunctionality.FlushDispatcher();

            foreach (object obj in sec.Blocks)
            {
                Paragraph p = obj as Paragraph;
                if (p == null) continue;

                Run rnInside = p.Inlines.FirstInline as Run;

                TestLog log = new TestLog("Verify LineHeight");
                if (!p.LineHeight.Equals(sec.LineHeight))
                {
                    log.Result = TestResult.Fail;
                    log.LogEvidence("Error: LineHeight Not Inherited. Section LineHeight=" + sec.LineHeight + " Paragraph LineHeight=" + p.LineHeight +
                        " for Paragraph - '" + rnInside.Text.Substring(0, 30) + "....'");
                }
                else
                {
                    log.Result = TestResult.Pass;                    
                }
                log.Close();

                log = new TestLog("Verify FlowDirection");
                if (!p.FlowDirection.Equals(sec.FlowDirection))
                {
                    log.Result = TestResult.Fail;
                    log.LogEvidence("Error: FlowDirection Not Inherited. Section FlowDirection=" + sec.FlowDirection + " Paragraph FlowDirection=" + p.FlowDirection +
                       " for Paragraph - '" + rnInside.Text.Substring(0, 30) + "....'");
                }
                else
                {
                    log.Result = TestResult.Pass;
                }
                log.Close();

                log = new TestLog("Verify TextAlignment");
                if (!p.TextAlignment.Equals(sec.TextAlignment))
                {
                    log.Result = TestResult.Fail;
                    log.LogEvidence("Error: TextAlignment Not Inherited. Section TextAlignment=" + sec.TextAlignment + " Paragraph TextAlignment=" + p.TextAlignment +
                       " for Paragraph - '" + rnInside.Text.Substring(0, 30) + "....'");
                }
                else
                {
                    log.Result = TestResult.Pass;
                }
                log.Close();

                log = new TestLog("Verify FontFamily");
                if (!p.FontFamily.Equals(sec.FontFamily))
                {
                    log.Result = TestResult.Fail;
                    log.LogEvidence("Error: FontFamily Not Inherited. Section FontFamily=" + sec.FontFamily + " Paragraph FontFamily=" + p.FontFamily +
                       " for Paragraph - '" + rnInside.Text.Substring(0, 30) + "....'");
                }
                else
                {
                    log.Result = TestResult.Pass;
                }
                log.Close();

                log = new TestLog("Verify FontStretch");
                if (!p.FontStretch.Equals(sec.FontStretch))
                {
                    log.Result = TestResult.Fail;
                    log.LogEvidence("Error: FontStretch Not Inherited. Section FontStretch=" + sec.FontStretch + " Paragraph FontStretch=" + p.FontStretch +
                       " for Paragraph - '" + rnInside.Text.Substring(0, 30) + "....'");
                }
                else
                {
                    log.Result = TestResult.Pass;
                }
                log.Close();

                log = new TestLog("Verify FontStyle");
                if (!p.FontStyle.Equals(sec.FontStyle))
                {
                    log.Result = TestResult.Fail;
                    log.LogEvidence("Error: FontStyle Not Inherited. Section FontStyle=" + sec.FontStyle + " Paragraph FontStyle=" + p.FontStyle +
                       " for Paragraph - '" + rnInside.Text.Substring(0, 30) + "....'");
                }
                else
                {
                    log.Result = TestResult.Pass;
                }
                log.Close();

                log = new TestLog("Verify FontWeight");
                if (!p.FontWeight.Equals(sec.FontWeight))
                {
                    log.Result = TestResult.Fail;
                    log.LogEvidence("Error: FontWeight Not Inherited. Section FontWeight=" + sec.FontWeight + " Paragraph FontWeight=" + p.FontWeight +
                       " for Paragraph - '" + rnInside.Text.Substring(0, 30) + "....'");
                }
                else
                {
                    log.Result = TestResult.Pass;
                }
                log.Close();
            }
        }
    }
}
