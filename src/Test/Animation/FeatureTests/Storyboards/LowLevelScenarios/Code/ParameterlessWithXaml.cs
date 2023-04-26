// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboards.LowLevelScenarios</area>
    /// <priority>1</priority>
    /// <description>
    /// These tests verify that a Storyboard succeeds when set Markup, in different 
    /// fundamental scenarios.
    ///  Note:
    ///    For details of the new feature being introduced in .Net3.5-SP1, refer to the test spec
    ///    located at:  Test\Animation\Specifications\Storyboard Parameterless Methods Test Spec.docx
    /// </description>
    /// </summary>
    [Test(1, "Storyboards.LowLevelScenarios", "ParameterlessWithXamlTest")]
    public class ParameterlessWithXamlTest : XamlTest
    {
        #region Test case members

        public static VisualVerifier    verifier;
        private string                  _inputString;
        private Rectangle               _rectangle;  //Used in multiple scenarios.
        private Storyboard              _storyboard;
        private Int32                   _durationInMarkup = 1;  //In seconds.
        private DispatcherTimer         _aTimer = null;
        
        #endregion


        #region Constructor

        [Variation("FrameworkElement", Priority=0)]
        [Variation("Animatable", Priority=0)]
        [Variation("Nested")]
        [Variation("Viewport2D_Visual3D")]
        [Variation("DataBinding")]
        [Variation("InParent")]
        [Variation("InSibling")]
        [Variation("InElement", Priority=0)]
        [Variation("DirectTargeting")]
        [Variation("MultipleAnimations")]

        /******************************************************************************
        * Function:          ParameterlessWithXamlTest Constructor
        ******************************************************************************/
        public ParameterlessWithXamlTest(string variation) : base(@"ParameterlessWithXaml.xaml")
        {
            _inputString = variation;
            InitializeSteps += new TestStep(FindStoryboard);
            RunSteps += new TestStep(StartStoryboard);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        /******************************************************************************
           * Function:          FindStoryboard
           ******************************************************************************/
        /// <summary>
        /// Retrieve the Storyboard that is specified in the Resources section in Markup.
        /// Then add a new Animation to the Storyboard, and start it.
        /// <returns>Returns success if the Storyboard was found</returns>
        /// </summary>
        private TestResult FindStoryboard()
        {
            GlobalLog.LogStatus("---FindStoryboard---");

            Window.Title        = "Animation";
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.Topmost      = true;
            Window.WindowStyle  = WindowStyle.None;
            verifier = new VisualVerifier();

            switch (_inputString)
            {
                case "FrameworkElement":
                    _rectangle = (Rectangle)RootElement.FindName("Rectangle1");
                    CheckForNull(_rectangle, "Rectangle1");

                    _rectangle.Width = 0d;
                    _rectangle.Fill = Brushes.Green;

                    _storyboard = (Storyboard)RootElement.TryFindResource(_inputString);
                    _rectangle.Visibility = Visibility.Visible;
                    break;
                case "Animatable":
                    _rectangle = (Rectangle)RootElement.FindName("Rectangle1");
                    CheckForNull(_rectangle, "Rectangle1");

                    _storyboard = (Storyboard)RootElement.TryFindResource(_inputString);
                    _rectangle.Visibility = Visibility.Visible;
                    break;
                case "DataBinding":
                    _rectangle = (Rectangle)RootElement.FindName("Rectangle1");
                    CheckForNull(_rectangle, "Rectangle1");

                    _storyboard = (Storyboard)RootElement.TryFindResource(_inputString);
                    _rectangle.Visibility = Visibility.Visible;
                    break;
                case "Nested":
                    SolidColorBrush SCB = (SolidColorBrush)RootElement.FindName("RectangleSCB");
                    CheckForNull(SCB, "RectangleSCB");

                    _rectangle = (Rectangle)RootElement.FindName("Rectangle1");
                    CheckForNull(_rectangle, "Rectangle1");

                    _rectangle.Width = 0d;
                    _rectangle.Height = 0d;
                    SCB.Color = Colors.Green;
                    SCB.Opacity = 0d;

                    _storyboard = (Storyboard)RootElement.TryFindResource(_inputString);
                    _rectangle.Visibility = Visibility.Visible;
                    break;
                case "Viewport2D_Visual3D":
                    Viewport3D viewport3D = (Viewport3D)LogicalTreeHelper.FindLogicalNode((DependencyObject)RootElement,"Viewport3D2");
                    CheckForNull(viewport3D, "Viewport3D");

                    _storyboard = (Storyboard)RootElement.TryFindResource(_inputString);
                    viewport3D.Visibility = Visibility.Visible;
                    break;
                case "InParent":
                    Canvas grandparent = (Canvas)RootElement.FindName("Canvas3a");
                    CheckForNull(grandparent, "Canvas3a");

                    Canvas parent = (Canvas)RootElement.FindName("Canvas3b");
                    CheckForNull(parent, "Canvas3b");

                    _storyboard = (Storyboard)parent.TryFindResource(_inputString);
                    grandparent.Visibility = Visibility.Visible;
                    break;
                case "InSibling":
                    Button button = (Button)RootElement.FindName("Button1");   //The sibling.
                    CheckForNull(button, "Button1");

                    Polygon polygon = (Polygon)RootElement.FindName("Polygon1");
                    CheckForNull(polygon, "Polygon1");

                    _storyboard = (Storyboard)button.TryFindResource(_inputString);
                    polygon.Visibility = Visibility.Visible;
                    break;
                case "InElement":
                    ListBox listBox = (ListBox)RootElement.FindName("ListBox1");
                    CheckForNull(listBox, "ListBox1");

                    _storyboard = (Storyboard)listBox.TryFindResource(_inputString);
                    listBox.Visibility = Visibility.Visible;
                    break;
                case "DirectTargeting":
                    TextBox textbox = (TextBox)RootElement.FindName("TextBox1");
                    CheckForNull(textbox, "TextBox1");

                    _storyboard = (Storyboard)RootElement.TryFindResource(_inputString);
                    textbox.Visibility = Visibility.Visible;
                    break;
                case "MultipleAnimations":
                    Expander expander = (Expander)RootElement.FindName("Expander1");
                    CheckForNull(expander, "Expander1");

                    _storyboard = (Storyboard)expander.TryFindResource(_inputString);
                    expander.Visibility = Visibility.Visible;
                    break;
            }

            if (_storyboard == null)
            {
                GlobalLog.LogEvidence("Error!!! The Storyboard was not found.");
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }
        }
        
        /******************************************************************************
        * Function:          StartStoryboard
        ******************************************************************************/
        /// <summary>
        /// Begins the Storyboard and starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns></returns>
        TestResult StartStoryboard()
        {
            GlobalLog.LogStatus("---StartStoryboard---");

            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = TimeSpan.FromSeconds(_durationInMarkup + 1);
            _aTimer.Start();

            _storyboard.Begin();  //START THE ANIMATION.

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          OnTick
        ******************************************************************************/
        /// <summary>
        /// Invoked every time the DispatcherTimer ticks. Used to control the timing of verification.
        /// </summary>
        /// <returns></returns>
        private void OnTick(object sender, EventArgs e)          
        {
            GlobalLog.LogStatus("---OnTick---");

            _aTimer.Stop();
            Signal("AnimationDone", TestResult.Pass);
        }


        /******************************************************************************
        * Function:          CheckForNull
        ******************************************************************************/
        /// <summary>
        /// Checks for null on the object passed in. Throws
        /// </summary>
        /// <returns></returns>
        private void CheckForNull(object obj, string checkedItem)
        {
            if (obj == null)
            {
                throw new TestValidationException("ERROR!!! The " + checkedItem + " was not found.");
            }
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and issue a Pass/Fail result.
        /// </summary>
        /// <returns></returns>
        TestResult Verify()
        {
            GlobalLog.LogStatus("---Verify---");

            WaitForSignal("AnimationDone");

            float tolerance = 0.10f;
            int x           = 100;
            int y           = 100;
            Color expColor  = Colors.Green;

            if (_inputString == "Viewport2D_Visual3D")
            {
                expColor = Colors.White;
            }

            verifier.InitRender(Window);

            Color actColor = verifier.getColorAtPoint(x,y);

            bool testPassed = AnimationUtilities.CompareColors(expColor, actColor, tolerance);

            GlobalLog.LogEvidence("---------- Result at (" + x + "," + y + ") ------");
            GlobalLog.LogEvidence(" Actual   : " + actColor.ToString());
            GlobalLog.LogEvidence(" Expected : " + expColor.ToString());
            
            if (testPassed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        #endregion
    }
}
