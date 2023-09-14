// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.TextFormatting;

using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>TextBlock</area>
    /// <owner>Microsoft</owner>
    /// <priority>2</priority>
    /// <description>
    /// Textblock trailing space verification.
    /// </description>
    /// </summary>
    [Test(0, "TextBlock", "TrailingSpacesVerification")]
    class TrailingSpacesVerification: AvalonTest
    {               
        private Window _testWin;
        private TextBlock _txtBl;
        private string _testTextAlignment;

        [Variation("Left")]
        [Variation("Right")]
        [Variation("Justify")]
        [Variation("Center")]
        public TrailingSpacesVerification(string testTextAlignment)
            : base()
        {
            this._testTextAlignment = testTextAlignment;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);          
        }
       
        /// <summary>
        /// Setup test
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {                       
            _testWin = TestWin.Launch(typeof(Window), 400, 400, 0, 0, "SpaceTrail.xaml", true, "LayoutTestWindow");
            _txtBl = (TextBlock)_testWin.Content;
            
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _testWin.Close();
            return TestResult.Pass;
        }
      
        /// <summary>
        /// Run tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            switch (_testTextAlignment)
            {
                case "Left":
                    {
                        TestLog.Current.LogStatus("STARTING TEST VERIFICATION FOR TEXTALIGNMENT LEFT...");
                        _txtBl.TextAlignment = TextAlignment.Left;
                        CommonFunctionality.FlushDispatcher();
                        TestForTrailingSpaces();
                        break;
                    }
                case "Right":
                    {
                        TestLog.Current.LogStatus("STARTING TEST VERIFICATION FOR TEXTALIGNMENT RIGHT...");
                        _txtBl.TextAlignment = TextAlignment.Right;
                        CommonFunctionality.FlushDispatcher();
                        TestForTrailingSpaces();
                        break;
                    }
                case "Justify":
                    {
                        TestLog.Current.LogStatus("STARTING TEST VERIFICATION FOR TEXTALIGNMENT JUSTIFY...");
                        _txtBl.TextAlignment = TextAlignment.Justify;
                        CommonFunctionality.FlushDispatcher();
                        TestForTrailingSpaces();
                        break;
                    }
                case "Center":
                    {
                        TestLog.Current.LogStatus("STARTING TEST VERIFICATION FOR TEXTALIGNMENT CENTER...");
                        _txtBl.TextAlignment = TextAlignment.Center;
                        CommonFunctionality.FlushDispatcher();
                        TestForTrailingSpaces();
                        break;
                    }
            }
            return TestResult.Pass;
        }
       
        private void TestForTrailingSpaces()
        {
            ReflectionHelper rh = ReflectionHelper.WrapObject(_txtBl);

            IEnumerable lines = (IEnumerable)rh.GetField("_subsequentLines");
            rh.CallMethod("EnsureTextBlockCache", null);

            TextBlockCacheW textBlockCache = new TextBlockCacheW(rh.GetField("_textBlockCache"));
            TextRunCache textRunCache = textBlockCache.TextRunCache;
            LinePropertiesW linepropw = new LinePropertiesW(textBlockCache.LineProperties);
            
            double wrappingWidth = (double)rh.CallMethod("CalcWrappingWidth", new object[] { _txtBl.RenderSize.Width });
            Vector contentOffset = (Vector)rh.CallMethod("CalcContentOffset", new object[] { _txtBl.RenderSize, wrappingWidth });
            LineW linew = new LineW(rh.CallMethod("CreateLine", new object[] { linepropw.InnerObject }));

            int dcp = 0, i = 0;
            Vector lineOffset = contentOffset;

            object firstLineMetricsObj = rh.GetField("_firstLine");
            TestingTrailingSpaceLoop(rh, textRunCache, linepropw, wrappingWidth, contentOffset, linew, ref dcp, ref i, lineOffset, firstLineMetricsObj);

            foreach (object lineMetricsObj in lines)
            {
                TestingTrailingSpaceLoop(rh, textRunCache, linepropw, wrappingWidth, contentOffset, linew, ref dcp, ref i, lineOffset, lineMetricsObj);
            }
        }

        private void TestForTrailingSpaces(int i, LineW linew, TextLine theline)
        {
            // linew Width has to be equal to one of the line widths under it
            if ((linew.Width != theline.Width) && (linew.Width != theline.WidthIncludingTrailingWhitespace))
            {
                TestLog.Current.LogEvidence("FAILED for line " + (i + 1));
                TestLog.Current.LogEvidence("The Line Width was not equal to The underlying TextLine's Width");
                TestLog.Current.LogEvidence(string.Format("The Line Width ({0}) should be equal to one of the widths below:", linew.Width));
                TestLog.Current.LogEvidence(string.Format("theline.Width = {0}", theline.Width));
                TestLog.Current.LogEvidence(string.Format("theline.WidthIncludingTrailingWhitespace = {0} ", theline.WidthIncludingTrailingWhitespace));
                TestLog.Current.Result = TestResult.Fail;
            }

            switch (i)
            {
                case 1:
                case 4:
                    if (linew.Width != theline.WidthIncludingTrailingWhitespace)
                    {
                        TestLog.Current.LogEvidence("FAILED for line " + (i + 1));
                        TestLog.Current.LogEvidence(string.Format("Line.Width ({0}) is not equal to TextLine.WidthIncludingTrailingWhitespace ({1})", linew.Width, theline.WidthIncludingTrailingWhitespace));
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    break;
                case 0:
                case 2:
                case 3:  
                default:
                    if (linew.Width != theline.Width)
                    {
                        TestLog.Current.LogEvidence("FAILED for line " + (i + 1));
                        TestLog.Current.LogEvidence(string.Format("Line.Width ({0}) is not equal to TextLine.Width ({1})", linew.Width, theline.Width));
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    break;               
            }           
        }

        private void TestingTrailingSpaceLoop(ReflectionHelper rh, TextRunCache textRunCache, LinePropertiesW linepropw, double wrappingWidth, Vector contentOffset, LineW linew, ref int dcp, ref int i, Vector lineOffset, object lineMetricsObj)
        {
            LineMetricsW lineMetrics = new LineMetricsW(lineMetricsObj);
            bool showParagraphEllipsis = (bool)rh.CallMethod("ParagraphEllipsisShownOnLine", new object[] { i, lineOffset.Y - contentOffset.Y });

            MethodInfo mInfo = _txtBl.GetType().GetMethod("GetLineProperties", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(bool), linepropw.InnerObject.GetType() }, null);
            TextParagraphProperties textParaPropw = (TextParagraphProperties)(mInfo.Invoke(_txtBl, new object[] { dcp == 0, linepropw.InnerObject }));

            // parameters for calling Format
            TextLineBreak textLineBreak;
            textLineBreak = lineMetrics.TextLineBreak;

            ReflectionHelper rhLine = ReflectionHelper.WrapObject(linew.InnerObject);
            rhLine.CallMethod("Format", dcp, wrappingWidth, textParaPropw, textLineBreak, textRunCache, showParagraphEllipsis);
            TextLine theline = linew.GetLine();

            TestForTrailingSpaces(i, linew, theline);
           
            i++;
            lineOffset.Y += lineMetrics.Height;
            dcp += lineMetrics.Length;
        }        
    }
}
