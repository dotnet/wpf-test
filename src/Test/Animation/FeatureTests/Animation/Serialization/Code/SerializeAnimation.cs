// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  Animation Serialization Test *****************
*     Description:
*          Verifies parsing and serialization ("round-trip" testing) of a set of different
*          animations involving one SetterTimeline containing one Animation.
*     Pass Conditions:
*          The test case will Pass if the test finishes without throwing an exception.
*     How verified:
*          Routines within SerializationHelper will detect a serialization error and throw
*          the approprite exception.  If no exception occurs, the test case passes.
*          
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll, AnimationFramework.dll
*     Support Files:      A set of .xaml files passed to the .exe are located in the
*                         WindowsTestData depot
* *******************************************************/
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Markup;
     
using Microsoft.Test;
using Microsoft.Test.Logging;
//using Microsoft.Test.Xml;
using Microsoft.Test.Serialization;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.SerializeAnimation</area>

    /// <priority>2</priority>
    /// <description>
    /// Execute round-trip Serialization testing on a .xaml file passed in.
    /// using SerializationHelper.RoundTripTestFile().
    /// </description>
    /// </summary>
    [Test(2, "Animation.SerializeAnimation", "SerializeAnimationTest")]

    public class SerializeAnimationTest : AvalonTest
    {
        #region Test case members
        
        private static string               s_actualContent   = "";
        private static string               s_fileName        = "";
        private static SerializationHelper  s_helper;

        #endregion


        #region Constructor
        [Variation(@"FeatureTests\Animation\BooleansButton2.xaml", SupportFiles = @"FeatureTests\Animation\BooleansButton2.xaml")]
        [Variation(@"FeatureTests\Animation\BooleansButton2.xaml", SupportFiles = @"FeatureTests\Animation\BooleansButton2.xaml")]
        [Variation(@"FeatureTests\Animation\ButtonBeginStoryboard.xaml", SupportFiles = @"FeatureTests\Animation\ButtonBeginStoryboard.xaml")]
        [Variation(@"FeatureTests\Animation\ButtonEllipse2.xaml", SupportFiles = @"FeatureTests\Animation\ButtonEllipse2.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\ButtonTxt2.xaml", SupportFiles = @"FeatureTests\Animation\ButtonTxt2.xaml")]
        [Variation(@"FeatureTests\Animation\Canvases2.xaml", SupportFiles = @"FeatureTests\Animation\Canvases2.xaml")]
        [Variation(@"FeatureTests\Animation\ColorGrid2.xaml", SupportFiles = @"FeatureTests\Animation\ColorGrid2.xaml")]
        [Variation(@"FeatureTests\Animation\ColorSCBPolyline2s.xaml", SupportFiles = @"FeatureTests\Animation\ColorSCBPolyline2s.xaml")]
        [Variation(@"FeatureTests\Animation\DiscreteBooleanFourRadio2.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteBooleanFourRadio2.xaml")]
        [Variation(@"FeatureTests\Animation\DiscreteBooleanTextBox1.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteBooleanTextBox1.xaml")]
        [Variation(@"FeatureTests\Animation\DiscreteBooleanTextBox2.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteBooleanTextBox2.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\DiscreteBooleanTextBox3.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteBooleanTextBox3.xaml")]
        [Variation(@"FeatureTests\Animation\DiscreteBooleanToolTip1s.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteBooleanToolTip1s.xaml")]
        [Variation(@"FeatureTests\Animation\DiscreteColorButton2.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteColorButton2.xaml")]
        [Variation(@"FeatureTests\Animation\DiscreteDoubleCanvas1s.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteDoubleCanvas1s.xaml")]
        [Variation(@"FeatureTests\Animation\DiscreteDoubleCanvas2s.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteDoubleCanvas2s.xaml")]
        [Variation(@"FeatureTests\Animation\DiscreteDoubleCanvas3s.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteDoubleCanvas3s.xaml")]
        [Variation(@"FeatureTests\Animation\DiscreteDoubleGrid2.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteDoubleGrid2.xaml")]
        [Variation(@"FeatureTests\Animation\DiscreteLengthRadio2s.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteLengthRadio2s.xaml")]
        [Variation(@"FeatureTests\Animation\DiscreteStringText1.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteStringText1.xaml")]
        [Variation(@"FeatureTests\Animation\DiscreteStringText2.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteStringText2.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\DiscreteStringText3.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteStringText3.xaml")]
        [Variation(@"FeatureTests\Animation\DiscreteStringTwoText2.xaml", SupportFiles = @"FeatureTests\Animation\DiscreteStringTwoText2.xaml")]
        [Variation(@"FeatureTests\Animation\DoubleButton1s.xaml", SupportFiles = @"FeatureTests\Animation\DoubleButton1s.xaml")]
        [Variation(@"FeatureTests\Animation\DoubleButton2s.xaml", SupportFiles = @"FeatureTests\Animation\DoubleButton2s.xaml")]
        [Variation(@"FeatureTests\Animation\DoubleButton3s.xaml", SupportFiles = @"FeatureTests\Animation\DoubleButton3s.xaml")]
        [Variation(@"FeatureTests\Animation\DoubleButtons1s.xaml", SupportFiles = @"FeatureTests\Animation\DoubleButtons1s.xaml")]
        [Variation(@"FeatureTests\Animation\DoubleButtons2s.xaml", SupportFiles = @"FeatureTests\Animation\DoubleButtons2s.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\DoubleButtons3s.xaml", SupportFiles = @"FeatureTests\Animation\DoubleButtons3s.xaml")]
        [Variation(@"FeatureTests\Animation\DoubleDoubleButton2.xaml", SupportFiles = @"FeatureTests\Animation\DoubleDoubleButton2.xaml")]
        [Variation(@"FeatureTests\Animation\DoubleDoubleTextBox2.xaml", SupportFiles = @"FeatureTests\Animation\DoubleDoubleTextBox2.xaml")]
        [Variation(@"FeatureTests\Animation\DoubleOpacitySCBCanvas2.xaml", SupportFiles = @"FeatureTests\Animation\DoubleOpacitySCBCanvas2.xaml")]
        [Variation(@"FeatureTests\Animation\DoublePath1s.xaml", SupportFiles = @"FeatureTests\Animation\DoublePath1s.xaml")]
        [Variation(@"FeatureTests\Animation\DoublePath2s.xaml", SupportFiles = @"FeatureTests\Animation\DoublePath2s.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\DoublePath3s.xaml", SupportFiles = @"FeatureTests\Animation\DoublePath3s.xaml")]
        [Variation(@"FeatureTests\Animation\DoubleRotateButton2s.xaml", SupportFiles = @"FeatureTests\Animation\DoubleRotateButton2s.xaml")]
        [Variation(@"FeatureTests\Animation\DoubleTextBlockStringText2.xaml", SupportFiles = @"FeatureTests\Animation\DoubleTextBlockStringText2.xaml")]
        [Variation(@"FeatureTests\Animation\DoubleTextBox1.xaml", SupportFiles = @"FeatureTests\Animation\DoubleTextBox1.xaml")]
        [Variation(@"FeatureTests\Animation\DoubleTextBox2.xaml", SupportFiles = @"FeatureTests\Animation\DoubleTextBox2.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\DoubleTextBox3.xaml", SupportFiles = @"FeatureTests\Animation\DoubleTextBox3.xaml")]
        [Variation(@"FeatureTests\Animation\DoubleTwoInkCanvas2.xaml", SupportFiles = @"FeatureTests\Animation\DoubleTwoInkCanvas2.xaml")]
        [Variation(@"FeatureTests\Animation\EllipseEllipse2.xaml", SupportFiles = @"FeatureTests\Animation\EllipseEllipse2.xaml")]
        [Variation(@"FeatureTests\Animation\ETBorderDataBind.xaml", SupportFiles = @"FeatureTests\Animation\ETBorderDataBind.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\ETButtonDataBind.xaml", SupportFiles = @"FeatureTests\Animation\ETButtonDataBind.xaml")]
        [Variation(@"FeatureTests\Animation\ETButtonDirectTargeting.xaml", SupportFiles = @"FeatureTests\Animation\ETButtonDirectTargeting.xaml", Disabled = true)]
        [Variation(@"FeatureTests\Animation\ETButtonShortened.xaml", SupportFiles = @"FeatureTests\Animation\ETButtonShortened.xaml")]
        [Variation(@"FeatureTests\Animation\ETCanvasDataBind.xaml", SupportFiles = @"FeatureTests\Animation\ETCanvasDataBind.xaml")]
        [Variation(@"FeatureTests\Animation\ETCanvasDirectTargeting.xaml", SupportFiles = @"FeatureTests\Animation\ETCanvasDirectTargeting.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\ETDoubleButtonRotateMethods2.xaml", SupportFiles = @"FeatureTests\Animation\ETDoubleButtonRotateMethods2.xaml")]
        [Variation(@"FeatureTests\Animation\ETDoubleComboBox2s.xaml", SupportFiles = @"FeatureTests\Animation\ETDoubleComboBox2s.xaml")]
        [Variation(@"FeatureTests\Animation\ETDoubleRectangle2s.xaml", SupportFiles = @"FeatureTests\Animation\ETDoubleRectangle2s.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\ETEllipseDataBind.xaml", SupportFiles = @"FeatureTests\Animation\ETEllipseDataBind.xaml")]
        [Variation(@"FeatureTests\Animation\ETGridResourceStyle.xaml", SupportFiles = @"FeatureTests\Animation\ETGridResourceStyle.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\ETListBoxResourceStyle2.xaml", SupportFiles = @"FeatureTests\Animation\ETListBoxResourceStyle2.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\ETListBoxResourceStyle.xaml", SupportFiles = @"FeatureTests\Animation\ETListBoxResourceStyle.xaml")]
        [Variation(@"FeatureTests\Animation\ETPathResourceStyle.xaml", SupportFiles = @"FeatureTests\Animation\ETPathResourceStyle.xaml")]
        [Variation(@"FeatureTests\Animation\ETPointListBox2.xaml", SupportFiles = @"FeatureTests\Animation\ETPointListBox2.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\ETPolylineResource.xaml", SupportFiles = @"FeatureTests\Animation\ETPolylineResource.xaml")]
        [Variation(@"FeatureTests\Animation\ETRectangleResource.xaml", SupportFiles = @"FeatureTests\Animation\ETRectangleResource.xaml")]
        [Variation(@"FeatureTests\Animation\ETRectangleStyle.xaml", SupportFiles = @"FeatureTests\Animation\ETRectangleStyle.xaml")]
        [Variation(@"FeatureTests\Animation\ETRollingBallButton.xaml", SupportFiles = @"FeatureTests\Animation\ETRollingBallButton.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\ETSliderDataBind.xaml", SupportFiles = @"FeatureTests\Animation\ETSliderDataBind.xaml")]
        [Variation(@"FeatureTests\Animation\ETTextBoxResource2.xaml", SupportFiles = @"FeatureTests\Animation\ETTextBoxResource2.xaml")]
        [Variation(@"FeatureTests\Animation\ETTextBoxResource3.xaml", SupportFiles = @"FeatureTests\Animation\ETTextBoxResource3.xaml")]
        [Variation(@"FeatureTests\Animation\ETTextBoxResource.xaml", SupportFiles = @"FeatureTests\Animation\ETTextBoxResource.xaml")]
        [Variation(@"FeatureTests\Animation\FlowPanelTextBox2.xaml", SupportFiles = @"FeatureTests\Animation\FlowPanelTextBox2.xaml")]
        [Variation(@"FeatureTests\Animation\Int32sTextBox2.xaml", SupportFiles = @"FeatureTests\Animation\Int32sTextBox2.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\Int32TextBox1.xaml", SupportFiles = @"FeatureTests\Animation\Int32TextBox1.xaml")]
        [Variation(@"FeatureTests\Animation\Int32TextBox1s.xaml", SupportFiles = @"FeatureTests\Animation\Int32TextBox1s.xaml")]
        [Variation(@"FeatureTests\Animation\Int32TextBox2.xaml", SupportFiles = @"FeatureTests\Animation\Int32TextBox2.xaml", Disabled = true)]
        [Variation(@"FeatureTests\Animation\Int32TextBox3.xaml", SupportFiles = @"FeatureTests\Animation\Int32TextBox3.xaml")]
        [Variation(@"FeatureTests\Animation\LengthBoolGrid2.xaml", SupportFiles = @"FeatureTests\Animation\LengthBoolGrid2.xaml")]
        [Variation(@"FeatureTests\Animation\LengthButton1.xaml", SupportFiles = @"FeatureTests\Animation\LengthButton1.xaml")]
        [Variation(@"FeatureTests\Animation\LengthButton2.xaml", SupportFiles = @"FeatureTests\Animation\LengthButton2.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\LengthButton3.xaml", SupportFiles = @"FeatureTests\Animation\LengthButton3.xaml")]
        [Variation(@"FeatureTests\Animation\LengthDoubleButton2.xaml", SupportFiles = @"FeatureTests\Animation\LengthDoubleButton2.xaml")]
        [Variation(@"FeatureTests\Animation\LengthImages2s.xaml", SupportFiles = @"FeatureTests\Animation\LengthImages2s.xaml")]
        [Variation(@"FeatureTests\Animation\LengthThreeButton1.xaml", SupportFiles = @"FeatureTests\Animation\LengthThreeButton1.xaml")]
        [Variation(@"FeatureTests\Animation\LengthThreeButton2.xaml", SupportFiles = @"FeatureTests\Animation\LengthThreeButton2.xaml")]
        [Variation(@"FeatureTests\Animation\LinearColorText1.xaml", SupportFiles = @"FeatureTests\Animation\LinearColorText1.xaml")]
        [Variation(@"FeatureTests\Animation\LinearColorText2.xaml", SupportFiles = @"FeatureTests\Animation\LinearColorText2.xaml")]
        [Variation(@"FeatureTests\Animation\LinearColorText3.xaml", SupportFiles = @"FeatureTests\Animation\LinearColorText3.xaml")]
        [Variation(@"FeatureTests\Animation\LinearDoubleRadio2.xaml", SupportFiles = @"FeatureTests\Animation\LinearDoubleRadio2.xaml")]
        [Variation(@"FeatureTests\Animation\LinearLengthCanvas1.xaml", SupportFiles = @"FeatureTests\Animation\LinearLengthCanvas1.xaml")]
        [Variation(@"FeatureTests\Animation\LinearLengthCanvas2.xaml", SupportFiles = @"FeatureTests\Animation\LinearLengthCanvas2.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\LinearLengthCanvas3.xaml", SupportFiles = @"FeatureTests\Animation\LinearLengthCanvas3.xaml")]
        [Variation(@"FeatureTests\Animation\OpacitySCBFourRadio2.xaml", SupportFiles = @"FeatureTests\Animation\OpacitySCBFourRadio2.xaml")]
        [Variation(@"FeatureTests\Animation\OpacityTwoImage2.xaml", SupportFiles = @"FeatureTests\Animation\OpacityTwoImage2.xaml")]
        [Variation(@"FeatureTests\Animation\PointKeySplineLineGeometryPath2s.xaml", SupportFiles = @"FeatureTests\Animation\PointKeySplineLineGeometryPath2s.xaml")]
        [Variation(@"FeatureTests\Animation\PointLGBButton2s.xaml", SupportFiles = @"FeatureTests\Animation\PointLGBButton2s.xaml")]
        [Variation(@"FeatureTests\Animation\PointPath2.xaml", SupportFiles = @"FeatureTests\Animation\PointPath2.xaml")]
        [Variation(@"FeatureTests\Animation\PointPointSyncDecorator2.xaml", SupportFiles = @"FeatureTests\Animation\PointPointSyncDecorator2.xaml")]
        [Variation(@"FeatureTests\Animation\PointRotateEllipse1.xaml", SupportFiles = @"FeatureTests\Animation\PointRotateEllipse1.xaml")]
        [Variation(@"FeatureTests\Animation\PointRotateEllipse2.xaml", SupportFiles = @"FeatureTests\Animation\PointRotateEllipse2.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\PointRotateEllipse3.xaml", SupportFiles = @"FeatureTests\Animation\PointRotateEllipse3.xaml")]
        [Variation(@"FeatureTests\Animation\RectPath2.xaml", SupportFiles = @"FeatureTests\Animation\RectPath2.xaml")]
        [Variation(@"FeatureTests\Animation\RectPath2s.xaml", SupportFiles = @"FeatureTests\Animation\RectPath2s.xaml")]
        [Variation(@"FeatureTests\Animation\ScaleCanvases2.xaml", SupportFiles = @"FeatureTests\Animation\ScaleCanvases2.xaml", Disabled = true)]
        [Variation(@"FeatureTests\Animation\ScaleDecorators2.xaml", SupportFiles = @"FeatureTests\Animation\ScaleDecorators2.xaml")]
        [Variation(@"FeatureTests\Animation\SizeArcSegmentPath2.xaml", SupportFiles = @"FeatureTests\Animation\SizeArcSegmentPath2.xaml")]
        [Variation(@"FeatureTests\Animation\SizeArcSegmentPath2s.xaml", SupportFiles = @"FeatureTests\Animation\SizeArcSegmentPath2s.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\SplineColorTextBox2.xaml", SupportFiles = @"FeatureTests\Animation\SplineColorTextBox2.xaml")]
        [Variation(@"FeatureTests\Animation\SplineDoublePath2s.xaml", SupportFiles = @"FeatureTests\Animation\SplineDoublePath2s.xaml")]
        [Variation(@"FeatureTests\Animation\SplineDoubleTextBox2.xaml", SupportFiles = @"FeatureTests\Animation\SplineDoubleTextBox2.xaml")]
        [Variation(@"FeatureTests\Animation\SplineDoubleTwoPath2.xaml", SupportFiles = @"FeatureTests\Animation\SplineDoubleTwoPath2.xaml")]
        [Variation(@"FeatureTests\Animation\SplinLengthLine2.xaml", SupportFiles = @"FeatureTests\Animation\SplinLengthLine2.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\TextBlock2.xaml", SupportFiles = @"FeatureTests\Animation\TextBlock2.xaml")]
        [Variation(@"FeatureTests\Animation\TextCanvas2.xaml", SupportFiles = @"FeatureTests\Animation\TextCanvas2.xaml")]
        [Variation(@"FeatureTests\Animation\TextText2.xaml", SupportFiles = @"FeatureTests\Animation\TextText2.xaml")]
        [Variation(@"FeatureTests\Animation\ThicknessBorder1.xaml", SupportFiles = @"FeatureTests\Animation\ThicknessBorder1.xaml")]
        [Variation(@"FeatureTests\Animation\ThicknessBorder2.xaml", SupportFiles = @"FeatureTests\Animation\ThicknessBorder2.xaml")]
        [Variation(@"FeatureTests\Animation\ThicknessBorder2s.xaml", SupportFiles = @"FeatureTests\Animation\ThicknessBorder2s.xaml", Priority = 1)]
        [Variation(@"FeatureTests\Animation\ThicknessBorder3.xaml", SupportFiles = @"FeatureTests\Animation\ThicknessBorder3.xaml")]
        [Variation(@"FeatureTests\Animation\ThicknessPolyline2s.xaml", SupportFiles = @"FeatureTests\Animation\ThicknessPolyline2s.xaml")]

        /// <summary>
        /// SerializeAnimationTest. Sets up the test with the given markupFile
        /// </summary>
        /// <returns></returns>
        public SerializeAnimationTest(string markupFile)
        {
            s_fileName = markupFile;
            InitializeSteps += new TestStep(InitializeHelper);
            RunSteps += new TestStep(RoundTrip);
        }

        #endregion


        #region Test Steps

        /// <summary>
        /// Initialize: create a new Window and add an Ellipse to it.
        /// </summary>
        /// <returns></returns>
        TestResult InitializeHelper()
        {
            
            s_helper = new SerializationHelper();
            s_helper.DoXamlComparison = false;
            s_helper.XamlSerialized += new XamlSerializedEventHandler(_OnXamlSerialized);

            return TestResult.Pass;
        }


        /// <summary>
        /// RoundTrip: call ClientTestRuntime routines to conduct a RoundTrip serialization test.
        /// </summary>
        /// <returns>A TestResult object, indicating Pass or Fail</returns>
        TestResult RoundTrip()
        {
            TestResult testResult;

            try
            {
                int indx = s_fileName.LastIndexOf("\\");
                if (indx >= 0)
                {
                    s_fileName       = s_fileName.Remove(0,indx+1);
                }

                s_helper.RoundTripTestFile(s_fileName, XamlWriterMode.Expression, true);
                GlobalLog.LogStatus("\r\n\r\n**** Round-trip testing -- File: " + s_fileName + " ****");

                string expectedContent = "Avalon!";

                GlobalLog.LogEvidence("--------------------------------------------");
                GlobalLog.LogEvidence("--Expected Content: ---" + expectedContent + "---");
                GlobalLog.LogEvidence("--Actual Content:   ---" + s_actualContent   + "---");
                GlobalLog.LogEvidence("--------------------------------------------");

                if (s_actualContent == expectedContent)
                {
                    testResult = TestResult.Pass;
                }
                else
                {
                    testResult = TestResult.Fail;
                }

            }
            catch (Exception e1)
            {
                GlobalLog.LogEvidence("***ROUNDTRIP FAILURE***\n\n" + e1); 
                testResult = TestResult.Fail;
            }
            
            return testResult;
        }

        private static void _OnXamlSerialized(object sender, XamlSerializedEventArgs args)
        {
           GlobalLog.LogStatus("Serialized xaml: " + args.Xaml);
        }

        /// <summary>
        /// Verify the content of an element in Markup.
        /// If the element has no text content, its existence is verified by setting a value for
        /// "content" if it is found in the logical tree.
        /// </summary>
        public static void VerifyContent(UIElement root)
        {
            GlobalLog.LogStatus("-----VerifyContent-----");

            try
            {
               if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("decorator") >= 0)
               {
                    GlobalLog.LogStatus("-----Decorator-----");
                    Decorator dec = (Decorator)LogicalTreeHelper.FindLogicalNode((DependencyObject)root, "Animate");
                    if (dec != null)
                    {
                         Button buttonInDecorator = (Button)dec.Child;
                         s_actualContent = (string)buttonInDecorator.Content;
                    }
               } 
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("button") >= 0)
               {
                    GlobalLog.LogStatus("-----Button-----");
                    Button button = (Button)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    if (button != null)
                    {
                         s_actualContent = (string)button.Content;
                    }
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("radio") >= 0)
               {
                    GlobalLog.LogStatus("-----RadioButton-----");
                    RadioButton radio = (RadioButton)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    if (radio != null)
                    {
                         s_actualContent = (string)radio.Content;
                    }
               }               
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("textbox") >= 0)
               {
                    GlobalLog.LogStatus("-----TextBox-----");
                    TextBox textbox = (TextBox)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    if (textbox != null)
                    {
                         s_actualContent = (string)textbox.Text;
                    }
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("textblock") >= 0)
               {
                    GlobalLog.LogStatus("-----TextBlock-----");
                    TextBlock textblock = (TextBlock)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    if (textblock != null)
                    {
                         //Run runInTextBlock = (Run)LogicalTreeHelper.FindLogicalNode(root, "Run1");
                         //actualContent = (string)runInTextBlock.Text;
                         s_actualContent = "Avalon!";
                    }
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("text") >= 0)
               {
                    GlobalLog.LogStatus("-----TextBlock Discrete-----");
                    TextBlock text = (TextBlock)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    if (text != null)
                    {
                         //Run runInText = (Run)LogicalTreeHelper.FindLogicalNode(root, "Run1");
                         //actualContent = (string)runInText.Text;
                         s_actualContent = "Avalon!";
                    }
               }               
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("combobox") >= 0)
               {
                    GlobalLog.LogStatus("-----ComboBox-----");
                    ComboBox combobox = (ComboBox)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    if (combobox != null)
                    {
                         s_actualContent = (string)combobox.Text;
                    }
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("listbox") >= 0)
               {
                    GlobalLog.LogStatus("-----ListBox-----");
                    ListBox listbox = (ListBox)LogicalTreeHelper.FindLogicalNode((DependencyObject)root, "Animate");
                    if (listbox != null)
                    {
                         s_actualContent = "Avalon!";
                    }
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("border") >= 0)
               {
                    GlobalLog.LogStatus("-----Border-----");
                    //Looking for a TextBlock element inside the Border.
                    Border border = (Border)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    if (border != null)
                    {
                         Run runInBorder = (Run)LogicalTreeHelper.FindLogicalNode(root, "Run1");
                         s_actualContent = (string)runInBorder.Text;
                    }
               }               
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("inkcanvas") >= 0)
               {
                    GlobalLog.LogStatus("-----InkCanvas-----");
                    //Retrieving contents of a TextBlock element inside the InkCanvas.
                    InkCanvas ink = (InkCanvas)LogicalTreeHelper.FindLogicalNode((DependencyObject)root, "Animate");
                    if (ink != null)
                    {
                         Run runInInkCanvas = (Run)LogicalTreeHelper.FindLogicalNode(root, "Run1");
                         s_actualContent = (string)runInInkCanvas.Text;
                    }
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("canvas") >= 0)
               {
                    GlobalLog.LogStatus("-----Canvas-----");
                    //Looking for a TextBlock or a Button element inside the Canvas.
                    Canvas canvas1 = (Canvas)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    if (canvas1 != null)
                    {
                         Run runInCanvas = (Run)LogicalTreeHelper.FindLogicalNode(root, "Run1");
                         s_actualContent = (string)runInCanvas.Text;
                    }
                    else
                    {
                        Canvas canvas2 = (Canvas)LogicalTreeHelper.FindLogicalNode(root, "Animate1");
                        if (canvas2 != null)
                        {
                             Button buttonInCanvas = (Button)canvas2.Children[0];
                             s_actualContent = (string)buttonInCanvas.Content;
                        }
                    }
               } 
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("grid") >= 0)
               {
                    GlobalLog.LogStatus("-----Grid-----");
                    //Looking for a Button element inside the Grid.
                    Grid grid = (Grid)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    if (grid != null)
                    {
                         Button buttonInGrid = (Button)grid.Children[0];
                         s_actualContent = (string)buttonInGrid.Content;
                    }
               } 
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("polyline") >= 0)
               {
                    GlobalLog.LogStatus("-----Polyline-----");
                    Polyline polyline = (Polyline)LogicalTreeHelper.FindLogicalNode((DependencyObject)root, "Animate");
                    if (polyline != null)
                    {
                         s_actualContent = "Avalon!";
                    }
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("rectangle") >= 0)
               {
                    GlobalLog.LogStatus("-----Rectangle-----");
                    Rectangle rectangle = (Rectangle)LogicalTreeHelper.FindLogicalNode((DependencyObject)root, "Animate");
                    if (rectangle != null)
                    {
                         s_actualContent = "Avalon!";
                    }
               }
               else if ((s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("line") >= 0)
                        && (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("spline") < 0))
               {
                    //Ignore "Spline" in the file name.
                    GlobalLog.LogStatus("-----Line-----");
                    Line line = (Line)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    if (line != null)
                    {
                         s_actualContent = "Avalon!";
                    }
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("ellipse") >= 0)
               {
                    GlobalLog.LogStatus("-----Ellipse-----");
                    Ellipse ellipse = (Ellipse)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    Ellipse ellipse1 = (Ellipse)LogicalTreeHelper.FindLogicalNode(root, "Ellipse1");
                    if (ellipse != null || ellipse1 != null)
                    {
                         s_actualContent = "Avalon!";
                    }
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("path") >= 0)
               {
                    GlobalLog.LogStatus("-----Path-----");
                    Path path = (Path)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    if (path != null)
                    {
                          s_actualContent = "Avalon!";
                    }
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("frame") >= 0)
               {
                    GlobalLog.LogStatus("-----Frame-----");
                    FrameworkElement frame = (FrameworkElement)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    if (frame != null)
                    {
                         s_actualContent = "Avalon!";
                    }
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("tooltip") >= 0)
               {
                    GlobalLog.LogStatus("-----ToolTip-----");
                    s_actualContent = "Avalon!";
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("image") >= 0)
               {
                    GlobalLog.LogStatus("-----Image-----");
                    Image image = (Image)LogicalTreeHelper.FindLogicalNode((DependencyObject)root, "Animate");
                    if (image != null)
                    {
                         s_actualContent = "Avalon!";
                    }
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("treeview") >= 0)
               {
                    GlobalLog.LogStatus("-----TreeView-----");
                    TreeView treeview = (TreeView)LogicalTreeHelper.FindLogicalNode((DependencyObject)root, "Animate");
                    if (treeview != null)
                    {
                         s_actualContent = "Avalon!";
                    }
               }
               else if (s_fileName.ToLower(CultureInfo.InvariantCulture).IndexOf("slider") >= 0)
               {
                    GlobalLog.LogStatus("-----Slider-----");
                    Slider slider = (Slider)LogicalTreeHelper.FindLogicalNode(root, "Animate");
                    if (slider != null)
                    {
                          s_actualContent = "Avalon!";
                    }
               }
               else
               {
                    GlobalLog.LogEvidence("***TEST FAILED: element cannot be found.");
                    throw new TestValidationException("Verifying Content Failed");
               }
            }
            catch (Exception e2)
            {
                GlobalLog.LogEvidence("***VERIFYCONTENT FAILURE***\n\n" + e2);
                throw new TestValidationException("Verifying Content Failed");
            }
        }
        #endregion
    }
}
